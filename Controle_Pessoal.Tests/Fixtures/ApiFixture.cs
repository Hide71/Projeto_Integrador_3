using Controle_Pessoal.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

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
                { "ConnectionStrings:DefaultConnection", databaseConnectionString },
                { "JwtSettings:Key", "rEtcZr3Mhyo8i9ZPhVne4CggYYwrBWUrLLKomVFKrGACyhhMtBpf3gTk9m3LrQRv8M4BKq" },
                { "JwtSettings:Audience", "integration_tests" },
                { "JwtSettings:Issuer", "integration_tests" },
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues!)
                .Build();

            builder
                .UseEnvironment("IntegrationTest")
                .UseConfiguration(configuration)
                .ConfigureAppConfiguration(c =>
                {
                    c.Sources.Clear();
                    c.AddInMemoryCollection(configurationValues!).Build();
                })
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton(x => Substitute.For<IGoogleAccessTokenManager>());
                });
        }
    }
}
