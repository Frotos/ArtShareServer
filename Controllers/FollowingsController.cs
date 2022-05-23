using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class FollowingsController : ControllerBase {
    private readonly EFDBContext _context;
    private readonly IFollowingRepository _followingRepository;
    
    public FollowingsController(EFDBContext context, IFollowingRepository followingRepository) {
      _context = context;
      _followingRepository = followingRepository;
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> Get(int id) {
      var following = await _followingRepository.Get(id);

      if (following != null) {
        return Ok(JsonConvert.SerializeObject(following));
      } else {
        return NotFound("Following with passed id doesn't exist");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Create(Following following) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null && following.FollowingUserId == user.Id) {
          var createdFollowing = await _followingRepository.Create(following);

          if (createdFollowing != null) {
            JObject jObject = new JObject {{"id", new JValue(following.Id)}};
            return Ok(jObject.ToString());
          } else {
            return BadRequest();
          }
        } else {
          return BadRequest("You can't follow another user using not yours account");
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }

    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> Delete(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null) {
          await _followingRepository.Delete(id, user);
          return Ok();
        } else {
          return BadRequest("You can't follow another user using not yours account");
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }
  }
}