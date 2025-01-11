import { Component } from '@angular/core';
import { Favorite } from '../../interfaces/favorite.interface';
import { GithubService } from '../../services/github.service';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.scss',
})
export class FavoritesComponent {
  favorites: Favorite[] = [];

  constructor(
    private githubService: GithubService,
    private spinnerService: SpinnerService
  ) {}

  removeFromFavorites(repo: any) {
    this.favorites = this.favorites.filter((fav) => fav !== repo);
    alert(
      'TODO: add a service and an endpoint (not specified in the exercise)'
    );
    // TODO: add a service and an endpoint (not specified in the exercise) ***
  }

  ngOnInit(): void {
    this.spinnerService.show();
    this.githubService.getFavorites().subscribe({
      next: (favorites) => {
        this.spinnerService.hide();
        this.favorites = favorites;
      },
      error: (err) => {
        this.spinnerService.hide();
        console.error('Failed to load favorites', err);
      },
    });
  }
}
