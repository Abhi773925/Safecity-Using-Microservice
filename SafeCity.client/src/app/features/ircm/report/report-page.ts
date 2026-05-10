import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { IrcmApiService } from '../../../core/services/api/ircm-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';

@Component({
  selector: 'app-report-page',
  standalone: true,
  imports: [FormsModule, Sidebar],
  templateUrl: './report-page.html',
  styleUrl: './report-page.css',
})
export class ReportPage {
  type = 0;
  location = '';
  submitting = false;
  message = '';
  isError = false;

  typeOptions = [
    { value: 0, label: 'Crime' },
    { value: 1, label: 'Fire' },
    { value: 2, label: 'Accident' },
    { value: 3, label: 'Other' },
  ];

  constructor(private api: IrcmApiService, private router: Router) {}

  async submit(): Promise<void> {
    if (!this.location.trim()) {
      this.message = 'Location is required.';
      this.isError = true;
      return;
    }

    this.submitting = true;
    this.message = '';
    this.isError = false;

    const payload = {
      type: Number(this.type),
      location: this.location.trim(),
      date: new Date().toISOString(),
      status: 0,
      citizenID: 0,
    };

    try {
      const msg = await this.api.createIncident(payload);
      this.message = msg || 'Incident reported successfully.';
      this.type = 0;
      this.location = '';
      this.router.navigate(['/ircm/incidents']);
    } catch (err: any) {
      this.isError = true;
      this.message = err?.response?.data?.message || 'Failed to report incident.';
    } finally {
      this.submitting = false;
    }
  }
}
