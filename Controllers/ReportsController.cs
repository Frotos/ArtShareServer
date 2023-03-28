using System.Security.Claims;
using System.Threading.Tasks;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Models;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtShareServer.Controllers {
  //TODO: Create other endpoints
  [ApiController]
  [Route("api/[controller]")]
  public class ReportsController : ControllerBase {
    private readonly IContentReportRepository _contentReportRepository;
    private readonly ICommentReportRepository _commentReportRepository;
    private readonly IUserRepository _userRepository;
    
    public ReportsController(IContentReportRepository contentReportRepository, ICommentReportRepository commentReportRepository, IUserRepository userRepository) {
      _contentReportRepository = contentReportRepository;
      _commentReportRepository = commentReportRepository;
      _userRepository = userRepository;
    }

    [Route("content")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Create([FromBody] CreateContentReportRequest request) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      var reportToCreate = new ContentReport {ContentId = request.ContentId, UserId = userId};
      var report = await _contentReportRepository.Create(reportToCreate);
      
      return Ok(new {report.Id});
    }

    [Route("comment")]
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemeConstants.AuthSchemeName)]
    public async Task<IActionResult> Create([FromBody] CreateCommentReportRequest request) {
      var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      var reportToCreate = new CommentReport() {CommentId = request.CommentId, UserId = userId};
      var report = await _commentReportRepository.Create(reportToCreate);

      return Ok(new {report.Id});
    }
  }
}