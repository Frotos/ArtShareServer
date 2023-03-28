using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface IFollowingRepository {
    public Task<Following> Create(int followUserId, int userId);
    public Task<Following> Get(int id);
    public Task Delete(int id, int userId);
  }
}