import { Injectable } from '@angular/core';
import axios from 'axios';
import { API_BASE_URL } from '../../../shared/constants';

export interface Resource {
  resourceID: number;
  type: string;
  availability: string;
  location: string;
  unitName: string;
}

export interface Dispatch {
  dispatchID: number;
  resourceID: number;
  incidentID: number;
  dispatcherID: number;
  status: string;
}

export interface DispatcherDashboard {
  incidentPending: Array<{ incidentID: number; type: string; location: string; status: string }>;
  resources: Resource[];
}

@Injectable({ providedIn: 'root' })
export class EdraApiService {
  private resourceBase = `${API_BASE_URL}/resources`;
  private dispatchBase = `${API_BASE_URL}/dispatcher`;

  async getDashboard(): Promise<DispatcherDashboard> {
    const res = await axios.get<DispatcherDashboard>(`${this.dispatchBase}/dashboard`);
    return res.data;
  }

  async getResources(): Promise<Resource[]> {
    const res = await axios.get<{ data: Resource[] }>(`${this.resourceBase}/list`);
    return res.data.data ?? [];
  }

  async addResource(payload: { type: number; location: string; availability: number; unitName: string }) {
    await axios.post(`${this.resourceBase}/add`, payload);
  }

  async updateResource(id: number, payload: { type: number; availability: number; location: string; unitName: string }) {
    await axios.put(`${this.resourceBase}/update/${id}`, payload);
  }

  async getDispatches(): Promise<Dispatch[]> {
    const res = await axios.get<{ data: Dispatch[] }>(`${this.dispatchBase}/list`);
    return res.data.data ?? [];
  }

  async dispatchResource(payload: { resourceID: number; incidentID: number }) {
    await axios.post(`${this.dispatchBase}/dispatch`, payload);
  }

  async assignResource(payload: { resourceID: number; incidentID: number }) {
    await axios.post(`${this.dispatchBase}/assign-resource`, payload);
  }

  async updateDispatchStatus(dispatchId: number, payload: { status: number }) {
    await axios.patch(`${this.dispatchBase}/update-status/${dispatchId}`, payload);
  }

  async completeDispatch(dispatchId: number) {
    await axios.post(`${this.dispatchBase}/complete-dispatch/${dispatchId}`);
  }
}
