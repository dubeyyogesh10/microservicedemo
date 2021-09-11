using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/commands/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly ICommandRepo repo;
        private readonly IMapper map;

        public PlatformController(ICommandRepo repo, IMapper map)
        {
            this.repo = repo;
            this.map = map;
        }


        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms from CommandsService");

            var platformItems = this.repo.GetAllPlatforms();

            return Ok(this.map.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");

            return Ok("Inbound test of from Platforms Controller");
        }
    }
}