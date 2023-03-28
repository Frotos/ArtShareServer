using System.Security.Claims;
using System.Threading.Tasks;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Models.DTOs;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtShareServer.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class ContentController : ControllerBase {
    private readonly IContentRepository _contentRepository;
    private readonly IUserRepository _userRepository;

    public ContentController(IContentRepository contentRepository, IUserRepository userRepository) {
      _contentRepository = contentRepository;
      _userRepository = userRepository;
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Create([FromBody] ContentDto content) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      var contentId = await _contentRepository.Create(content, userId);
      
      return Ok(new {Id = contentId, Message = "Content uploaded"});
    }

    [Route(("{id:int}"))]
    [HttpGet]
    public IActionResult Get(int id) {
      var content = _contentRepository.GetSingle(id);

      return Ok(content);
    }
    
    [Route("page/{page:int}")]
    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> GetPaged(int page, [FromQuery] ContentFiltersRequest filtersRequest) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var content = await _contentRepository.GetPaged(page, userId, filtersRequest);

      return Ok(content);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Update([FromBody] UpdateContentRequest updatedContent) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      await _contentRepository.Update(updatedContent, userId);
      
      return Ok(new {Message = "Content uploaded"});
    }

    [Route("{id:int}")]
    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Delete(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      await _contentRepository.Delete(id, user);
      return Ok("Content successfully deleted");
    }

    [Route("like/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Like(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      _contentRepository.Like(id, user);
      return Ok("Content successfully unliked");
    }

    [Route("unlike/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Unlike(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
        
      _contentRepository.Unlike(id, user);
      return Ok("Content successfully liked");
    }

    [Route("dislike/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Dislike(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
      
      _contentRepository.Dislike(id, user);
      return Ok("Content successfully disliked");
    }

    [Route("undislike/{id:int}")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Undislike(int id) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var user = await _userRepository.Get(userId);
        
      _contentRepository.Undislike(id, user);
      return Ok("Content successfully undisliked");
    }
  }
}