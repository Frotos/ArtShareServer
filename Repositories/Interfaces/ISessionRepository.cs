using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface ISessionRepository {
    Task<Session> Create(int userId);
    Task<Session> Update(string id, Session updatedSession);
    Task<Session> Update(string id, int userId = 0, User user = null, string ip = null, string last = null);
    void Delete(string id, User user);
    Task<Session> Get(string id);
  }
}