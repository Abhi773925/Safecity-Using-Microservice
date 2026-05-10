import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IrcmApiService, Incident } from '../../../core/services/api/ircm-api.service';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';
import { Sidebar } from '../../../layout/sidebar/sidebar';

@Component({
  selector: 'app-incidents-page',
  standalone: true,
  imports: [FormsModule, DatePipe, Sidebar],
  templateUrl: './incidents-page.html',
  styleUrl: './incidents-page.css',
})
export class IncidentsPage implements OnInit {
  incidents: Incident[] = [];
  search = '';
  loading = false;
  message = '';
  staffAccess = false;

  constructor(private api: IrcmApiService) { }

  ngOnInit(): void {
    this.staffAccess = ['police', 'police_officer', 'emergency_dispatcher', 'city_administrator', 'fire_fighter']
      .includes(normalizeRole(getUserRole()));
    this.load();
  }

  get filtered(): Incident[] {
    const q = this.search.trim().toLowerCase();
    return this.incidents.filter(i =>
      !q ||
      String(i.incidentID).includes(q) ||
      i.type.toLowerCase().includes(q) ||
      i.location.toLowerCase().includes(q)
    );
  }

  async load(): Promise<void> {
    this.loading = true;
    this.message = '';
    try {
      this.incidents = this.staffAccess
        ? await this.api.getAllIncidents()
        : await this.api.getMyIncidents();
    } catch (err: any) {
      this.message = err?.response?.data?.message || 'Failed to load incidents.';
    } finally {
      this.loading = false;
    }
  }

  async updateStatus(incidentID: number, option: number): Promise<void> {
    try {
      await this.api.updateIncidentStatus(incidentID, option);
      await this.load();
    } catch {
      this.message = 'Failed to update status.';
    }
  }
}
