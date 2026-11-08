# Especificación Técnica: Perfeccionamiento de Perfil, Edición de Skills y Estructura Visual de Subida de CVs

- **Identificador:** 001-perfil-skills-cv-upload
- **Estado:** borrador
- **Fecha de Creación:** 2026-11-06
- **Metodología:** SDD (Spec-Driven Development)

---

## 1. Propósito y Contexto

El sistema de seguimiento y evaluación de candidatos requiere una experiencia de usuario clara, predecible y estéticamente refinada bajo principios de diseño minimalista estilo Apple. Actualmente, la captura de información carece de restricciones explícitas de longitud en los campos de texto, el nombre del perfil no se preserva de manera consistente cuando no existen habilidades registradas o la respuesta viene vacía, coexisten elementos decorativos redundantes (iconos no esenciales), el peso de las habilidades opera bajo una escala desfasada (1 a 10 en lugar de 1 a 100), y no es posible modificar directamente las habilidades una vez visualizadas en el panel de detalle sin riesgo de perder modificaciones no guardadas. Adicionalmente, el flujo de reclutamiento requiere habilitar la interfaz visual de recepción de currículums (CVs) mediante una zona de arrastrar y soltar minimalista.

Esta especificación formaliza el comportamiento funcional esperado, las restricciones de datos y las reglas de interacción visual.

---

## 2. Objetivos

- **Integridad y consistencia de datos:** Restringir la longitud máxima de textos de perfiles y habilidades para evitar desbordamientos visuales y asegurar concisión.
- **Continuidad de contexto:** Garantizar que el reclutador identifique inequívocamente el perfil que está consultando, sin importar si este tiene o no habilidades asociadas o si los datos vienen vacíos.
- **Minimalismo y sobriedad visual:** Eliminar iconos decorativos innecesarios para centrar la atención en la tipografía, el espacio en blanco y los datos reales.
- **Capacidad de edición y protección contra pérdida de datos:** Permitir la edición interactiva de habilidades con confirmación estilo Apple al intentar salir con cambios sin guardar.
- **Normalización de ponderación:** Estandarizar la escala de prioridad del 1 al 100 para evaluar con mayor granularidad el peso de cada habilidad.
- **Habilitación de captura de documentos:** Proveer una interfaz visual intuitiva para cargar CVs de forma individual o masiva mediante drag-and-drop.

---

## 3. Requisitos Funcionales (Sintaxis EARS)

### 3.1 Límites de Longitud de Entrada

- **REQ-LEN-01 (Ubiquitous):** El sistema debe impedir el ingreso de nombres de perfil que excedan 50 caracteres en los formularios de creación y edición.
- **REQ-LEN-02 (Ubiquitous):** El sistema debe impedir el ingreso de nombres de característica o skill que excedan 50 caracteres en los formularios de creación y edición.
- **REQ-LEN-03 (Ubiquitous):** El sistema debe impedir el ingreso de descripciones de característica que excedan 250 caracteres.
- **REQ-LEN-04 (Ubiquitous):** El sistema debe impedir el ingreso de criterios específicos de evaluación que excedan 250 caracteres.
- **REQ-LEN-05 (Event-driven):** CUANDO el usuario alcance el límite máximo de caracteres en cualquiera de los campos de texto, el sistema debe bloquear la escritura adicional de caracteres.
- **REQ-LEN-06 (Event-driven):** CUANDO el usuario enfoque un campo con límite de caracteres, el sistema debe mostrar un indicador discreto con el conteo actual y el total permitido.

### 3.2 Persistencia y Visibilidad del Nombre del Perfil

- **REQ-PRF-01 (Ubiquitous):** El sistema debe mantener visible el nombre del perfil en el encabezado principal de la vista de detalle en todo momento.
- **REQ-PRF-02 (State-driven):** MIENTRAS la información de características del perfil esté vacía o no contenga habilidades asociadas, el sistema debe presentar el nombre del perfil en el encabezado superior y mostrar el mensaje de bienvenida a agregar skills en el cuerpo principal.
- **REQ-PRF-03 (State-driven):** MIENTRAS el servicio de datos devuelva una respuesta válida sin registros de habilidades, el sistema debe preservar el nombre del perfil obtenido del contexto de selección o de la identificación consultada sin sustituirlo por etiquetas genéricas ni dejarlo en blanco.

