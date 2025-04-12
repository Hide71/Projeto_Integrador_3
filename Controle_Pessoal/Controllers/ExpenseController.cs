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
using Controle_Pessoal.Auth;

namespace Controle_Pessoal.Controllers
{
    [ApiController]
    [Route("v1")]
    [Authorize]

    public class ExpenseController : ControllerBase
    {
        [HttpPost("expenses")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateExpenseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var expense = new Expense
                {
                    Description = request.Description,
                    Amount = request.Amount,
                    Date = request.Date,
                    UserId = HttpContext.GetUserId(),
                    CategoryId = request.CategoryId,
                    AccountId = request.AccountId,
                    Account = null
                };

                var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId);
                if (expense.Amount < 0)
                {
                    account.Balance -= expense.Amount;
                }
                else
                {
                    account.Balance += expense.Amount;
                }

                context.Add(expense);
                await context.SaveChangesAsync();
                return Created(string.Empty, new
                {
                    expense.Id,
                    expense.Description,
                    expense.Amount,
                    expense.Date,
                    expense.CategoryId,
                    expense.AccountId,
                    expense.UserId
                });
            }
            catch (Exception )
            {
                return BadRequest();   
            }
           
        }

        [HttpGet("expenses")]
        public async Task<IActionResult> Get(
            [FromServices] AppDbContext context)
        {
            var expenses = await context.Expenses
            .Where(c => c.UserId == HttpContext.GetUserId())
            .AsNoTracking()
            .ToListAsync();
            return Ok(expenses);
        }
        [HttpGet("expenses/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context, 
            [FromRoute] int id) 
            {
            var expenses = await context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == HttpContext.GetUserId() && x.Id == id);
            return  expenses == null
            ? NotFound() 
            : Ok(expenses);
        }
        [HttpPut("expenses/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id,
            [FromBody] UpdateExpenseRequest request)
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var expense = await context.Expenses
               .FirstOrDefaultAsync(x => x.UserId == HttpContext.GetUserId() && x.Id == id);
                if(expense == null)
                {
                    return NotFound();
                }
                try
                {
                    expense.Description = request.Description;
                    expense.Amount = request.Amount;
                    expense.Date = request.Date;
                    expense.UserId = request.UserId;
                    expense.CategoryId = request.CategoryId;
                    expense.AccountId = request.AccountId;
                    await context.SaveChangesAsync();
                    return Ok(expense);

                }
                catch (Exception)
                {
                    
                    return BadRequest();
                }
            }
             [HttpDelete("expenses/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var expense = await context.Expenses
            .FirstOrDefaultAsync(x => x.UserId == HttpContext.GetUserId() && x.Id == id);
            try
            {
                context.Expenses.Remove(expense);
                await context.SaveChangesAsync();
                return Ok("Deletado com sucesso!");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    
    }
}
