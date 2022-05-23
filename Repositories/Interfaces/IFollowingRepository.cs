using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface IFollowingRepository {
    public Task<Following> Create(Following following);
    public Task<Following> Get(int id);
    public Task Delete(int id, User user);
  }
}