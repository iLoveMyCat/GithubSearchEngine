import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { GithubService } from '../../services/github.service';
import { SearchResult } from '../../interfaces/search-result';
import { Favorite } from '../../interfaces/favorite.interface';
import {
  debounceTime,
  distinctUntilChanged,
  Subject,
  Subscription,
  switchMap,
} from 'rxjs';
import { ErrorHandlerService } from '../../services/error-handler.service';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent implements OnInit {
  searchQuery: string = '';
  searchResults: SearchResult[] = [];
  suggestions: SearchResult[] = [];
  searchDone: boolean = false;
  searchSubject = new Subject<string>();

  errorMessage: string = '';
  isRateLimited: boolean = false;
  showSuggestions: boolean = false;

  constructor(
    private githubService: GithubService,
    private errorHandler: ErrorHandlerService,
    private spinnerService: SpinnerService
  ) {}
  ngOnInit(): void {
    // Handle real-time suggestions
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((query) => {
          return this.githubService.searchRepositories(query, 5); // limit to 5
        })
      )
      .subscribe({
        next: (results) => {
          this.suggestions = results;
        },
        error: (err) => console.error(err),
      });
  }

  // listen for clicks
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    //  if click is outside the search input and dropdown, hide suggestions
    if (!target.closest('.search-wrapper')) {
      this.toggleSuggestions(false);
    }
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.toggleSuggestions(false); // hide suggestions if clicked search
      this.suggestions = []; // hide suggestions if clicked search
      this.spinnerService.show();
      this.githubService.searchRepositories(this.searchQuery).subscribe({
        next: (results) => {
          this.searchResults = results;
          this.searchDone = true;
          this.suggestions = [];
          this.spinnerService.hide();
        },
        error: (err) => {
          this.spinnerService.hide();
          this.errorHandler.handleError(
            err,
            (msg) => (this.errorMessage = msg),
            () =>
              (this.isRateLimited = this.errorHandler.isCurrentlyRateLimited())
          );
        },
      });
    }
  }

  onInputChange(query: string): void {
    if (query.length > 2) {
      this.searchSubject.next(query); // Trigger suggestions
      this.toggleSuggestions(true); // allow suggestions dropedown if input changed
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
    this.spinnerService.show();

    this.githubService.addToFavorites(favorite).subscribe({
      next: (response) => {
        alert('Favorite added successfully');
        console.log(response.message);
      },
      error: (err) => {
        this.spinnerService.hide();
        this.errorHandler.handleError(
          err,
          (msg) => (this.errorMessage = msg),
          () =>
            (this.isRateLimited = this.errorHandler.isCurrentlyRateLimited())
        );
      },
    });
  }

  toggleSuggestions(isShow: boolean): void {
    this.showSuggestions = isShow;
  }
}
