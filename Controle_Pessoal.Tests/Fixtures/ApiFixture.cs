using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Controle_Pessoal.Tests.Fixtures
{
    public sealed class ApiFixture : WebApplicationFactory<Program>
    {
        private readonly DatabaseServerFixture _databaseServerFixture;

        public ApiFixture(DatabaseServerFixture databaseServerFixture)
        {
            _databaseServerFixture = databaseServerFixture;
        }

        public string DatabaseName { get; private set; }

        public DatabaseServerFixture DatabaseServerFixture => _databaseServerFixture;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            DatabaseName = Guid.NewGuid().ToString();
            var databaseConnectionString = _databaseServerFixture.ConnectionString;
            databaseConnectionString = databaseConnectionString.Replace("Database=postgres", $"Database={DatabaseName}");

            var configurationValues = new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", databaseConnectionString }
            };  

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues!)
                .Build();

            builder
                //.UseEnvironment(EnvironmentDefaults.IntegrationTest)
                .UseConfiguration(configuration)
                .ConfigureAppConfiguration(c =>
                {
                    c.Sources.Clear();
                    c.AddInMemoryCollection(configurationValues!).Build();
                });
        }
    }
}
