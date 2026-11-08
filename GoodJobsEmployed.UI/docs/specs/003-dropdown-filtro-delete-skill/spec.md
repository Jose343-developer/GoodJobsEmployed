# Especificación Técnica: Filtrado Reactivo de Perfiles en Cascada y Eliminación de Skills

- **Identificador:** 003-dropdown-filtro-delete-skill
- **Estado:** aprobado
- **Fecha de Creación:** 2026-11-07
- **Fecha de Aprobación:** 2026-11-07
- **Metodología:** SDD (Spec-Driven Development)

---

## 1. Propósito y Contexto

Durante el flujo de evaluación de CVs en `/subir-cvs`, se detectó que los usuarios podían seleccionar perfiles que carecían de habilidades asociadas (0 skills configuradas), lo cual provocaba fallos de negocio en la API de evaluación. Para garantizar integridad, el dropdown de selección debe filtrar y listar únicamente los perfiles aptos (con al menos una skill configurada).

Asimismo, en la vista de detalle de perfil (`/perfiles/:id`), el usuario requiere la capacidad de eliminar habilidades previamente registradas mediante una acción directa en cada tarjeta de skill, protegida por una alerta modal de confirmación destructiva estilo Apple que persista en el endpoint `DELETE /api/Perfil/DeleteSkill/{idperfil}/{idskill}`.

---

## 2. Requisitos Funcionales (Sintaxis EARS)

### 2.1 Filtrado Reactivo en Dropdown de Selección (/subir-cvs)
- **REQ-DRP-01 (Ubiquitous):** El sistema debe filtrar en tiempo real los perfiles cargados desde el servicio, excluyendo del listado desplegable a cualquier perfil que no tenga al menos una característica válida (`caracteristicas.length === 0` o `idCaracteristica === null`).
- **REQ-DRP-02 (Ubiquitous):** Cada opción seleccionable del dropdown debe presentar el nombre del perfil acompañado del conteo de skills configuradas (ej. *"Desarrollador (2 skills)"*).
- **REQ-DRP-03 (State-driven):** MIENTRAS no existan perfiles con habilidades configuradas en el sistema, el dropdown debe permanecer deshabilitado y desplegar el mensaje explicativo: *"No hay perfiles con skills configuradas. Ve a 'Ver Perfiles' para configurar al menos una."* junto a un enlace directo de navegación.

### 2.2 Eliminación de Skills en Tarjetas de Perfil (/perfiles/:id)
- **REQ-SKL-01 (Ubiquitous):** El sistema debe desplegar un botón de eliminación con icono vectorial SVG de papelera en la esquina superior derecha de cada tarjeta de habilidad (`.skill-card`).
- **REQ-SKL-02 (Event-driven):** CUANDO el usuario presione el botón de eliminación de una skill, el sistema debe detener la propagación del evento (`$event.stopPropagation()`) para evitar la selección o apertura de detalles de la tarjeta.
- **REQ-SKL-03 (Event-driven):** CUANDO se accione el botón de eliminación, el sistema debe presentar una alerta modal estilo Apple que solicite confirmación explícita indicando el nombre de la habilidad a eliminar.
- **REQ-SKL-04 (Event-driven):** CUANDO el usuario confirme la eliminación en el modal, el sistema debe invocar `DELETE http://localhost:5047/api/Perfil/DeleteSkill/{idperfil}/{idskill}`.
- **REQ-SKL-05 (State-driven):** MIENTRAS la petición de eliminación esté en curso, el sistema debe mostrar un estado de carga en el botón confirmatorio ("Eliminando...") y bloquear cierres accidentales.
- **REQ-SKL-06 (Event-driven):** TRAS una eliminación exitosa, el sistema debe actualizar reactivamente el Signal del perfil removiendo la habilidad sin recargar la página.
- **REQ-SKL-07 (State-driven):** MIENTRAS la última skill de un perfil sea eliminada, el sistema debe transicionar inmediatamente al estado de perfil vacío ("Viene vacío. Quiere agregar un nuevo skill") y dicho perfil debe dejar de figurar de inmediato en el dropdown de `/subir-cvs`.
