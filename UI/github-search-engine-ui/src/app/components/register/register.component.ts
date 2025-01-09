import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService) {}

  register(event: Event): void {
    event.preventDefault();
    debugger;
    this.authService.register(this.username, this.password).subscribe({
      next: () => {
        alert('User registered successfully!');
      },
      error: (err) => {
        this.errorMessage = err.error?.Message || 'An error occurred.';
      },
    });
  }
}
