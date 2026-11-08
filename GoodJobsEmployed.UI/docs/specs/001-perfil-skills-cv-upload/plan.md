# Plan Técnico de Implementación: Perfeccionamiento de Perfil, Edición de Skills y Carga Visual de CVs

- **Identificador:** 001-perfil-skills-cv-upload
- **Fase:** 3 - Planificación Arquitectural (SDD)
- **Estado:** listo para ejecución
- **Fecha:** 2026-11-06

---

## 1. Arquitectura y Principios de Diseño

El frontend opera bajo la constitución del proyecto (`GoodJobsEmployed.UI/docs/constitution.md`) y las pautas fijadas en `AGENTS.md`:
1. **Framework y Reactividad:** Angular 22+, componentes Standalone (`imports: [...]`), gestión de estado mediante **Angular Signals** (`signal()`, `computed()`) para mantener reactividad síncrona sin sobrecarga ni librerías adicionales.
2. **Desacoplamiento Estricto:** La lógica de consumo de API y transformación de datos reside exclusivamente en `PerfilService`. Los componentes únicamente manejan el estado de presentación local e interacción del usuario.
3. **Diseño Visual Estilo Apple:**
   - Paleta de colores: fondo principal blanco (`#ffffff`), acento de interacción verde (`#29b765`), bordes sutiles (`#e5e7eb`), fondos de apoyo gris claro (`#f9fafb`) y contraste oscuro para Dark Mode (`#1e2022`).
   - Tipografía: *Barlow Condensed*, sans-serif.
   - Esquinas redondeadas: `border-radius: 12px` en tarjetas y botones, `16px` en paneles y modales.
   - Iconografía funcional limpia: se eliminan los pictogramas decorativos redundantes (pulso cardíaco, estrella, maletín) y se preservan únicamente los funcionales con trazo nítido (cerrar "X", eliminar archivo en subida, editar "lápiz").
4. **Validaciones en el Cliente y Prevención de Errores:**
   - Restricción estricta de longitud con `maxlength` nativo en el DOM respaldado por validaciones lógicas en Typescript (mínimo 1 carácter no vacío).
   - Rechazo de números decimales y valores fuera de [1, 100] para los puntajes.
   - Alerta modal estilo Apple con tres acciones explícitas ("Guardar", "No guardar", "Cancelar") ante cambios pendientes de una skill.

---

## 2. Mapa de Archivos Afectados

```
GoodJobsEmployed.UI/
├── src/app/
│   ├── app.routes.ts                                  [Modificar: registrar ruta /subir-cvs]
│   ├── app.html                                       [Modificar: enlazar botón "Subir CVs" a routerLink="/subir-cvs"]
│   ├── models/
│   │   └── perfil.model.ts                            [Modificar: tipado ampliado para Caracteristica y estado de edición]
│   ├── services/
│   │   └── perfil.service.ts                          [Modificar: helper o payload para actualización atómica de skills]
│   └── components/
│       ├── perfiles-list/
│       │   ├── perfiles-list.component.html           [Modificar: maxlength="50", validación no vacío, limpieza visual]
│       │   └── perfiles-list.component.ts             [Modificar: validación estricta de longitud y espacios]
│       ├── perfil-detalle/
│       │   ├── perfil-detalle.component.ts            [Modificar: fallback "Perfil #[id]", modo edición skill, dirty state, alerta 3 botones, validación 1-100 enteros]
│       │   ├── perfil-detalle.component.html          [Modificar: remoción iconos decorativos, botón/formulario editar skill, modal Apple "¿Desea guardar o no guardar?"]
│       │   └── perfil-detalle.component.css           [Modificar: estilos de alerta Apple, inputs de edición y limpieza de reglas obsoletas]
│       └── cv-upload/
│           ├── cv-upload.component.ts                 [Crear: componente standalone para drag-and-drop, validación .pdf/.docx, cálculo de KB/MB]
│           ├── cv-upload.component.html               [Crear: zona drag-and-drop con respuesta visual, listado de archivos y botón remover]
│           └── cv-upload.component.css                [Crear: estética Apple para dropzone y file-chips]
```

---

## 3. Especificación Detallada por Componente y Servicio

### 3.1 `PerfilDetalleComponent` (`perfil-detalle`)

