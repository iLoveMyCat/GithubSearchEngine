import { Component } from '@angular/core';
import { GithubService } from '../../services/github.service';
import { SearchResult } from '../../interfaces/search-result';
import { Favorite } from '../../interfaces/favorite.interface';
import { debounceTime, distinctUntilChanged, Subject, switchMap } from 'rxjs';
import { ErrorHandlerService } from '../../services/error-handler.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent {
  searchQuery: string = '';
  searchResults: SearchResult[] = [];
  suggestions: SearchResult[] = [];
  searchDone: boolean = false;
  private searchSubject = new Subject<string>();

  errorMessage: string = '';
  isRateLimited: boolean = false;

  constructor(
    private githubService: GithubService,
    private errorHandler: ErrorHandlerService
  ) {
    // Handle real-time suggestions
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((query) => this.githubService.searchRepositories(query, 5)) // Limit suggestions to 5
      )
      .subscribe({
        next: (results) => {
          this.suggestions = results;
        },
        error: (err) => console.error(err),
      });
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.githubService.searchRepositories(this.searchQuery).subscribe({
        next: (results) => {
          this.searchResults = results;
          this.searchDone = true;
          this.suggestions = [];
        },
        error: (err) =>
          this.errorHandler.handleError(
            err,
            (msg) => (this.errorMessage = msg),
            () =>
              (this.isRateLimited = this.errorHandler.isCurrentlyRateLimited())
          ),
      });
    }
  }

  onInputChange(query: string): void {
    if (query.length > 2) {
      this.searchSubject.next(query); // Trigger suggestions
    } else {
      this.suggestions = []; // Clear dropdown if input is short
    }
  }

  selectSuggestion(suggestion: SearchResult): void {
    this.searchQuery = suggestion.repositoryName;
    this.onSearch(); // Perform full search when suggestion is clicked
    this.suggestions = []; // Hide dropdown
  }

  addToFavorites(repository: SearchResult): void {
    const favorite: Favorite = {
      repositoryName: repository.repositoryName,
      repositoryUrl: repository.repositoryUrl,
    };

    this.githubService.addToFavorites(favorite).subscribe({
      next: (response) => {
        alert('Favorite added successfully');
        console.log(response.message);
      },
      error: (err) =>
        this.errorHandler.handleError(
          err,
          (msg) => (this.errorMessage = msg),
          () =>
            (this.isRateLimited = this.errorHandler.isCurrentlyRateLimited())
        ),
    });
  }
}
