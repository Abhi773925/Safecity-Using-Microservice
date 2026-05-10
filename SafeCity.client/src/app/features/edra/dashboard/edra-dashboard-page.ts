import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { EdraApiService } from '../../../core/services/api/edra-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';

@Component({
  selector: 'app-edra-dashboard-page',
  standalone: true,
  imports: [Sidebar],
  templateUrl: './edra-dashboard-page.html',
  styleUrl: './edra-dashboard-page.css',
})
export class EdraDashboardPage implements OnInit {
  loading = false;
  error = '';
  pendingIncidents: any[] = [];
  availableResources: any[] = [];
  get incidents() { return this.pendingIncidents; }
  get resources() { return this.availableResources; }

  constructor(private api: EdraApiService) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  async loadDashboard(): Promise<void> {
    this.loading = true;
    this.error = '';
    try {
      const data = await this.api.getDashboard();
      this.pendingIncidents = data.incidentPending ?? [];
      this.availableResources = data.resources ?? [];
    } catch (err: any) {
      this.error = err?.response?.data?.message || 'Failed to load dashboard.';
    } finally {
      this.loading = false;
    }
  }
}
