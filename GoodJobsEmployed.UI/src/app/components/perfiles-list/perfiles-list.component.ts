import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PerfilService } from '../../services/perfil.service';
import { Perfil } from '../../models/perfil.model';

@Component({
  selector: 'app-perfiles-list',
  imports: [RouterLink],
  templateUrl: './perfiles-list.component.html',
  styleUrl: './perfiles-list.component.css'
})
export class PerfilesListComponent implements OnInit {
  private readonly perfilService = inject(PerfilService);

  readonly perfiles = signal<Perfil[]>([]);
  readonly isLoading = signal<boolean>(true);
  readonly errorMessage = signal<string | null>(null);

  readonly currentPage = signal<number>(1);
  readonly itemsPerPage = 10;

  readonly totalPages = computed(() => {
    const total = this.perfiles().length;
    return total > 0 ? Math.ceil(total / this.itemsPerPage) : 1;
  });

  readonly paginatedPerfiles = computed(() => {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    return this.perfiles().slice(start, start + this.itemsPerPage);
  });

  ngOnInit(): void {
    this.cargarPerfiles();
  }

  cargarPerfiles(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.perfilService.getAll().subscribe({
      next: (response) => {
        if (response.correct && response.objects) {
          this.perfiles.set(response.objects);
        } else {
          this.errorMessage.set(
            response.errorMessage || 'No se pudieron recuperar los perfiles del servidor.'
          );
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set(
          'No se pudo establecer conexión con el servidor (http://localhost:5047). Por favor verifica que la API esté iniciada y la configuración de red sea correcta.'
        );
        this.isLoading.set(false);
      }
    });
  }

  onSelectPerfil(perfil: Perfil): void {
    // Preparado para GetById en la siguiente fase
    console.log('Perfil seleccionado:', perfil.idPerfil, perfil.nombre);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update((p) => p - 1);
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update((p) => p + 1);
    }
  }
}
