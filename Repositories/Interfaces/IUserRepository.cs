using System.Collections.Generic;
using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface IUserRepository {
    public Task<User> Create(User user);
    public Task<User> Update(User updatedUser);
    public Task<List<User>> GetAll();
    public Task<User> Get(int id);
    public Task Delete(int id);
  }
}