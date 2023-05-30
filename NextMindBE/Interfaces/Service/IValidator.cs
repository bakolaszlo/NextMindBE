namespace NextMindBE.Interfaces.Service
{
    public interface IValidator<T>
    {
        bool ValidateData(List<T> data, string sessionId);
    }
}
