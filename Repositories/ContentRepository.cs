using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using NinjaNye.SearchExtensions;

namespace ArtShareServer.Repositories {
  public class ContentRepository : IContentRepository {
    private readonly EFDBContext _context;

    public ContentRepository(EFDBContext context) {
      _context = context;
    }

    public List<Content> Get(int userId) {
      var user = _context.Set<User>().FirstOrDefault(u => u.Id == userId);

      var images = _context.Images.Include(i => i.Likes)
          .Include(i => i.Dislikes)
          .Include(i => i.User)
          .Include(i => i.User.Avatar)
          .Select(i => i).ToList();

      foreach (var image in images) {
        if (user != null) {
          image.User.Password = null!;
          image.User.Email = null;

          image.User.Followers = _context.Followings
              .Include(u => u.FollowUser)
              .Include(u => u.FollowingUser)
              .Select(f => f)
              .Where(f => f.FollowUser.Id == image.User.Id)
              .ToList();

          if (image.User.Followers.Select(f => f).Any(f => f.FollowingUser.Id == userId)) {
            image.User.IsFollowedByUser = true;
          } else {
            image.User.IsFollowedByUser = false;
          }

          image.User.IsFollowedByUser = image.User.Followers.Select(f => f).Any(f => f.FollowingUserId == user.Id);

          if (user.Likes.Count > 0) {
            foreach (var like in user.Likes) {
              if (like.ContentId == image.Id) {
                image.IsLikedByUser = true;
                break;
              }

              image.IsLikedByUser = false;
            }
          } else {
            image.IsLikedByUser = false;
          }

          if (user.Dislikes.Count > 0) {
            foreach (var dislike in user.Dislikes) {
              if (dislike.ContentId == image.Id) {
                image.IsDislikedByUser = true;
                break;
              }

              image.IsDislikedByUser = false;
            }
          } else {
            image.IsDislikedByUser = false;
          }

          if (image.LikesCount != image.Likes.Count) {
            image.LikesCount = image.Likes.Count;
            _context.SaveChanges();
          }

          if (image.DislikesCount != image.Dislikes.Count) {
            image.DislikesCount = image.Dislikes.Count;
            _context.SaveChanges();
          }
        }
      }

      images = images.OrderBy(i => !i.User.IsFollowedByUser)
          .ThenByDescending(i => i)
          .ToList();

      return images;
    }

    public Content GetSingle(int id) {
      var content = _context.Set<Content>().FirstOrDefault(m => m.Id == id);

      return content;
    }

    public async Task<int> Create(Content content, int userId) {
      var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/content/images", $"{content.Id}.jpeg");
      await File.WriteAllBytesAsync(path, Convert.FromBase64String(content.ContentBase64));
      content.ContentPath = path;
      
      foreach (var tag in content.Tags) {
        tag.Contents.Add(content);
      }

      var dbUser = _context.Users.FirstOrDefault(u => u.Id == userId);

      if (dbUser != null) {
        dbUser.PostsCount++;
        content.User = dbUser;
        _context.Set<Content>().Add(content);
        await _context.SaveChangesAsync();

        return content.Id;
      }

      return 0;
    }

    public void Delete(int id, User user) {
      var image = _context.Images.FirstOrDefault(i => i.Id == id);

      if (image != null) {
        if (image.User == user) {
          _context.Likes.RemoveRange(_context.Likes.Select(l => l).Where(l => l.ContentId == id));
          _context.Dislikes.RemoveRange(_context.Dislikes.Select(d => d).Where(d => d.ContentId == id));
          _context.ContentReports.RemoveRange(_context.ContentReports.Select(r => r)
                                                  .Where(r => r.Content.Id == id));
          _context.Images.Remove(image);
          _context.SaveChanges();
        } else {
          throw new UnauthorizedAccessException("You cannot delete content that does not belong to you");
        }
      } else {
        throw new ContentNotFoundException("You're trying to delete non-existent content");
      }
    }

    //TODO: Implement
    public Content Update(Content obj) {
      throw new System.NotImplementedException();
    }

