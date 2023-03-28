using System.Collections.Generic;
using System.Threading.Tasks;
using ArtShareServer.Models;
using ArtShareServer.Models.DTOs;

namespace ArtShareServer.Repositories.Interfaces {
  //TODO: refactor to async
  public interface ICommentRepository {
    public Task<Comment> Create(Comment comment);
    public Task<Comment> Update(Comment updatedComment);
    public Task<Comment> Get(int id);
    public Task<List<Comment>> GetAll(User user);
    public Task<List<CommentDto>> GetPaged(int contentId, int page, int userId);
    public Task Delete(int id, User user);
    public Task Like(int id, User user);
    public Task Unlike(int id, User user);
    public Task Dislike(int id, User user);
    public Task Undislike(int id, User user);
  }
}