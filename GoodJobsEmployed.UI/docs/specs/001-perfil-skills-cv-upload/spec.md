# Especificación Técnica: Perfeccionamiento de Perfil, Edición de Skills y Estructura Visual de Subida de CVs

- **Identificador:** 001-perfil-skills-cv-upload
- **Estado:** aprobado
- **Fecha de Creación:** 2026-11-06
- **Fecha de Aprobación:** 2026-11-06
- **Metodología:** SDD (Spec-Driven Development)

---

## 1. Propósito y Contexto

El sistema de seguimiento y evaluación de candidatos requiere una experiencia de usuario predecible, accesible y visualmente depurada bajo principios de diseño minimalista estilo Apple. Tras la revisión y aprobación con las clarificaciones de la Fase 2, se fijan las reglas operativas finales para la gestión de perfiles, modificación de competencias (skills) y recepción visual de currículums (CVs).

Esta especificación consolida de forma exhaustiva el QUÉ y el POR QUÉ del sistema, cubriendo límites rígidos de caracteres, normalización del puntaje a enteros de 1 a 100, continuidad contextual del perfil aun sin datos asociados, remoción de iconografía decorativa superflua, edición interactiva con resguardo ante abandono involuntario, y la interfaz visual para recepción de CVs en formatos PDF y DOCX.

---

## 2. Objetivos

- **Integridad y concisión de datos:** Imponer longitudes máximas y requerimientos mínimos en nombres (50 caracteres máx., mínimo 1 carácter no vacío) y descripciones/criterios (250 caracteres máx., opcionales).
- **Continuidad de contexto y resiliencia:** Garantizar que el encabezado del perfil permanezca siempre identificado inequívocamente al navegar a `/perfiles/:id`, desplegando "Perfil #[id]" como fallback si la API retorna una colección vacía.
- **Sobriedad visual y estética Apple:** Eliminar únicamente los iconos decorativos redundantes (pulso/cardiograma, estrella, maletín), preservando los iconos funcionales limpios (cerrar "X", eliminar archivo en subida, editar).
- **Edición interactiva protegida:** Permitir la edición directa de competencias existentes desde su panel de detalle, salvaguardando los datos mediante una alerta modal estilo Apple con tres acciones explícitas ("Guardar", "No guardar", "Cancelar") ante intentos de salida con cambios pendientes.
- **Normalización estricta de ponderación:** Operar el peso/prioridad exclusivamente con números enteros comprendidos entre 1 y 100, denegando y explicando cualquier valor decimal o fuera de rango.
- **Habilitación de carga visual de CVs:** Proveer una zona visual de arrastrar y soltar (drag-and-drop) compatible con archivos PDF y DOCX, exhibiendo el tamaño en KB/MB y botón para remover elementos de forma individual.

---

## 3. Requisitos Funcionales (Sintaxis EARS)

### 3.1 Límites de Longitud de Entrada

- **REQ-LEN-01 (Ubiquitous):** El sistema debe aplicar truncado nativo mediante el atributo `maxlength="50"` en los campos de entrada de nombres de perfil y nombres de skill/característica en formularios de creación y edición.
- **REQ-LEN-02 (Ubiquitous):** El sistema debe requerir que los nombres de perfil y nombres de habilidad contengan como mínimo un carácter no vacío (no admitir cadenas de puros espacios en blanco).
- **REQ-LEN-03 (Ubiquitous):** El sistema debe aplicar truncado nativo mediante el atributo `maxlength="250"` en los campos de descripción de habilidad y criterio específico de evaluación.
- **REQ-LEN-04 (Ubiquitous):** El sistema debe tratar los campos de descripción y criterio específico como opcionales durante la captura y edición de habilidades.
- **REQ-LEN-05 (Event-driven):** CUANDO el usuario alcance los límites de caracteres permitidos (50 o 250), el sistema debe bloquear físicamente la captura adicional de caracteres.
- **REQ-LEN-06 (Event-driven):** CUANDO el usuario interactúe con los campos con restricción de longitud, el sistema debe presentar un indicador o límite nativo claro que garantice que no se exceda el total permitido.

### 3.2 Persistencia y Fallback del Nombre del Perfil

- **REQ-PRF-01 (Ubiquitous):** El sistema debe mantener visible el nombre del perfil en el encabezado principal de la vista de detalle en todo momento.
- **REQ-PRF-02 (State-driven):** MIENTRAS la información de características del perfil esté vacía o no contenga habilidades asociadas, el sistema debe presentar el nombre del perfil en el encabezado superior y mostrar la invitación "Viene vacío. Quiere agregar un nuevo skill" junto a los botones de acción correspondientes.
- **REQ-PRF-03 (Event-driven):** CUANDO se acceda directamente mediante URL a `/perfiles/:id` y la respuesta del servicio devuelva éxito pero con una lista de objetos vacía (`objects: []`), el sistema debe desplegar "Perfil #[id]" en el encabezado principal como fallback para evitar que el título quede en blanco.
- **REQ-PRF-04 (State-driven):** MIENTRAS el perfil posea un nombre válido retornado por el servicio o conservado en la navegación, el sistema debe privilegiar dicho nombre sobre el texto de fallback.

