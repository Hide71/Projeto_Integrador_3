using Controle_Pessoal.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace Controle_Pessoal.Tests.Endpoints.Users
{
    [Trait("Api", "Users - Create")]
    public class CreateUserTests : ApiTest
    {
        public CreateUserTests(ApiFixture monnyApiFixture) : base(monnyApiFixture)
        {
        }

        [Fact]
        public async Task DeveCriarUsuarioERetornarCreated_QuandoRequisicaoForValida()
        {
            // Arrange
            var request= new
            {
                username = Faker.Person.UserName,
                email = Faker.Person.Email,
                url = Faker.Person.Website
            };

            // Act
            var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user", request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.Equal("v1/users/1", httpResponse.Headers.Location.ToString());
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
