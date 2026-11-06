import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, Perfil } from '../models/perfil.model';

@Injectable({
  providedIn: 'root'
})
export class PerfilService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5047/api/Perfil';

  getAll(): Observable<ApiResponse<Perfil>> {
    return this.http.get<ApiResponse<Perfil>>(`${this.apiUrl}/GetAll`);
  }
}
