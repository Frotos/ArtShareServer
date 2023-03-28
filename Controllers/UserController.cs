using System.Security.Claims;
using System.Threading.Tasks;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase {
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository) {
      _userRepository = userRepository;
    }
    
    [Route("session_user")]
    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Get() {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
        
      return Ok(user);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Update([FromBody] User updatedUser) {
      await _userRepository.Update(updatedUser);
      return Ok(updatedUser);
    }

    //TODO: Test it
    [Route("{id:int}")]
    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Delete(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      
      await _userRepository.Delete(userId);
      return Ok();
    }
  }
}