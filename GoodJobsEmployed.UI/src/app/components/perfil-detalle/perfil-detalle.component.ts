import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PerfilService } from '../../services/perfil.service';
import { Caracteristica, Perfil } from '../../models/perfil.model';

@Component({
  selector: 'app-perfil-detalle',
  imports: [RouterLink, FormsModule],
  templateUrl: './perfil-detalle.component.html',
  styleUrl: './perfil-detalle.component.css'
})
export class PerfilDetalleComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly perfilService = inject(PerfilService);

  readonly perfil = signal<Perfil | null>(null);
  readonly isLoading = signal<boolean>(true);
  readonly errorMessage = signal<string | null>(null);
  readonly isPerfilVacio = signal<boolean>(false);
  readonly selectedCaracteristica = signal<Caracteristica | null>(null);

  // Estado para el modal de agregar skill / característica
  readonly isAddSkillModalOpen = signal<boolean>(false);
  readonly skillNombre = signal<string>('');
  readonly skillPeso = signal<number | string>(50);
  readonly skillDescripcion = signal<string>('');
  readonly skillCriterio = signal<string>('');
  readonly isSavingSkill = signal<boolean>(false);
  readonly skillErrorMessage = signal<string | null>(null);

  // Estado para el modo de edición interactiva de skill seleccionada
  readonly isEditingSelectedSkill = signal<boolean>(false);
  readonly editSkillNombre = signal<string>('');
  readonly editSkillPeso = signal<number | string>(50);
  readonly editSkillDescripcion = signal<string>('');
  readonly editSkillCriterio = signal<string>('');
  readonly isSavingEditSkill = signal<boolean>(false);
  readonly editSkillErrorMessage = signal<string | null>(null);

  // Estado para alerta modal estilo Apple ante cambios sin guardar
  readonly isConfirmExitModalOpen = signal<boolean>(false);
  private pendingSkillToSelect: Caracteristica | null = null;

  // Estado para modal de confirmación de eliminación de skill
  readonly isDeleteSkillModalOpen = signal<boolean>(false);
  readonly skillToDelete = signal<Caracteristica | null>(null);
  readonly isDeletingSkill = signal<boolean>(false);
  readonly deleteSkillErrorMessage = signal<string | null>(null);

  private perfilId: number | null = null;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.perfilId = Number(idParam);
      this.cargarPerfil();
    } else {
      this.errorMessage.set('Identificador de perfil no válido.');
      this.isLoading.set(false);
    }
  }

  cargarPerfil(): void {
    if (!this.perfilId) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.isPerfilVacio.set(false);
    this.selectedCaracteristica.set(null);
    this.isEditingSelectedSkill.set(false);

    this.perfilService.getById(this.perfilId).subscribe({
      next: (response) => {
        if (response.correct) {
          const tieneObjetos =
            (response.objects && response.objects.length > 0) || !!response.object;

          if (tieneObjetos) {
            const perfilEncontrado = response.objects ? response.objects[0] : response.object!;
            this.perfil.set(perfilEncontrado);
            this.isPerfilVacio.set(
              !perfilEncontrado.caracteristicas || perfilEncontrado.caracteristicas.length === 0
            );
          } else {
            // Fallback de resiliencia: mostrar siempre Perfil #[id] con contador en 0
            this.perfil.set({
              idPerfil: this.perfilId!,
              nombre: `Perfil #${this.perfilId}`,
              caracteristicas: []
            });
            this.isPerfilVacio.set(true);
          }
        } else {
          this.errorMessage.set(
            response.errorMessage || 'No se encontró la información del perfil solicitado.'
          );
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set(
          'No se pudo conectar con el servidor para obtener los detalles del perfil.'
        );
        this.isLoading.set(false);
      }
    });
  }

  hasUnsavedChanges(): boolean {
    const sel = this.selectedCaracteristica();
    if (!sel || !this.isEditingSelectedSkill()) return false;

    const nombreModificado = this.editSkillNombre().trim() !== sel.nombreCaracteristica;
    const pesoModificado = Number(this.editSkillPeso()) !== sel.pesoPrioridad;
    const descModificada =
      this.editSkillDescripcion().trim() !== (sel.descripcionCaracteristica || '');
    const critModificado =
      this.editSkillCriterio().trim() !== (sel.criterioEspecifico || '');

    return nombreModificado || pesoModificado || descModificada || critModificado;
  }

  onSelectCaracteristica(caracteristica: Caracteristica): void {
    if (this.selectedCaracteristica()?.idCaracteristica === caracteristica.idCaracteristica) {
      this.handleCloseAttempt();
      return;
    }

    if (this.isEditingSelectedSkill() && this.hasUnsavedChanges()) {
      this.pendingSkillToSelect = caracteristica;
      this.isConfirmExitModalOpen.set(true);
      return;
    }

    this.isEditingSelectedSkill.set(false);
    this.selectedCaracteristica.set(caracteristica);
  }

  onCloseDetalle(): void {
    this.handleCloseAttempt();
  }

  private handleCloseAttempt(): void {
    if (this.isEditingSelectedSkill() && this.hasUnsavedChanges()) {
      this.pendingSkillToSelect = null;
      this.isConfirmExitModalOpen.set(true);
      return;
    }

    this.isEditingSelectedSkill.set(false);
    this.selectedCaracteristica.set(null);
  }

  onStartEditSkill(): void {
    const sel = this.selectedCaracteristica();
    if (!sel) return;

    this.editSkillNombre.set(sel.nombreCaracteristica);
    this.editSkillPeso.set(sel.pesoPrioridad);
    this.editSkillDescripcion.set(sel.descripcionCaracteristica || '');
    this.editSkillCriterio.set(sel.criterioEspecifico || '');
    this.editSkillErrorMessage.set(null);
    this.isEditingSelectedSkill.set(true);
  }

  onCancelEditSkill(): void {
    if (this.hasUnsavedChanges()) {
      this.pendingSkillToSelect = null;
      this.isConfirmExitModalOpen.set(true);
    } else {
      this.isEditingSelectedSkill.set(false);
    }
  }

  // Acciones de la alerta modal estilo Apple ("¿Desea guardar o no guardar?")
  onConfirmModalGuardar(): void {
    this.onGuardarEditSkill(true);
  }

  onConfirmModalNoGuardar(): void {
    this.isConfirmExitModalOpen.set(false);
    this.isEditingSelectedSkill.set(false);

    if (this.pendingSkillToSelect) {
      const nextSkill = this.pendingSkillToSelect;
      this.pendingSkillToSelect = null;
      this.selectedCaracteristica.set(nextSkill);
    } else {
      this.selectedCaracteristica.set(null);
    }
  }

  onConfirmModalCancelar(): void {
    this.isConfirmExitModalOpen.set(false);
    this.pendingSkillToSelect = null;
  }

  onGuardarEditSkill(fromExitModal: boolean = false): void {
    if (!this.perfilId) return;
    const sel = this.selectedCaracteristica();
    if (!sel) return;

    const nombre = this.editSkillNombre().trim();
    const pesoNum = Number(this.editSkillPeso());
    const desc = this.editSkillDescripcion().trim();
    const crit = this.editSkillCriterio().trim();

    if (!nombre) {
      this.editSkillErrorMessage.set('El nombre de la skill es requerido.');
      if (fromExitModal) this.isConfirmExitModalOpen.set(false);
      return;
    }

    if (nombre.length > 50) {
      this.editSkillErrorMessage.set('El nombre no puede exceder 50 caracteres.');
      if (fromExitModal) this.isConfirmExitModalOpen.set(false);
      return;
    }

    if (isNaN(pesoNum) || !Number.isInteger(pesoNum) || pesoNum < 1 || pesoNum > 100) {
      this.editSkillErrorMessage.set(
        'El puntaje debe ser un número entero entre 1 y 100 (no se permiten decimales).'
      );
      if (fromExitModal) this.isConfirmExitModalOpen.set(false);
      return;
    }

    if (desc.length > 250) {
      this.editSkillErrorMessage.set('La descripción no puede exceder 250 caracteres.');
      if (fromExitModal) this.isConfirmExitModalOpen.set(false);
      return;
    }

    if (crit.length > 250) {
      this.editSkillErrorMessage.set('El criterio no puede exceder 250 caracteres.');
      if (fromExitModal) this.isConfirmExitModalOpen.set(false);
      return;
    }

    this.isSavingEditSkill.set(true);
    this.editSkillErrorMessage.set(null);

    const payload = {
      idPerfil: this.perfilId,
      nombre: this.perfil()?.nombre || '',
      perfilCaracteristicas: [
        {
          idPerfil: this.perfilId,
          idCaracteristica: sel.idCaracteristica,
          pesoPrioridad: pesoNum,
          criterioEspecifico: crit,
          caracteristica: {
            idCaracteristica: sel.idCaracteristica,
            nombreCaracteristica: nombre,
            descripcionCaracteristica: desc,
            perfilCaracteristicas: []
          }
        }
      ]
    };

    this.perfilService.update(this.perfilId, payload).subscribe({
      next: (response) => {
        if (response.correct) {
          const updatedSkill: Caracteristica = {
            idCaracteristica: sel.idCaracteristica,
            nombreCaracteristica: nombre,
            descripcionCaracteristica: desc,
            pesoPrioridad: pesoNum,
            criterioEspecifico: crit
          };

          this.perfil.update((p) => {
            if (!p || !p.caracteristicas) return p;
            return {
              ...p,
              caracteristicas: p.caracteristicas.map((c) =>
                c.idCaracteristica === sel.idCaracteristica ? updatedSkill : c
              )
            };
          });

          this.selectedCaracteristica.set(updatedSkill);
          this.isEditingSelectedSkill.set(false);
          this.isConfirmExitModalOpen.set(false);

          if (fromExitModal && this.pendingSkillToSelect) {
            const nextSkill = this.pendingSkillToSelect;
            this.pendingSkillToSelect = null;
            this.selectedCaracteristica.set(nextSkill);
          } else if (fromExitModal) {
            this.selectedCaracteristica.set(null);
          }
        } else {
          this.editSkillErrorMessage.set(
            response.errorMessage || 'No se pudo actualizar la skill.'
          );
          if (fromExitModal) this.isConfirmExitModalOpen.set(false);
        }
        this.isSavingEditSkill.set(false);
      },
      error: () => {
        this.editSkillErrorMessage.set(
          'Error al conectar con la API para actualizar la skill.'
        );
        if (fromExitModal) this.isConfirmExitModalOpen.set(false);
        this.isSavingEditSkill.set(false);
      }
    });
  }

  onOpenAddSkillModal(): void {
    this.skillNombre.set('');
    this.skillPeso.set(50);
    this.skillDescripcion.set('');
    this.skillCriterio.set('');
    this.skillErrorMessage.set(null);
    this.isAddSkillModalOpen.set(true);
  }

  onCloseAddSkillModal(): void {
    if (this.isSavingSkill()) return;
    this.isAddSkillModalOpen.set(false);
    this.skillErrorMessage.set(null);
  }

  onGuardarSkill(): void {
    if (!this.perfilId) return;

    const nombre = this.skillNombre().trim();
    const pesoNum = Number(this.skillPeso());
    const desc = this.skillDescripcion().trim();
    const crit = this.skillCriterio().trim();

    if (!nombre) {
      this.skillErrorMessage.set('El nombre de la skill o característica es requerido.');
      return;
    }

    if (nombre.length > 50) {
      this.skillErrorMessage.set('El nombre de la skill no puede exceder 50 caracteres.');
      return;
    }

    if (isNaN(pesoNum) || !Number.isInteger(pesoNum) || pesoNum < 1 || pesoNum > 100) {
      this.skillErrorMessage.set(
        'El puntaje debe ser un número entero entre 1 y 100 (no se permiten decimales).'
      );
      return;
    }

    if (desc.length > 250) {
      this.skillErrorMessage.set('La descripción no puede exceder 250 caracteres.');
      return;
    }

    if (crit.length > 250) {
      this.skillErrorMessage.set('El criterio no puede exceder 250 caracteres.');
      return;
    }

    this.isSavingSkill.set(true);
    this.skillErrorMessage.set(null);

    const payload = {
      idPerfil: this.perfilId,
      nombre: this.perfil()?.nombre || '',
      perfilCaracteristicas: [
        {
          idPerfil: this.perfilId,
          idCaracteristica: 0,
          pesoPrioridad: pesoNum,
          criterioEspecifico: crit,
          caracteristica: {
            idCaracteristica: 0,
            nombreCaracteristica: nombre,
            descripcionCaracteristica: desc,
            perfilCaracteristicas: []
          }
        }
      ]
    };

    this.perfilService.update(this.perfilId, payload).subscribe({
      next: (response) => {
        if (response.correct) {
          this.isAddSkillModalOpen.set(false);
          this.cargarPerfil();
        } else {
          this.skillErrorMessage.set(
            response.errorMessage || 'No se pudo guardar la nueva skill en el perfil.'
          );
        }
        this.isSavingSkill.set(false);
      },
      error: () => {
        this.skillErrorMessage.set(
          'Error al conectar con la API para agregar la skill al perfil.'
        );
        this.isSavingSkill.set(false);
      }
    });
  }

  onAgregarCaracteristica(): void {
    this.onOpenAddSkillModal();
  }

  onOpenDeleteSkillModal(event: Event, skill: Caracteristica): void {
    event.stopPropagation();
    this.skillToDelete.set(skill);
    this.deleteSkillErrorMessage.set(null);
    this.isDeleteSkillModalOpen.set(true);
  }

  onCloseDeleteSkillModal(): void {
    if (this.isDeletingSkill()) return;
    this.isDeleteSkillModalOpen.set(false);
    this.skillToDelete.set(null);
    this.deleteSkillErrorMessage.set(null);
  }

  onConfirmDeleteSkill(): void {
    const skill = this.skillToDelete();
    if (!skill || !this.perfilId) return;

    this.isDeletingSkill.set(true);
    this.deleteSkillErrorMessage.set(null);

    this.perfilService.deleteSkill(this.perfilId, skill.idCaracteristica).subscribe({
      next: (response) => {
        if (response?.correct !== false) {
          // Remover la skill del perfil reactivamente
          this.perfil.update((p) => {
            if (!p || !p.caracteristicas) return p;
            const restantes = p.caracteristicas.filter(
              (c) => c.idCaracteristica !== skill.idCaracteristica
            );
            return { ...p, caracteristicas: restantes };
          });

          // Si la skill eliminada estaba en visualización/edición, limpiar
          if (this.selectedCaracteristica()?.idCaracteristica === skill.idCaracteristica) {
            this.selectedCaracteristica.set(null);
            this.isEditingSelectedSkill.set(false);
          }

          // Si ya no quedan skills, marcar como vacío
          const totalRestantes = this.perfil()?.caracteristicas?.length || 0;
          if (totalRestantes === 0) {
            this.isPerfilVacio.set(true);
          }

          this.isDeleteSkillModalOpen.set(false);
          this.skillToDelete.set(null);
        } else {
          this.deleteSkillErrorMessage.set(
            response?.errorMessage || 'No se pudo eliminar la skill del perfil.'
          );
        }
        this.isDeletingSkill.set(false);
      },
      error: (err) => {
        const errorMsg =
          err?.error?.errorMessage ||
          err?.message ||
          'Error al conectar con la API para eliminar la skill.';
        this.deleteSkillErrorMessage.set(errorMsg);
        this.isDeletingSkill.set(false);
      }
    });
  }
}
