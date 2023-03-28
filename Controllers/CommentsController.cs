using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Models;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class CommentsController : ControllerBase {
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CommentsController(ICommentRepository commentRepository, IUserRepository userRepository) {
      _commentRepository = commentRepository;
      _userRepository = userRepository;
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest request) {
      if (request.ContentId <= 0 || string.IsNullOrEmpty(request.CommentText)) {
        return BadRequest("You passed incorrect content id or comment text");
      }

      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      var commentToCreate = new Comment {ContentId = request.ContentId, CommentText = request.CommentText, PublishDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"), User = user};
      var comment = await _commentRepository.Create(commentToCreate);
      
      return Ok(new{comment.Id});
    }

    // TODO: Maybe change updatedComment to updatedText and change only text of comment
    [HttpPut]
    public async Task<IActionResult> Update(Comment updatedComment) {
      var comment = await _commentRepository.Update(updatedComment);
      
      return Ok(comment);
    }
    
    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Get() {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      var comments = await _commentRepository.GetAll(user);
      
      return Ok(comments);
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> Get(int id) {
      var comment = await _commentRepository.Get(id);

      return Ok(comment);
    }

    [Route("{contentId:int}/page/{page:int}")]
    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> GetPaged(int contentId, int page) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var comments = await _commentRepository.GetPaged(contentId, page, userId);
      
      return Ok(comments);
    }

    // TODO: Maybe remove including avatar
    [Route("{id:int}")]
    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Delete(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      await _commentRepository.Delete(id, user);

      return Ok();
    }

    [Route("like/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Like(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      await _commentRepository.Like(id, user);

      return Ok();
    }

    [Route("unlike/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Unlike(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      await _commentRepository.Unlike(id, user);

      return Ok();
    }

    [Route("dislike/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Dislike(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);

      await _commentRepository.Dislike(id, user);

      return Ok();
    }

    [Route("undislike/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Undislike(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      await _commentRepository.Undislike(id, user);

      return Ok();
    }
  }
}