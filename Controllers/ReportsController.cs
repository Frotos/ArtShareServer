using System.Threading.Tasks;
using ArtShareServer.Models;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ArtShareServer.Controllers {
  //TODO: Create other endpoints
  [ApiController]
  [Route("api/[controller]")]
  public class ReportsController : ControllerBase {
    private readonly ISessionRepository _sessionRepository;
    private readonly IContentReportRepository _contentReportRepository;
    private readonly ICommentReportRepository _commentReportRepository;
    
    public ReportsController(ISessionRepository sessionRepository, IContentReportRepository contentReportRepository, ICommentReportRepository commentReportRepository) {
      _sessionRepository = sessionRepository;
      _contentReportRepository = contentReportRepository;
      _commentReportRepository = commentReportRepository;
    }

    [Route("content")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContentReportRequest request) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var session = await _sessionRepository.Get(sessionId);
        var user = session.User;

        if (user != null) {
          var reportToCreate = new ContentReport() {ContentId = request.ImageContentId, UserId = user.Id};
          var report = await _contentReportRepository.Create(reportToCreate);

          if (report != null) {
            JObject jObject = new JObject {{"id", new JValue(report.Id)}};
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
    
    [Route("comment")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentReportRequest request) {
      if (Request.Headers.ContainsKey("SessionId")) {
        var sessionId = Request.Headers["SessionId"].ToString();
        var session = await _sessionRepository.Get(sessionId);
        var user = session.User;

        if (user != null) {
          var reportToCreate = new CommentReport() {CommentId = request.CommentId, UserId = user.Id};
          var report = await _commentReportRepository.Create(reportToCreate);

          if (report != null) {
            JObject jObject = new JObject {{"id", new JValue(report.Id)}};
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
  }
}