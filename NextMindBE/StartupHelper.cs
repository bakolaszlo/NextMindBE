namespace NextMindBE
{
    public class StartupHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var originalConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (originalConnectionString != null)
            {
                return originalConnectionString;
            }

            return configuration["ConnectionString"];
        }
    }
}
