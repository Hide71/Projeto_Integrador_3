using Controle_Pessoal.Tests.Fixtures;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Controle_Pessoal.Auth;
using Controle_Pessoal.Entities;

namespace Controle_Pessoal.Tests.Endpoints.Users
{
    [Trait("Api", "Users - Create")]
    public class CreateUserTests : ApiTest
    {
        public CreateUserTests(ApiFixture apiFixture) : base(apiFixture)
        {
        }

        [Fact]
        public async Task DeveCriarUsuarioERetornarCreated_QuandoRequisicaoForValida()
        {
            // Arrange
            var request = new
            {
                name = Faker.Person.UserName,
                email = Faker.Person.Email,
                password = Faker.Internet.Password()
            };

            // Act
            var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user", request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.NotNull(httpResponse.Headers.Location);
            Assert.Equal("v1/users/1", httpResponse.Headers.Location.ToString());

            var createdUser = await Db.Users.FirstAsync(TestContext.Current.CancellationToken);
            Assert.Equal(1, createdUser.Id);
            Assert.Equal(request.name, createdUser.Name);
            Assert.Equal(request.email, createdUser.Email);
            Assert.NotEmpty(createdUser.ProfilePicture);
            Assert.Equal(PasswordHasher.HashPassword(request.password), createdUser.Password);
        }

        [Fact]
        public async Task DeveRetornarBadRequest_QuandoJaExistirUsuarioComOEmailInformado()
        {
            // Arrange
            var user = CreateFaker<User>()
                .RuleFor(x => x.Name, faker => faker.Person.UserName)
                .RuleFor(x => x.Email, faker => faker.Person.Email)
                .RuleFor(x => x.ProfilePicture, faker => faker.Person.Website)
                .RuleFor(x => x.Password, faker => faker.Internet.Password())
                .Generate();

            Db.Users.AddRange(user);
            await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
            Db.ChangeTracker.Clear();

            var request = new
            {
                name = Faker.Person.UserName,
                email = user.Email,
                password = Faker.Internet.Password()
            };

            // Act
            var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user", request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            
            var responseBody = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Equal("Já existe um usuário cadastrado com o e-mail informado", responseBody);
        }

        [Fact]
        public async Task DeveRetornarBadRequest_QuandoRequisicaoForValida()
        {
            // Arrange
            var request = new
            {
                username = "",
                email = "",
                url = ""
            };

            // Act
            var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user", request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }
    }
}
