using Bogus;
using Controle_Pessoal.Auth;
using Controle_Pessoal.Context;
using Controle_Pessoal.Entities;
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
        private readonly ApiFixture _apiFixture;

        protected ApiTest(ApiFixture apiFixture)
        {
            _apiFixture = apiFixture;
            _scope = _apiFixture.Services.CreateScope();

            Faker = new Faker("pt_BR");
            ApiClient = _apiFixture.CreateClient();
            ServiceProvider = _scope.ServiceProvider;
            Db = ServiceProvider.GetRequiredService<AppDbContext>();
        }

        protected AppDbContext Db { get; private set; }

        protected HttpClient ApiClient { get; private set; }

        protected Faker Faker { get; private set; } 

        protected IServiceProvider ServiceProvider { get; }

        protected static Faker<T> CreateFaker<T>()
            where T:class
        {
            return new Faker<T>("pt_BR");
        }

        protected static User CreateUser(Action<User>? configure = null)
        {
            var user = CreateFaker<User>()
                .RuleFor(x => x.Name, faker => faker.Person.UserName)
                .RuleFor(x => x.Email, faker => faker.Person.Email)
                .RuleFor(x => x.ProfilePicture, faker => faker.Person.Website)
                .RuleFor(x => x.Password, faker => faker.Internet.Password())
                .Generate();

            configure?.Invoke(user);

            return user;
        }

        protected void SetRequestUser(User user)
        {
            var tokenGenerator = ServiceProvider.GetRequiredService<TokenGenerator>();
            var accessToken = tokenGenerator.GenerateToken(user);
            ApiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        public async ValueTask InitializeAsync()
        {
            await using var conn = new NpgsqlConnection(_apiFixture.DatabaseServerFixture.ConnectionString);
            await conn.ExecuteAsync($"""CREATE DATABASE "{_apiFixture.DatabaseName}" TEMPLATE "{_apiFixture.DatabaseServerFixture.TemplateDataBaseName}";""");
        }

        public async ValueTask DisposeAsync()
        {
            await Db.Database.EnsureDeletedAsync();
            _scope.Dispose();
        }
    }
}