#### A. Fallback del Nombre de Perfil (REQ-PRF-01, REQ-PRF-03, REQ-PRF-04)
- **Problema:** Al acceder directamente por URL a `/perfiles/:id` en un perfil que no tiene características registradas, el backend retorna `objects: []`, asignando `perfil.set(null)` y dejando el encabezado vacío o en blanco.
- **Solución:**
  - Si `response.objects` viene vacío, instanciar un perfil mínimo con `idPerfil = this.perfilId` y `nombre = 'Perfil #' + this.perfilId`, y `caracteristicas = []`.
  - Marcar `isPerfilVacio.set(true)` para presentar la vista de bienvenida ("Viene vacío. Quiere agregar un nuevo skill"), pero asegurando que el encabezado exhiba "Perfil #[id]" con su contador en 0.

#### B. Limpieza Iconográfica (REQ-VIS-01, REQ-VIS-02, REQ-VIS-03, REQ-VIS-04)
- **Tarjeta de skill (`.skill-card`):** Eliminar el contenedor `.skill-icon-pill` y el SVG de pulso/cardiograma. La jerarquía visual queda guiada por el puntaje `xx pts`, el nombre de la skill y su descripción.
- **Panel de detalle (`.detail-header`):** Eliminar el contenedor `.detail-icon-box` y el SVG de la estrella.
- **Empty state (`.empty-skills-card`):** Eliminar el contenedor `.empty-icon-wrap` y el SVG del maletín.
- **Iconos funcionales conservados:** El botón de cierre "X" (`.btn-close-detail`) y el nuevo botón de edición "lápiz" (`.btn-edit-skill`).

#### C. Modo Edición de Skills y Dirty State (REQ-SKL-01 a 04, REQ-ALT-01 a 05)
- **Signals locales para edición:**
  - `isEditingSelectedSkill = signal<boolean>(false);`
  - `editSkillNombre = signal<string>('');`
  - `editSkillPeso = signal<number | string>(1);`
  - `editSkillDescripcion = signal<string>('');`
  - `editSkillCriterio = signal<string>('');`
  - `isConfirmExitModalOpen = signal<boolean>(false);`
  - `editSkillErrorMessage = signal<string | null>(null);`
  - `isSavingEditSkill = signal<boolean>(false);`
- **Detección de Cambios (Dirty Check):**
  - Un método o computed `hasUnsavedChanges(): boolean` compara los valores actuales de los signals de edición contra los valores de `selectedCaracteristica()`.
  - Si el usuario presiona "Cerrar" ("X") o intenta cambiar de skill mientras `isEditingSelectedSkill()` es `true`:
    - Si `hasUnsavedChanges() === true`: se abre el modal de alerta estilo Apple "¿Desea guardar o no guardar?".
    - Si `hasUnsavedChanges() === false`: se cancela la edición y se cierra el panel de inmediato.
- **Modal de Alerta Estilo Apple (3 acciones explícitas):**
  1. **"Guardar":** Invoca la validación y guardado. Si es exitoso, persiste y cierra el modo de edición y el panel.
  2. **"No guardar":** Descarta los cambios locales, revierte los signals a los valores del objeto original y sale del modo de edición cerrando el panel.
  3. **"Cancelar":** Cierra únicamente la alerta modal y deja al usuario en el formulario de edición con sus modificaciones intactas.

#### D. Validación Estricta de Puntajes (1 a 100 Enteros) (REQ-SCR-01 a 03)
- En la función de guardado (tanto en modal de agregar como en panel de edición):
  ```typescript
  const pesoNum = Number(peso);
  if (isNaN(pesoNum) || !Number.isInteger(pesoNum) || pesoNum < 1 || pesoNum > 100) {
    this.errorMessage.set('El puntaje debe ser un número entero entre 1 y 100 (no se permiten decimales).');
    return;
  }
  ```
- En el modal de agregar skill: cambiar el valor inicial a 50 (rango 1-100) y actualizar los atributos `min="1" max="100" step="1"`.

#### E. Persistencia de la Edición
- Al modificar una skill, se toma la lista actual de características del perfil, se actualiza el elemento modificado con sus nuevos valores (nombre, descripción, peso, criterio), y se envía el payload completo a `perfilService.update(perfilId, payloadCompleto)` que utiliza el endpoint `PUT api/Perfil/UpdatePerfil/{idPerfil}` del backend.

---

### 3.2 `PerfilesListComponent` (`perfiles-list`)

- **Límites de Longitud (REQ-LEN-01, REQ-LEN-02):**
  - En el modal de creación de perfil (`#inputNuevoPerfilNombre`): agregar `maxlength="50"`.
  - En el modal de edición de perfil (`#inputPerfilNombre`): agregar `maxlength="50"`.
  - En Typescript (`onGuardarNuevoPerfil` y `onGuardarEditPerfil`): validar que `nombre.trim().length > 0` (mínimo 1 carácter no vacío) y `nombre.trim().length <= 50`.