### 3.3 Limpieza Visual e Iconográfica

- **REQ-VIS-01 (Ubiquitous):** El sistema debe suprimir el icono decorativo de pulso/cardiograma en las tarjetas de habilidad de la cuadrícula.
- **REQ-VIS-02 (Ubiquitous):** El sistema debe suprimir el icono decorativo de estrella en el panel de detalle de la habilidad seleccionada.
- **REQ-VIS-03 (Ubiquitous):** El sistema debe suprimir el icono decorativo de maletín en los estados vacíos de perfiles y habilidades.
- **REQ-VIS-04 (Ubiquitous):** El sistema debe conservar únicamente los iconos con valor funcional indispensable y estilo limpio (botón de cerrar "X", acción de eliminar archivo en subida y botón de editar "lápiz").

### 3.4 Edición Interactiva de Skills

- **REQ-SKL-01 (Event-driven):** CUANDO el usuario visualice el panel de detalle de una habilidad seleccionada, el sistema debe ofrecer un botón funcional de edición ("Editar").
- **REQ-SKL-02 (Event-driven):** CUANDO el usuario active el modo de edición, los campos de nombre (máx. 50), peso/puntaje (1 a 100), descripción (máx. 250) y criterio específico (máx. 250) deben transformarse en controles editables reflejando los valores actuales.
- **REQ-SKL-03 (Event-driven):** CUANDO el usuario presione "Guardar cambios", el sistema debe validar las restricciones de datos y propagar la actualización persistente del perfil.
- **REQ-SKL-04 (Unwanted event):** SI alguno de los campos modificados infringe el nombre obligatorio no vacío o el rango de puntaje entero, ENTONCES el sistema debe cancelar el guardado y marcar explícitamente el error en la interfaz.

### 3.5 Alerta de Confirmación de Cambios sin Guardar (Estilo Apple)

- **REQ-ALT-01 (Event-driven):** CUANDO el usuario intente cerrar el panel de detalle o abandonar la edición habiendo alterado uno o más valores sin haberlos guardado, el sistema debe interceptar la acción y desplegar una alerta estilo Apple con la leyenda "¿Desea guardar o no guardar?".
- **REQ-ALT-02 (Ubiquitous):** La alerta de confirmación debe disponer de tres opciones explícitas de acción: "Guardar", "No guardar" y "Cancelar".
- **REQ-ALT-03 (Event-driven):** CUANDO el usuario elija "Guardar" en la alerta, el sistema debe validar los datos, persistir los cambios de la habilidad y salir del modo de edición.
- **REQ-ALT-04 (Event-driven):** CUANDO el usuario elija "No guardar" en la alerta, el sistema debe revertir todos los cambios a sus valores originales, descartar las modificaciones y salir del modo de edición.
- **REQ-ALT-05 (Event-driven):** CUANDO el usuario elija "Cancelar" en la alerta, el sistema debe cerrar la alerta y permanecer en el modo de edición manteniendo intactos los cambios efectuados por el usuario.

### 3.6 Escala Estricta de Puntajes (Enteros 1 a 100)

- **REQ-SCR-01 (Ubiquitous):** El sistema debe normar la escala de ponderación de habilidades estrictamente como números enteros dentro del intervalo de 1 a 100.
- **REQ-SCR-02 (Event-driven):** CUANDO se capture o edite el puntaje de una habilidad, el sistema debe admitir solo valores enteros comprendidos entre 1 y 100.
- **REQ-SCR-03 (Unwanted event):** SI el usuario ingresa un valor fraccionario o decimal (por ejemplo 85.5) o fuera del rango 1-100, ENTONCES el sistema debe denegar el registro y mostrar el mensaje explicativo: "El puntaje debe ser un número entero entre 1 y 100 (no se permiten decimales)".
- **REQ-SCR-04 (Ubiquitous):** El sistema debe formatear visualmente el puntaje en tarjetas y paneles con el sufijo "pts" (ejemplo: "85 pts").

### 3.7 Interfaz Visual para Subida de CVs

