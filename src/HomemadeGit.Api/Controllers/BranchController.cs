using HomemadeGit.Core.DTOs.Branches;
using HomemadeGit.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomemadeGit.Api.Controllers
{
    [Route("api/repositories/{repositoryId:int}/branches")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BranchResponse>), 200)]
        public async Task<ActionResult<List<BranchResponse>>> GetBranches([FromRoute] int repositoryId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var branches = await _branchService.GetBranchesAsync(userId, repositoryId);

                return Ok(branches);
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

        [HttpPost]
        [ProducesResponseType(typeof(BranchResponse), 200)]
        public async Task<ActionResult<BranchResponse>> CreateBranch([FromRoute] int repositoryId, [FromBody] CreateBranchRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();

                var branch = await _branchService.CreateBranchAsync(userId, repositoryId, request);

                return Ok(branch);
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
