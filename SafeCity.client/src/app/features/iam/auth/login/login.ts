import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../../../core/services/api/auth-api.service';
import { saveToken } from '../../../../shared/auth-utils';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  isSubmitting = false;
  showPassword = false;
  errorMessage = '';
  successMessage = '';

  loginForm: FormGroup;

  constructor(
    private authApi: AuthApiService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.loginForm = this.fb.nonNullable.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  get emailControl() { return this.loginForm.controls['email']; }
  get passwordControl() { return this.loginForm.controls['password']; }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  async submitLogin(): Promise<void> {
    if (this.loginForm.invalid || this.isSubmitting) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';
    this.isSubmitting = true;

    const payload = {
      email: this.loginForm.getRawValue().email,
      password: this.loginForm.getRawValue().password,
    };

    try {
      const res = await this.authApi.login(payload);
      saveToken(res.data.accessToken);
      this.successMessage = res.message;
      this.loginForm.reset();
      window.location.reload();
    } catch (err: any) {
      this.errorMessage = err?.response?.data?.message || 'Login failed. Please try again.';
    } finally {
      this.isSubmitting = false;
    }
  }
}
