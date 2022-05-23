using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class ContentController : ControllerBase {
    private readonly EFDBContext _context;
    private readonly IContentRepository _contentRepository;

    public ContentController(EFDBContext context, IContentRepository contentRepository) {
      _context = context;
      _contentRepository = contentRepository;
    }

    // TODO: Refactor to similar style like in CommentsController
    [HttpGet]
    public IActionResult Get() {
      if (Request.Headers.ContainsKey("UserId")) {
        if (int.TryParse(Request.Headers["UserId"], out int userId)) {
          var images = _contentRepository.Get(userId);

          return Ok(JsonConvert.SerializeObject(images)); 
        } else {
          return BadRequest("Passed incorrect user id");
        }
      } else {
        return BadRequest("User id in headers not found");
      }
    }

    // TODO: Refactor to similar style like in CommentsController
    [Route(("{id:int}"))]
    [HttpGet]
    public IActionResult Get(int id) {
      var image = _contentRepository.GetSingle(id);

      if (image != null) {
        return Ok(JsonConvert.SerializeObject(image));
      } else {
        return NotFound("Content with provided id doesn't exist");
      }
    }

    // To get content authorizations isn't required
    [Route("page/{page:int}")]
    [HttpGet]
    public async Task<IActionResult> GetPaged(int page, [FromQuery] ContentFiltersRequest filtersRequest) {
      if (page > 0) {
        int.TryParse(Request.Headers["UserId"], out int userId);
        var images = await _contentRepository.GetPaged(page, userId, filtersRequest);
    
        if (images == null) {
          return NotFound("Page cannot be found");
        }
    
        return Ok(JsonConvert.SerializeObject(images));
      } else {
        return BadRequest("Page can't be less than 1");
      }
    }

    // TODO: Refactor to similar style like in CommentsController
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Content content) {
      int.TryParse(Request.Headers["UserId"], out int userId);

      var contentId = await _contentRepository.Create(content, userId);

      if (contentId > 0) {
        JObject jObject = new JObject {{"id", new JValue(contentId)}};
        return Ok(jObject.ToString());
      }

      return BadRequest();
    }

    [Route("{id:int}")]
    [HttpDelete]
    public IActionResult Delete(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        try {
          _contentRepository.Delete(id, user);
          return Ok("Content successfully deleted");
        }
        catch (UnauthorizedAccessException unauthorizedAccessException) {
          return Unauthorized(unauthorizedAccessException.Message);
        }
        catch (ContentNotFoundException argumentNullException) {
          return NotFound(argumentNullException.Message);
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }

    [Route("like/{id:int}")]
    [HttpPost]
    public IActionResult Like(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        try {
          _contentRepository.Like(id, user);
          return Ok("Content successfully unliked");
        }
        catch (ContentNotFoundException contentNotFoundException) {
          return NotFound(contentNotFoundException.Message);
        }
        catch (UnauthorizedAccessException unauthorizedAccessException) {
          return Unauthorized(unauthorizedAccessException.Message);
        }
        catch (Exception e) {
          return BadRequest(e.Message);
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }

    [Route("unlike/{id:int}")]
    [HttpPost]
    public IActionResult Unlike(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        try {
          _contentRepository.Unlike(id, user);
          return Ok("Content successfully liked");
        }
        catch (ArgumentException argumentException) {
          return BadRequest(argumentException.Message);
        }
        catch (ContentNotFoundException contentNotFoundException) {
          return NotFound(contentNotFoundException.Message);
        }
        catch (UnauthorizedAccessException unauthorizedAccessException) {
          return Unauthorized(unauthorizedAccessException.Message);
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }

    [Route("dislike/{id:int}")]
    [HttpPost]
    public IActionResult Dislike(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        try {
          _contentRepository.Dislike(id, user);
        }
        catch (ContentNotFoundException contentNotFoundException) {
          return NotFound(contentNotFoundException.Message);
        }
        catch (UnauthorizedAccessException unauthorizedAccessException) {
          return Unauthorized(unauthorizedAccessException.Message);
        }
        catch (Exception e) {
          return BadRequest(e.Message);
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }

      return Ok();
    }

    [Route("undislike/{id:int}")]
    [HttpPost]
    public IActionResult Undislike(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        try {
          _contentRepository.Undislike(id, user);
        }
        catch (ArgumentException argumentException) {
          return BadRequest(argumentException.Message);
        }
        catch (ContentNotFoundException contentNotFoundException) {
          return NotFound(contentNotFoundException.Message);
        }
        catch (UnauthorizedAccessException unauthorizedAccessException) {
          return Unauthorized(unauthorizedAccessException.Message);
        }

        return Ok();
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }
  }
}