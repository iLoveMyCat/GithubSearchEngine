import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
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
        `${this.apiUrl}/login`,
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
    // I considered to still make the username null(logout in UI) even if we fail to get OK response from the server.
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe(() => {
      this.clearState();
    });
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
