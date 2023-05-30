namespace NextMindBE.Interfaces.Service
{
    public interface IProcessingService
    {
        public bool ProcessPayload(HttpRequest request, byte[] payload, PayloadType payloadType);
    }
}