### 3.3 Limpieza Visual e Iconográfica

- **REQ-VIS-01 (Ubiquitous):** El sistema debe prescindir de iconos decorativos en las tarjetas de habilidad, eliminando el pictograma de pulso/cardiograma y dejando un diseño limpio gobernado por tipografía y jerarquía visual.
- **REQ-VIS-02 (Ubiquitous):** El sistema debe prescindir del icono decorativo de estrella en el panel de detalle de la habilidad seleccionada.
- **REQ-VIS-03 (Ubiquitous):** El sistema debe prescindir del icono de maletín en las vistas y tarjetas de estado vacío, presentando un área despejada con tipografía nítida y botón de acción principal.

### 3.4 Edición Interactiva de Skills

- **REQ-SKL-01 (Event-driven):** CUANDO el usuario seleccione una habilidad para ver su detalle, el sistema debe permitir activar el modo de edición de sus campos (nombre, peso, descripción y criterio específico).
- **REQ-SKL-02 (Event-driven):** CUANDO el usuario confirme la acción de guardar cambios en una habilidad en edición, el sistema debe validar los datos ingresados y actualizar la información de la habilidad en la lista.
- **REQ-SKL-03 (Event-driven):** CUANDO el guardado de la habilidad sea exitoso, el sistema debe reflejar los valores actualizados inmediatamente en la vista y salir del modo de edición.
- **REQ-SKL-04 (Unwanted event):** SI alguno de los campos modificados infringe las reglas de longitud obligatorias o el rango numérico permitido, ENTONCES el sistema debe bloquear el guardado y señalar de forma clara el error en el campo infractor.

### 3.5 Alerta de Confirmación de Cambios sin Guardar

- **REQ-ALT-01 (Event-driven):** CUANDO el usuario intente cerrar el panel o modal de edición de una habilidad habiendo modificado uno o más campos sin guardar, el sistema debe desplegar una alerta estilo Apple con la interrogante "¿Desea guardar o no guardar?".
- **REQ-ALT-02 (Event-driven):** CUANDO la alerta de confirmación esté visible, el sistema debe ofrecer dos opciones principales: "Guardar" y "No guardar" (junto con la posibilidad de "Cancelar" para permanecer en la edición).
- **REQ-ALT-03 (Event-driven):** CUANDO el usuario seleccione "Guardar" en la alerta, el sistema debe proceder a validar y guardar los cambios pendientes y posteriormente cerrar el panel.
- **REQ-ALT-04 (Event-driven):** CUANDO el usuario seleccione "No guardar" en la alerta, el sistema debe descartar todos los cambios realizados, restaurar los valores previos de la habilidad y cerrar el panel.
- **REQ-ALT-05 (Event-driven):** CUANDO el usuario seleccione "Cancelar" en la alerta, el sistema debe cerrar la alerta y mantener el panel en modo edición sin alterar los datos introducidos.

### 3.6 Corrección de Escala de Puntajes

- **REQ-SCR-01 (Ubiquitous):** El sistema debe establecer la escala válida de peso o prioridad de cada habilidad en un rango de 1 a 100 puntos enteros.
- **REQ-SCR-02 (Event-driven):** CUANDO el usuario registre o edite el puntaje de una habilidad, el sistema debe admitir exclusivamente números enteros comprendidos entre 1 (peso mínimo) y 100 (peso máximo indispensable).
- **REQ-SCR-03 (Unwanted event):** SI el usuario ingresa un valor menor a 1, mayor a 100 o con decimales, ENTONCES el sistema debe impedir el guardado y mostrar el mensaje explicativo correspondiente.
- **REQ-SCR-04 (Ubiquitous):** El sistema debe mostrar el puntaje en las tarjetas y detalles con formato de sufijo en puntos sobre 100 (por ejemplo: "85 pts").

