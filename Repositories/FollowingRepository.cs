using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class FollowingRepository : IFollowingRepository {
    private readonly EFDBContext _context;
    
    public FollowingRepository(EFDBContext context) {
      _context = context;
    }

    public async Task<Following> Create(int followUserId, int userId) {
      if (followUserId <= 0 && followUserId == userId) {
        throw new BadRequestHttpException("Passed incorrect following");
      }
      
      if (await _context.Followings.AnyAsync(f => f.FollowUserId == followUserId &&
                                                  f.FollowingUserId == userId)) {
        throw new BadRequestHttpException("Passed already existed following");
      }

      var following = new Following() {FollowUserId = followUserId, FollowingUserId = userId};
      
      await _context.Followings.AddAsync(following);
      await _context.SaveChangesAsync();

      return following;
    }

    public async Task<Following> Get(int id) {
      var following = await _context.Followings.FirstOrDefaultAsync(f => f.Id == id);

      if (following == null) {
        throw new NotFoundHttpException("Following with passed id not found");
      }

      return following;
    }

    public async Task Delete(int id, int userId) {
      var following = await _context.Followings.FirstOrDefaultAsync(
          f => f.FollowUserId == id && f.FollowingUserId == userId);

      if (following == null) {
        throw new NotFoundHttpException("Following with passed id not found");
      }
      
      _context.Followings.Remove(following);
      await _context.SaveChangesAsync();
    }
  }
}