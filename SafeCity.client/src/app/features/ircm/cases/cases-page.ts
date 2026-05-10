import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IrcmApiService, Case } from '../../../core/services/api/ircm-api.service';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';
import { Sidebar } from '../../../layout/sidebar/sidebar';

@Component({
  selector: 'app-cases-page',
  standalone: true,
  imports: [FormsModule, DatePipe, Sidebar],
  templateUrl: './cases-page.html',
  styleUrl: './cases-page.css',
})
export class CasesPage implements OnInit {
  cases: Case[] = [];
  loading = false;
  submitting = false;
  closing = false;
  message = '';
  isError = false;
  search = '';
  staffAccess = false;

  incidentID = 0;
  description = '';
  caseID = 0;
  closeNote = '';

  constructor(private api: IrcmApiService) { }

  ngOnInit(): void {
    this.staffAccess = ['police', 'police_officer', 'emergency_dispatcher', 'city_administrator', 'fire_fighter']
      .includes(normalizeRole(getUserRole()));
    this.load();
  }

  get filtered(): Case[] {
    const q = this.search.trim().toLowerCase();
    return this.cases.filter(c =>
      !q ||
      String(c.caseID).includes(q) ||
      String(c.incidentID).includes(q) ||
      c.description.toLowerCase().includes(q)
    );
  }

  async load(): Promise<void> {
    this.loading = true;
    this.message = '';
    try {
      this.cases = this.staffAccess
        ? await this.api.getAllCases()
        : await this.api.getMyCases();
    } catch (err: any) {
      this.isError = true;
      this.message = err?.response?.data?.message || 'Failed to load cases.';
    } finally {
      this.loading = false;
    }
  }

  async createCase(): Promise<void> {
    if (!this.incidentID || !this.description.trim()) {
      this.message = 'Incident ID and description are required.';
      this.isError = true;
      return;
    }
    this.submitting = true;
    this.message = '';
    this.isError = false;

    const payload = {
      incidentID: this.incidentID,
      description: this.description.trim(),
      status: 0,
      resolutionDate: new Date().toISOString(),
    };

    try {
      await this.api.createCase(this.incidentID, payload);
      this.incidentID = 0;
      this.description = '';
      await this.load();
    } catch (err: any) {
      this.isError = true;
      this.message = err?.response?.data?.message || 'Failed to create case.';
    } finally {
      this.submitting = false;
    }
  }

  async closeCase(): Promise<void> {
    if (!this.caseID || !this.closeNote.trim()) {
      this.message = 'Case ID and closing note are required.';
      this.isError = true;
      return;
    }
    this.closing = true;
    this.message = '';
    this.isError = false;

    const payload = {
      description: this.closeNote.trim(),
      status: 1,
      resolutionDate: new Date().toISOString(),
    };

    try {
      await this.api.closeCase(this.caseID, payload);
      this.caseID = 0;
      this.closeNote = '';
      await this.load();
    } catch (err: any) {
      this.isError = true;
      this.message = err?.response?.data?.message || 'Failed to close case.';
    } finally {
      this.closing = false;
    }
  }
}
