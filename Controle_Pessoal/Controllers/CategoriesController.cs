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
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        [Route("category")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            var categories = await context.Categories
                .Where(c => c.UserId == HttpContext.GetUserId())
                .AsNoTracking()
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet]
        [Route("category/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context, 
            [FromRoute]int id)
        {
            var category = await context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == HttpContext.GetUserId() && x.Id == id);

            return category is null
                ? NotFound()
                : Ok(category);
        }

        [HttpPost("category")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateCategoryRequest request)
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
                    UserId = HttpContext.GetUserId()
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

            var category = await context.Categories
                .FirstOrDefaultAsync(x => x.UserId == HttpContext.GetUserId() && x.Id == id);

            if (category is null)
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
            var category = await context.Categories
            .FirstOrDefaultAsync(x => x.UserId == HttpContext.GetUserId() && x.Id == id);
            if (category is null)
            {
                return NotFound();
            }

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
