import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../../../core/services/api/auth-api.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  showPassword = false;
  showConfirmPassword = false;

  roleOptions = [
    { id: 1, label: 'Citizen' },
    { id: 2, label: 'Police Officer' },
    { id: 3, label: 'Fire Fighter' },
    { id: 4, label: 'Emergency Dispatcher' },
    { id: 5, label: 'Compliance Officer' },
    { id: 6, label: 'City Administrator' },
  ];

  registerForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    roleId: new FormControl(1, { nonNullable: true, validators: [Validators.required, Validators.min(1)] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    phone: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.pattern(/^[0-9]{10}$/)] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] }),
    confirmPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    acceptTerms: new FormControl(false, { nonNullable: true, validators: [Validators.requiredTrue] }),
  });

  constructor(private authApi: AuthApiService, private router: Router) {}

  togglePasswordVisibility(): void { this.showPassword = !this.showPassword; }
  toggleConfirmPasswordVisibility(): void { this.showConfirmPassword = !this.showConfirmPassword; }

  async handleCreateAccount(): Promise<void> {
    if (this.registerForm.invalid || this.isSubmitting) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.isSubmitting = true;
    this.successMessage = '';
    this.errorMessage = '';

    const v = this.registerForm.getRawValue();

    const payload = {
      name: v.name.trim(),
      roleId: Number(v.roleId),
      email: v.email.trim(),
      phone: v.phone.trim(),
      password: v.password,
    };

    try {
      await this.authApi.register(payload);
      this.successMessage = 'Account created successfully.';
      this.registerForm.reset({ roleId: 1, acceptTerms: false });
      setTimeout(() => this.router.navigate(['/login']), 2000);
    } catch (err: any) {
      this.errorMessage = err?.response?.data?.message || err?.response?.data || 'Registration failed.';
    } finally {
      this.isSubmitting = false;
    }
  }
}
