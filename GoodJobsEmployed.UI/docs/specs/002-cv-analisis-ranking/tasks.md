# Tareas de Implementación: Análisis de CVs y Ranking con Tacómetro

- **Identificador:** 002-cv-analisis-ranking
- **Estado:** en progreso

---

### Tarea 1: Modelos y Servicio de Candidatos
- **RF Cubiertos:** REQ-ENV-01, REQ-ENV-03, REQ-RNK-01
- **Archivos:** `src/app/models/candidato.model.ts`, `src/app/services/candidato.service.ts`
- **Hecho cuando:** Existen los contratos de datos en TypeScript y el servicio dispone de `analizarCv` con `FormData` y Signals de almacenamiento de ranking.
- **Estado:** [x] completada

### Tarea 2: Selector de Perfil y Envío Secuencial en Subida de CVs
- **RF Cubiertos:** REQ-SEL-01, REQ-SEL-02, REQ-SEL-03, REQ-ENV-01, REQ-ENV-02, REQ-ENV-03, REQ-ENV-04
- **Archivos:** `src/app/components/cv-upload/cv-upload.component.ts`, `cv-upload.component.html`, `cv-upload.component.css`
- **Hecho cuando:** La vista `/subir-cvs` permite elegir el perfil, procesa la lista secuencialmente con indicador de avance, maneja fallos individuales y redirige a `/ranking`.
- **Estado:** [x] completada

### Tarea 3: Componente de Ranking y Tacómetro Visual
- **RF Cubiertos:** REQ-RNK-01, REQ-RNK-02, REQ-RNK-03, REQ-RNK-04, REQ-RNK-05, REQ-RNK-06, REQ-RNK-07
- **Archivos:** `src/app/components/cv-ranking/cv-ranking.component.ts`, `cv-ranking.component.html`, `cv-ranking.component.css`, `src/app/app.routes.ts`
- **Hecho cuando:** La ruta `/ranking` visualiza la lista clasificada, renderiza el tacómetro SVG con color según porcentaje (verde/naranja/rojo) y expande la evaluación detallada.
- **Estado:** [x] completada

### Tarea 4: Validación y Verificación E2E con Brave Browser
- **RF Cubiertos:** Todos
- **Archivos:** `MEMORY.md`, verificación en navegador
- **Hecho cuando:** `npm run build` compila con 0 errores de presupuesto, y la prueba en Brave confirma la interacción completa con captura de pantalla.
- **Estado:** [x] completada
