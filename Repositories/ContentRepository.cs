using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Infrastructure;
using ArtShareServer.Models;
using ArtShareServer.Models.DTOs;
using ArtShareServer.Models.Requests;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using NinjaNye.SearchExtensions;
using User = ArtShareServer.Models.User;

namespace ArtShareServer.Repositories {
  public class ContentRepository : IContentRepository {
    private readonly EFDBContext _context;
    private readonly Mapper _mapper;

    public ContentRepository(EFDBContext context, Mapper mapper) {
      _context = context;
      _mapper = mapper;
    }
    
    public async Task<int> Create(ContentDto contentDto, int userId) {
      var content = _mapper.MapDtoToContent(contentDto);
      var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

      if (dbUser == null) {
        throw new ForbiddenHttpException("");
      }
      
      foreach (var tag in contentDto.Tags) {
        var dbTag = _context.Tags.Select(t => t).SingleOrDefault(t => t.Name == tag.Name);

        if (dbTag != null) {
          dbTag.Contents.Add(content);
          content.Tags.Add(dbTag);
        } else {
          var tagToCreate = new Tag() {Name = tag.Name.ToLower(), Contents = new List<Content>() {content}};
          await _context.Tags.AddAsync(tagToCreate);
        }
      }

      dbUser.PostsCount++;
      content.User = dbUser;
      await _context.Set<Content>().AddAsync(content);
        
      await _context.SaveChangesAsync();
        
      var path = string.Empty;
      switch (content.ContentType) {
        case ContentTypesEnum.Image:
          path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/content/images", $"{content.Id}.jpeg");
          break;
        case ContentTypesEnum.Video:
          path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/content/videos", $"{content.Id}.mp4");
          break;
        case ContentTypesEnum.Audio:
          path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/content/audios", $"{content.Id}.mp3");
          break;
      }
      
      await File.WriteAllBytesAsync(path, Convert.FromBase64String(contentDto.ContentBase64));
      content.ContentPath = path;
        
      await _context.SaveChangesAsync();

      return content.Id;
    }

    public async Task<Content> GetSingle(int id) {
      var content = await _context.Set<Content>().FirstOrDefaultAsync(m => m.Id == id);

      if (content == null) {
        throw new NotFoundHttpException("Content with passed id not found");
      }

      return content;
    }
    
    public async Task<List<ContentDto>> GetPaged(int page, int userId, ContentFiltersRequest filters) {
      if (page <= 0) {
        throw new BadRequestHttpException("Page can't be less than 1");
      }
      
      var user = _context.Set<User>().FirstOrDefault(u => u.Id == userId);
      var pageSize = 10;

      var query = _context.Content.Include(i => i.Likes)
          .Include(i => i.Dislikes)
          .Include(i => i.User)
          .Include(i => i.User.Avatar)
          .Include(i => i.User.Followers)
          .Include(c => c.Tags)
          .Select(i => i);

      //TODO: Made better verification
      if (filters.SearchPhrases != null) {
        //TODO: Add search by tags
        query = query.Search(i => i.Title, i => i.Description).Containing(filters.SearchPhrases);
      }
        
      if (filters.Favourites) {
        query = query.Where(i => i.Likes.Any(l => l.UserId == userId));
      }

      if (filters.AuthorId != 0) {
        query = query.Where(i => i.UserId == filters.AuthorId);
      }

      if (filters.UploadedByUser) {
        query = query.Where(c => c.UserId == userId);
      }

      var contents = await query.OrderBy(c => !c.User.Followers.Any(f => f.FollowingUserId == userId))
          .ThenByDescending(i => i)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();
      var mappedContent = new List<ContentDto>(contents.Count);

      foreach (var content in contents) {
        if (user != null) {
          var isLiked = user.Likes.Any(like => like.ContentId == content.Id);
          var isDisliked = user.Dislikes.Any(dislike => dislike.ContentId == content.Id);
          var contentBase64 = Convert.ToBase64String(await File.ReadAllBytesAsync(content.ContentPath));
          var isFollowed = content.User.Followers.Any(f => f.FollowingUserId == userId);
          var isUploadedByUser = content.UserId == userId;
          
          mappedContent.Add(  _mapper.MapContentToDto(content, isLiked, isDisliked, isFollowed, contentBase64, isUploadedByUser));
        }
      }

      return mappedContent;
    }
    
