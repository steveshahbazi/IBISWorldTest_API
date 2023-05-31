using AutoMapper;
using IBISWorldTest.Data;
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
        protected APIResponse _response;
        private readonly ILogger<GlossaryController> _logger;
        private readonly IMapper _mapper;
        private readonly ITermRepository _dbTerm;

        public GlossaryController(ITermRepository dbTerm, ILogger<GlossaryController> logger, IMapper mapper)
        {
            _dbTerm = dbTerm;
            _logger = logger;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTerms()
        {
            _logger.LogInformation("Getting all terms");
            var terms = await _dbTerm.GetAllAsync();
            return Ok(_mapper.Map<List<TermDTO>>(terms.OrderBy(t => t.Name)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTerm(TermDTO createTermDTO)
        {
            if (createTermDTO == null)
            {
                _logger.LogError("Add Term Error, null object is passed.");
                return BadRequest(createTermDTO);
            }
            if (await _dbTerm.GetAsync(t => t.Name != null && t.Name.ToLower() == createTermDTO.Name) != null)
            {
                _logger.LogError("Add Term Error, Term already exists.");
                ModelState.AddModelError("CustomError", "Term already exists.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var latestTerm = await _dbTerm.GetAsync(t => true, false);
            //if we reach to maximum capacity 
            if (latestTerm != null && latestTerm.Id == int.MaxValue)
            {
                _logger.LogError("Add Term Error, maximum id reached.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var model = _mapper.Map<Term>(createTermDTO);
            model.CreationDate = DateTime.UtcNow;

            await _dbTerm.CreateAsync(model);

            return CreatedAtAction(nameof(GetTermById), new { id = model?.Id }, model);
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
            return Ok(_mapper.Map<TermDTO>(term));
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}", Name = "UpdateTerm")]
        public async Task<IActionResult> UpdateTerm(int id, [FromBody]TermDTO updateTermDto)
        {
            if(updateTermDto == null || id != updateTermDto.Id)
                return BadRequest();
            var existingTerm = await _dbTerm.GetAsync(t => t.Id == id, tracked:false);
            if (existingTerm == null)
                return NotFound();

            var model = _mapper.Map<Term>(updateTermDto);
            await _dbTerm.UpdateAsync(model);

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
