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
        public async Task<IActionResult>GetAsync([FromServices] AppDbContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("category/{id}")]
        public async Task<IActionResult>GetByIdAsync([FromServices] AppDbContext context, 
        [FromRoute]int id){
            var categories = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
            return categories == null? NotFound(): Ok(categories);
        }

        [HttpPost("category")]
        public async Task<IActionResult> PostAsync([FromServices] AppDbContext context, [FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var category = new Category
                {
                    CategoryName = request.CategoryName,
                };

                await context.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"v1/categories/{category.Id}", category);
            }
            catch (Exception)
            {
                return BadRequest();   
            }
        }

        [HttpPut("category/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] UpdateCategoryRequest request,
            [FromRoute] int id)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id==id);

            if(category == null)
            {
                return NotFound();
            }

            try
            {
                category.CategoryName = request.CategoryName;
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            try
            {
                context.Categories.Remove(category);
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
