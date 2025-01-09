import { Component } from '@angular/core';
import { GithubService } from '../../services/github.service';
import { SearchResult } from '../../interfaces/search-result';
import { Favorite } from '../../interfaces/favorite.interface';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent {
  searchQuery: string = '';
  searchDone: boolean = false;
  searchResults: SearchResult[] = [];
  favorites: SearchResult[] = [];

  constructor(private githubService: GithubService) {}

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.githubService.searchRepositories(this.searchQuery).subscribe({
        next: (results) => {
          debugger;
          this.searchResults = results;
          this.searchDone = true;
        },
        error: (err) => console.error(err),
      });
    }
  }

  addToFavorites(repository: SearchResult): void {
    const favorite: Favorite = {
      repositoryName: repository.repositoryName,
      repositoryUrl: repository.repositoryUrl,
    };

    this.githubService.addToFavorites(favorite).subscribe({
      next: (response) => {
        debugger;
        this.favorites.push(repository); // add locally
        console.log(response.message);
      },
      error: (err) => console.error('Failed to add favorite', err),
    });
  }
}