- **REQ-UPL-01 (Ubiquitous):** El sistema debe disponer de una sección visual accesible desde la navegación principal a través de la ruta `/subir-cvs`.
- **REQ-UPL-02 (Ubiquitous):** El sistema debe integrar una zona de arrastrar y soltar ("drag-and-drop") con estética minimalista de Apple, bordes suaves y respuesta reactiva ante el arrastre activo.
- **REQ-UPL-03 (Ubiquitous):** El sistema debe admitir la selección unitaria o múltiple de documentos exclusivamente en formatos PDF (.pdf) y Word (.docx).
- **REQ-UPL-04 (Unwanted event):** SI el usuario intenta soltar o cargar un archivo de extensión diferente a .pdf o .docx, ENTONCES el sistema debe omitir dicho archivo y presentar un aviso discreto indicando los formatos válidos.
- **REQ-UPL-05 (Event-driven):** CUANDO los archivos sean recibidos satisfactoriamente, el sistema debe desplegar un listado visual conteniendo: nombre completo del archivo, indicador de tamaño legible expresado en KB o MB, y un botón individual funcional para remover el archivo de la lista.
- **REQ-UPL-06 (Ubiquitous):** La vista de subida de CVs debe limitarse a la gestión visual en cliente, sin desencadenar procesamiento de IA ni transmisión de datos sensibles a endpoints externos en esta etapa.

---

## 4. Requisitos No Funcionales

- **Estética y Diseño:** Apego riguroso al diseño minimalista estilo Apple: fondo principal blanco (`#ffffff`), tipografía sans-serif Barlow Condensed, acento de interacción verde (`#29b765`), bordes redondeados (`border-radius: 12px` y `16px`) y microinteracciones fluidas.
- **Modo Oscuro:** Compatibilidad en contraste con texto blanco y fondo gris oscuro (no negro plano).
- **Idioma Oficial:** Toda la interfaz, etiquetas, mensajes de error, confirmaciones y textos informativos deben formularse en español.
- **Rendimiento:** Transiciones locales instantáneas, validaciones reactivas en memoria y cero saltos de renderizado.
- **Privacidad:** No persistir ni filtrar contenido sensible de los archivos de currículum en el cliente.

---

## 5. Casos Límite y Manejo de Errores

| Caso Límite | Comportamiento Esperado |
| :--- | :--- |
| El usuario intenta ingresar solo espacios en blanco en el nombre de la skill o perfil. | El sistema no permite el guardado y señala que el campo debe contener al menos un carácter visible. |
| El usuario intenta pegar un texto de más de 50 caracteres en el nombre. | El sistema trunca físicamente el texto a 50 caracteres gracias a `maxlength="50"`. |
| El usuario ingresa 0, 101, números negativos o letras en el puntaje. | El sistema rechaza la entrada y resalta el error informativo de rango 1 a 100. |
| El usuario ingresa un decimal en el puntaje (ej. 99.9). | El sistema bloquea el guardado informando que no se admiten decimales. |
| Se navega a `/perfiles/:id` y el servicio no tiene registro del nombre. | Se presenta "Perfil #[id]" como título predeterminado en el encabezado. |
| Se intenta arrastrar una imagen o ejecutable a la zona de subida de CVs. | El sistema descarta el archivo y alerta amigablemente que solo se admiten formatos PDF y DOCX. |

---

## 6. Criterios de Aceptación

- [ ] Los campos de nombres cuentan con `maxlength="50"` y validación de mínimo 1 carácter no vacío en perfiles y skills.
- [ ] Los campos de descripción y criterio cuentan con `maxlength="250"` y son opcionales.
- [ ] Se muestra "Perfil #[id]" en el encabezado cuando se ingresa directamente a `/perfiles/:id` y la respuesta de datos no contiene elementos.
- [ ] Se han retirado los iconos de cardiograma/pulso en tarjetas, estrella en detalle y maletín en empty state.
- [ ] Se mantienen los iconos funcionales limpios: cerrar ("X"), eliminar archivo en subida y editar ("lápiz").
- [ ] Se puede activar el modo edición de una skill desde su panel de detalle y guardar los cambios.
- [ ] Se despliega la alerta modal estilo Apple "¿Desea guardar o no guardar?" con 3 acciones: "Guardar", "No guardar" y "Cancelar" ante cambios sin guardar.
- [ ] La ponderación de skills rechaza números decimales y valida estrictamente enteros del 1 al 100.
- [ ] Existe la vista `/subir-cvs` con zona drag-and-drop, soporte para PDF y DOCX, visualización de peso en KB/MB y botón para remover archivos.

---

## 7. Fuera de Alcance

- Endpoints de backend para procesamiento semántico o ingesta automática con Inteligencia Artificial.
- Persistencia de los archivos binarios subidos en disco o servicios cloud en esta fase.
- Manejo de roles de usuario o autenticación para privilegios de edición.
