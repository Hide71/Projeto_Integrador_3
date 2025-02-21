using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Controle_Pessoal.Context;
using Controle_Pessoal.Entities;

namespace Controle_Pessoal.Controllers
{
    [ApiController]
    [Route("v1")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("user")]
        public async Task<IActionResult>GetAsync([FromServices] AppDbContext context){
            var users = await context.Users
            .AsNoTracking()
            .ToListAsync();
            return Ok(users);
        }
        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult>GetByIdAsync([FromServices] AppDbContext context, 
        [FromRoute]int id){
            var users = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id);
            return users == null? NotFound(): Ok(users);
        }
        [HttpPost("user")]
        public async Task<IActionResult>PostAsync([FromServices] AppDbContext context,
        [FromBody] User user){
            if (!ModelState.IsValid){
                return BadRequest();

            }

            try
            {
                await context.AddAsync(user);
                await context.SaveChangesAsync();
                return Created($"v1/users/{user.id}", user);
                
            }
            catch (Exception e)
            {
                return BadRequest();
                
            }
             


        }
        [HttpPut("user/{id}")]
        public async Task<IActionResult>PutAsync([FromServices] AppDbContext context,
        [FromBody] User user, [FromRoute] int id){
            if (!ModelState.IsValid){
                return BadRequest();
            }
            var _user = await context.Users
            .FirstOrDefaultAsync(x => x.id==id);

            if(_user == null){
                return NotFound();
            }
            try
            {
                _user.username = user.username;
                _user.email = user.email;
                _user.url = user.url;
                _user.expenses = user.expenses;
                context.Users.Update(_user);
                await context.SaveChangesAsync();
                return Ok(_user);
                
            }
            catch (Exception e)
            {
                return BadRequest();
        
            }

        }
        [HttpDelete("user/{id}")]
        public async Task<IActionResult>DeleteAsync([FromServices] AppDbContext context, 
        [FromRoute] int id){
            var user = await context.Users.FirstOrDefaultAsync(x => x.id == id);
            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok();
                
            }
            catch (Exception e)
            {
                return BadRequest();
            }
            

        }

        
        

   
    }
}