- **Limpieza visual:** Verificar que no existan iconos decorativos no funcionales redundantes.

---

### 3.3 `CvUploadComponent` (`cv-upload`) [Nuevo Componente]

- **Selector:** `app-cv-upload`
- **Ruta:** `/subir-cvs`
- **Estado (Signals):**
  - `archivosCargados = signal<{ id: string; archivo: File; nombre: string; tamanoFormateado: string; extension: string }[]>([]);`
  - `isDragOver = signal<boolean>(false);`
  - `advertenciaFormato = signal<string | null>(null);`
- **Lógica de Drag & Drop y Selección:**
  - Zona receptora con eventos `(dragover)`, `(dragleave)`, `(drop)` y botón explorador con `<input type="file" multiple accept=".pdf,.docx">`.
  - Filtro estricto: solo archivos con extensión `.pdf` o `.docx` (o MIME types correspondientes). Si un archivo no coincide, emitir mensaje amigable en `advertenciaFormato`: *"Solo se admiten documentos en formato PDF (.pdf) y Word (.docx)"*.
  - Función de formateo de peso: si bytes < 1048576 presentar en KB (`(bytes / 1024).toFixed(2) + ' KB'`), caso contrario en MB (`(bytes / 1048576).toFixed(2) + ' MB'`).
- **Lista de Archivos:**
  - Renderizado de tarjetas minimalistas estilo Apple con nombre de archivo, peso legible y botón con icono funcional "X" para remover individualmente el archivo (`removerArchivo(id)`).
  - Estado vacío instructivo cuando la lista de archivos esté en 0.

---

### 3.4 Navegación y Rutas (`app.routes.ts` y `app.html`)

- En `app.routes.ts`:
  - Importar `CvUploadComponent` y agregar `{ path: 'subir-cvs', component: CvUploadComponent }`.
- En `app.html`:
  - Reemplazar el botón estático `<button type="button" class="nav-btn">Subir CVs</button>` por `<a routerLink="/subir-cvs" routerLinkActive="active" class="nav-btn text-decoration-none">Subir CVs</a>`.

---

## 4. Decisiones Técnicas y Justificaciones

| Decisión | Justificación |
| :--- | :--- |
| **Uso de Signals para todo el estado local** | Alineado a la arquitectura moderna de Angular (v17-v22), garantiza renderizado granular sin zoneless issues y cumple con el principio de mínima dependencia. |
| **`maxlength` nativo en conjunto con validación en controlador** | El atributo `maxlength` impide físicamente al usuario sobrepasar el límite desde el teclado, mientras que la validación en el componente previene inyecciones o bypass por pegado/manipulación de DOM. |
| **Tres botones en alerta modal Apple ("Guardar", "No guardar", "Cancelar")** | Sigue las Human Interface Guidelines (HIG) de Apple para documentos y formularios con cambios sin guardar: "Guardar" aplica, "No guardar" descarta sin confirmación adicional, y "Cancelar" regresa al contexto de edición sin destruir nada. |
| **Persistencia de edición vía `UpdatePerfil` existente** | El backend ya cuenta con `cadPerfil` opción 2 en `UpdatePerfil/{idPerfil}` que maneja `perfilCaracteristicas`. Reutilizar el servicio existente evita cambios en el backend y minimiza riesgo de regresiones. |
| **Validación síncrona en cliente para CVs** | La especificación delimita la carga a nivel visual; aislar la recepción de archivos en cliente permite validar formatos y tamaños de forma instantánea sin latencia de red ni exposición de datos privados del candidato. |

---

## 5. Alternativas Descartadas

- **Instalar librerías de componentes (ej. ngx-dropzone, SweetAlert2):** Descartado por violar la regla 1 de `docs/constitution.md` (no añadir dependencias ni frameworks adicionales). La dropzone y el modal de alerta se implementan nativamente con TypeScript y CSS estilo Apple.
- **Uso de Reactive Forms con FormBuilder:** Descartado por ser excesivamente pesado para los formularios de 1 a 4 campos del proyecto; el uso de Signals con `ngModel` bidireccional es más ligero, conciso y consistente con el resto del código base.
- **Redirigir a `/perfiles` cuando un perfil esté vacío:** Descartado porque rompe el flujo del reclutador que desea consultar o comenzar a asociar habilidades a un perfil recién creado desde su enlace directo. El fallback "Perfil #[id]" mantiene la experiencia continua.
