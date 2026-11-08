# Tareas: Filtrado de Perfiles en Cascada y Borrado de Skills

- **Identificador:** 003-dropdown-filtro-delete-skill
- **Estado:** completado

---

### Tarea 1: Servicio REST para Borrado de Skill
- **RF Cubiertos:** REQ-SKL-04
- **Archivos:** `src/app/services/perfil.service.ts`
- **Hecho cuando:** `deleteSkill(idPerfil, idSkill)` consume `DELETE /api/Perfil/DeleteSkill/{idperfil}/{idskill}`.
- **Estado:** [x] completada

### Tarea 2: Filtrado Reactivo en Dropdown de Subida de CVs
- **RF Cubiertos:** REQ-DRP-01, REQ-DRP-02, REQ-DRP-03
- **Archivos:** `src/app/components/cv-upload/cv-upload.component.ts`, `cv-upload.component.html`, `cv-upload.component.css`
- **Hecho cuando:** El `<select>` solo muestra perfiles con skills y avisa si no hay disponibles.
- **Estado:** [x] completada

### Tarea 3: Botón de Borrado en Cards y Modal de Confirmación Apple
- **RF Cubiertos:** REQ-SKL-01, REQ-SKL-02, REQ-SKL-03, REQ-SKL-05, REQ-SKL-06, REQ-SKL-07
- **Archivos:** `src/app/components/perfil-detalle/perfil-detalle.component.ts`, `perfil-detalle.component.html`, `perfil-detalle.component.css`
- **Hecho cuando:** Cada card de skill tiene botón de papelera, abre modal de confirmación y actualiza el Signal del perfil localmente.
- **Estado:** [x] completada

### Tarea 4: Validación y Verificación E2E con Brave Browser
- **RF Cubiertos:** Todos los de la spec
- **Archivos:** `MEMORY.md`, verificación en navegador Brave
- **Hecho cuando:** `npm run build` compila con 0 errores de presupuesto, y la prueba en Brave confirma el filtrado y el modal de borrado.
- **Estado:** [x] completada
