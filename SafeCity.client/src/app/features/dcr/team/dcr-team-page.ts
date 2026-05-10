import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgForOf } from '@angular/common';
import { DcrApiService, Team } from '../../../core/services/api/dcr-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole, isTokenValid } from '../../../shared/auth-utils';

@Component({
  selector: 'app-dcr-team-page',
  standalone: true,
  imports: [FormsModule, NgForOf, Sidebar],
  templateUrl: './dcr-team-page.html',
  styleUrl: './dcr-team-page.css',
})
export class DcrTeamPage implements OnInit {
  canManage = false;
  teams: Team[] = [];
  availableTeams: Team[] = [];
  loading = false;
  submitting = false;
  message = '';
  messageType: 'success' | 'error' = 'success';
  searchText = '';
  teamName = '';
  teamLeadID = 0;
  status = 'Active';
  teamStatusMap: Record<number, string> = {};

  readonly statusOptions = ['Active', 'Inactive'];

  constructor(private api: DcrApiService) { }

  ngOnInit(): void {
    this.canManage = normalizeRole(getUserRole()) === 'city_administrator';
    this.refresh();
  }

  get filteredTeams(): Team[] {
    const q = this.searchText.trim().toLowerCase();
    return this.teams.filter(t =>
      !q || t.teamName.toLowerCase().includes(q) || String(t.teamID).includes(q) || t.status.toLowerCase().includes(q)
    );
  }

  async refresh(): Promise<void> {
    if (!isTokenValid()) { this.showMessage('Please login again.', 'error'); return; }
    if (!this.canManage) { this.teams = []; this.availableTeams = []; return; }
    this.loading = true;
    try {
      const allTeams = await this.api.getAllTeams();
      const availableTeams = await this.api.getAvailableTeams();
      this.teams = allTeams;
      this.availableTeams = availableTeams;
      this.teamStatusMap = Object.fromEntries(allTeams.map(t => [t.teamID, t.status]));
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to load teams.', 'error');
      this.teams = [];
      this.availableTeams = [];
    } finally {
      this.loading = false;
    }
  }

  async createTeam(): Promise<void> {
    if (!this.canManage) return;
    if (!this.teamName.trim() || !this.teamLeadID) {
      this.showMessage('Team name and lead ID required.', 'error');
      return;
    }
    this.submitting = true;

    const payload = {
      teamName: this.teamName.trim(),
      teamLeadID: Number(this.teamLeadID),
      status: this.status === 'Inactive' ? 1 : 0,
    };

    try {
      await this.api.createTeam(payload);
      this.showMessage('Team created.', 'success');
      this.resetForm();
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to create team.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  async updateTeamStatus(teamID: number, status: string): Promise<void> {
    if (!this.canManage || !teamID) return;
    this.submitting = true;

    const payload = { newStatus: status === 'Inactive' ? 1 : 0 };

    try {
      await this.api.updateTeamStatus(teamID, payload);
      this.showMessage('Team status updated.', 'success');
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to update team.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  resetForm(): void { this.teamName = ''; this.teamLeadID = 0; this.status = 'Active'; }

  private showMessage(msg: string, type: 'success' | 'error'): void {
    this.message = msg;
    this.messageType = type;
  }
}
