using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GymAutomationApi.Configuration;
using GymAutomationApi.Interfaces;
using GymAutomationApi.Models;
using GymAutomationApi.Utils.Exceptions;
using Microsoft.Extensions.Options;
using System.Text;

namespace GymAutomationApi.Services
{
    public class GoogleCalendarService : ICalendarService
    {

        private readonly GoogleConfig googleConfig;
        private readonly ILogger<GoogleCalendarService> logger;

        // El "Scope" define QUÉ permisos pedimos. 
        // 'CalendarEvents' permite leer y escribir eventos en tu calendario.
        private readonly string[] Scopes = { CalendarService.Scope.CalendarEvents };


        public GoogleCalendarService(IOptions<GoogleConfig> googleConfig, ILogger<GoogleCalendarService> logger)
        {
            this.googleConfig = googleConfig.Value;
            this.logger = logger;
        }

        private Event createEvent(WorkoutExtraction workoutExtraction)
        {
            var muscleRoutine = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Piernas y ABS" },
                { DayOfWeek.Tuesday, "Espalda y Bíceps" },
                { DayOfWeek.Wednesday, "Pecho, tríceps y laterales" },
                { DayOfWeek.Thursday, "Piernas y ABS" },
                { DayOfWeek.Friday, "Espalda y Bíceps" },
                { DayOfWeek.Saturday, "Pecho, tríceps y laterales" }
            };

            var today = DateTime.Now.DayOfWeek;

            var eventTitle = muscleRoutine.TryGetValue(today, out var routine)
                ? routine
                : "Descanso";

            var description = new StringBuilder();

            for (int i = 0; i < workoutExtraction.Exercises.Count; i++)
            {
                var ex = workoutExtraction.Exercises[i];
                description.AppendLine($"{i + 1}. {ex.Exercise}:");
                foreach (var s in ex.Sets)
                {
                    description.AppendLine($" - {s.SetNumber} serie - {s.Reps} repeticiones - {s.Weight}kg");
                }
            }

            var newEvent = new Event()
            {
                Summary = eventTitle,
                Description = description.ToString()
            };

            return newEvent;
        }

        public async Task<CalendarResponse> UpdateCalendarEvent(WorkoutExtraction processedData)
        {
            if (processedData == null) throw new BadRequestException("The object cannot empty.");

            logger.LogInformation("Updating Google Calendar event.");

            try
            {
                // Credenciales del Proyecto en Google Cloud
                var clientSecrets = new ClientSecrets
                {
                    ClientId = googleConfig.ClientId,
                    ClientSecret = googleConfig.ClientSecret
                };

                // El 'GoogleWebAuthorizationBroker'
                // Si no hay token, abre el navegador. Si hay token, lo refresca solo.
                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)
                );

                // Creacion del servicio del calendario para inicialiazar
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = googleConfig.ApplicationName
                });

                // Se crea el evento
                Event newEvent = createEvent(processedData);

                await service.Events.Insert(newEvent, "primary").ExecuteAsync();

                return new CalendarResponse
                {
                    Success = true,
                    Message = "Evento de entrenamiento agregado al calendario de Google."
                };
            }
            catch (GoogleApiException ex)
            {
                logger.LogError(ex, "Google Calendar API error. Message: {Message}", ex.Message);
                throw new ExternalServiceException("Failed to create Google Calendar event.");
            }
        }
    }
}
