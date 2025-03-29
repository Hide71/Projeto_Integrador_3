using Controle_Pessoal.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Controle_Pessoal.Entities;

namespace Controle_Pessoal.Tests.Endpoints.Users
{
    [Trait("Api", "Users - List")]
    public class GetUsersTests : ApiTest
    {
        public GetUsersTests(ApiFixture apiFixture) : base(apiFixture)
        {
        }

        [Fact]
        public async Task DeveRetornarTodosOsUsuarios()
        {
            // Arrange
            User[] users =
            [
                new()
                {
                    Name = Faker.Person.UserName,
                    Email = Faker.Person.Email,
                    ProfilePicture = Faker.Person.Website,
                    Password = Faker.Internet.Password(),
                },
                new()
                {
                    Name = Faker.Person.UserName,
                    Email = Faker.Person.Email,
                    ProfilePicture = Faker.Person.Website,
                    Password = Faker.Internet.Password(),
                },
                new()
                {
                    Name = Faker.Person.UserName,
                    Email = Faker.Person.Email,
                    ProfilePicture = Faker.Person.Website,
                    Password = Faker.Internet.Password(),
                },
            ];

            Db.Users.AddRange(users);
            await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

            // Act
            var httpResponse = await ApiClient.GetAsync("/v1/user", TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            
            var responseBody = await httpResponse.Content.ReadFromJsonAsync<User[]>(TestContext.Current.CancellationToken);
            Assert.Equal(3, responseBody.Length);
        }

        [Fact]
        public async Task DeveRetornarVazio_QuandoNaoExistirUsuariosCadastrados()
        {
            // Act
            var httpResponse = await ApiClient.GetAsync("/v1/user", TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            
            var responseBody = await httpResponse.Content.ReadFromJsonAsync<User[]>(TestContext.Current.CancellationToken);
            Assert.Equal(0, responseBody.Length);
        }
    }
}
