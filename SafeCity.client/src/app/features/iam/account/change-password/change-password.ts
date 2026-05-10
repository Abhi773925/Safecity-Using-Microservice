import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthApiService } from '../../../../core/services/api/auth-api.service';
import { isTokenValid, clearTokens } from '../../../../shared/auth-utils';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css',
})
export class ChangePassword {
  submitting = false;
  errorMessage = '';

  changePasswordForm = new FormGroup({
    email: new FormControl('', { nonNullable: true }),
    existingPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] }),
    newPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] }),
  });

  constructor(private authApi: AuthApiService, private router: Router) {}

  async changePasswordRequest(): Promise<void> {
    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }
    if (!isTokenValid()) {
      alert('Session expired. Please login again.');
      this.router.navigate(['/login']);
      return;
    }

    this.submitting = true;
    this.errorMessage = '';

    const v = this.changePasswordForm.getRawValue();

    const payload = {
      email: v.email.trim(),
      existingPassword: v.existingPassword,
      newPassword: v.newPassword,
    };

    try {
      await this.authApi.changePassword(payload);
      alert('Password changed successfully. Please login again.');
      clearTokens();
      this.router.navigate(['/login']);
    } catch (err: any) {
      this.errorMessage = err?.response?.data?.message || 'Failed to change password.';
    } finally {
      this.submitting = false;
    }
  }
}
