using System;
using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class AuthorizationController : ControllerBase {
    private readonly EFDBContext _context;
    private ISessionRepository _sessionRepository;
    private IUserRepository _userRepository;

    public AuthorizationController(EFDBContext context, ISessionRepository sessionRepository, IUserRepository userRepository) {
      _context = context;
      _sessionRepository = sessionRepository;
      _userRepository = userRepository;
    }

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Register(User user) {
      try {
        _userRepository.Create(user);
        var session = await _sessionRepository.Create(user.Id);
        return Ok(JsonConvert.SerializeObject(session));
      }
      catch (Exception exception) {
        return BadRequest(exception.Message);
      }
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login(User user) {
      if (Request.Headers.Keys.Contains("SessionId")) {
        var sessionId = Request.Headers["SessionId"];
        // validate session
        var dbSession = await _sessionRepository.Get(sessionId);
        if (dbSession != null) {
          var dbUser = _userRepository.Get(dbSession.UserId);
          if (dbUser != null)
          {
            // update session
            // TODO: Format can't be hardcoded
            dbSession = await _sessionRepository.Update(dbSession.Id, user: dbSession.User,
                                                      last: DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            return Ok(JsonConvert.SerializeObject(dbSession));
          }
        }
        else {
          return BadRequest("Provided incorrect session id");
        }
      }
      else {
        // TODO: Create method in user repository
        var dbUser = _context.Users.FirstOrDefault(u => (u.Username == user.Username ||
                                                             u.Email == user.Email) &&
                                                            u.Password == user.Password);
    
        if (dbUser != null) {
          var session = await _sessionRepository.Create(dbUser.Id);
              
          return Ok(JsonConvert.SerializeObject(session));
        }
        else {
          return BadRequest("Provided incorrect data");
        }
      }

      return BadRequest("Provided incorrect session");
    }

    [Route("logout")]
    [HttpDelete]
    public async Task<IActionResult> LogOut() {
      if (Request.Headers.Keys.Contains("SessionId")) {
        var sessionId = Request.Headers["SessionId"];
        var session = await _sessionRepository.Get(sessionId);
        
        if (session != null) {
          if (Request.Headers.Keys.Contains("UserId")) {
            var userId = Request.Headers["UserId"];

            if (int.TryParse(userId, out int parsedUserId)) {
              var user = _userRepository.Get(parsedUserId);

              if (user != null) {
                try {
                  _sessionRepository.Delete(sessionId, user);
                }
                catch (UnauthorizedAccessException unauthorizedAccessException) {
                  return Unauthorized(unauthorizedAccessException.Message);
                }
                catch (SessionNotFoundException sessionNotFoundException) {
                  return NotFound(sessionNotFoundException.Message);
                }
              } 
            }
          }
        }
      }

      return Ok();
    }
  }
}