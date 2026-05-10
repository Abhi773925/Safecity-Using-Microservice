import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthApiService } from '../../../../core/services/api/auth-api.service';
import { getUserId, isTokenValid, clearTokens } from '../../../../shared/auth-utils';

@Component({
  selector: 'app-delete-account',
  standalone: true,
  imports: [],
  templateUrl: './delete-account.html',
  styleUrl: './delete-account.css',
})
export class DeleteAccount {
  constructor(private authApi: AuthApiService, private router: Router) {}

  async deleteAccountBody(): Promise<void> {
    if (!isTokenValid()) { alert('Session expired. Please login again.'); return; }
    const userId = getUserId();
    if (!userId) { alert('Invalid token. Please login again.'); return; }
    try {
      await this.authApi.deleteAccount(userId);
      alert('Account deleted successfully.');
      clearTokens();
      this.router.navigate(['/']);
    } catch {
      alert('Failed to delete account. Please try again.');
    }
  }
}
