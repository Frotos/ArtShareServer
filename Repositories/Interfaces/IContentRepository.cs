using System.Collections.Generic;
using System.Threading.Tasks;
using ArtShareServer.Models;
using ArtShareServer.Models.Requests;

namespace ArtShareServer.Repositories.Interfaces {
  public interface IContentRepository {
    public Task<int> Create(Content content, int userId);
    public List<Content> Get(int userId);
    public Content GetSingle(int id);
    public void Delete(int id, User user);
    public Content Update(Content content);
    public Task<List<Content>> GetPaged(int page, int userId, ContentFiltersRequest filters);
    public void Like(int id, User user);
    public void Unlike(int id, User user);
    public void Dislike(int id, User user);
    public void Undislike(int id, User user);
  }
}