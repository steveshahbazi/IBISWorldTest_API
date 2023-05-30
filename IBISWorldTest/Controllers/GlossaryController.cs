using IBISWorldTest.Data;
using IBISWorldTest.Models;
using IBISWorldTest.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBISWorldTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlossaryController : ControllerBase
    {
        private readonly GlossaryStore glossaryStore;

        public GlossaryController()
        {
            glossaryStore = new GlossaryStore(); // In-memory storage for simplicity
        }

        [HttpGet]
        public IActionResult GetAllTerms()
        {
            var sortedTerms = glossaryStore.terms.OrderBy(t => t.Name).ToList();
            return Ok(sortedTerms);
        }

        [HttpPost]
        public IActionResult AddTerm(TermDTO term)
        {
            // Generate a unique ID for the term
            term.Id = glossaryStore.terms.Count + 1;
            glossaryStore.terms.Add(term);
            return CreatedAtAction(nameof(GetTermById), new { id = term.Id }, term);
        }

        [HttpGet("{id}")]
        public IActionResult GetTermById(int id)
        {
            var term = glossaryStore.terms.FirstOrDefault(t => t.Id == id);
            if (term == null)
                return NotFound();
            return Ok(term);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTerm(int id, Term term)
        {
            var existingTerm = glossaryStore.terms.FirstOrDefault(t => t.Id == id);
            if (existingTerm == null)
                return NotFound();

            existingTerm.Name = term.Name;
            existingTerm.Definition = term.Definition;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTerm(int id)
        {
            var term = glossaryStore.terms.FirstOrDefault(t => t.Id == id);
            if (term == null)
                return NotFound();

            glossaryStore.terms.Remove(term);
            return NoContent();
        }
    }
}