    public async Task<List<Content>> GetPaged(int page, int userId, ContentFiltersRequest filters) {
      var user = _context.Set<User>().FirstOrDefault(u => u.Id == userId);
      var pageSize = 10;

      var query = _context.Images.Include(i => i.Likes)
          .Include(i => i.Dislikes)
          .Include(i => i.User)
          .Include(i => i.User.Avatar)
          .Select(i => i);

      //TODO: Made better verification
      if (filters.SearchPhrases != null) {
        var searchPattern = GenerateSearchPattern(filters.SearchPhrases);
        //TODO: Add search by tags
        query = query.Search(i => i.Title, i => i.Description).Containing(filters.SearchPhrases);
      }
        
      if (filters.Favourites) {
        query = query.Where(i => i.Likes.Any(l => l.UserId == userId));
      }

      if (filters.AuthorId != 0) {
        query = query.Where(i => i.UserId == filters.AuthorId);
      }

      var images = await query.OrderByDescending(i => i)
          .ToListAsync();

      foreach (var image in images) {
        if (user != null) {
          image.User.Password = null!;
          image.User.Email = null;

          image.User.Followers = await _context.Followings
              .Include(u => u.FollowUser)
              .Include(u => u.FollowingUser)
              .Select(f => f)
              .Where(f => f.FollowUser.Id == image.User.Id)
              .ToListAsync();

          image.User.IsFollowedByUser = image.User.Followers.Select(f => f).Any(f => f.FollowingUserId == userId);

          if (user.Likes.Count > 0) {
            foreach (var like in user.Likes) {
              if (like.ContentId == image.Id) {
                image.IsLikedByUser = true;
                break;
              }

              image.IsLikedByUser = false;
            }
          } else {
            image.IsLikedByUser = false;
          }

          if (user.Dislikes.Count > 0) {
            foreach (var dislike in user.Dislikes) {
              if (dislike.ContentId == image.Id) {
                image.IsDislikedByUser = true;
                break;
              }

              image.IsDislikedByUser = false;
            }
          } else {
            image.IsDislikedByUser = false;
          }

          if (image.LikesCount != image.Likes.Count) {
            image.LikesCount = image.Likes.Count;
            await _context.SaveChangesAsync();
          }

          if (image.DislikesCount != image.Dislikes.Count) {
            image.DislikesCount = image.Dislikes.Count;
            await _context.SaveChangesAsync();
          }
        }

        image.ContentBase64 = Convert.ToBase64String(await File.ReadAllBytesAsync(image.ContentPath));
      }

      images = images.OrderBy(i => !i.User.IsFollowedByUser)
          .ThenByDescending(i => i)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      return images;
    }

    public void Like(int id, User user) {
      if (user != null) {
        var image = _context.Images.Include(i => i.Likes)
            .Include(i => i.Dislikes)
            .FirstOrDefault(i => i.Id == id);

        if (image != null) {
          if (image.Likes.Any(like => like.UserId == user.Id) ||
              image.Dislikes.Any(dislike => dislike.UserId == user.Id)) {
            throw new AlreadyReactedException("You're already liked this content");
          }

          _context.Likes.Add(new ContentLike() {UserId = user.Id, ContentId = image.Id});
          _context.SaveChanges();

          image.Likes = _context.Likes.Select(l => l).Where(l => l.ContentId == image.Id).ToList();
          image.LikesCount = image.Likes.Count;
          _context.SaveChanges();
        } else {
          throw new ContentNotFoundException("You're trying to like non-existent content");
        }
      } else {
        // TODO: Change error message
        throw new UnauthorizedAccessException("There aren't user, connected to this session");
      }
    }

    public void Unlike(int id, User user) {
      if (user != null)
      {
        var image = _context.Images.FirstOrDefault(i => i.Id == id);
        if (image != null)
        {
          var like = _context.Likes.FirstOrDefault(l => l.ContentId == image.Id && l.UserId == user.Id);

          if (like != null)
          {
            _context.Likes.Remove(like);
            _context.SaveChanges();
                
            image.Likes.Remove(like);
            image.LikesCount = image.Likes.Count;
            _context.SaveChanges();
          } else {
            throw new ArgumentException("You can't unlike not liked content");
          }
        } else {
          throw new ContentNotFoundException("You're trying to unlike non-existent content");
        }
      } else {
        throw new UnauthorizedAccessException("There aren't user, connected to this session");
      }
    }

    public void Dislike(int id, User user) {
      if (user != null) {
        var image = _context.Images.Include(i => i.Likes)
            .Include(i => i.Dislikes)
            .FirstOrDefault(i => i.Id == id);

        if (image != null) {
          if (image.Dislikes.Any(dislike => dislike.UserId == user.Id) ||
              image.Likes.Any(like => like.UserId == user.Id)) {
            throw new AlreadyReactedException("You're already disliked this content");
          }

          _context.Dislikes.Add(new ContentDislike() {UserId = user.Id, ContentId = image.Id});
          _context.SaveChanges();

          image.Dislikes = _context.Dislikes.Select(d => d).Where(d => d.ContentId == image.Id).ToList();
          image.DislikesCount = image.Dislikes.Count;
          _context.SaveChanges();
        } else {
          throw new ContentNotFoundException("You're trying to dislike non-existent content");
        }
      } else {
        throw new UnauthorizedAccessException("There aren't user, connected to this session");
      }
    }

    public void Undislike(int id, User user) {
      if (user != null)
      {
        var image = _context.Images.FirstOrDefault(i => i.Id == id);
        if (image != null)
        {
          var dislike = _context.Dislikes.FirstOrDefault(d => d.ContentId == image.Id && d.UserId == user.Id);

          if (dislike != null)
          {
            _context.Dislikes.Remove(dislike);
            _context.SaveChanges();
                
            image.Dislikes.Remove(dislike);
            image.DislikesCount = image.Likes.Count;
            _context.SaveChanges();
          } else {
            throw new ArgumentException("You cant undislike not disliked content");
          }
        } else {
          throw new ContentNotFoundException("You're trying to unlike non-existent content");
        }
      } else {
        throw new UnauthorizedAccessException("There aren't user, connected to this session");
      }
    }

    private string GenerateSearchPattern(string[] phrases) {
      var sb = new StringBuilder("[");

      if (phrases != null) {
        sb.AppendJoin('|', phrases);
      }
      
      sb.Append(']');

      return sb.ToString();
    }
  }
}