### 3.7 Interfaz Visual para Subida de CVs

- **REQ-UPL-01 (Ubiquitous):** El sistema debe proveer una vista dedicada accesible desde el menú principal para la carga de currículums.
- **REQ-UPL-02 (Ubiquitous):** El sistema debe incluir una zona visual de arrastrar y soltar ("drag-and-drop") con bordes redondeados, fondo suave y tipografía estilo Apple.
- **REQ-UPL-03 (Ubiquitous):** El sistema debe permitir la selección tanto de un archivo individual como de múltiples archivos en simultáneo.
- **REQ-UPL-04 (Event-driven):** CUANDO el usuario arrastre archivos sobre la zona activa, el sistema debe transformar sutilmente el aspecto de la zona (borde activo, contraste sutil) indicando la disponibilidad para recibir los documentos.
- **REQ-UPL-05 (Event-driven):** CUANDO los archivos sean soltados o seleccionados mediante explorador, el sistema debe listar visualmente los archivos colocados (nombre del archivo, tamaño legible en KB/MB y botón individual para remover).
- **REQ-UPL-06 (Ubiquitous):** En esta fase, la interfaz de subida debe operar de manera puramente visual y de presentación en cliente, sin desencadenar procesamiento intensivo ni servicios de terceros.

---

## 4. Requisitos No Funcionales

- **Estilo Visual:** Todo elemento nuevo o modificado debe seguir la estética minimalista de Apple (paleta de blancos, gris oscuro en textos y modo oscuro, acento verde `#29b765` en acciones principales, bordes redondeados y tipografía sobria).
- **Idioma Oficial:** Toda la interfaz, mensajes al usuario, títulos, descripciones y confirmaciones deben estar íntegramente redactados en español formal y accesible.
- **Usabilidad y Claridad:** Los tiempos de respuesta ante interacciones locales (abrir/cerrar modales, alternar edición, drag-and-drop) deben ser instantáneos y sin saltos visuales bruscos.
- **Privacidad y Seguridad:** Los documentos ingresados en la interfaz de subida no deben transmitirse a destinos no autorizados ni almacenarse sin consentimiento explícito.

---

## 5. Casos de Uso

### CU-01: Visualizar perfil sin skills registradas
- **Actor:** Reclutador / Evaluador.
- **Precondición:** Existe un perfil registrado que no tiene habilidades cargadas.
- **Flujo Principal:**
  1. El usuario navega al detalle del perfil.
  2. El sistema muestra el encabezado con el nombre exacto del perfil y el contador "0 skills configuradas".
  3. El sistema muestra un área central limpia indicando la ausencia de habilidades y ofreciendo el botón "Agregar Skill" / "Agregar primera skill".
- **Resultado:** El usuario tiene total certidumbre del perfil que consulta y puede comenzar la carga de competencias.

### CU-02: Edición de una skill existente
- **Actor:** Reclutador / Evaluador.
- **Precondición:** El perfil contiene al menos una habilidad.
- **Flujo Principal:**
  1. El usuario hace clic sobre una tarjeta de skill.
  2. El sistema despliega el panel de detalle de la habilidad.
  3. El usuario presiona la opción para editar los datos de la habilidad.
  4. Los campos de nombre (máx 50), peso (1 a 100), descripción (máx 250) y criterio (máx 250) se habilitan para edición.
  5. El usuario modifica el puntaje de 10 a 85 y ajusta el criterio de evaluación.
  6. El usuario hace clic en "Guardar cambios".
  7. El sistema valida los límites de caracteres y el rango 1-100.
  8. El sistema actualiza la tarjeta y el detalle con los nuevos valores.
- **Resultado:** La habilidad queda actualizada con los nuevos requerimientos.

