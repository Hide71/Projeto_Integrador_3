using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Controle_Pessoal.Context;
using Controle_Pessoal.Entities;
using Controle_Pessoal.Models;
using Controle_Pessoal.Auth;
using Microsoft.AspNetCore.Authorization;

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
            [FromRoute]int id)
        {
            var users = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return users == null
                ? NotFound()
                : Ok(users);
        }
        
        [HttpPost("user")]
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
                    Username = request.Username,
                    Email = request.Email,
                    Password = PasswordHasher.HashPassword(request.Password),
                };

                await context.AddAsync(user);
                await context.SaveChangesAsync();
                return Created($"v1/users/{user.Id}", new
                {
                    user.Id,
                    user.Email,
                    user.Username,
                    user.Url,
                });
            }
            catch (System.Exception)
            {
            
                return BadRequest();
                
            }
        }

        [Authorize]
        [HttpPut("user/{id}")]
        public async Task<IActionResult>PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] UpdateUserRequest request,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id==id);

            if(user == null)
            {
                return NotFound();
            }

            try
            {
                user.Username = request.Username;
                user.Email = request.Email;
                user.Url = request.Url;
                await context.SaveChangesAsync();
                return Ok(user);
            }
            catch (System.Exception )
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

            try
            {
                var passwordHash = PasswordHasher.HashPassword(request.Password);
                var user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == passwordHash, cancellationToken);

                if (user is null)
                {
                    return Unauthorized("Usuário ou senha inválidos");
                }
                
                var accessToken = tokenGenerator.GenerateToken(user.Id, user.Email);
                return Ok(new
                {
                    access_token = accessToken
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest();                
            }
        }
    }
}
