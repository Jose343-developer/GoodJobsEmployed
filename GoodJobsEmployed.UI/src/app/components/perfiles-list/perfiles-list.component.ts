import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PerfilService } from '../../services/perfil.service';
import { Perfil } from '../../models/perfil.model';

@Component({
  selector: 'app-perfiles-list',
  imports: [RouterLink, FormsModule],
  templateUrl: './perfiles-list.component.html',
  styleUrl: './perfiles-list.component.css'
})
export class PerfilesListComponent implements OnInit {
  private readonly perfilService = inject(PerfilService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly perfiles = signal<Perfil[]>([]);
  readonly isLoading = signal<boolean>(true);
  readonly errorMessage = signal<string | null>(null);

  // Estado para el modal de edición de perfil
  readonly isEditModalOpen = signal<boolean>(false);
  readonly perfilToEdit = signal<Perfil | null>(null);
  readonly editNombre = signal<string>('');
  readonly isSavingEdit = signal<boolean>(false);
  readonly editErrorMessage = signal<string | null>(null);

  // Estado para el modal de creación de nuevo perfil
  readonly isAddModalOpen = signal<boolean>(false);
  readonly newPerfilNombre = signal<string>('');
  readonly isSavingAdd = signal<boolean>(false);
  readonly addErrorMessage = signal<string | null>(null);

  // Estado para el modal de eliminación de perfil
  readonly isDeleteModalOpen = signal<boolean>(false);
  readonly perfilToDelete = signal<Perfil | null>(null);
  readonly isDeleting = signal<boolean>(false);
  readonly deleteErrorMessage = signal<string | null>(null);

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

    if (this.route.snapshot.queryParamMap.get('crear') === 'true') {
      this.onOpenAddModal();
    }
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
    this.router.navigate(['/perfiles', perfil.idPerfil]);
  }

  onEditPerfil(event: Event, perfil: Perfil): void {
    event.stopPropagation();
    this.perfilToEdit.set(perfil);
    this.editNombre.set(perfil.nombre);
    this.editErrorMessage.set(null);
    this.isEditModalOpen.set(true);
  }

  onCloseEditModal(): void {
    if (this.isSavingEdit()) return;
    this.isEditModalOpen.set(false);
    this.perfilToEdit.set(null);
    this.editErrorMessage.set(null);
  }

  onGuardarEditPerfil(): void {
    const perfilActual = this.perfilToEdit();
    const nuevoNombre = this.editNombre().trim();

    if (!perfilActual) return;

    if (!nuevoNombre) {
      this.editErrorMessage.set('El nombre del perfil no puede estar vacío.');
      return;
    }

    if (nuevoNombre.length > 50) {
      this.editErrorMessage.set('El nombre del perfil no puede exceder 50 caracteres.');
      return;
    }

    this.isSavingEdit.set(true);
    this.editErrorMessage.set(null);

    this.perfilService.update(perfilActual.idPerfil, {
      idPerfil: perfilActual.idPerfil,
      nombre: nuevoNombre
    }).subscribe({
      next: (response) => {
        if (response.correct) {
          // Actualización reactiva local del nombre del perfil
          this.perfiles.update((list) =>
            list.map((p) =>
              p.idPerfil === perfilActual.idPerfil ? { ...p, nombre: nuevoNombre } : p
            )
          );
          this.isEditModalOpen.set(false);
          this.perfilToEdit.set(null);
        } else {
          this.editErrorMessage.set(
            response.errorMessage || 'No se pudo actualizar el nombre del perfil.'
          );
        }
        this.isSavingEdit.set(false);
      },
      error: () => {
        this.editErrorMessage.set(
          'Error al conectar con la API para actualizar el perfil.'
        );
        this.isSavingEdit.set(false);
      }
    });
  }

  onOpenAddModal(): void {
    this.newPerfilNombre.set('');
    this.addErrorMessage.set(null);
    this.isAddModalOpen.set(true);
  }

  onCloseAddModal(): void {
    if (this.isSavingAdd()) return;
    this.isAddModalOpen.set(false);
    this.addErrorMessage.set(null);
  }

  onGuardarNuevoPerfil(): void {
    const nombre = this.newPerfilNombre().trim();

    if (!nombre) {
      this.addErrorMessage.set('El nombre del perfil es requerido.');
      return;
    }

    if (nombre.length > 50) {
      this.addErrorMessage.set('El nombre del perfil no puede exceder 50 caracteres.');
      return;
    }

    this.isSavingAdd.set(true);
    this.addErrorMessage.set(null);

    this.perfilService.add({ nombre }).subscribe({
      next: (response) => {
        if (response.correct) {
          this.isAddModalOpen.set(false);
          this.newPerfilNombre.set('');
          this.cargarPerfiles();
        } else {
          this.addErrorMessage.set(
            response.errorMessage || 'No se pudo crear el nuevo perfil.'
          );
        }
        this.isSavingAdd.set(false);
      },
      error: () => {
        this.addErrorMessage.set(
          'Error al conectar con la API para registrar el perfil.'
        );
        this.isSavingAdd.set(false);
      }
    });
  }

  onAgregarPerfil(): void {
    this.onOpenAddModal();
  }

  onDeletePerfil(event: Event, perfil: Perfil): void {
    event.stopPropagation();
    this.perfilToDelete.set(perfil);
    this.deleteErrorMessage.set(null);
    this.isDeleteModalOpen.set(true);
  }

  onCloseDeleteModal(): void {
    if (this.isDeleting()) return;
    this.isDeleteModalOpen.set(false);
    this.perfilToDelete.set(null);
    this.deleteErrorMessage.set(null);
  }

  onConfirmDelete(): void {
    const perfil = this.perfilToDelete();
    if (!perfil) return;

    this.isDeleting.set(true);
    this.deleteErrorMessage.set(null);

    this.perfilService.delete(perfil.idPerfil).subscribe({
      next: (response) => {
        if (response?.correct !== false) {
          this.perfiles.update((list) => list.filter((p) => p.idPerfil !== perfil.idPerfil));
          this.isDeleteModalOpen.set(false);
          this.perfilToDelete.set(null);
        } else {
          this.deleteErrorMessage.set(response?.errorMessage || 'No se pudo eliminar el perfil.');
        }
        this.isDeleting.set(false);
      },
      error: (err) => {
        const errorMsg =
          err?.error?.errorMessage ||
          'No se pudo eliminar el perfil. Compruebe si tiene dependencias asociadas.';
        this.deleteErrorMessage.set(errorMsg);
        this.isDeleting.set(false);
      }
    });
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
