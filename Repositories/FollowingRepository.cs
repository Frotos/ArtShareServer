using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class FollowingRepository : IFollowingRepository {
    private readonly EFDBContext _context;
    
    public FollowingRepository(EFDBContext context) {
      _context = context;
    }

    public async Task<Following> Create(Following following) {
      if (following != null) {
        if (!await _context.Followings.AnyAsync(f => f.FollowUserId == following.FollowUserId &&
                                                     f.FollowingUserId == following.FollowingUserId)) {
          _context.Followings.Add(following);
          await _context.SaveChangesAsync();

          return following;
        }
      }

      return null;
    }

    public async Task<Following> Get(int id) {
      var following = await _context.Followings.FirstOrDefaultAsync(f => f.Id == id);

      return following;
    }

    public async Task Delete(int id, User user) {
      var following = await _context.Followings.FirstOrDefaultAsync(
          f => f.FollowUserId == id && f.FollowingUserId == user.Id);

      if (following != null) {
        _context.Followings.Remove(following);
        await _context.SaveChangesAsync();
      }
    }
  }
}