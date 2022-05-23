using System.Collections.Generic;
using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface ICommentReportRepository {
    public Task<CommentReport> Create(CommentReport report);
    public Task<CommentReport> Update(CommentReport updatedReport);
    public Task<CommentReport> Get(int id);
    public Task<List<CommentReport>> GetAll();
    public void Delete(int id);
  }
}