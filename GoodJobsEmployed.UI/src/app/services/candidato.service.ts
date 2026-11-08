import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/perfil.model';
import { AnalisisCvResponse, CandidatoResultadoItem } from '../models/candidato.model';

@Injectable({
  providedIn: 'root'
})
export class CandidatoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5047/api/Candidato';

  // Signal compartido para almacenar los resultados del último análisis
  readonly rankingResultados = signal<CandidatoResultadoItem[]>([]);
  readonly perfilSeleccionadoNombre = signal<string>('');

  analizarCv(idPerfil: number, archivo: File): Observable<ApiResponse<AnalisisCvResponse>> {
    const formData = new FormData();
    // Proveer ambas claves comunes para máxima compatibilidad con el backend
    formData.append('cvFile', archivo, archivo.name);
    formData.append('cv', archivo, archivo.name);

    return this.http.post<ApiResponse<AnalisisCvResponse>>(
      `${this.apiUrl}/AnalizarCV/${idPerfil}`,
      formData
    );
  }

  setResultadosRanking(resultados: CandidatoResultadoItem[], nombrePerfil: string): void {
    // Ordenar de mayor compatibilidad a menor; los fallidos van al final
    const ordenados = [...resultados].sort((a, b) => {
      if (!a.exito && b.exito) return 1;
      if (a.exito && !b.exito) return -1;
      const compA = a.resultado?.porcentajeCompatibilidad || 0;
      const compB = b.resultado?.porcentajeCompatibilidad || 0;
      return compB - compA;
    });

    this.rankingResultados.set(ordenados);
    this.perfilSeleccionadoNombre.set(nombrePerfil);
  }

  limpiarResultados(): void {
    this.rankingResultados.set([]);
    this.perfilSeleccionadoNombre.set('');
  }
}
