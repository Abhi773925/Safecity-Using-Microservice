import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { setupAxiosInterceptors } from './app/core/interceptors/axios.interceptor';

// Attach auth token to every axios request automatically
setupAxiosInterceptors();

bootstrapApplication(App, appConfig).catch((err) => console.error(err));