    public async Task<Content> Update(UpdateContentRequest updatedContent, int userId) {
      var content = await _context.Content
          .Include(c => c.Tags)
          .FirstOrDefaultAsync(c => c.Id == updatedContent.Id && c.UserId == userId);
      
      if (content == null) {
        throw new BadRequestHttpException("Passed incorrect content");
      }

      content.Title = updatedContent.Title;
      content.Description = updatedContent.Description;

      foreach (var tag in content.Tags) {
        var dbTag = _context.Tags.Select(t => t).SingleOrDefault(t => t.Name == tag.Name);

        if (dbTag != null) {
          dbTag.Contents.Remove(content);
        }
      }

      content.Tags = new List<Tag>();

      foreach (var tag in updatedContent.Tags) {
        var dbTag = _context.Tags.Select(t => t).SingleOrDefault(t => t.Name == tag.Name);

        if (dbTag != null) {
          dbTag.Contents.Add(content);
          content.Tags.Add(dbTag);
        } else {
          var tagToCreate = new Tag() {Name = tag.Name, Contents = new List<Content>() {content}};
          await _context.Tags.AddAsync(tagToCreate);
        }
      }
      
      await _context.SaveChangesAsync();
      return content;
    }

    public async Task Delete(int id, User user) {
      var content = await _context.Content.FirstOrDefaultAsync(i => i.Id == id);

      if (content != null) {
        if (content.User == user) {
          _context.Likes.RemoveRange(_context.Likes.Select(l => l).Where(l => l.ContentId == id));
          _context.Dislikes.RemoveRange(_context.Dislikes.Select(d => d).Where(d => d.ContentId == id));
          _context.ContentReports.RemoveRange(_context.ContentReports.Select(r => r).Where(r => r.Content.Id == id));
          _context.Content.Remove(content);
          user.PostsCount--;
          await _context.SaveChangesAsync();
        } else {
          throw new ForbiddenHttpException("You cannot delete content that does not belong to you");
        }
      } else {
        throw new NotFoundHttpException("You're trying to delete non-existent content");
      }
    }

    public void Like(int id, User user) {
      if (user != null) {
        var image = _context.Content.Include(i => i.Likes)
            .Include(i => i.Dislikes)
            .FirstOrDefault(i => i.Id == id);

        if (image != null) {
          if (image.Likes.Any(like => like.UserId == user.Id) ||
              image.Dislikes.Any(dislike => dislike.UserId == user.Id)) {
            throw new BadRequestHttpException("You're already liked this content");
          }

          _context.Likes.Add(new ContentLike() {UserId = user.Id, ContentId = image.Id});
          _context.SaveChanges();

          image.Likes = _context.Likes.Select(l => l).Where(l => l.ContentId == image.Id).ToList();
          image.LikesCount = image.Likes.Count;
          _context.SaveChanges();
        } else {
          throw new NotFoundHttpException("You're trying to like non-existent content");
        }
      } else {
        throw new UnauthorizedHttpException("Unauthorized users can't react to content");
      }
    }

    public void Unlike(int id, User user) {
      if (user != null)
      {
        var image = _context.Content.FirstOrDefault(i => i.Id == id);
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
            throw new BadRequestHttpException("You can't unlike not liked content");
          }
        } else {
          throw new NotFoundHttpException("You're trying to unlike non-existent content");
        }
      } else {
        throw new UnauthorizedAccessException("Unauthorized users can't react to content");
      }
    }

    public void Dislike(int id, User user) {
      if (user != null) {
        var image = _context.Content.Include(i => i.Likes)
            .Include(i => i.Dislikes)
            .FirstOrDefault(i => i.Id == id);

        if (image != null) {
          if (image.Dislikes.Any(dislike => dislike.UserId == user.Id) ||
              image.Likes.Any(like => like.UserId == user.Id)) {
            throw new BadRequestHttpException("You're already disliked this content");
          }

          _context.Dislikes.Add(new ContentDislike() {UserId = user.Id, ContentId = image.Id});
          _context.SaveChanges();

          image.Dislikes = _context.Dislikes.Select(d => d).Where(d => d.ContentId == image.Id).ToList();
          image.DislikesCount = image.Dislikes.Count;
          _context.SaveChanges();
        } else {
          throw new NotFoundHttpException("You're trying to dislike non-existent content");
        }
      } else {
        throw new UnauthorizedHttpException("Unauthorized users can't react to content");
      }
    }

    public void Undislike(int id, User user) {
      if (user != null)
      {
        var image = _context.Content.FirstOrDefault(i => i.Id == id);
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
            throw new BadRequestHttpException("You cant undislike not disliked content");
          }
        } else {
          throw new NotFoundHttpException("You're trying to unlike non-existent content");
        }
      } else {
        throw new UnauthorizedHttpException("Unauthorized users can't react to content");
      }
    }
  }
}