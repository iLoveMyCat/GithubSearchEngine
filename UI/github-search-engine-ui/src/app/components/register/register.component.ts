import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private spinnerService: SpinnerService
  ) {}

  register(event: Event): void {
    event.preventDefault();
    this.spinnerService.show();
    this.authService.register(this.username, this.password).subscribe({
      next: () => {
        this.spinnerService.hide();
        alert('User registered successfully!');
      },
      error: (err) => {
        this.spinnerService.hide();
        this.errorMessage = err.error?.Message || 'An error occurred.';
      },
    });
  }
}
