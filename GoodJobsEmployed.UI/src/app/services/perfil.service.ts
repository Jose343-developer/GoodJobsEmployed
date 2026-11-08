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

  getById(idPerfil: number): Observable<ApiResponse<Perfil>> {
    return this.http.get<ApiResponse<Perfil>>(`${this.apiUrl}/GetPerfilById/${idPerfil}`);
  }

  update(idPerfil: number, perfil: { idPerfil: number; nombre: string; perfilCaracteristicas?: any[] }): Observable<ApiResponse<Perfil>> {
    return this.http.put<ApiResponse<Perfil>>(`${this.apiUrl}/UpdatePerfil/${idPerfil}`, perfil);
  }

  add(perfil: { nombre: string }): Observable<ApiResponse<Perfil>> {
    return this.http.post<ApiResponse<Perfil>>(`${this.apiUrl}/AddPerfil`, perfil);
  }

  delete(idPerfil: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/DeletePerfil/${idPerfil}`);
  }

  deleteSkill(idPerfil: number, idSkill: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/DeleteSkill/${idPerfil}/${idSkill}`);
  }
}
