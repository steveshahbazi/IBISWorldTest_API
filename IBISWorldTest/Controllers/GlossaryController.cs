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
        [HttpGet]
        public IActionResult GetAllTerms()
        {
            var sortedTerms = GlossaryStore.terms.OrderBy(t => t.Name).ToList();
            return Ok(sortedTerms);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddTerm(TermDTO termDTO)
        {
            if (termDTO == null)
                return BadRequest(termDTO);
            if (GlossaryStore.terms.FirstOrDefault(t => t.Name?.ToLower() == termDTO.Name) != null)
                ModelState.AddModelError("CustomError", "Term already exists.");
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            int lastId = GlossaryStore.terms.OrderByDescending(t => t.Id).FirstOrDefault().Id;
            //if we reach to maximum capacity (this should not be the case if we use a database)
            if (lastId == int.MaxValue)
                return StatusCode(StatusCodes.Status500InternalServerError);
            // Generate a unique ID for the term
            termDTO.Id = lastId + 1;
            GlossaryStore.terms.Add(termDTO);
            return CreatedAtAction(nameof(GetTermById), new { id = termDTO.Id }, termDTO);
        }

        [HttpGet("{id:int}", Name ="GetTerm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTermById(int id)
        {
            if (id <= 0)
                return BadRequest();
            var term = GlossaryStore.terms.FirstOrDefault(t => t.Id == id);
            if (term == null)
                return NotFound();
            return Ok(term);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}", Name = "UpdateTerm")]
        public IActionResult UpdateTerm(int id, [FromBody]TermDTO termDto)
        {
            if(termDto == null || id != termDto.Id)
                return BadRequest();
            var existingTerm = GlossaryStore.terms.FirstOrDefault(t => t.Id == id);
            if (existingTerm == null)
                return NotFound();

            existingTerm.Name = termDto.Name;
            existingTerm.Definition = termDto.Definition;

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name ="DeleteTerm")]
        public IActionResult DeleteTerm(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var term = GlossaryStore.terms.FirstOrDefault(t => t.Id == id);
            if (term == null)
                return NotFound();

            GlossaryStore.terms.Remove(term);
            return NoContent();
        }
    }
}
