using System;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/commands/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        public PlatformController()
        {

        }

        [HttpPost]
        public ActionResult Get()
        {
            Console.WriteLine("in command service");
            return Ok("Commad service from platform controller");
        }
    }
}