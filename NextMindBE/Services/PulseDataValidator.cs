using NextMindBE.Interfaces.Service;

namespace NextMindBE.Services
{
    public class PulseDataValidator : IValidator<float>
    {

        private const float LOCKOUT_TRESHOLD = 30f;
        public bool ValidateData(List<float> data, string _)
        {
            if(data.Any(data => data == 0)) { return false; }
            bool hasDifferenceGreaterThanThreshold = data.Any(x =>
                                                          data.Any(y => Math.Abs(x - y) > LOCKOUT_TRESHOLD));
            return !hasDifferenceGreaterThanThreshold;
        }
    }
}
