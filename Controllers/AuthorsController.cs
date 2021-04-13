using AutoMapper;
using BookStore.API.Contracts;
using BookStore.API.Data;
using BookStore.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Controllers talk to DTOs that the Mapper maps to/from Data objects that repository uses
namespace BookStore.API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the bookstore's DB
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper; // transferring data between data object and DTO

        public AuthorsController(IAuthorRepository authorRepository,
            ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [AllowAnonymous] // [AllowAnonynous] => no authentication needed; [Authorize] => any authenticated user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors); // DTOs allow to control what data is presented to API user relative to data coming from DB
                _logger.LogInfo($"{location}: Successful");
                return Ok(response); // 200, with payload
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException}");
            }
        }

        /// <summary>
        /// Get an Author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An Author's record</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call for id:{id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"{location}: id:{id} was not found");
                    return NotFound();
                }

                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{location}: Successfully got record with id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException}");                
            }
        }

        /// <summary>
        /// Create an Author
        /// </summary>
        /// <param name="authorDTO">Author with required Firstname and Lastname and an optional Bio</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")] // comma-separated-list
        // Results in 401 Unauthorized w/o token [not authenticated] and 403 Forbidden with a 'customer' claim token [authenticated but not authorized]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call");
                if (authorDTO == null)
                { // This endpoint would not even be hit if no body is provided (Dafault Code: 415 - Unsupported Media Type), nevertheles..
                    _logger.LogWarn($"{location}: Empty Request was submitted");
                    return BadRequest(ModelState); 
                }
                if (!ModelState.IsValid) // tracks validation status of data
                { // This endpoint would not even be hit if any "Required" field is not provided or is provided an empty value (Dafault Code: 400), nevertheles..
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Failure");
                }

                _logger.LogInfo($"{location}: Successful");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException}");
            }
        }

        /// <summary>
        /// Update an Author
        /// </summary>
        /// <param name="id">Id of author to be updated</param>
        /// <param name="authorDTO">Author with required Firstname and Lastname and an optional Bio</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call for id:{id}");
                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _logger.LogWarn($"{location}: Bad data for Id:{id}");
                    return BadRequest();
                }

                var exists = await _authorRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"{location}: id:{id} was not found");
                    return NotFound();
                }

                if (!ModelState.IsValid) 
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: id:{id} Failure");
                }

                _logger.LogInfo($"{location}: id:{id} Successful");
                return NoContent(); // success
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException}");
            }
        }

        /// <summary>
        /// Remove an Author by Id
        /// </summary>
        /// <param name="id">Id of author to be removed</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call for id:{id}");
                if (id < 1)
                {
                    _logger.LogWarn($"{location}: Bad data for Id:{id}");
                    return BadRequest();
                }

                var exists = await _authorRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"{location}: id:{id} was not found");
                    return NotFound();
                }

                var author = await _authorRepository.FindById(id);
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"{location}: id:{id} Failure");
                }

                _logger.LogInfo($"{location}: id:{id} Successful");
                return NoContent(); // success
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }

    }
}
