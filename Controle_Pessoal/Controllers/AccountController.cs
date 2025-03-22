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
using Microsoft.AspNetCore.Authorization;

namespace Controle_Pessoal.Controllers
{
    [ApiController]
    [Route("v1")]
    public class AccountsController : ControllerBase
    {
        [HttpGet("account")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            var accounts = await context.Accounts.AsNoTracking().ToListAsync();
            return Ok(accounts);

        }
        [HttpGet("account/{id}")]
        public async Task<IActionResult>GetByIdAsync([FromServices] AppDbContext context, [FromRoute] int id)
        {
            var accounts = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return accounts == null? NotFound(): Ok(accounts);
        }
        
        [HttpPost("account")]
        public async Task<IActionResult> PostAsync([FromServices] AppDbContext context,[FromBody]CreateAccountRequest request)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var account = new Account{
                Description = request.Description,
                Balance = request.Balance,
                TypeAccount = request.TypeAccount,
            };
            await context.AddAsync(account);
            await context.SaveChangesAsync();
            return Created(string.Empty, account);

                
            }
            catch (System.Exception)
            {
                
                return BadRequest();
            }
        }
        [HttpPut("account/{id}")]
        public async Task<IActionResult> PutAsync([FromServices] AppDbContext context, [FromBody] UpdateAccountRequest request, [FromRoute] int id)
        {
            if (!ModelState.IsValid){
                return BadRequest();
            }
            var account = await context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            if (account == null){
                return NotFound();
            }
            try
            {
                account.Description = request.Description;
                account.Balance = request.Balance;
                account.TypeAccount = request.TypeAccount;
                await context.SaveChangesAsync();
                return Ok(account);
            }
            catch (Exception)
            {
                
                return BadRequest();
            }
        }
        [HttpDelete("account/{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] AppDbContext context, [FromRoute] int id)
        {
            var account = await context.Accounts.FirstOrDefaultAsync(x =>x.Id == id);
            try
            {
                context.Accounts.Remove(account);
                await context.SaveChangesAsync();
                return Ok("Deletado com sucesso");
                
            }
            catch (Exception)
            {
                
                return NotFound();
            }

        }
    }
}