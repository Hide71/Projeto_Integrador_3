using System;
using System.Net;
using System.Net.Http.Json;
using Controle_Pessoal.Entities;
using Controle_Pessoal.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Controle_Pessoal.Tests.Endpoints.Users;

[Trait("Api", "Users - Update")]
public class UpdateUserTests : ApiTest
{
    public UpdateUserTests(ApiFixture apiFixture) : base(apiFixture)
    {
    }

    [Fact]
    public async Task DeveAtualizarUsuarioERetornar200_QuandoEleExistir()
    {
        // Arrange
        var user = CreateFaker<User>()
            .RuleFor(x => x.Username, faker => faker.Person.UserName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.Url, faker => faker.Person.Website)
            .RuleFor(x => x.Password, faker => faker.Internet.Password())
            .Generate();

        Db.Users.AddRange(user);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var request = new
        {
            username = Faker.Person.UserName,
            email = Faker.Person.Email,
            url = Faker.Person.Website
        };

        // Act
        var httpResponse = await ApiClient.PutAsJsonAsync($"/v1/user/{user.Id}", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        Db.ChangeTracker.Clear();

        var updatedUser = await Db.Users.FirstAsync(TestContext.Current.CancellationToken);
        Assert.Equal(user.Id, updatedUser.Id);
        Assert.Equal(request.username, updatedUser.Username);
        Assert.Equal(request.email, updatedUser.Email);
        Assert.Equal(request.url, updatedUser.Url);
    }

    [Fact]
    public async Task DeveRetornar400_QuandoARequisicaoForInvalida()
    {
        // Arrange        
        var request = new
        {
            username = "",
            email = Faker.Person.Email,
            url = Faker.Person.Website
        };

        // Act
        var httpResponse = await ApiClient.PutAsJsonAsync("/v1/user/1", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    [Fact]
    public async Task DeveRetornar404_QuandoUsuarioNaoExistir()
    {
        // Arrange        
        var request = new
        {
            username = Faker.Person.UserName,
            email = Faker.Person.Email,
            url = Faker.Person.Website
        };

        // Act
        var httpResponse = await ApiClient.PutAsJsonAsync("/v1/user/1", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }
}
