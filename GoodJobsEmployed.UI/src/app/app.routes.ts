import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { PerfilesListComponent } from './components/perfiles-list/perfiles-list.component';
import { PerfilDetalleComponent } from './components/perfil-detalle/perfil-detalle.component';
import { CvUploadComponent } from './components/cv-upload/cv-upload.component';
import { CvRankingComponent } from './components/cv-ranking/cv-ranking.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'perfiles', component: PerfilesListComponent },
  { path: 'perfiles/:id', component: PerfilDetalleComponent },
  { path: 'subir-cvs', component: CvUploadComponent },
  { path: 'ranking', component: CvRankingComponent },
  { path: '**', redirectTo: '' }
];
