using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class AuthorizationController : ControllerBase {
    private readonly EFDBContext _context;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;

    public AuthorizationController(EFDBContext context, ISessionRepository sessionRepository, IUserRepository userRepository) {
      _context = context;
      _sessionRepository = sessionRepository;
      _userRepository = userRepository;
    }

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Register(User user) {
      await _userRepository.Create(user);
      var session = await _sessionRepository.Create(user);
      return Ok(session);
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login(User user) {
      if (Request.Headers.Keys.Contains(HeaderNames.Authorization)) {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var dbUser = await _userRepository.Get(userId);
        if (dbUser != null) {
          var token = Request.Headers[HeaderNames.Authorization];
          // TODO: Format can't be hardcoded
          var dbSession = await _sessionRepository.Update(token, last: DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
          return Ok(dbSession);
        }
      }
      else {
        // TODO: Create method in user repository
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => (u.Username == user.Username ||
                                                             u.Email == user.Email) &&
                                                            u.Password == user.Password);
      
        var session = await _sessionRepository.Create(dbUser);
              
        return Ok(session);
      }
      
      return BadRequest("Provided incorrect session");
    }

    [Route("logout")]
    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> LogOut() {
      await _sessionRepository.Delete(Request.Headers[HeaderNames.Authorization]);
      
      return Ok();
    }
  }
}