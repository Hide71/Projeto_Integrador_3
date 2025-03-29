using Controle_Pessoal.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Controle_Pessoal.Tests.Endpoints.Categories
{
    [Trait("Api", "Categories - Create")]
    public class CreateCategoryTests : ApiTest
    {
        public CreateCategoryTests(ApiFixture apiFixture) : base(apiFixture)
        {
        }

        [Fact]
        public async Task DeveCriarCategoriaERetornarCreated_QuandoRequisicaoForValida()
        {
            // Arrange
            var user = CreateUser();

            Db.Users.Add(user);
            await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
            Db.ChangeTracker.Clear();

            SetRequestUser(user);

            var request = new
            {
                categoryname = Faker.Commerce.Categories(1)[0],
            };

            // Act
            var httpResponse = await ApiClient.PostAsJsonAsync("/v1/category", request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.NotNull(httpResponse.Headers.Location);
            Assert.Equal("v1/categories/1", httpResponse.Headers.Location.ToString());

            Db.ChangeTracker.Clear();

            var categories = await Db.Categories.ToListAsync(TestContext.Current.CancellationToken);
            Assert.Single(categories);

            Assert.Equal(1, categories[0].Id);
            Assert.Equal(request.categoryname, categories[0].CategoryName);
            Assert.Equal(user.Id, categories[0].UserId);
        }
    }
}
