import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ErrorHandlerService {
  private isRateLimited: boolean = false;

  constructor() {}

  handleError(
    error: HttpErrorResponse,
    setError: (message: string) => void,
    disableUI?: () => void
  ): void {
    debugger;
    console.error('Error caught:', error);
    if (error.status === 429) {
      setError('Too many requests. Please wait and try again.');
      this.isRateLimited = true;

      if (disableUI) disableUI();

      // Auto-recover after 10 seconds
      setTimeout(() => {
        this.isRateLimited = false;
        setError('');
        if (disableUI) disableUI();
      }, 10000);
    } else if (error.status === 500) {
      setError('Server error occurred. Please try again later.');
    } else if (error.status === 404) {
      setError('Resource not found.');
    } else {
      setError('An unexpected error occurred.');
    }
  }

  isCurrentlyRateLimited(): boolean {
    return this.isRateLimited;
  }
}
