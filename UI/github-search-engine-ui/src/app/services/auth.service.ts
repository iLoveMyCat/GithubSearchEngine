import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';

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

  isLoggedIn(): boolean {
    return this.usernameSubject.value !== null;
  }
}
