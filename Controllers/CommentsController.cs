using System;
using System.Linq;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArtShareServer.Controllers {
  //TODO: Remove context and use sessions repository
  [ApiController]
  [Route("api/[controller]")]
  public class CommentsController : ControllerBase {
    private readonly EFDBContext _context;
    private readonly ICommentRepository _commentRepository;

    public CommentsController(EFDBContext context, ICommentRepository commentRepository) {
      _context = context;
      _commentRepository = commentRepository;
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] CreateCommentRequest request) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null) {
          if (request.ContentId <= 0 || string.IsNullOrEmpty(request.CommentText)) {
            return BadRequest("You passed incorrect content id or comment text");
          }
          
          var commentToCreate = new Comment() {ImageContentId = request.ContentId, CommentText = request.CommentText, PublishDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"), User = user};
          var comment = _commentRepository.Create(commentToCreate);

          if (comment != null) {
            JObject jObject = new JObject {{"id", new JValue(comment.Id)}};
            return Ok(jObject.ToString());
          } else {
            // TODO: Write error message
            return BadRequest();
          }
        } else {
          // TODO: Write error message
          return Unauthorized();
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }

    // TODO: Maybe change updatedComment to updatedText and change only text of comment
    [HttpPut]
    public IActionResult Update(Comment updatedComment) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions.Include(s => s.User).FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null) {
          var comment = _commentRepository.Update(updatedComment);

          if (comment != null) {
            return Ok(JsonConvert.SerializeObject(comment));
          } else {
            return BadRequest();
          }
        } else {
          return Unauthorized();
        }
      } else {
        return BadRequest("Can't find session id in request headers");
      }
    }
    
    [HttpGet]
    public IActionResult Get() {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions
            .Include(s => s.User)
            .Include(s => s.User.Avatar)
            .FirstOrDefault(s => s.Id == sessionId)?.User;

          if (user != null) {
            var comments = _commentRepository.GetAll(user);

            return Ok(JsonConvert.SerializeObject(comments));
          } else {
            return BadRequest("User with passed id doesn't exist");
          }
      } else {
        return BadRequest("User id in headers not found");
      }
    }

    [Route("{id:int}")]
    [HttpGet]
    public IActionResult Get(int id) {
      var comment = _commentRepository.Get(id);

      if (comment != null) {
        return Ok(JsonConvert.SerializeObject(comment));
      } else {
        return NotFound("Comment with passed id not found");
      }
    }

    [Route("{contentId:int}/page/{page:int}")]
    [HttpGet]
    public IActionResult GetPaged(int contentId, int page) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions
            .Include(s => s.User)
            .Include(s => s.User.Avatar)
            .FirstOrDefault(s => s.Id == sessionId)?.User;

          if (user != null) {
            var comments = _commentRepository.GetPaged(contentId, page, user);

            if (comments == null) {
              return BadRequest("Page is less than 1");
            }

            return Ok(JsonConvert.SerializeObject(comments));
          } else {
            return BadRequest("User with passed id doesn't exist");
          }
      } else {
        return BadRequest("User id in headers not found");
      }
    }

    // TODO: Maybe remove including avatar
    [Route("{id:int}")]
    [HttpDelete]
    public IActionResult Delete(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions
            .Include(s => s.User)
            .Include(s => s.User.Avatar)
            .FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null) {
          try {
            _commentRepository.Delete(id, user);
          }
          catch (UnauthorizedAccessException unauthorizedAccessException) {
            return Unauthorized(unauthorizedAccessException.Message);
          }
          catch (ArgumentException argumentException) {
            return NotFound(argumentException.Message);
          }
        } else {
          return BadRequest("User with passed id doesn't exist");
        }
      } else {
        return BadRequest("User id in headers not found");
      }

      return Ok();
    }

    [Route("like/{id:int}")]
    [HttpPost]
    public IActionResult Like(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions
            .Include(s => s.User)
            .Include(s => s.User.Avatar)
            .FirstOrDefault(s => s.Id == sessionId)?.User;


        if (user != null) {
          try {
            _commentRepository.Like(id, user);
          }
          catch (CommentNotFoundException commentNotFoundException) {
            return NotFound(commentNotFoundException.Message);
          }
          catch (AlreadyReactedException alreadyReactedException) {
            return BadRequest(alreadyReactedException.Message);
          }
          catch (ArgumentException argumentException) {
            return NotFound(argumentException.Message);
          }
        } else {
          return BadRequest("User with passed id doesn't exist");
        }
      } else {
        return BadRequest("User id in headers not found");
      }

      return Ok();
    }

    [Route("unlike/{id:int}")]
    [HttpPost]
    public IActionResult Unlike(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions
            .Include(s => s.User)
            .Include(s => s.User.Avatar)
            .FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null) {
          try {
            _commentRepository.Unlike(id, user);
          }
          catch (CommentNotFoundException commentNotFoundException) {
            return NotFound(commentNotFoundException.Message);
          }
          catch (ArgumentException argumentException) {
            return NotFound(argumentException.Message);
          }
        } else {
          return BadRequest("User with passed id doesn't exist");
        }
      } else {
        return BadRequest("User id in headers not found");
      }

      return Ok();
    }

    [Route("dislike/{id:int}")]
    [HttpPost]
    public IActionResult Dislike(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions
            .Include(s => s.User)
            .Include(s => s.User.Avatar)
            .FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null) {
          try {
            _commentRepository.Dislike(id, user);
          }
          catch (CommentNotFoundException commentNotFoundException) {
            return NotFound(commentNotFoundException.Message);
          }
          catch (AlreadyReactedException alreadyReactedException) {
            return BadRequest(alreadyReactedException.Message);
          }
          catch (ArgumentException argumentException) {
            return NotFound(argumentException.Message);
          }
        } else {
          return BadRequest("User with passed id doesn't exist");
        }
      } else {
        return BadRequest("User id in headers not found");
      }

      return Ok();
    }

    [Route("undislike/{id:int}")]
    [HttpPost]
    public IActionResult Undislike(int id) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var user = _context.Sessions
            .Include(s => s.User)
            .Include(s => s.User.Avatar)
            .FirstOrDefault(s => s.Id == sessionId)?.User;

        if (user != null) {
          try {
            _commentRepository.Undislike(id, user);
          }
          catch (CommentNotFoundException commentNotFoundException) {
            return NotFound(commentNotFoundException.Message);
          }
          catch (ArgumentException argumentException) {
            return NotFound(argumentException.Message);
          }
        } else {
          return BadRequest("User with passed id doesn't exist");
        }
      } else {
        return BadRequest("User id in headers not found");
      }

      return Ok();
    }
  }
}