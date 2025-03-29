using Controle_Pessoal.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Controle_Pessoal.Entities;
using Controle_Pessoal.Auth;
using System.Text.Json.Serialization;

namespace Controle_Pessoal.Tests.Endpoints.Users;

[Trait("Api", "Users - Login")]
public class LoginTests : ApiTest
{
    public LoginTests(ApiFixture apiFixture) : base(apiFixture)
    {
    }

    [Fact]
    public async Task DeveRetornarUmAccessToken_QuandoUsuarioExistir()
    {
        // Arrange
        var userPassword = Faker.Internet.Password(20);

        var user = new User
        {
            Name = Faker.Internet.UserName(),
            Email = Faker.Person.Email,
            Password = PasswordHasher.HashPassword(userPassword),
            ProfilePicture = Faker.Person.Website
        };

        Db.Users.Add(user);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
        Db.ChangeTracker.Clear();

        var request = new
        {
            email = user.Email,
            password = userPassword,
        };

        // Act
        var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user/login", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        
        var responseBody = await httpResponse.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.NotEmpty(responseBody.AccessToken);
    }

    [Fact]
    public async Task DeveRetornarUnauthorized_QuandoSenhaForInvalida()
    {
        // Arrange
        var userPassword = Faker.Internet.Password(20);

        var user = new User
        {
            Name = Faker.Internet.UserName(),
            Email = Faker.Person.Email,
            Password = PasswordHasher.HashPassword(userPassword),
            ProfilePicture = Faker.Person.Website
        };

        Db.Users.Add(user);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
        Db.ChangeTracker.Clear();

        var request = new
        {
            email = user.Email,
            password = Guid.NewGuid().ToString(),
        };

        // Act
        var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user/login", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        
        var responseBody = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.Equal("Usuário ou senha inválidos", responseBody);
    }
    
    [Fact]
    public async Task DeveRetornarUnauthorized_QuandoEmailForInvalido()
    {
        // Arrange
        var userPassword = Faker.Internet.Password(20);

        var user = new User
        {
            Name = Faker.Internet.UserName(),
            Email = Faker.Person.Email,
            Password = PasswordHasher.HashPassword(userPassword),
            ProfilePicture = Faker.Person.Website
        };

        Db.Users.Add(user);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
        Db.ChangeTracker.Clear();

        var request = new
        {
            email = Faker.Internet.Email(),
            password = userPassword,
        };

        // Act
        var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user/login", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        
        var responseBody = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.Equal("Usuário ou senha inválidos", responseBody);
    }
}

file class LoginResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}
