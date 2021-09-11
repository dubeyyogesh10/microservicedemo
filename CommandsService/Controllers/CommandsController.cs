using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/commands/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo repo;
        private readonly IMapper map;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            repo = repository;
            map = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"---> hit GetCommandsForPlatform " + platformId);

            if (!repo.PlatformExits(platformId))
            {
                return NotFound();
            }

            var commands = repo.GetCommandsForPlatform(platformId);
            return Ok(map.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            if (!repo.PlatformExits(platformId))
            {
                return NotFound();
            }

            var command = repo.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(map.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            if (!repo.PlatformExits(platformId))
            {
                return NotFound();
            }

            var command = map.Map<Command>(commandDto);

            repo.CreateCommand(platformId, command);
            repo.SaveChanges();

            var commandReadDto = map.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}