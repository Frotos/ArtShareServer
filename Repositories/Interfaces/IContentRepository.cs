using System.Collections.Generic;
using System.Threading.Tasks;
using ArtShareServer.Models;
using ArtShareServer.Models.DTOs;
using ArtShareServer.Models.Requests;

namespace ArtShareServer.Repositories.Interfaces {
  public interface IContentRepository {
    public Task<int> Create(ContentDto content, int userId);
    public Task<Content> GetSingle(int id);
    public Task<List<ContentDto>> GetPaged(int page, int userId, ContentFiltersRequest filters);
    public Task<Content> Update(UpdateContentRequest updatedContent, int userId);
    public Task Delete(int id, User user);
    public void Like(int id, User user);
    public void Unlike(int id, User user);
    public void Dislike(int id, User user);
    public void Undislike(int id, User user);
  }
}