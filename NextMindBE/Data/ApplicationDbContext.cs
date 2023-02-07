using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using NextMindBE.Model;

namespace NextMindBE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<SensorData> SensorData { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<SensorOnCalibrationEnd> SensorOnCalibrationEnd { get; set; }
        public DbSet<SessionHistory> SessionHistory { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the value converter for the Animal
            modelBuilder.Entity<SensorOnCalibrationEnd>()
            .Property(x => x.SensorValues)
            .HasConversion(new ValueConverter<float[], string>(
                v => JsonConvert.SerializeObject(v), // Convert to string for persistence
                v => JsonConvert.DeserializeObject<float[]>(v))); // Convert to List<String> for use

            modelBuilder.Entity<SensorData>()
            .Property(x => x.SensorValues)
            .HasConversion(new ValueConverter<float[], string>(
                v => JsonConvert.SerializeObject(v), // Convert to string for persistence
                v => JsonConvert.DeserializeObject<float[]>(v))); // Convert to List<String> for use

        }

    }
}
