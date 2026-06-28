using HomemadeGit.Core.DTOs.Repositories;
using HomemadeGit.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomemadeGit.Api.Controllers
{
    [Route("api/repositories")]
    [ApiController]
    public class RepositoryController : ControllerBase
    {
        private readonly IRepositoryService _repositoryService;

        public RepositoryController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RepositoryResponse), 200)]
        public async Task<ActionResult<RepositoryResponse>> CreateRepository([FromBody] CreateRepositoryRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var repository = await _repositoryService.CreateRepositoryAsync(userId, request);

                return Ok(repository);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { errors = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<RepositoryListItemResponse>), 200)]
        public async Task<ActionResult<List<RepositoryListItemResponse>>> SearchRepositories([FromQuery] string? query)
        {
            try
            {
                var userId = GetCurrentUserId();

                var request = new SearchRepositoriesRequest
                {
                    Query = query ?? string.Empty
                };

                var repositories = await _repositoryService.SearchRepositoriesAsync(userId, request);

                return Ok(repositories);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { errors = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.Message });
            }
        }

        [HttpGet("{repositoryId:int}")]
        [ProducesResponseType(typeof(RepositoryResponse), 200)]
        public async Task<ActionResult<RepositoryResponse>> GetRepositoryById([FromRoute] int repositoryId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var repository = await _repositoryService.GetRepositoryByIdAsync(userId, repositoryId);

                return Ok(repository);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { errors = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.Message });
            }
        }

        [HttpPut("{repositoryId:int}")]
        [ProducesResponseType(typeof(RepositoryResponse), 200)]
        public async Task<ActionResult<RepositoryResponse>> UpdateRepository([FromRoute] int repositoryId, [FromBody] UpdateRepositoryRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var repository = await _repositoryService.UpdateRepositoryAsync(userId, repositoryId, request);

                return Ok(repository);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { errors = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.Message });
            }
        }

        [HttpDelete("{repositoryId:int}")]
        public async Task<ActionResult> DeleteRepository([FromRoute] int repositoryId)
        {
            try
            {
                var userId = GetCurrentUserId();

                await _repositoryService.DeleteRepositoryAsync(userId, repositoryId);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { errors = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
                throw new UnauthorizedAccessException("X-User-Id header is missing");

            if (!int.TryParse(userIdHeader, out var userId))
                throw new UnauthorizedAccessException("X-User-Id header is invalid");

            return userId;
        }
    }
}
