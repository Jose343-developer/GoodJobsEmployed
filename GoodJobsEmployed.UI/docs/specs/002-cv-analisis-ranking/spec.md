# Especificación Técnica: Análisis Secuencial de CVs, Selector de Perfil y Ranking con Tacómetro

- **Identificador:** 002-cv-analisis-ranking
- **Estado:** aprobado
- **Fecha de Creación:** 2026-11-07
- **Fecha de Aprobación:** 2026-11-07
- **Metodología:** SDD (Spec-Driven Development)

---

## 1. Propósito y Contexto

El sistema de selección y seguimiento requiere procesar los currículums cargados en `/subir-cvs` evaluándolos contra un perfil profesional específico mediante la API REST (`/api/Candidato/AnalizarCV/{idperfil}`). Para asegurar estabilidad y granularidad en el proceso, los CVs se transmitirán de manera secuencial (uno a uno), soportando fallos aislados sin interrumpir el lote completo.

Una vez procesados, el sistema redirige automáticamente a una nueva vista de **Ranking**, donde se presentan los candidatos evaluados, ordenados descendentemente por su compatibilidad, con un diseño gráfico tipo **tacómetro / gauge semicircular dinámico** cuyo color refleja el puntaje (verde para puntajes altos, naranja para valores medios y rojo para bajos), junto con la justificación general y el desglose de cada habilidad analizada.

---

## 2. Objetivos

- **Selección Previa de Perfil:** Obligar a que el usuario seleccione un perfil activo antes de iniciar el análisis de los CVs cargados.
- **Transmisión Secuencial Resiliente:** Enviar los archivos uno por uno a la API REST. Si un archivo particular produce error de API o de base de datos, marcar ese archivo con error específico en el resultado y continuar procesando los restantes.
- **Redirección Fluida:** Concluido el análisis de los archivos, navegar de forma automática a la vista de `/ranking`.
- **Visualización de Tacómetro Dinámico:** Representar el porcentaje de compatibilidad mediante un componente visual tipo tacómetro/gauge con degradado/color semántico:
  - $\ge 70\%$: Verde institucional (`#29b765`)
  - $40\% - 69\%$: Naranja (`#ff9500` / `#f59e0b`)
  - $< 40\%$: Rojo (`#dc3545`)
- **Detalle de Competencias:** Permitir consultar el desglose individual de cada skill evaluada (`cumple`, `puntajeObtenido`, `pesoPrioridad`, `criterioEspecifico`).
- **Respeto a AGENTS.md y Presupuestos:** Mantener estética minimalista estilo Apple, sin librerías externas ni emojis, con tipografía Barlow condensed y hojas de estilo optimizadas (<4kB).

---

## 3. Requisitos Funcionales (Sintaxis EARS)

### 3.1 Selector de Perfil y Preparación de Análisis
- **REQ-SEL-01 (Ubiquitous):** El sistema debe proveer en la vista `/subir-cvs` un selector accesible de perfiles profesionales alimentado por `PerfilService.getAll()`.
- **REQ-SEL-02 (State-driven):** MIENTRAS no se haya seleccionado un perfil válido o no haya al menos un archivo cargado en la lista, el sistema debe mantener inhabilitado el botón de acción *"Analizar CVs"*.
- **REQ-SEL-03 (Event-driven):** CUANDO el usuario seleccione un perfil válido y existan documentos cargados, el sistema debe habilitar el botón *"Analizar CVs"*.

### 3.2 Envío Secuencial y Manejo de Errores
- **REQ-ENV-01 (Event-driven):** CUANDO el usuario presione *"Analizar CVs"*, el sistema debe iniciar el procesamiento secuencial (uno a la vez) enviando cada archivo en un `FormData` con la clave binaria `"cvFile"`.
- **REQ-ENV-02 (State-driven):** MIENTRAS la cola de análisis esté en ejecución, el sistema debe desplegar un indicador de progreso visual ("Analizando CV X de N...") y deshabilitar controles de carga para evitar condiciones de carrera.
- **REQ-ENV-03 (Unwanted behavior):** SI la API responde con error HTTP (400, 500) o `correct: false` para un CV particular, el sistema debe registrar dicho ítem con estado fallido detallando el mensaje ("Error con la API o base de datos") y continuar de inmediato con el siguiente archivo sin abortar el lote.
- **REQ-ENV-04 (Event-driven):** CUANDO todos los archivos hayan terminado de procesarse (exitosos y fallidos), el sistema debe almacenar el lote analizado y redirigir automáticamente a `/ranking`.

### 3.3 Vista de Ranking y Visualización en Tacómetro
- **REQ-RNK-01 (Ubiquitous):** La vista `/ranking` debe presentar la lista de candidatos evaluados ordenados descendentemente por `porcentajeCompatibilidad` (los ítems con error se ubican al final).
- **REQ-RNK-02 (Ubiquitous):** Cada tarjeta de candidato exitoso debe desplegar un indicador visual en forma de tacómetro (gauge SVG semicircular relleno) que ilustre el porcentaje del 0 al 100%.
- **REQ-RNK-03 (State-driven):** MIENTRAS el porcentaje sea mayor o igual a 70%, el tacómetro debe colorearse en verde (`#29b765`); cuando se sitúe entre 40% y 69%, debe colorearse en naranja (`#ff9500`); y cuando sea inferior a 40%, debe colorearse en rojo (`#dc3545`).
- **REQ-RNK-04 (Ubiquitous):** Cada tarjeta del ranking debe mostrar el nombre del archivo evaluado, la `justificacionGeneral` emitida por el evaluador y un botón para desplegar la lista de `evaluacionDetallada`.
- **REQ-RNK-05 (State-driven):** MIENTRAS la sección de evaluación detallada esté expandida, el sistema debe exhibir cada característica con su nombre, peso, estado de cumplimiento (icono vectorial de verificación o cruz) y puntaje obtenido.
- **REQ-RNK-06 (Event-driven):** CUANDO un archivo contenga estado de error, la tarjeta debe renderizarse con borde y distintivo de advertencia, indicando que no se pudo procesar y el motivo del fallo.
- **REQ-RNK-07 (Ubiquitous):** La vista de ranking debe incluir una acción para *"Volver a subir CVs"* o *"Regresar al inicio"*.
