using System;
using System.Net;
using Controle_Pessoal.Entities;
using Controle_Pessoal.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Controle_Pessoal.Tests.Endpoints.Users;

[Trait("Api", "Users - Delete")]
public class DeleteUserTests : ApiTest
{
    public DeleteUserTests(ApiFixture apiFixture) : base(apiFixture)
    {
    }

    [Fact]
    public async Task DeveExcluirUsarioERetornar200_QuandoEleExistir()
    {
        // Arrange
        var users = CreateFaker<User>()
            .RuleFor(x => x.Username, faker => faker.Person.UserName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.Url, faker => faker.Person.Website)
            .RuleFor(x => x.Password, faker => faker.Internet.Password())
            .Generate(2);

        Db.Users.AddRange(users);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userIdToDelete = users[1].Id;

        // Act
        var httpResponse = await ApiClient.DeleteAsync($"/v1/user/{userIdToDelete}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        var responseBody = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal("Deletado com sucesso!", responseBody);

        Db.ChangeTracker.Clear();

        var usersAfterDelete = await Db.Users.ToListAsync(TestContext.Current.CancellationToken);
        Assert.Single(usersAfterDelete);
        Assert.NotEqual(userIdToDelete, usersAfterDelete[0].Id);
    }

    [Fact]
    public async Task DeveRetornar404_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var users = CreateFaker<User>()
            .RuleFor(x => x.Username, faker => faker.Person.UserName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.Url, faker => faker.Person.Website)
            .RuleFor(x => x.Password, faker => faker.Internet.Password())
            .Generate(2);

        Db.Users.AddRange(users);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var httpResponse = await ApiClient.DeleteAsync("/v1/user/100", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);

        Db.ChangeTracker.Clear();

        var usersAfter = await Db.Users.ToListAsync(TestContext.Current.CancellationToken);
        Assert.Equal(2, usersAfter.Count);
    }
}
