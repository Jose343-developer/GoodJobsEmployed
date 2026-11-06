import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { PerfilesListComponent } from './components/perfiles-list/perfiles-list.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'perfiles', component: PerfilesListComponent },
  { path: '**', redirectTo: '' }
];
