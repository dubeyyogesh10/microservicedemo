namespace CommandsService.EventProcessing
{
    public interface IEventProcessing
    {
        void ProcessEvent(string message);
    }
}