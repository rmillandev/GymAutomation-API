# 🏋️‍♂️ GymAuto-API

**GymAuto-API** es el motor de orquestación para la automatización de rutinas de entrenamiento. Esta Web API, construida sobre **.NET Core**, utiliza Inteligencia Artificial (Gemini) para interpretar el progreso del usuario y sincronizarlo automáticamente con **Google Calendar**.



## 🛠️ Tecnologías Principales
- **Framework:** .NET (ASP.NET Core)
- **IA:** Google Gemini 2.5 Flash
- **Integración:** Google Calendar API
- **Lenguaje:** C#

## 🚀 Flujo de Trabajo
1. La API recibe un string de texto desde la app móvil (ej. "Subele 5kg en Sentadillas para la proxima semana").
2. **Gemini Service:** Procesa el texto y extrae una estructura JSON con el ejercicio, peso y repeticiones.
3. **Calendar Service:** Busca la rutina programada para la próxima semana en Google Calendar.
4. **Update:** Modifica la descripción del evento con los nuevos objetivos de carga.
