using System.Collections.Generic;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  //TODO: refactor to async
  public interface ICommentRepository {
    public Comment Create(Comment comment);
    public Comment Update(Comment updatedComment);
    public Comment Get(int id);
    public List<Comment> GetAll(User user);
    public List<Comment> GetPaged(int contentId, int page, User user);
    public void Delete(int id, User user);
    public void Like(int id, User user);
    public void Unlike(int id, User user);
    public void Dislike(int id, User user);
    public void Undislike(int id, User user);
  }
}