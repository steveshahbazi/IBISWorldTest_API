using AutoMapper;
using IBISWorldTest.Data;
using IBISWorldTest.Models;
using IBISWorldTest.Models.DTO;
using IBISWorldTest.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IBISWorldTest.Controllers
{
    [Route("api/glossaryAPI")]
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
        public async Task<ActionResult<APIResponse>> GetAllTerms()
        {
            try
            {
                _logger.LogInformation("Getting all terms");
                var terms = await _dbTerm.GetAllAsync();
                _response.Result = _mapper.Map<List<TermDTO>>(terms.OrderBy(t => t.Name));
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> AddTerm(TermDTO createTermDTO)
        {
            try
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

                _response.Result = _mapper.Map<TermDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtAction(nameof(GetTermById), new { id = model?.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetTerm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetTermById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var term = await _dbTerm.GetAsync(t => t.Id == id);
                if (term == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }

                _response.Result = _mapper.Map<TermDTO>(term);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}", Name = "UpdateTerm")]
        public async Task<ActionResult<APIResponse>> UpdateTerm(int id, [FromBody] TermDTO updateTermDto)
        {
            try
            {
                if (updateTermDto == null || id != updateTermDto.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var existingTerm = await _dbTerm.GetAsync(t => t.Id == id, tracked: false);
                if (existingTerm == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }

                var model = _mapper.Map<Term>(updateTermDto);
                await _dbTerm.UpdateAsync(model);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteTerm")]
        public async Task<ActionResult<APIResponse>> DeleteTerm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var term = await _dbTerm.GetAsync(t => t.Id == id);
                if (term == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }

                await _dbTerm.RemoveAsync(term);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }
    }
}
