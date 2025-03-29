using Controle_Pessoal.Auth;
using Controle_Pessoal.Entities;
using Controle_Pessoal.Tests.Fixtures;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using System.Net.Http.Json;
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

    [Fact]
    public async Task DeveRetornarAccessToken_QuandoLoginForFeitoUtilizandoGoogleParaUmUsuarioExistente()
    {
        // Arrange
        var googleAccessToken = Faker.Internet.Password(20);

        var user = new User
        {
            Name = Faker.Internet.UserName(),
            Email = Faker.Person.Email,
            Password = "123",
            ProfilePicture = Faker.Image.PicsumUrl()
        };

        Db.Users.Add(user);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
        Db.ChangeTracker.Clear();

        var googleTokenManager = ServiceProvider.GetService<IGoogleAccessTokenManager>()!;
        googleTokenManager.ParseAndValidateAsync(googleAccessToken).Returns(new GoogleJsonWebSignature.Payload
        {
            Email = user.Email
        });

        var request = new
        {
            email = "googleAuth",
            password = googleAccessToken,
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
    public async Task DeveCadastrarUsuarioERetornarAccessToken_QuandoLoginForFeitoUtilizandoGoogleParaUmUsuarioNaoCadastrado()
    {
        // Arrange
        var googleAccessToken = Faker.Internet.Password(20);

        var googleAccessTokenValidationPayload = new GoogleJsonWebSignature.Payload
        {
            Email = Faker.Person.Email,
            Name = Faker.Person.FullName,
            Subject = Faker.Random.Int(10000).ToString(),
            Picture = Faker.Image.PicsumUrl()
        };
        var googleTokenManager = ServiceProvider.GetService<IGoogleAccessTokenManager>()!;
        googleTokenManager.ParseAndValidateAsync(googleAccessToken).Returns(googleAccessTokenValidationPayload);

        var request = new
        {
            email = "googleAuth",
            password = googleAccessToken,
        };

        // Act
        var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user/login", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        var responseBody = await httpResponse.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(responseBody);
        Assert.NotEmpty(responseBody.AccessToken);

        var createdUser = await Db.Users.FirstAsync(TestContext.Current.CancellationToken);
        Assert.Equal(1, createdUser.Id);
        Assert.Equal(googleAccessTokenValidationPayload.Name, createdUser.Name);
        Assert.Equal(googleAccessTokenValidationPayload.Email, createdUser.Email);
        Assert.Equal(googleAccessTokenValidationPayload.Picture, createdUser.ProfilePicture);
        Assert.Equal(googleAccessTokenValidationPayload.Subject, createdUser.Password);
        Assert.Equal(googleAccessTokenValidationPayload.Subject, createdUser.GoogleId);
    }

    [Fact]
    public async Task DeveRetornarBadRequest_QuandoValidacaoDoAccessTokenDoGoogleRetornarErro()
    {
        // Arrange
        var googleAccessToken = Faker.Internet.Password(20);
        var googleTokenManager = ServiceProvider.GetService<IGoogleAccessTokenManager>()!;
        googleTokenManager.ParseAndValidateAsync(googleAccessToken).ThrowsAsync(new InvalidJwtException("An error"));

        var request = new
        {
            email = "googleAuth",
            password = googleAccessToken,
        };

        // Act
        var httpResponse = await ApiClient.PostAsJsonAsync("/v1/user/login", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);

        var responseBody = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal("Access Token do Google inválido", responseBody);
    }
}

file class LoginResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}
