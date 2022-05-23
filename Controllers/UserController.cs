using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase {
    //TODO: Remove this db context
    private readonly EFDBContext _context;
    private readonly IUserRepository _userRepository;

    public UserController(EFDBContext context, IUserRepository userRepository) {
      _context = context;
      _userRepository = userRepository;
    }

    //TODO: Maybe create only one method to get user, and just put user id in headers in client app
    [Route("session_user")]
    [HttpGet]
    public async Task<IActionResult> Get() {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var userId = await _context.Sessions.Where(s => s.Id == sessionId).Select(s => s.UserId).FirstOrDefaultAsync();

        var user = _userRepository.Get(userId);

        if (user != null) {
          return Ok(JsonConvert.SerializeObject(user));
        } else {
          return NotFound("User not found");
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] User updatedUser) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var userId = await _context.Sessions.Where(s => s.Id == sessionId && s.UserId == updatedUser.Id).Select(s => s.UserId).FirstOrDefaultAsync();

        var user = _userRepository.Get(userId);

        if (user != null) {
          await _userRepository.Update(updatedUser);
          return Ok(JsonConvert.SerializeObject(user));
        } else {
          return NotFound("User not found");
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }

    //TODO: Test it
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> Update(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var userId = await _context.Sessions.Where(s => s.Id == sessionId && s.UserId == id).Select(s => s.UserId).FirstOrDefaultAsync();

        _userRepository.Delete(userId);
        return Ok();
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }
  }
}