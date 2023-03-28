using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface ISessionRepository {
    Task<Session> Create(User user);
    Task<Session> Update(string id, Session updatedSession);
    Task<Session> Update(string id, string ip = null, string last = null);
    Task Delete(string token);
    Task<Session> Get(string id);
  }
}