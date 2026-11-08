import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CandidatoService } from '../../services/candidato.service';
import { CandidatoResultadoItem } from '../../models/candidato.model';

@Component({
  selector: 'app-cv-ranking',
  imports: [RouterLink],
  templateUrl: './cv-ranking.component.html',
  styleUrl: './cv-ranking.component.css'
})
export class CvRankingComponent {
  private readonly candidatoService = inject(CandidatoService);
  private readonly router = inject(Router);

  readonly rankingResultados = this.candidatoService.rankingResultados;
  readonly perfilNombre = this.candidatoService.perfilSeleccionadoNombre;

  // Control de tarjetas desplegadas para ver detalles
  readonly expandedItems = signal<Set<string>>(new Set());

  toggleDetalle(id: string): void {
    this.expandedItems.update((set) => {
      const nuevo = new Set(set);
      if (nuevo.has(id)) {
        nuevo.delete(id);
      } else {
        nuevo.add(id);
      }
      return nuevo;
    });
  }

  isExpanded(id: string): boolean {
    return this.expandedItems().has(id);
  }

  getGaugeColor(porcentaje: number | undefined): string {
    const p = porcentaje || 0;
    if (p >= 70) return '#29b765'; // Verde
    if (p >= 40) return '#ff9500'; // Naranja
    return '#dc3545';             // Rojo
  }

  getDashOffset(porcentaje: number | undefined): number {
    const p = Math.max(0, Math.min(100, porcentaje || 0));
    const maxLen = 125.66;
    return maxLen - (maxLen * p) / 100;
  }

  getCandidatosExitosos(): CandidatoResultadoItem[] {
    return this.rankingResultados().filter((c) => c.exito);
  }

  getCandidatosConError(): CandidatoResultadoItem[] {
    return this.rankingResultados().filter((c) => !c.exito);
  }

  getPromedioCompatibilidad(): number {
    const exitosos = this.getCandidatosExitosos();
    if (exitosos.length === 0) return 0;
    const suma = exitosos.reduce(
      (acc, curr) => acc + (curr.resultado?.porcentajeCompatibilidad || 0),
      0
    );
    return Math.round((suma / exitosos.length) * 10) / 10;
  }

  onVolverASubir(): void {
    this.router.navigate(['/subir-cvs']);
  }
}
