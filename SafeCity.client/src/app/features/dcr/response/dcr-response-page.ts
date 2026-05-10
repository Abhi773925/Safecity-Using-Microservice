import { Component, OnInit } from '@angular/core';
import { DatePipe, NgClass, NgForOf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DcrApiService, Crisis, Team, Deployment } from '../../../core/services/api/dcr-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole, isTokenValid, getUserId } from '../../../shared/auth-utils';

@Component({
  selector: 'app-dcr-response-page',
  standalone: true,
  imports: [FormsModule, DatePipe, NgForOf, NgClass, Sidebar],
  templateUrl: './dcr-response-page.html',
  styleUrl: './dcr-response-page.css',
})
export class DcrResponsePage implements OnInit {
  canOperate = false;
  canCloseMission = false;
  canViewCrisis = false;
  canViewTeams = false;

  crises: Crisis[] = [];
  teams: Team[] = [];
  deployments: Deployment[] = [];
  loadingDeployments = false;
  submitting = false;
  message = '';
  messageType: 'success' | 'error' = 'success';

  searchText = '';
  crisisId = 0;
  teamId = 0;
  specialInstructions = 'Proceed with caution.';
  responseId = 0;
  newTeamId = 0;
  progressStatus = 1;
  progressNote = '';
  closeNote = '';

  readonly responseStatusOptions = [
    { value: 0, label: 'Pending' }, { value: 1, label: 'Active' },
    { value: 2, label: 'Stabilized' }, { value: 3, label: 'Resolved' },
    { value: 4, label: 'Closed' }, { value: 5, label: 'Cancelled' },
  ];

  constructor(private api: DcrApiService) { }

  ngOnInit(): void {
    const role = normalizeRole(getUserRole());
    this.canOperate = ['city_administrator', 'emergency_dispatcher', 'police', 'police_officer'].includes(role);
    this.canCloseMission = ['city_administrator', 'emergency_dispatcher'].includes(role);
    this.canViewCrisis = ['city_administrator', 'emergency_dispatcher'].includes(role);
    this.canViewTeams = role === 'city_administrator';
    this.refresh();
  }

  get filteredDeployments(): Deployment[] {
    const q = this.searchText.trim().toLowerCase();
    return this.deployments.filter(d =>
      !q || String(d.responseId).includes(q) || d.teamName.toLowerCase().includes(q) ||
      d.crisisLocation.toLowerCase().includes(q) || d.status.toLowerCase().includes(q)
    );
  }

  get selectedDeployment(): Deployment | null {
    return this.deployments.find(d => d.responseId === this.responseId) ?? null;
  }

  async refresh(): Promise<void> {
    if (!isTokenValid()) { this.showMessage('Please login again.', 'error'); return; }
    if (!this.canOperate) { this.deployments = []; return; }
    this.loadingDeployments = true;
    try {
      this.deployments = await this.api.getActiveDeployments();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to load deployments.', 'error');
    } finally {
      this.loadingDeployments = false;
    }
    await this.loadLookups();
  }

  async loadLookups(): Promise<void> {
    try {
      if (this.canViewCrisis) this.crises = await this.api.getActiveCrises();
      if (this.canViewTeams) this.teams = await this.api.getAvailableTeams();
    } catch { /* silent */ }
  }

  async deployTeam(): Promise<void> {
    if (!this.crisisId || !this.teamId) {
      this.showMessage('Crisis ID and Team ID required.', 'error');
      return;
    }
    this.submitting = true;

    const payload = {
      crisisId: this.crisisId,
      teamId: this.teamId,
      specialInstructions: this.specialInstructions || 'Proceed with caution.',
    };

    try {
      await this.api.deployTeam(payload);
      this.showMessage('Team deployed.', 'success');
      this.resetDeployForm();
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to deploy.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  async reassignTeam(): Promise<void> {
    if (!this.responseId || !this.newTeamId) {
      this.showMessage('Response ID and new team ID required.', 'error');
      return;
    }
    this.submitting = true;
    try {
      await this.api.reassignTeam(this.responseId, this.newTeamId);
      this.showMessage('Team reassigned.', 'success');
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to reassign.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  async updateProgress(): Promise<void> {
    if (!this.responseId) { this.showMessage('Response ID required.', 'error'); return; }

    const userId = getUserId();
    const dep = this.selectedDeployment;
    if (dep && userId !== null && dep.teamLead !== userId) {
      this.showMessage(`Only team lead (${dep.teamLead}) can update progress.`, 'error');
      return;
    }
    this.submitting = true;

    const payload = {
      newStatus: Number(this.progressStatus),
      updateNote: this.progressNote.trim() || 'Ground update.',
    };

    try {
      await this.api.updateProgress(this.responseId, payload);
      this.showMessage('Progress updated.', 'success');
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to update progress.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  async closeMission(): Promise<void> {
    if (!this.canCloseMission || !this.responseId) return;
    this.submitting = true;

    const payload = { finalClosingNote: this.closeNote.trim() || 'Mission completed.' };

    try {
      await this.api.closeMission(this.responseId, payload);
      this.showMessage('Mission closed.', 'success');
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to close mission.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  async cancelDeployment(id: number): Promise<void> {
    this.submitting = true;
    try {
      await this.api.cancelDeployment(id);
      this.showMessage('Deployment cancelled.', 'success');
      await this.refresh();
    } catch (err: any) {
      this.showMessage(err?.response?.data?.message || 'Failed to cancel.', 'error');
    } finally {
      this.submitting = false;
    }
  }

  selectDeployment(id: number): void { this.responseId = id; }
  resetDeployForm(): void { this.crisisId = 0; this.teamId = 0; this.specialInstructions = 'Proceed with caution.'; }

  statusClass(status: string): string {
    if (status === 'Active') return 'bg-emerald-100 text-emerald-700';
    if (status === 'Resolved' || status === 'Closed') return 'bg-slate-200 text-slate-700';
    if (status === 'Cancelled') return 'bg-red-100 text-red-700';
    return 'bg-amber-100 text-amber-700';
  }

  private showMessage(msg: string, type: 'success' | 'error'): void {
    this.message = msg;
    this.messageType = type;
  }
}
