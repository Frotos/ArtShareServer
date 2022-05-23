using System.Collections.Generic;
using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface IContentReportRepository {
    public Task<ContentReport> Create(ContentReport report);
    public Task<ContentReport> Update(ContentReport updatedReport);
    // TODO: create verification for user rights
    public Task<ContentReport> Get(int id);
    // TODO: create verification for user rights
    public Task<List<ContentReport>> GetAll();
    // TODO: create verification for user rights
    public void Delete(int id);
  }
}