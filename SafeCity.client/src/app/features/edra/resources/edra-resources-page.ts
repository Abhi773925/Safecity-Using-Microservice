import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EdraApiService, Resource } from '../../../core/services/api/edra-api.service';
import { Sidebar } from '../../../layout/sidebar/sidebar';
import { getUserRole, normalizeRole } from '../../../shared/auth-utils';

@Component({
  selector: 'app-edra-resources-page',
  standalone: true,
  imports: [FormsModule, Sidebar],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './edra-resources-page.html',
  styleUrl: './edra-resources-page.css',
})
export class EdraResourcesPage implements OnInit {
  dispatcherAccess = false;
  resources: Resource[] = [];
  filteredResources: Resource[] = [];
  loading = false;
  submitting = false;
  error = '';
  searchText = '';
  newResourceType = '';
  newResourceLocation = '';

  readonly typeOptions = ['Vehicle', 'Equipment', 'Personnel', 'Medical', 'Rescue'];
  readonly availabilityOptions = ['Available', 'OnTask', 'UnderMaintenance'];

  private readonly typeMap: Record<string, number> = {
    Vehicle: 0, Equipment: 1, Personnel: 2, Medical: 3, Rescue: 4,
  };
  private readonly availabilityMap: Record<string, number> = {
    Available: 0, OnTask: 1, UnderMaintenance: 2,
  };

  constructor(private api: EdraApiService) { }

  ngOnInit(): void {
    this.dispatcherAccess = ['emergency_dispatcher', 'city_administrator']
      .includes(normalizeRole(getUserRole()));
    this.loadResources();
  }

  async loadResources(): Promise<void> {
    this.loading = true;
    this.error = '';
    try {
      this.resources = await this.api.getResources();
    } catch (err: any) {
      this.error = err?.response?.data?.message || 'Failed to load resources.';
    } finally {
      this.loading = false;
    }
  }

  async addResource(): Promise<void> {
    const typeValue = this.typeMap[this.newResourceType];
    if (!this.newResourceType || typeValue === undefined || !this.newResourceLocation.trim()) {
      alert('Please select a resource type and enter a location.');
      return;
    }
    this.submitting = true;

    const payload = {
      type: typeValue,
      location: this.newResourceLocation.trim(),
      availability: 0,
      unitName: `${this.newResourceType}-${Date.now().toString().slice(-6)}`,
    };

    try {
      await this.api.addResource(payload);
      this.newResourceType = '';
      this.newResourceLocation = '';
      await this.loadResources();
    } catch (err: any) {
      alert(err?.response?.data?.message || 'Failed to add resource.');
    } finally {
      this.submitting = false;
    }
  }

  async updateResource(id: number, type: string, location: string, availability: string): Promise<void> {
    const typeValue = this.typeMap[type];
    const availValue = this.availabilityMap[availability];
    if (!id || typeValue === undefined || availValue === undefined || !location.trim()) return;

    this.submitting = true;

    const payload = {
      type: typeValue,
      availability: availValue,
      location: location.trim(),
      unitName: `${type}-${id}`,
    };

    try {
      await this.api.updateResource(id, payload);
      await this.loadResources();
    } catch (err: any) {
      alert(err?.response?.data?.message || 'Failed to update resource.');
    } finally {
      this.submitting = false;
    }
  }

  updateResourceEvent(event: any): void {
    this.updateResource(event.id, event.type, event.location, event.availability);
  }
}
