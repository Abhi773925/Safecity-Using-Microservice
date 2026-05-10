import { Injectable } from '@angular/core';
import axios from 'axios';
import { API_BASE_URL } from '../../../shared/constants';

export interface Incident {
  incidentID: number;
  citizenID: number;
  type: string;
  location: string;
  date: string;
  status: string;
}

export interface Case {
  caseID: number;
  incidentID: number;
  assignedOfficerID: number;
  description: string;
  status: string;
  resolutionDate: string;
}

@Injectable({ providedIn: 'root' })
export class IrcmApiService {
  private incidentBase = `${API_BASE_URL}/incident`;
  private caseBase = `${API_BASE_URL}/case`;

  async getAllIncidents(): Promise<Incident[]> {
    const res = await axios.get<{ data: Incident[] }>(`${this.incidentBase}/list/all`);
    return res.data.data ?? [];
  }

  async getMyIncidents(): Promise<Incident[]> {
    const res = await axios.get<{ data: Incident[] }>(`${this.incidentBase}/my-incidents`);
    return res.data.data ?? [];
  }

  async createIncident(payload: { type: number; location: string; date: string; status: number; citizenID: number }) {
    const res = await axios.post<{ message: string }>(`${this.incidentBase}/create`, payload);
    return res.data.message;
  }

  async updateIncidentStatus(incidentID: number, option: number) {
    const res = await axios.patch<{ message: string }>(`${this.incidentBase}/${incidentID}/status?option=${option}`);
    return res.data.message;
  }

  async getAllCases(): Promise<Case[]> {
    const res = await axios.get<{ data: Case[] }>(`${this.caseBase}/all-cases`);
    return res.data.data ?? [];
  }

  async getMyCases(): Promise<Case[]> {
    const res = await axios.get<{ data: Case[] }>(`${this.caseBase}/my-cases`);
    return res.data.data ?? [];
  }

  async createCase(incidentID: number, payload: { incidentID: number; description: string; status: number; resolutionDate: string }) {
    const res = await axios.post<{ message: string }>(`${this.caseBase}/create/${incidentID}`, payload);
    return res.data.message;
  }

  async closeCase(caseID: number, payload: { description: string; status: number; resolutionDate: string }) {
    const res = await axios.post<{ message: string }>(`${this.caseBase}/submission/${caseID}`, payload);
    return res.data.message;
  }
}
