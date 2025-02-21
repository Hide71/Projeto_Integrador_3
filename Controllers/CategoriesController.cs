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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("category")]
        public async Task<IActionResult>GetAsync([FromServices] AppDbContext context){
            var categories = await context.Categories
            .AsNoTracking()
            .ToListAsync();
            return Ok(categories);
        }
        [HttpGet]
        [Route("category/{id}")]
        public async Task<IActionResult>GetByIdAsync([FromServices] AppDbContext context, 
        [FromRoute]int id){
            var categories = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id);
            return categories == null? NotFound(): Ok(categories);
        }
        [HttpPost("category")]
        public async Task<IActionResult>PostAsync([FromServices] AppDbContext context,
        [FromBody] Category category){
            if (!ModelState.IsValid){
                return BadRequest();

            }

            try
            {
                await context.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"v1/categories/{category.id}", category);
                
            }
            catch (Exception e)
            {
                return BadRequest();
                
            }

        }
        [HttpPut("category/{id}")]
        public async Task<IActionResult>PutAsync([FromServices] AppDbContext context,
        [FromBody] Category category, [FromRoute] int id){
            if (!ModelState.IsValid){
                return BadRequest();
            }
            var _category = await context.Categories
            .FirstOrDefaultAsync(x => x.id==id);

            if(_category == null){
                return NotFound();
            }
            try
            {
                _category.categoryName= category.categoryName;
                _category.expenses = category.expenses;
                context.Categories.Update(_category);
                await context.SaveChangesAsync();
                return Ok(_category);
                
            }
            catch (Exception e)
            {
                return BadRequest();
        
            }

        }
        [HttpDelete("category/{id}")]
        public async Task<IActionResult>DeleteAsync([FromServices] AppDbContext context, 
        [FromRoute] int id){
            var category = await context.Categories.FirstOrDefaultAsync(x => x.id == id);
            try
            {
                context.Categories.Remove(category);
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
