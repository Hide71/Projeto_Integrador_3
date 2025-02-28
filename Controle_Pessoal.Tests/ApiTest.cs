using Bogus;
using Controle_Pessoal.Context;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Controle_Pessoal.Tests.Fixtures
{
    public abstract class ApiTest :
        IClassFixture<ApiFixture>,
        IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        private readonly ApiFixture _monnyApiFixture;

        protected ApiTest(ApiFixture monnyApiFixture)
        {
            _monnyApiFixture = monnyApiFixture;
            _scope = _monnyApiFixture.Services.CreateScope();

            Faker = new Faker("pt_BR");
            ApiClient = _monnyApiFixture.CreateClient();
            ServiceProvider = _scope.ServiceProvider;
            Db = ServiceProvider.GetRequiredService<AppDbContext>();
        }

        protected AppDbContext Db { get; private set; }

        protected HttpClient ApiClient { get; private set; }

        protected Faker Faker { get; private set; } 

        protected IServiceProvider ServiceProvider { get; }

        public async ValueTask InitializeAsync()
        {
            await using var conn = new NpgsqlConnection(_monnyApiFixture.DatabaseServerFixture.ConnectionString);
            await conn.ExecuteAsync($"""CREATE DATABASE "{_monnyApiFixture.DatabaseName}" TEMPLATE "{_monnyApiFixture.DatabaseServerFixture.TemplateDataBaseName}";""");
        }

        public async ValueTask DisposeAsync()
        {
            await Db.Database.EnsureDeletedAsync();
            _scope.Dispose();
        }
    }
}
