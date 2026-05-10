import { Injectable } from '@angular/core';
import axios from 'axios';
import { API_BASE_URL } from '../../../shared/constants';

export interface Patrol {
  patrolId: number;
  officerId: number;
  area: string;
  date: string;
  status: string;
}

export interface FieldReport {
  reportId: number;
  patrolId: number;
  notes: string;
  date: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class PfomApiService {
  private patrolBase = `${API_BASE_URL}/patrol`;
  private reportBase = `${API_BASE_URL}/fieldreport`;

  async getMyPatrols(): Promise<Patrol[]> {
    const res = await axios.get<Patrol[]>(`${this.patrolBase}/my-patrols`);
    return res.data ?? [];
  }

  async getPatrolHistory(): Promise<Patrol[]> {
    const res = await axios.get<Patrol[]>(`${this.patrolBase}/my-patrol-history`);
    return res.data ?? [];
  }

  async schedulePatrol(payload: { officerId: number; area: string; date: string; status: number }) {
    await axios.post(`${this.patrolBase}/schedule`, payload);
  }

  async updatePatrolStatus(payload: { patrolId: number; newStatus: number }) {
    await axios.patch(`${this.patrolBase}/update-status`, payload);
  }

  async getAllFieldReports(): Promise<FieldReport[]> {
    const res = await axios.get<{ data: FieldReport[] }>(`${this.reportBase}/all`);
    return res.data.data ?? [];
  }

  async getMyFieldReports(): Promise<FieldReport[]> {
    const res = await axios.get<FieldReport[]>(`${this.reportBase}/my-history`);
    return res.data ?? [];
  }

  async submitFieldReport(payload: { patrolId: number; notes: string; date: string; status: number }) {
    await axios.post(`${this.reportBase}/submit`, payload);
  }

  async reviewFieldReport(reportId: number, newStatus: number) {
    await axios.patch(`${this.reportBase}/${reportId}/review?newStatus=${newStatus}`);
  }
}
