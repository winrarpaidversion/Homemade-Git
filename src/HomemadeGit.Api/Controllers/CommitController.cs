using HomemadeGit.Core.DTOs;
using HomemadeGit.Core.DTOs.Commits;
using HomemadeGit.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomemadeGit.Api.Controllers
{
    [ApiController]
    public class CommitController : ControllerBase
    {
        private readonly ICommitService _commitService;

        public CommitController(ICommitService commitService)
        {
            _commitService = commitService;
        }

        [HttpPost("api/repositories/{repositoryId:int}/branches/{branchId:int}/commits")]
        [ProducesResponseType(typeof(CommitResponse), 200)]
        public async Task<ActionResult<CommitResponse>> CreateCommit([FromRoute] int repositoryId, [FromRoute] int branchId, [FromBody] CreateCommitRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var commit = await _commitService.CreateCommitAsync(userId, repositoryId, branchId, request);

                return Ok(commit);

            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new {errors = ex.Message});

            }
            catch(Exception ex)
            {
                return BadRequest(new { errors = ex.Message });
            }
        }

        [HttpGet("api/repositories/{repositoryId:int}/commits")]
        [ProducesResponseType(typeof(IReadOnlyList<CommitListItemResponse>), 200)]
        public async Task<ActionResult<IReadOnlyList<CommitListItemResponse>>> GetRepositoryCommits([FromRoute] int repositoryId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var commits = await _commitService.GetRepositoryCommitsAsync(userId, repositoryId);

                return Ok(commits);
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

        [HttpGet("api/commits/{commitId:int}")]
        [ProducesResponseType(typeof(CommitResponse), 200)]
        public async Task<ActionResult<CommitResponse>> GetCommitById([FromRoute] int commitId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var commit = await _commitService.GetCommitByIdAsync(userId, commitId);

                return Ok(commit);
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

        [HttpGet("api/commits/{commitId:int}/snapshot")]
        [ProducesResponseType(typeof(RepositorySnapshotResponse), 200)]
        public async Task<ActionResult<RepositorySnapshotResponse>> GetCommitSnapshot([FromRoute] int commitId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var snapshot = await _commitService.GetCommitSnapshotAsync(userId, commitId);

                return Ok(snapshot);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { errors = ex.Message });
            }
            catch(Exception ex)
            {
                return BadRequest(new {errors = ex.Message});
            }
        }

        [HttpGet("api/repositories/{repositoryId:int}/clone")]
        [ProducesResponseType(typeof(RepositorySnapshotResponse), 200)]
        public async Task<ActionResult<RepositorySnapshotResponse>> CloneRepository([FromRoute] int repositoryId, [FromQuery] int? branchId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var snapshot = await _commitService.CloneRepositoryAsync(userId, repositoryId, branchId);

                return Ok(snapshot);
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

        [HttpPost("api/repositories/{repositoryId:int}/branches/{branchId:int}/reset")]
        public async Task<ActionResult> ResetBranchToCommit([FromRoute] int repositoryId, [FromRoute] int branchId, [FromBody] ResetBranchRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                await _commitService.ResetBranchToCommitAsync(userId, repositoryId, branchId, request);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
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
