# Tareas de Implementación: Perfeccionamiento de Perfil, Edición de Skills y Carga Visual de CVs

- **Identificador:** 001-perfil-skills-cv-upload
- **Fase:** 3 - Desglose de Tareas (SDD)
- **Estado:** listo para asignación
- **Fecha:** 2026-11-06

---

## Resumen de Tareas

| Id | Tarea | Requisitos Cubiertos | Archivos Afectados |
| :--- | :--- | :--- | :--- |
| **T1** | Restricciones de longitud y validaciones en gestión de perfiles | REQ-LEN-01, REQ-LEN-02, REQ-LEN-05 | `perfiles-list.component.html`, `perfiles-list.component.ts` |
| **T2** | Fallback de nombre y visibilidad continua en detalle de perfil | REQ-PRF-01, REQ-PRF-02, REQ-PRF-03, REQ-PRF-04 | `perfil-detalle.component.ts`, `perfil-detalle.component.html` |
| **T3** | Depuración iconográfica y remoción de elementos decorativos | REQ-VIS-01, REQ-VIS-02, REQ-VIS-03, REQ-VIS-04 | `perfil-detalle.component.html`, `perfil-detalle.component.css` |
| **T4** | Normalización de puntaje entero 1-100 y límites en alta de skills | REQ-SCR-01, REQ-SCR-02, REQ-SCR-03, REQ-SCR-04, REQ-LEN-01 a 04 | `perfil-detalle.component.ts`, `perfil-detalle.component.html` |
| **T5** | Modo de edición interactiva de skills en panel de detalle | REQ-SKL-01, REQ-SKL-02, REQ-SKL-03, REQ-SKL-04, REQ-SCR-01 a 04 | `perfil-detalle.component.ts`, `perfil-detalle.component.html`, `perfil-detalle.component.css`, `perfil.service.ts` |
| **T6** | Alerta modal estilo Apple ante cambios sin guardar (3 acciones) | REQ-ALT-01, REQ-ALT-02, REQ-ALT-03, REQ-ALT-04, REQ-ALT-05 | `perfil-detalle.component.ts`, `perfil-detalle.component.html`, `perfil-detalle.component.css` |
| **T7** | Creación del componente visual de recepción de CVs (`cv-upload`) | REQ-UPL-02, REQ-UPL-03, REQ-UPL-04, REQ-UPL-05, REQ-UPL-06 | `cv-upload.component.ts`, `cv-upload.component.html`, `cv-upload.component.css` |
| **T8** | Integración de navegación y enrutamiento hacia `/subir-cvs` | REQ-UPL-01 | `app.routes.ts`, `app.html` |

---

## Detalle de Tareas

### T1: Restricciones de longitud y validaciones en gestión de perfiles
- **RF Cubiertos:** REQ-LEN-01, REQ-LEN-02, REQ-LEN-05
- **Archivos:**
  - `src/app/components/perfiles-list/perfiles-list.component.html`
  - `src/app/components/perfiles-list/perfiles-list.component.ts`
- **Descripción:**
  - Incorporar el atributo `maxlength="50"` en `#inputNuevoPerfilNombre` e `#inputPerfilNombre`.
  - En los métodos `onGuardarNuevoPerfil()` y `onGuardarEditPerfil()`, comprobar que el texto no esté vacío tras aplicar `.trim()` (mínimo 1 carácter no vacío) y que no exceda 50 caracteres.
- **Hecho cuando:**
  - Ningún formulario de perfil permite escribir ni pegar más de 50 caracteres en el nombre.
  - Al presionar guardar con solo espacios en blanco, se bloquea la petición y se muestra el mensaje "El nombre del perfil es requerido".

---

### T2: Fallback de nombre y visibilidad continua en detalle de perfil
- **RF Cubiertos:** REQ-PRF-01, REQ-PRF-02, REQ-PRF-03, REQ-PRF-04
- **Archivos:**
  - `src/app/components/perfil-detalle/perfil-detalle.component.ts`
  - `src/app/components/perfil-detalle/perfil-detalle.component.html`