### CU-03: Intento de abandono con cambios no guardados
- **Actor:** Reclutador / Evaluador.
- **Precondición:** El usuario está editando una habilidad y ha cambiado texto o peso.
- **Flujo Principal:**
  1. El usuario hace clic en el botón de cerrar ("X") o fuera del panel de edición.
  2. El sistema detecta que existen modificaciones pendientes y bloquea el cierre inmediato.
  3. El sistema muestra una alerta modal estilo Apple con el mensaje: "¿Desea guardar o no guardar?".
  4. El usuario selecciona "No guardar".
  5. El sistema descarta las modificaciones, restituye los valores originales y cierra el panel.
- **Resultado:** No se registran cambios accidentales ni se pierde información involuntariamente.

### CU-04: Carga visual de múltiples CVs
- **Actor:** Reclutador / Evaluador.
- **Precondición:** El usuario accede a la sección de carga de CVs.
- **Flujo Principal:**
  1. El usuario visualiza la zona de arrastrar y soltar con la leyenda "Arrastra y suelta tus CVs aquí o haz clic para explorar".
  2. El usuario arrastra 3 archivos en formato PDF hacia la zona.
  3. La zona reacciona visualmente con un contorno resaltado.
  4. El usuario suelta los archivos.
  5. El sistema presenta la lista de los 3 archivos con sus nombres, peso en KB y un botón para descartar individualmente cualquiera de ellos.
- **Resultado:** El usuario visualiza de forma ordenada los currículums listos para una posterior etapa de procesamiento.

---

## 6. Casos Límite y Manejo de Errores

| Caso Límite | Comportamiento Esperado |
| :--- | :--- |
| El usuario intenta pegar un texto de 300 caracteres en el nombre de la skill (máximo 50). | El sistema trunca o no permite el ingreso más allá de 50 caracteres y señala que se alcanzó el límite. |
| El usuario introduce un puntaje de 0, 101 o valor negativo. | El sistema marca el campo en estado inválido, deshabilita el botón de guardar y muestra un mensaje informando que el valor debe estar entre 1 y 100. |
| El usuario introduce valores no numéricos o decimales en el puntaje. | El sistema rechaza los caracteres no numéricos o redondea estrictamente a número entero entre 1 y 100. |
| La consulta del perfil devuelve éxito pero con un objeto vacío. | El sistema preserva el nombre obtenido de la navegación y muestra el estado vacío sin romper la estructura de la página. |
| El usuario intenta cerrar la edición pero no hizo ninguna modificación. | El sistema cierra inmediatamente el panel sin desplegar la alerta de confirmación. |
| El usuario intenta subir archivos no soportados en la zona de CVs. | El sistema muestra un aviso discreto indicando los formatos admitidos (ej. PDF, DOCX) y no agrega archivos no válidos a la lista. |

---

## 7. Criterios de Aceptación

- [ ] Todos los campos de formulario respetan estrictamente los límites: Nombre perfil (50), Nombre skill (50), Descripción (250), Criterio específico (250).
- [ ] El nombre del perfil se visualiza siempre en el encabezado de `/perfiles/:id`, aun cuando no contenga skills o los datos retornados sean vacíos.
- [ ] Se han retirado los iconos decorativos señalados (pulso/cardiograma, estrella, maletín en empty state), manteniendo la estética minimalista y limpia.
- [ ] Es posible editar interactivamente los campos de una skill desde su panel de detalle y guardar los cambios.
- [ ] Se despliega la alerta "¿Desea guardar o no guardar?" estilo Apple al intentar salir de una edición con cambios pendientes.
- [ ] La escala de peso/prioridad de las skills opera exclusivamente en el rango de números enteros del 1 al 100.
- [ ] La interfaz para subida de CVs cuenta con la zona drag-and-drop con soporte para archivos únicos o múltiples y vista previa de archivos agregados con opción a remover.

---

## 8. Fuera de Alcance

- Lógica de backend para parsing, extracción semántica o ingesta con Inteligencia Artificial de los CVs (reservado para especificaciones posteriores).
- Almacenamiento en base de datos o almacenamiento en la nube de los archivos binarios subidos.
- Autenticación o roles de usuarios para permisos de edición.
