using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext context;
        public CommandRepo(AppDbContext context)
        {
            this.context = context;
        }
        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            this.context.Commands.Add(command);
        }

        public void CreatePlatform(Platform plat)
        {
            if (plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            this.context.Platforms.Add(plat);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return this.context.Platforms.Any(p => p.ExternalID == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return this.context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return this.context.Commands
               .Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return this.context.Commands
               .Where(c => c.PlatformId == platformId)
               .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExits(int platformId)
        {
            return this.context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (this.context.SaveChanges() >= 0);
        }
    }
}