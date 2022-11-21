using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS243N_Term_Project.Models;

namespace BreweryRESTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientInventorySubtractionsController : ControllerBase
    {
        private readonly BitsContext _context;

        public IngredientInventorySubtractionsController(BitsContext context)
        {
            _context = context;
        }

        // GET: api/IngredientInventorySubtractions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientInventorySubtraction>>> GetIngredientInventorySubtractions()
        {
            return await _context.IngredientInventorySubtractions.ToListAsync();
        }

        // GET: api/IngredientInventorySubtractions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientInventorySubtraction>> GetIngredientInventorySubtraction(int id)
        {
            var ingredientInventorySubtraction = await _context.IngredientInventorySubtractions.FindAsync(id);

            if (ingredientInventorySubtraction == null)
            {
                return NotFound();
            }

            return ingredientInventorySubtraction;
        }

        // PUT: api/IngredientInventorySubtractions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngredientInventorySubtraction(int id, IngredientInventorySubtraction ingredientInventorySubtraction)
        {
            if (id != ingredientInventorySubtraction.IngredientInventorySubtractionId)
            {
                return BadRequest();
            }

            _context.Entry(ingredientInventorySubtraction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientInventorySubtractionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/IngredientInventorySubtractions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IngredientInventorySubtraction>> PostIngredientInventorySubtraction(IngredientInventorySubtraction ingredientInventorySubtraction)
        {
            _context.IngredientInventorySubtractions.Add(ingredientInventorySubtraction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIngredientInventorySubtraction", new { id = ingredientInventorySubtraction.IngredientInventorySubtractionId }, ingredientInventorySubtraction);
        }

        // DELETE: api/IngredientInventorySubtractions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredientInventorySubtraction(int id)
        {
            var ingredientInventorySubtraction = await _context.IngredientInventorySubtractions.FindAsync(id);
            if (ingredientInventorySubtraction == null)
            {
                return NotFound();
            }

            _context.IngredientInventorySubtractions.Remove(ingredientInventorySubtraction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IngredientInventorySubtractionExists(int id)
        {
            return _context.IngredientInventorySubtractions.Any(e => e.IngredientInventorySubtractionId == id);
        }
    }
}
