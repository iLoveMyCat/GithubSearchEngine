import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SearchResult } from '../interfaces/search-result';
import { Favorite } from '../interfaces/favorite.interface';

@Injectable({
  providedIn: 'root',
})
export class GithubService {
  private baseUrl = `${environment.apiUrl}/github`;

  constructor(private http: HttpClient) {}

  searchRepositories(query: string): Observable<SearchResult[]> {
    return this.http.get<any>(`${this.baseUrl}/search?query=${query}`, {
      withCredentials: true,
    });
  }

  addToFavorites(favorite: Favorite): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.baseUrl}/AddToFavorites`,
      favorite,
      { withCredentials: true }
    );
  }

  getFavorites(): Observable<Favorite[]> {
    return this.http.get<Favorite[]>(`${this.baseUrl}/GetFavorites`, {
      withCredentials: true,
    });
  }
}
