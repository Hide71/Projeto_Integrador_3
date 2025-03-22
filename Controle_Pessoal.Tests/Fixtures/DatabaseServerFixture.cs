using Controle_Pessoal.Context;
using Controle_Pessoal.Tests.Fixtures;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(DatabaseServerFixture))]

namespace Controle_Pessoal.Tests.Fixtures
{
    public sealed class DatabaseServerFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer;

        public DatabaseServerFixture()
        {
            _postgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:alpine")
                .WithUsername("test")
                .WithPassword("test")
                .WithName("integration_tests_postgresql_db")
                .WithPortBinding(5431, 5432)
                .WithTmpfsMount("/var/lib/pg/data")
                .WithCommand("--log_statement=all")
                .WithPortBinding(5430, 5432)
                .WithReuse(true)
                .Build();

            TemplateDataBaseName = Guid.NewGuid().ToString();
        }

        public string ConnectionString { get; private set; }

        public string TemplateDataBaseName { get; }

        public async ValueTask InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();
            ConnectionString = _postgreSqlContainer.GetConnectionString();

            var templateDatabaseConnectionString = ConnectionString.Replace("Database=postgres", $"Database={TemplateDataBaseName}");
            templateDatabaseConnectionString += ";Pooling=false";

            var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(templateDatabaseConnectionString).Options);
            await db.Database.EnsureCreatedAsync();

            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.ExecuteAsync($"""ALTER DATABASE "{TemplateDataBaseName}" is_template=true;""");
        }

        public async ValueTask DisposeAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.ExecuteAsync($"""ALTER DATABASE "{TemplateDataBaseName}" is_template=false;""");
            await conn.ExecuteAsync($"""DROP DATABASE IF EXISTS "{TemplateDataBaseName}";""");
            await _postgreSqlContainer.DisposeAsync();
        }
    }
}
