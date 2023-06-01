using AutoMapper;
using IBISWorld_API;
using IBISWorld_API.Controllers;
using IBISWorld_API.Models;
using IBISWorld_API.Models.DTO;
using IBISWorld_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IBISWorld_API_Tests
{
    public class GlossaryControllerTests
    {
        private readonly GlossaryController _controller;
        private readonly Mock<ITermRepository> _mockTermRepository;
        private readonly Mock<ILogger<GlossaryController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;

        public GlossaryControllerTests()
        {
            _mockTermRepository = new Mock<ITermRepository>();
            _mockLogger = new Mock<ILogger<GlossaryController>>();
            _mockMapper = new Mock<IMapper>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Term, TermDTO>();
                cfg.CreateMap<TermDTO, Term>();
            });
            var mapper = config.CreateMapper();
            _controller = new GlossaryController(_mockTermRepository.Object, _mockLogger.Object, mapper);
        }

        [Fact]
        public async Task GetAllTerms_ReturnsOkResult()
        {
            // Arrange
            var terms = new List<Term>
            {
                new Term { Id = 1, Name = "Term 1", Definition = "Definition 1" },
                new Term { Id = 2, Name = "Term 2", Definition = "Definition 2" },
                new Term { Id = 3, Name = "Term 3", Definition = "Definition 3" }
            };
            var termDTOs = new List<TermDTO>
            {
                new TermDTO { Id = 1, Name = "Term 1", Definition = "Definition 1" },
                new TermDTO { Id = 2, Name = "Term 2", Definition = "Definition 2" },
                new TermDTO { Id = 3, Name = "Term 3", Definition = "Definition 3" }
            };

            
            //_mockMapper.Setup(m => m.Map<List<TermDTO>>(It.IsAny<List<Term>>())).Returns((List<Term> terms) => mapper.Map<List<TermDTO>>(terms));

            _mockTermRepository.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(terms);
            //_mockMapper.Setup(mapper => mapper.Map<List<TermDTO>>(It.IsAny<List<Term>>())).Returns(termDTOs);

            // Act
            var result = await _controller.GetAllTerms();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<APIResponse>(okResult.Value);
            var returnedTerms = Assert.IsType<List<TermDTO>>(response.Result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(termDTOs.Count, returnedTerms.Count);
        }

        [Fact]
        public async Task GetAllTerms_ReturnsErrorResponse_OnException()
        {
            // Arrange
            _mockTermRepository.Setup(repo => repo.GetAllAsync(null)).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetAllTerms();

            // Assert
            var response = Assert.IsType<APIResponse>(result.Result);
            Assert.False(response.IsSuccess);
            Assert.Single(response.ErrorMessages);
            Assert.Equal("Test Exception", response.ErrorMessages.First());
        }

        [Fact]
        public async Task AddTerm_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var createTermDTO = new TermDTO { Id = 1, Name = "New Term", Definition = "Definition" };
            var term = new Term { Id = 1, Name = "New Term", Definition = "Definition" };
            _mockTermRepository.Setup(repo => repo.GetAsync((t => t.Name != null && t.Name.ToLower() == createTermDTO.Name), false)).ReturnsAsync((Term)null);
            _mockTermRepository.Setup(repo => repo.GetAsync(t => true, false)).ReturnsAsync((Term)null);
            _mockMapper.Setup(mapper => mapper.Map<Term>(createTermDTO)).Returns(term);

            // Act
            var result = await _controller.AddTerm(createTermDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<APIResponse>(createdAtActionResult.Value);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.True(response.IsSuccess);
            Assert.Equal(createTermDTO, response.Result);
            Assert.Equal(nameof(GlossaryController.GetTermById), createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task AddTerm_ReturnsBadRequestResult_WhenCreateTermDTOIsNull()
        {
            // Arrange
            TermDTO createTermDTO = null;

            // Act
            var result = await _controller.AddTerm(createTermDTO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddTerm_ReturnsBadRequestResult_WhenTermAlreadyExists()
        {
            // Arrange
            var createTermDTO = new TermDTO { Id = 1, Name = "Existing Term", Definition = "Definition" };
            _mockTermRepository.Setup(repo => repo.GetAsync((t => t.Name != null && t.Name.ToLower() == createTermDTO.Name), false)).ReturnsAsync(new Term());

            // Act
            var result = await _controller.AddTerm(createTermDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int?)HttpStatusCode.BadRequest, badRequestResult.StatusCode);

            var errorMessage = (((SerializableError)badRequestResult.Value)["Name"] as string[])[0];
            Assert.Equal("Term already exists.", errorMessage);
        }

        [Fact]
        public async Task AddTerm_ReturnsBadRequestResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var createTermDTO = new TermDTO { Id = 1, Name = null, Definition = "Definition" };
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.AddTerm(createTermDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int?)HttpStatusCode.BadRequest, badRequestResult.StatusCode);

            var errorMessage = (((SerializableError)badRequestResult.Value)["Name"] as string[])[0];
            Assert.Equal("The Name field is required.", errorMessage);
        }

        [Fact]
        public async Task AddTerm_ReturnsStatusCode500_WhenMaximumIdIsReached()
        {
            // Arrange
            var createTermDTO = new TermDTO { Id = 1, Name = "New Term", Definition = "Definition" };
            var latestTerm = new Term { Id = int.MaxValue };
            _mockTermRepository.Setup(repo => repo.GetAsync(t => true, false)).ReturnsAsync(latestTerm);

            // Act
            var result = await _controller.AddTerm(createTermDTO);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result.Result);
            var response = Assert.IsType<APIResponse>(statusCodeResult.StatusCode);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)statusCodeResult.StatusCode);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task AddTerm_ReturnsErrorResponse_OnException()
        {
            // Arrange
            var createTermDTO = new TermDTO { Id = 1, Name = "New Term", Definition = "Definition" };
            _mockTermRepository.Setup(repo => repo.GetAsync((t => t.Name != null && t.Name.ToLower() == createTermDTO.Name), false)).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.AddTerm(createTermDTO);

            // Assert
            var response = Assert.IsType<APIResponse>(result.Result);
            Assert.False(response.IsSuccess);
            Assert.Single(response.ErrorMessages);
            Assert.Equal("Test Exception", response.ErrorMessages.First());
        }

        // Similar tests for other actions: GetTermById, UpdateTerm, DeleteTerm
        // ...
    }
}
