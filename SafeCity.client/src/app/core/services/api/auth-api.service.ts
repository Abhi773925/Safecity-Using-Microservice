import { Injectable } from '@angular/core';
import axios from 'axios';
import { API_BASE_URL } from '../../../shared/constants';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private base = `${API_BASE_URL}/auth`;

  async login(payload: { email: string; password: string }) {
    const res = await axios.post<{ message: string; data: { accessToken: string } }>(
      `${this.base}/login`, payload
    );
    return res.data;
  }

  async register(payload: { name: string; roleId: number; email: string; phone: string; password: string }) {
    const res = await axios.post<{ message: string }>(`${this.base}/register`, payload);
    return res.data.message;
  }

  async changePassword(payload: { email: string; existingPassword: string; newPassword: string }) {
    const res = await axios.put<{ message: string }>(`${this.base}/change/password`, payload);
    return res.data.message;
  }

  async deleteAccount(userId: number) {
    const res = await axios.delete<{ message: string }>(`${this.base}/delete/${userId}`);
    return res.data.message;
  }
}
