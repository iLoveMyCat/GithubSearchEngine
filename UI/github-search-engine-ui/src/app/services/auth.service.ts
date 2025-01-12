import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = `${environment.apiUrl}/auth`;
  private usernameSubject = new BehaviorSubject<string | null>(
    localStorage.getItem('username')
  );

  constructor(private http: HttpClient, private router: Router) {}

  getUsername(): Observable<string | null> {
    return this.usernameSubject.asObservable();
  }

  login(username: string, password: string): Observable<any> {
    return this.http
      .post<any>(
        `${this.baseUrl}/login`,
        { username, password },
        { withCredentials: true }
      )
      .pipe(
        tap((response) => {
          if (response.username) {
            this.usernameSubject.next(response.Username);
            localStorage.setItem('username', response.username);
          }
        })
      );
  }

  logout(): void {
    this.http
      .post(`${this.baseUrl}/logout`, {}, { withCredentials: true })
      .subscribe(() => {});
    this.clearState();
  }

  register(username: string, password: string): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/register`,
      { username, password },
      { withCredentials: true }
    );
  }

  clearState() {
    this.usernameSubject.next(null);
    localStorage.removeItem('username');
    this.router.navigate(['/login']);
  }

  checkSession(): Observable<any> {
    return this.http
      .get<any>(`${this.baseUrl}/session`, { withCredentials: true })
      .pipe(
        tap((response) => {
          if (response.isAuthenticated) {
            this.usernameSubject.next(response.username);
          } else {
            this.clearState();
          }
        }),
        catchError(() => {
          this.clearState();
          return throwError(() => 'Session expired');
        })
      );
  }

  initializeAuth(): Promise<void> {
    debugger;
    return this.checkSession()
      .toPromise()
      .catch(() => {});
  }

  isLoggedIn(): boolean {
    return this.usernameSubject.value !== null;
  }

  refreshToken(): Observable<any> {
    return this.http
      .post(`${this.baseUrl}/refresh-token`, {}, { withCredentials: true })
      .pipe(
        tap(() => console.log('Token refreshed')),
        catchError(() => {
          this.clearState();
          return throwError(() => 'Failed to refresh token');
        })
      );
  }
}
