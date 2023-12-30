namespace EventEmitter.EventEmitter
{
    internal interface IEventEmitter
    {
        Task EmitAsync<T>(T payload);
        Task EmitAsync<T>(IEnumerable<T> payloads);
    }
}
