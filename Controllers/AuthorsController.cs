using AutoMapper;
using BookStore.API.Contracts;
using BookStore.API.Data;
using BookStore.API.DTOs;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted Get All Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors); // DTOs allow to control what data is presented to API user relative to data coming from DB
                _logger.LogInfo("Successfully got all Authors");
                return Ok(response); // 200, with payload
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} {e.InnerException}");
            }
        }

        /// <summary>
        /// Get an Author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An Author's record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted to get Author with id:{id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"Author with id:{id} was not found");
                    return NotFound();
                }

                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully got Author with id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} {e.InnerException}");                
            }
        }

        /// <summary>
        /// Create an Author
        /// </summary>
        /// <param name="authorDTO">Author with required Firstname and Lastname and an optional Bio</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo("Author submission attempted");
                if (authorDTO == null)
                { // This endpoint would not even be hit if no body is provided (Dafault Code: 415 - Unsupported Media Type), nevertheles..
                    _logger.LogWarn("Empty Request was submitted");
                    return BadRequest(ModelState); 
                }
                if (!ModelState.IsValid) // tracks validation status of data
                { // This endpoint would not even be hit if any "Required" field is not provided or is provided an empty value (Dafault Code: 400), nevertheles..
                    _logger.LogWarn("Author data was incomplete");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return InternalError("Author creation failed");
                }

                _logger.LogInfo("Author created");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} {e.InnerException}");
            }
        }

        /// <summary>
        /// Update an Author
        /// </summary>
        /// <param name="id">Id of author to be updated</param>
        /// <param name="authorDTO">Author with required Firstname and Lastname and an optional Bio</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Author update with Id:{id} attempted");
                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _logger.LogWarn($"Update failed with bad data for Id:{id}");
                    return BadRequest();
                }

                var exists = await _authorRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"Author with id:{id} was not found");
                    return NotFound();
                }

                if (!ModelState.IsValid) 
                {
                    _logger.LogWarn("Author data was incomplete");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError("Update operation failed");
                }

                _logger.LogInfo($"Author with Id:{id} updated");
                return NoContent(); // success
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} {e.InnerException}");
            }
        }

        /// <summary>
        /// Remove an Author by Id
        /// </summary>
        /// <param name="id">Id of author to be removed</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"Author delete with Id:{id} attempted");
                if (id < 1)
                {
                    _logger.LogWarn($"Delete failed with bad data for Id:{id}");
                    return BadRequest();
                }

                var exists = await _authorRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"Author with id:{id} was not found");
                    return NotFound();
                }

                var author = await _authorRepository.FindById(id);
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError("Delete operation failed");
                }

                _logger.LogInfo($"Author with Id:{id} deleted");
                return NoContent(); // success
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} {e.InnerException}");
            }
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }

    }
}
