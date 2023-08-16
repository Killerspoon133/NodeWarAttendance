namespace AttendanceApp
{
    using System.Configuration;
    using System.IO.Abstractions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup() 
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
    }
}
