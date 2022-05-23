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
    
    public User Create(User user) {
      if (_context.Users.FirstOrDefault(u => u.Username == user.Username) != null) {
        throw new UsernameIsTakenException("There is already a user with this username");
      }
      
      if (_context.Users.FirstOrDefault(u => u.Email == user.Email) != null) {
        throw new EmailIsTakenException("There is already a user with this e-mail");
      }
      
      Avatar avatar = new Avatar() { Image = user.Avatar.Image};
      user.Avatar = avatar;
      _context.Avatars.Add(avatar);
      _context.SaveChanges();
      _context.Users.Add(user);
      _context.SaveChanges();

      return user;
    }

    public async Task<User> Update(User updatedUser) {
      var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatedUser.Id);

      dbUser.CopyValues(updatedUser);
      await _context.SaveChangesAsync();

      return dbUser;
    }

    public List<User> GetAll() {
      var dbUsers = _context.Users.Include(u => u.Avatar)
          .Include(u => u.Likes)
          .Include(u => u.Dislikes)
          .Include(u => u.CommentLikes)
          .Include(u => u.CommentDislikes)
          .Select(u => u).ToList();

      return dbUsers;
    }

    public User Get(int id) {
      var dbUser = _context.Users.Include(u => u.Avatar)
          .Include(u => u.Likes)
          .Include(u => u.Dislikes)
          .Include(u => u.CommentLikes)
          .Include(u => u.CommentDislikes)
          .FirstOrDefault(u => u.Id == id);

      // TODO: Maybe delete it
      if (dbUser != null)
      {
        //TODO: Move to another place
        var imagesCount = _context.Images.Count(i => i.User.Id == dbUser.Id);
        if (imagesCount >= 0 && imagesCount != dbUser.PostsCount)
        {
          dbUser.PostsCount = imagesCount;
        }
      
        //TODO: Move to another place
        var followersCount = _context.Followings.Count(f => f.FollowUser.Id == dbUser.Id);
        if (followersCount >= 0 && followersCount != dbUser.FollowersCount)
        {
          dbUser.FollowersCount = followersCount;
        }
      
        //TODO: Move to another place
        var followingsCount = _context.Followings.Count(f => f.FollowingUser.Id == dbUser.Id);
        if (followingsCount >= 0 && followingsCount != dbUser.FollowingsCount)
        {
          dbUser.FollowingsCount = followingsCount;
        }
        
        _context.SaveChanges();
        
        return dbUser;
      }

      return null;
    }

    public void Delete(int id) {
      var dbUser = _context.Users.FirstOrDefault(u => u.Id == id);

      if (dbUser == null) {
        throw new UserNotFoundException("Can't find user");
      }

      _context.Users.Remove(dbUser);
      _context.SaveChanges();
    }
  }
}