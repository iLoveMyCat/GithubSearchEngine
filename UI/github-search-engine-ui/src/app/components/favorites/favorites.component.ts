import { Component } from '@angular/core';
import { Favorite } from '../../interfaces/favorite.interface';
import { GithubService } from '../../services/github.service';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.scss',
})
export class FavoritesComponent {
  favorites: Favorite[] = [];

  constructor(private githubService: GithubService) {}

  ngOnInit(): void {
    this.githubService.getFavorites().subscribe({
      next: (favorites) => (this.favorites = favorites),
      error: (err) => console.error('Failed to load favorites', err),
    });
  }
}
