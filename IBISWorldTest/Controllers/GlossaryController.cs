﻿using IBISWorldTest.Data;
using IBISWorldTest.Models;
using IBISWorldTest.Models.DTO;
using IBISWorldTest.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBISWorldTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlossaryController : ControllerBase
    {
        private readonly ILogger<GlossaryController> _logger;
        private readonly ITermRepository _dbTerm;

        public GlossaryController(ITermRepository dbTerm, ILogger<GlossaryController> logger)
        {
            _dbTerm = dbTerm;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTerms()
        {
            _logger.LogInformation("Getting all terms");
            var sortedTerms = await _dbTerm.GetAllAsync();
            return Ok(sortedTerms.OrderBy(t => t.Name));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTerm(TermDTO termDTO)
        {
            if (termDTO == null)
            {
                _logger.LogError("Add Term Error, null object is passed.");
                return BadRequest(termDTO);
            }
            if (await _dbTerm.GetAsync(t => t.Name != null && t.Name.ToLower() == termDTO.Name) != null)
            {
                _logger.LogError("Add Term Error, Term already exists.");
                ModelState.AddModelError("CustomError", "Term already exists.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _dbTerm.GetAsync(t => true, false);
            //if we reach to maximum capacity 
            if (item != null && item.Id == int.MaxValue)
            {
                _logger.LogError("Add Term Error, maximum id reached.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            await _dbTerm.CreateAsync(new Term()
            {
                CreationDate = DateTime.UtcNow,
                Name = termDTO.Name,
                Definition = termDTO.Definition,
            });

            return CreatedAtAction(nameof(GetTermById), new { id = item?.Id }, item);
        }

        [HttpGet("{id:int}", Name ="GetTerm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTermById(int id)
        {
            if (id <= 0)
                return BadRequest();
            var term = await _dbTerm.GetAsync(t => t.Id == id);
            if (term == null)
                return NotFound();
            return Ok(term);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}", Name = "UpdateTerm")]
        public async Task<IActionResult> UpdateTerm(int id, [FromBody]TermDTO termDto)
        {
            if(termDto == null || id != termDto.Id)
                return BadRequest();
            var existingTerm = await _dbTerm.GetAsync(t => t.Id == id);
            if (existingTerm == null)
                return NotFound();

            existingTerm.Name = termDto.Name;
            existingTerm.Definition = termDto.Definition;
            await _dbTerm.UpdateAsync(existingTerm);

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name ="DeleteTerm")]
        public async Task<IActionResult> DeleteTerm(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var term = await _dbTerm.GetAsync(t => t.Id == id);
            if (term == null)
                return NotFound();

            await _dbTerm.RemoveAsync(term);

            return NoContent();
        }
    }
}
