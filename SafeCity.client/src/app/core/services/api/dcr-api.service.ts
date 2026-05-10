import { Injectable } from '@angular/core';
import axios from 'axios';
import { API_BASE_URL } from '../../../shared/constants';

export interface Crisis {
  crisisId: number;
  type: string;
  location: string;
  date: string;
  severity: string;
  status: string;
}

export interface Team {
  teamID: number;
  teamName: string;
  teamLeadID: number;
  status: string;
}

export interface Deployment {
  responseId: number;
  crisisId: number;
  crisisLocation: string;
  teamName: string;
  teamLead: number;
  status: string;
  deployedAt: string;
  instructions: string;
}

@Injectable({ providedIn: 'root' })
export class DcrApiService {
  private crisisBase = `${API_BASE_URL}/crisis`;
  private teamBase = `${API_BASE_URL}/team`;
  private responseBase = `${API_BASE_URL}/response`;

  async getActiveCrises(): Promise<Crisis[]> {
    const res = await axios.get<Crisis[]>(`${this.crisisBase}/active`);
    return res.data ?? [];
  }

  async createCrisis(payload: { type: number; location: string; date: string; severity: number; status: number }) {
    await axios.post(this.crisisBase, payload);
  }

  async updateCrisis(crisisId: number, status: number, severity: number) {
    await axios.patch(`${this.crisisBase}/${crisisId}?status=${status}&severity=${severity}`);
  }

  async getAllTeams(): Promise<Team[]> {
    const res = await axios.get<Team[]>(`${this.teamBase}/list`);
    return res.data ?? [];
  }

  async getAvailableTeams(): Promise<Team[]> {
    const res = await axios.get<Team[]>(`${this.teamBase}/available`);
    return res.data ?? [];
  }

  async createTeam(payload: { teamName: string; teamLeadID: number; status: number }) {
    await axios.post(`${this.teamBase}/create`, payload);
  }

  async updateTeamStatus(teamID: number, payload: { newStatus: number }) {
    await axios.patch(`${this.teamBase}/${teamID}/status`, payload);
  }

  async getActiveDeployments(): Promise<Deployment[]> {
    const res = await axios.get<Deployment[]>(`${this.responseBase}/active`);
    return res.data ?? [];
  }

  async deployTeam(payload: { crisisId: number; teamId: number; specialInstructions: string }) {
    await axios.post(`${this.responseBase}/deploy`, payload);
  }

  async reassignTeam(responseId: number, newTeamId: number) {
    await axios.patch(`${this.responseBase}/${responseId}/reassign/${newTeamId}`);
  }

  async updateProgress(responseId: number, payload: { newStatus: number; updateNote: string }) {
    await axios.patch(`${this.responseBase}/${responseId}/progress`, payload);
  }

  async closeMission(responseId: number, payload: { finalClosingNote: string }) {
    await axios.post(`${this.responseBase}/${responseId}/close`, payload);
  }

  async cancelDeployment(responseId: number) {
    await axios.delete(`${this.responseBase}/cancel/${responseId}`);
  }
}
