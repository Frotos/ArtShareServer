using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class UserRepository : IUserRepository {
    private readonly EFDBContext _context;

    public UserRepository(EFDBContext context) {
      _context = context;
    }
    
    public async Task<User> Create(User user) {
      if (await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username) != null) {
        throw new BadRequestHttpException("There is already a user with this username");
      }
      
      if (await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email) != null) {
        throw new BadRequestHttpException("There is already a user with this e-mail");
      }
      
      Avatar avatar = new Avatar() { Image = user.Avatar.Image};
      user.Avatar = avatar;
      await _context.Avatars.AddAsync(avatar);
      await _context.SaveChangesAsync();
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();

      return user;
    }

    public async Task<User> Update(User updatedUser) {
      var dbUser = await _context.Users
          .Include(u => u.Avatar)
          .FirstOrDefaultAsync(u => u.Id == updatedUser.Id);

      if (dbUser == null) {
        throw new NotFoundHttpException("User with passed id not found");
      }

      dbUser.CopyValues(updatedUser);
      await _context.SaveChangesAsync();

      return dbUser;
    }

    public async Task<List<User>> GetAll() {
      var dbUsers = await _context.Users.Include(u => u.Avatar)
          .Include(u => u.Likes)
          .Include(u => u.Dislikes)
          .Include(u => u.CommentLikes)
          .Include(u => u.CommentDislikes)
          .Select(u => u).ToListAsync();

      return dbUsers;
    }

    public async Task<User> Get(int id) {
      var dbUser = await _context.Users.Include(u => u.Avatar)
          .Include(u => u.Likes)
          .Include(u => u.Dislikes)
          .Include(u => u.CommentLikes)
          .Include(u => u.CommentDislikes)
          .FirstOrDefaultAsync(u => u.Id == id);

      if (dbUser == null) {
        throw new NotFoundHttpException("User with passed id not found");
      }

      //TODO: Move to another place
      var contentCount = await _context.Content.CountAsync(i => i.User.Id == dbUser.Id);
      if (contentCount >= 0 && contentCount != dbUser.PostsCount)
      {
        dbUser.PostsCount = contentCount;
      }
      
      //TODO: Move to another place
      var followersCount = await _context.Followings.CountAsync(f => f.FollowUser.Id == dbUser.Id);
      if (followersCount >= 0 && followersCount != dbUser.FollowersCount)
      {
        dbUser.FollowersCount = followersCount;
      }
      
      //TODO: Move to another place
      var followingsCount = await _context.Followings.CountAsync(f => f.FollowingUser.Id == dbUser.Id);
      if (followingsCount >= 0 && followingsCount != dbUser.FollowingsCount)
      {
        dbUser.FollowingsCount = followingsCount;
      }
        
      await _context.SaveChangesAsync();
        
      return dbUser;
    }

    public async Task Delete(int id) {
      var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

      if (dbUser == null) {
        throw new NotFoundHttpException("Can't find user");
      }

      _context.Users.Remove(dbUser);
      await _context.SaveChangesAsync();
    }
  }
}