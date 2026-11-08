import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { PerfilService } from '../../services/perfil.service';
import { CandidatoService } from '../../services/candidato.service';
import { Perfil } from '../../models/perfil.model';
import { CandidatoResultadoItem } from '../../models/candidato.model';

export interface CvArchivoItem {
  id: string;
  archivo: File;
  nombre: string;
  tamanoFormateado: string;
  extension: string;
}

@Component({
  selector: 'app-cv-upload',
  imports: [RouterLink, FormsModule],
  templateUrl: './cv-upload.component.html',
  styleUrl: './cv-upload.component.css'
})
export class CvUploadComponent implements OnInit {
  private readonly perfilService = inject(PerfilService);
  private readonly candidatoService = inject(CandidatoService);
  private readonly router = inject(Router);

  readonly archivosCargados = signal<CvArchivoItem[]>([]);
  readonly isDragOver = signal<boolean>(false);
  readonly advertenciaFormato = signal<string | null>(null);

  // Perfiles disponibles y selección
  readonly perfiles = signal<Perfil[]>([]);
  readonly isLoadingPerfiles = signal<boolean>(false);
  readonly selectedPerfilId = signal<number | null>(null);

  // Perfiles filtrados: Únicamente aquellos que SÍ tienen al menos 1 skill válida
  readonly perfilesConSkills = computed(() => {
    return this.perfiles().filter((p) => {
      if (!p.caracteristicas || p.caracteristicas.length === 0) return false;
      return p.caracteristicas.some((c) => c.idCaracteristica != null);
    });
  });

  getSkillsCount(perfil: Perfil): number {
    if (!perfil.caracteristicas) return 0;
    return perfil.caracteristicas.filter((c) => c.idCaracteristica != null).length;
  }

  // Estado del proceso de análisis secuencial
  readonly isAnalizando = signal<boolean>(false);
  readonly progresoActual = signal<number>(0);
  readonly progresoTotal = signal<number>(0);
  readonly archivoEnProceso = signal<string>('');
  readonly analisisErrorGlobal = signal<string | null>(null);

  private readonly extensionesValidas = ['.pdf', '.docx'];

  ngOnInit(): void {
    this.cargarPerfiles();
  }

  cargarPerfiles(): void {
    this.isLoadingPerfiles.set(true);
    this.perfilService.getAll().subscribe({
      next: (response) => {
        if (response.correct && response.objects) {
          this.perfiles.set(response.objects);
        }
        this.isLoadingPerfiles.set(false);
      },
      error: () => {
        this.isLoadingPerfiles.set(false);
      }
    });
  }

  onDragOver(event: DragEvent): void {
    if (this.isAnalizando()) return;
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(true);
  }

  onDragLeave(event: DragEvent): void {
    if (this.isAnalizando()) return;
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(false);
  }

  onDrop(event: DragEvent): void {
    if (this.isAnalizando()) return;
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver.set(false);

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.procesarArchivos(Array.from(event.dataTransfer.files));
    }
  }

  onFileSelected(event: Event): void {
    if (this.isAnalizando()) return;
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.procesarArchivos(Array.from(input.files));
      input.value = '';
    }
  }

  private procesarArchivos(archivos: File[]): void {
    this.advertenciaFormato.set(null);
    let archivosRechazados = 0;
    const nuevosItems: CvArchivoItem[] = [];

    for (const file of archivos) {
      const nombreLower = file.name.toLowerCase();
      const esValido = this.extensionesValidas.some((ext) => nombreLower.endsWith(ext));

      if (esValido) {
        const extension = nombreLower.endsWith('.pdf') ? 'PDF' : 'DOCX';
        nuevosItems.push({
          id: `${Date.now()}-${Math.random().toString(36).substring(2, 9)}`,
          archivo: file,
          nombre: file.name,
          tamanoFormateado: this.formatFileSize(file.size),
          extension
        });
      } else {
        archivosRechazados++;
      }
    }

    if (nuevosItems.length > 0) {
      this.archivosCargados.update((prev) => [...prev, ...nuevosItems]);
    }

    if (archivosRechazados > 0) {
      this.advertenciaFormato.set(
        'Solo se admiten documentos en formato PDF (.pdf) y Word (.docx). Algunos archivos no compatibles fueron omitidos.'
      );
    }
  }

  removerArchivo(id: string): void {
    if (this.isAnalizando()) return;
    this.archivosCargados.update((prev) => prev.filter((item) => item.id !== id));
  }

  limpiarTodos(): void {
    if (this.isAnalizando()) return;
    this.archivosCargados.set([]);
    this.advertenciaFormato.set(null);
  }

  async onIniciarAnalisis(): Promise<void> {
    const perfilId = this.selectedPerfilId();
    const archivos = this.archivosCargados();

    if (!perfilId || archivos.length === 0 || this.isAnalizando()) {
      return;
    }

    this.isAnalizando.set(true);
    this.analisisErrorGlobal.set(null);
    this.progresoTotal.set(archivos.length);
    this.progresoActual.set(0);

    const resultados: CandidatoResultadoItem[] = [];

    // Procesamiento secuencial estricto (uno a uno)
    for (let i = 0; i < archivos.length; i++) {
      const item = archivos[i];
      this.progresoActual.set(i + 1);
      this.archivoEnProceso.set(item.nombre);

      try {
        const respuesta = await firstValueFrom(
          this.candidatoService.analizarCv(perfilId, item.archivo)
        );

        if (respuesta && respuesta.correct && respuesta.object) {
          resultados.push({
            id: item.id,
            nombreArchivo: item.nombre,
            tamanoFormateado: item.tamanoFormateado,
            exito: true,
            resultado: respuesta.object
          });
        } else {
          resultados.push({
            id: item.id,
            nombreArchivo: item.nombre,
            tamanoFormateado: item.tamanoFormateado,
            exito: false,
            mensajeError: respuesta?.errorMessage || 'La API devolvió un resultado no satisfactorio.'
          });
        }
      } catch (error: any) {
        const errorDesc =
          error?.error?.errorMessage ||
          error?.message ||
          'No se pudo procesar el dato: error con la API o con la base de datos.';

        resultados.push({
          id: item.id,
          nombreArchivo: item.nombre,
          tamanoFormateado: item.tamanoFormateado,
          exito: false,
          mensajeError: errorDesc
        });
      }
    }

    // Redirección al ranking con los resultados guardados en el servicio
    const perfilNombre =
      this.perfiles().find((p) => p.idPerfil === perfilId)?.nombre || `Perfil #${perfilId}`;

    this.candidatoService.setResultadosRanking(resultados, perfilNombre);
    this.isAnalizando.set(false);
    this.router.navigate(['/ranking']);
  }

  getProgresoPorcentaje(): number {
    const total = this.progresoTotal();
    if (total === 0) return 0;
    return Math.round((this.progresoActual() / total) * 100);
  }

  private formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 KB';
    if (bytes < 1048576) {
      return (bytes / 1024).toFixed(2) + ' KB';
    }
    return (bytes / 1048576).toFixed(2) + ' MB';
  }
}
