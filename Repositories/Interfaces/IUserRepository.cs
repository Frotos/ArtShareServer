using System.Collections.Generic;
using System.Threading.Tasks;
using ArtShareServer.Models;

namespace ArtShareServer.Repositories.Interfaces {
  public interface IUserRepository {
    public User Create(User user);
    public Task<User> Update(User updatedUser);
    public List<User> GetAll();
    public User Get(int id);
    public void Delete(int id);
  }
}