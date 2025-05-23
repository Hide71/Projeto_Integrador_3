using System;
using System.Net;
using System.Net.Http.Json;
using Bogus;
using Controle_Pessoal.Entities;
using Controle_Pessoal.Tests.Fixtures;

namespace Controle_Pessoal.Tests.Endpoints.Users;

[Trait("Api", "Users - Get")]
public class GetUserTests : ApiTest
{
    public GetUserTests(ApiFixture apiFixture) : base(apiFixture)
    {
    }

    [Fact]
    public async Task DeveRetornar200_QuandoUsuarioExistir()
    {
        // Arrange
        var users = CreateFaker<User>()
            .RuleFor(x => x.Name, faker => faker.Person.UserName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.ProfilePicture, faker => faker.Person.Website)
            .RuleFor(x => x.Password, faker => faker.Internet.Password())
            .Generate(3);

        Db.Users.AddRange(users);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var httpResponse = await ApiClient.GetAsync("/v1/user/2", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        var responseBody = await httpResponse.Content.ReadFromJsonAsync<User>(TestContext.Current.CancellationToken);
        Assert.Equal(2, responseBody.Id);
        Assert.Equal(users[1].Name, responseBody.Name);
        Assert.Equal(users[1].Email, responseBody.Email);
        Assert.Equal(users[1].ProfilePicture, responseBody.ProfilePicture);
    }

    [Fact]
    public async Task DeveRetornar404_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var users = CreateFaker<User>()
            .RuleFor(x => x.Name, faker => faker.Person.UserName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.ProfilePicture, faker => faker.Person.Website)
            .RuleFor(x => x.Password, faker => faker.Internet.Password())
            .Generate(2);

        Db.Users.AddRange(users);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var httpResponse = await ApiClient.GetAsync("/v1/user/100", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }
}
