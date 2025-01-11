import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private spinnerService: SpinnerService
  ) {
    debugger;
  }

  login(event: Event): void {
    debugger;
    console.log('Username:', this.username);
    console.log('Password:', this.password);

    event.preventDefault();
    this.spinnerService.show();
    this.authService.login(this.username, this.password).subscribe({
      next: () => {
        this.router.navigate(['/']);
        this.spinnerService.hide();
      },
      error: (err) => {
        this.spinnerService.hide();
        this.errorMessage = 'Invalid username or password.';
      },
    });
  }
}
