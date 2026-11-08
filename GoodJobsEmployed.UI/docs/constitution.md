# Constitución del Proyecto: Frontend Lector de CV

1. **Stack Minimalista:** Usar exclusivamente Angular, TypeScript, CSS y Bootstrap nativo; prohibido añadir dependencias o frameworks adicionales.
2. **Spec-Driven:** Ningún cambio de código se realiza sin una especificación/plan previo aprobado; la implementación debe reflejar fielmente la spec.
3. **Desacoplamiento Lógica/UI:** Los componentes solo gestionan la presentación visual; la lógica de datos, estado (Signals) y APIs residen en servicios.
4. **Tests sin Dependencias:** Validar mediante compilación estricta (`ng build`), tipado de TypeScript y pruebas nativas existentes sin instalar herramientas extra.
5. **Privacidad del Candidato:** Los CVs y datos personales son estrictamente confidenciales; nunca se persisten, exponen ni loguean en el cliente.
6. **Idioma Oficial:** Toda la interfaz de usuario, textos, mensajes de error, comentarios y documentación se redactan en español.
