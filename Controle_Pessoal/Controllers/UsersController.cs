using Controle_Pessoal.Auth;
using Controle_Pessoal.Context;
using Controle_Pessoal.Entities;
using Controle_Pessoal.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controle_Pessoal.Controllers
{
    [ApiController]
    [Route("v1")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            var users = await context.Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var users = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return users == null
                ? NotFound()
                : Ok(users);
        }

        [HttpPost("user")]
        [AllowAnonymous]
        public async Task<IActionResult> PostAsync([FromServices] AppDbContext context, [FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = PasswordHasher.HashPassword(request.Password),
                    ProfilePicture = $"https://ui-avatars.com/api/?name={request.Name}&length=2&size=256&font-size=0.6&rounded=true&bold=true&background=68b7e0&color=941c80"
                };

                var userExists = await context.Users
                    .AsNoTracking()
                    .AnyAsync(x => x.Email == user.Email);

                if (userExists)
                {
                    return BadRequest("Já existe um usuário cadastrado com o e-mail informado");
                }

                context.Add(user);
                await context.SaveChangesAsync();
                return Created($"v1/users/{user.Id}", new
                {
                    user.Id,
                    user.Email,
                    user.Name,
                    user.ProfilePicture,
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPut("user/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] UpdateUserRequest request,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.Name = request.Username;
                user.Email = request.Email;
                user.ProfilePicture = request.Url;
                await context.SaveChangesAsync();
                return Ok(user);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }

        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
            {
                return NotFound();
            }

            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok("Deletado com sucesso!");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("user/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(
            [FromBody] UserLoginRequest request,
            [FromServices] AppDbContext context,
            [FromServices] TokenGenerator tokenGenerator,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User? user = null;
            if (request.Email == "googleAuth")
            {
                try
                {
                    var googleAccessToken = request.Password;
                    var googleAccessTokenInfo = await GoogleJsonWebSignature.ValidateAsync(googleAccessToken, new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = ["313667901167-d9cq0716r9ioll9uqdmf2qfa8nop0juv.apps.googleusercontent.com"]
                    });
                    user = await context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Email == googleAccessTokenInfo.Email, cancellationToken);

                    if (user is null)
                    {
                        user = new User
                        {
                            Name = googleAccessTokenInfo.Name,
                            Email = googleAccessTokenInfo.Email,
                            Password = googleAccessTokenInfo.Subject,
                            ProfilePicture = googleAccessTokenInfo.Picture,
                        };

                        context.Add(user);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
                catch (InvalidJwtException)
                {
                    return BadRequest("Access Token do Google inválido");
                }
            }
            else
            {
                var passwordHash = PasswordHasher.HashPassword(request.Password);
                user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == passwordHash, cancellationToken);

                if (user is null)
                {
                    return Unauthorized("Usuário ou senha inválidos");
                }
            }

            var accessToken = tokenGenerator.GenerateToken(user);
            return Ok(new
            {
                access_token = accessToken
            });
        }
    }
}