- **Descripción:**
  - En `cargarPerfil()`, cuando `response.correct` es `true` pero `response.objects` viene vacío o nulo, inicializar el perfil con `idPerfil = this.perfilId`, `nombre = 'Perfil #' + this.perfilId` y `caracteristicas = []`.
  - Asegurar que la vista de encabezado permanezca renderizada mostrando dicho título y el contador `0 skills configuradas`, mientras el cuerpo principal muestra el mensaje instructivo "Viene vacío. Quiere agregar un nuevo skill".
- **Hecho cuando:**
  - Al ingresar por URL a `/perfiles/999` (perfil sin skills), el encabezado nunca queda en blanco, mostrando "Perfil #999" y ofreciendo el botón para comenzar a cargar skills.

---

### T3: Depuración iconográfica y remoción de elementos decorativos
- **RF Cubiertos:** REQ-VIS-01, REQ-VIS-02, REQ-VIS-03, REQ-VIS-04
- **Archivos:**
  - `src/app/components/perfil-detalle/perfil-detalle.component.html`
  - `src/app/components/perfil-detalle/perfil-detalle.component.css`
- **Descripción:**
  - Eliminar el bloque SVG de cardiograma/pulso en `.skill-card`.
  - Eliminar el bloque SVG de estrella en el encabezado de `.detail-header`.
  - Eliminar el bloque SVG de maletín en los empty states de `.empty-skills-card`.
  - Preservar limpios y operativos los iconos de cerrar ("X") y de acción principal.
- **Hecho cuando:**
  - La interfaz de detalle de perfil y las tarjetas de skills se visualizan con tipografía nítida y espacio limpio sin iconos decorativos superfluos, conservando los botones funcionales.

---

### T4: Normalización de puntaje entero 1-100 y límites en alta de skills
- **RF Cubiertos:** REQ-SCR-01, REQ-SCR-02, REQ-SCR-03, REQ-SCR-04, REQ-LEN-01, REQ-LEN-03, REQ-LEN-04
- **Archivos:**
  - `src/app/components/perfil-detalle/perfil-detalle.component.ts`
  - `src/app/components/perfil-detalle/perfil-detalle.component.html`
- **Descripción:**
  - En el modal de agregar skill: agregar `maxlength="50"` al nombre y `maxlength="250"` a la descripción y criterio de evaluación.
  - Actualizar `min="1" max="100" step="1"` en el input de puntaje con valor inicial 50.
  - En `onGuardarSkill()`, validar que el puntaje sea entero (`Number.isInteger(pesoNum)`) y esté entre 1 y 100. Si tiene decimales o está fuera de rango, rechazar con: "El puntaje debe ser un número entero entre 1 y 100 (no se permiten decimales)".
  - Formatear el sufijo visual como `[peso] pts`.
- **Hecho cuando:**
  - El usuario no puede agregar una skill con más de 50 caracteres en el nombre ni más de 250 en descripciones, y un puntaje como "85.4" es explícitamente rechazado con el mensaje de error.

---

### T5: Modo de edición interactiva de skills en panel de detalle
- **RF Cubiertos:** REQ-SKL-01, REQ-SKL-02, REQ-SKL-03, REQ-SKL-04, REQ-SCR-01 a 04
- **Archivos:**
  - `src/app/components/perfil-detalle/perfil-detalle.component.ts`
  - `src/app/components/perfil-detalle/perfil-detalle.component.html`
  - `src/app/components/perfil-detalle/perfil-detalle.component.css`
  - `src/app/services/perfil.service.ts`
