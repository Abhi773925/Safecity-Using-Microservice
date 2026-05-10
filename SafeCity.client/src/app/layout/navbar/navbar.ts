import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { getToken, clearTokens, isTokenValid } from '../../shared/auth-utils';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit {
  isLoggedIn = false;
  isProfileMenuOpen = false;
  isMobileMenuOpen = false;
  userName = '';
  userEmail = '';
  userInitials = 'SC';

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.loadUser();
  }

  private loadUser(): void {
    this.isLoggedIn = isTokenValid();
    if (!this.isLoggedIn) return;

    const token = getToken();
    try {
      const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const claims = JSON.parse(atob(payload));

      const name = claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || claims['name'] || 'User';
      const email = claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || claims['email'] || '';
      const role = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || claims['role'] || '';

      this.userName = name;
      this.userEmail = email;
      localStorage.setItem('userRole', role);
      localStorage.setItem('userEmail', email);

      const parts = name.trim().split(/\s+/);
      this.userInitials = parts.length >= 2
        ? `${parts[0][0]}${parts[1][0]}`.toUpperCase()
        : name.slice(0, 2).toUpperCase();
    } catch {
      this.userName = '';
      this.userEmail = '';
    }
  }

  toggleProfileMenu(): void {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    this.isProfileMenuOpen = false;
  }

  logout(): void {
    clearTokens();
    this.isLoggedIn = false;
    this.userName = '';
    this.isProfileMenuOpen = false;
    this.router.navigate(['/login']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
