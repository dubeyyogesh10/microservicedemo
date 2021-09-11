using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessing
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IMapper map;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper map)
        {
            this.scopeFactory = scopeFactory;
            this.map = map;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    this.addPlatform(message);
                    break;

                default:
                    Console.WriteLine(" ----> could not determine event type");
                    break;
            }
        }

        private void addPlatform(string platformPublishMessage)
        {
            using (var scope = this.scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformpublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishMessage);

                try
                {
                    var plat = this.map.Map<Platform>(platformpublishedDto);

                    if (!repo.ExternalPlatformExists(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine($" ----> Platform added -> {plat.Name}");
                    }
                    else
                    {
                        Console.WriteLine($" ----> Platform already Exist -> {plat.Name}");
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($" ----> could not add platform to DB -> {ex.Message}");
                }
            }
        }

        private EventType DetermineEvent(string notifyMessage)
        {
            Console.WriteLine("Determing the EVENT ---------........");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notifyMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("Platform Published event type detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine(" ----> could not determine event type");
                    return EventType.Undetermined;
            }
        }
    }
    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}