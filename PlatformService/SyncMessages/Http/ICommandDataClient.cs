using System.Threading.Tasks;
using PlatformService.DTOs;

namespace PlatformService.SyncMessages.Http
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto plat);
    }
}