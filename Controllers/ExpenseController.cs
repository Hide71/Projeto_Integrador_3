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

namespace Controle_Pessoal.Controllers
{
    [ApiController]
    [Route("v1/expenses")]
    public class ExpenseController : ControllerBase
    {
        [HttpPost]
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
                    UserId = request.UserId,
                    CategoryId = request.CategoryId,
                };
                
                await context.AddAsync(expense);
                await context.SaveChangesAsync();
                return Created(string.Empty, expense);
            }
            catch (Exception )
            {
                return BadRequest();   
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] AppDbContext context
        )
        {
            var expenses = await context.Expenses.AsNoTracking().ToListAsync();
            return Ok(expenses);
        }
    }
}