- **Descripción:**
  - En `.skill-detail-panel`, agregar botón funcional "Editar" junto al botón de cerrar.
  - Implementar el estado `isEditingSelectedSkill`: al activarse, el panel sustituye el texto estático por campos de formulario (`input` con `maxlength="50"`, input de puntaje entero `1-100`, textareas con `maxlength="250"`).
  - Incluir botones "Guardar cambios" y "Cancelar".
  - Al guardar, validar reglas (nombre no vacío, puntaje entero 1-100), sincronizar el array de características y enviar el PUT de actualización mediante `perfilService.update()`.
- **Hecho cuando:**
  - Es posible hacer clic en "Editar" en cualquier skill seleccionada, modificar su nombre, peso entero, descripción o criterio, guardar con éxito y ver reflejados los cambios inmediatamente en la tarjeta y el panel.

---

### T6: Alerta modal estilo Apple ante cambios sin guardar (3 acciones)
- **RF Cubiertos:** REQ-ALT-01, REQ-ALT-02, REQ-ALT-03, REQ-ALT-04, REQ-ALT-05
- **Archivos:**
  - `src/app/components/perfil-detalle/perfil-detalle.component.ts`
  - `src/app/components/perfil-detalle/perfil-detalle.component.html`
  - `src/app/components/perfil-detalle/perfil-detalle.component.css`
- **Descripción:**
  - Implementar método de verificación de dirty state que detecte si alguno de los campos en edición difiere de la skill original.
  - Interceptar el cierre del panel o cambio de skill en modo edición si hay cambios pendientes, desplegando un modal estilo Apple con el título "¿Desea guardar o no guardar?".
  - Disponer los 3 botones de acción:
    1. **"Guardar":** Valida, persiste y cierra la edición.
    2. **"No guardar":** Descarta las modificaciones locales, restablece los valores originales y cierra el panel.
    3. **"Cancelar":** Cierra únicamente el modal de alerta y mantiene al usuario en la edición actual sin perder datos.
- **Hecho cuando:**
  - Al editar una skill y pulsar la "X" de cerrar, aparece la alerta con las tres opciones, comportándose fielmente según la opción seleccionada.

---

### T7: Creación del componente visual de recepción de CVs (`cv-upload`)
- **RF Cubiertos:** REQ-UPL-02, REQ-UPL-03, REQ-UPL-04, REQ-UPL-05, REQ-UPL-06
- **Archivos:**
  - `src/app/components/cv-upload/cv-upload.component.ts` (crear)
  - `src/app/components/cv-upload/cv-upload.component.html` (crear)
  - `src/app/components/cv-upload/cv-upload.component.css` (crear)
- **Descripción:**
  - Crear componente standalone `CvUploadComponent` con una zona de arrastrar y soltar con bordes redondeados y microinteracción activa al arrastrar (`isDragOver`).
  - Permitir selección individual o múltiple vía explorador o drop, filtrando estrictamente archivos `.pdf` y `.docx`.
  - Presentar la lista de archivos colocados con su nombre, tamaño legible en KB o MB (`formatFileSize()`), y botón con icono funcional "X" para remover individualmente.
  - Estado vacío informativo y aviso discreto si se intentan soltar extensiones no permitidas.
- **Hecho cuando:**
  - El usuario puede arrastrar o examinar múltiples archivos PDF y Word DOCX, verlos listados con su tamaño correspondiente en KB/MB, y remover cualquier archivo con su botón individual.

---

### T8: Integración de navegación y enrutamiento hacia `/subir-cvs`
- **RF Cubiertos:** REQ-UPL-01
- **Archivos:**
  - `src/app/app.routes.ts`
  - `src/app/app.html`
- **Descripción:**
  - En `app.routes.ts`, importar `CvUploadComponent` y definir la ruta `{ path: 'subir-cvs', component: CvUploadComponent }`.
  - En `app.html`, enlazar el botón "Subir CVs" de la barra superior mediante `routerLink="/subir-cvs" routerLinkActive="active"`.
- **Hecho cuando:**
  - Al hacer clic en "Subir CVs" en la barra superior de la aplicación, el usuario navega limpiamente a `/subir-cvs` y se despliega la interfaz de recepción de documentos.
