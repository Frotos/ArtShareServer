using System.Security.Claims;
using System.Threading.Tasks;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class FollowingsController : ControllerBase {
    private readonly IFollowingRepository _followingRepository;

    public FollowingsController(IFollowingRepository followingRepository) {
      _followingRepository = followingRepository;
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> Get(int id) {
      var following = await _followingRepository.Get(id);

      return Ok(following);
    }

    [Route("{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Follow(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var createdFollowing = await _followingRepository.Create(id, userId);
      
      return Ok(new {createdFollowing.Id});
    }

    [Route("{id:int}")]
    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Unfollow(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      await _followingRepository.Delete(id, userId);
      return Ok();
    }
  }
}