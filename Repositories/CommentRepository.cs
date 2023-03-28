using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Infrastructure;
using ArtShareServer.Models;
using ArtShareServer.Models.DTOs;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class CommentRepository : ICommentRepository {
    private readonly EFDBContext _context;
    private readonly Mapper _mapper;

    public CommentRepository(EFDBContext context, Mapper mapper) {
      _context = context;
      _mapper = mapper;
    }
    
    public async Task<Comment> Create(Comment comment) {
      if (comment.ContentId <= 0 || comment.User == null) {
        throw new BadRequestHttpException("Passed incorrect comment");
      }
      
      await _context.Comments.AddAsync(comment);
      await _context.SaveChangesAsync();

      return comment;
    }

    public async Task<Comment> Update(Comment updatedComment) {
      if (updatedComment == null) {
        throw new BadRequestHttpException("Updated comment can't be null");
      }
      // TODO: Create validation for comment model

      var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == updatedComment.Id);

      if (comment != null) {
        comment.Copy(updatedComment);
        await _context.SaveChangesAsync();

        return comment;
      }

      return null;
    }

    public async Task<Comment> Get(int id) {
      if (id <= 0) {
        throw new BadRequestHttpException("Id can't be less than 1");
      }
      
      var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

      if (comment == null) {
        throw new NotFoundHttpException("Comment with passed id not found");
      }

      return comment;
    }

    // TODO: Customize output for user ??
    public async Task<List<Comment>> GetAll(User user) {
      var comments = await _context.Comments.Select(c => c).ToListAsync();

      return comments;
    }

    public async Task<List<CommentDto>> GetPaged(int contentId, int page, int userId) {
      if (page <= 0) {
        throw new BadRequestHttpException("Page can't be less than 1");
      }
      
      var pageSize = 10;

      var user = await _context.Users
          .Include(u => u.CommentLikes)
          .Include(u => u.CommentDislikes)
          .FirstOrDefaultAsync(u => u.Id == userId);

      var comments = await _context.Comments
          .Include(c => c.User)
          .Include(c => c.User.Avatar)
          .Select(c => c).Where(c => c.ContentId == contentId).Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();
      var mappedComments = new List<CommentDto>(comments.Count);

      foreach (var comment in comments) {
        bool isLiked = user.CommentLikes.Any(l => l.CommentId == comment.Id);
        bool isDisliked = user.CommentDislikes.Any(d => d.CommentId == comment.Id);
        bool isUploadedByUser = comment.UserId == userId;
        
        mappedComments.Add(_mapper.MapCommentToDto(comment, isLiked,  isDisliked, isUploadedByUser));
      }

      return mappedComments;
    }

    public async Task Delete(int id, User user) {
      if (id > 0) {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null) {
          if (comment.User == user) {
            _context.CommentLikes.RemoveRange(_context.CommentLikes.Select(l => l).Where(l => l.CommentId == comment.Id));
            _context.CommentDislikes.RemoveRange(_context.CommentDislikes.Select(d => d).Where(d => d.CommentId == comment.Id));
            _context.CommentReports.RemoveRange(_context.CommentReports.Select(r => r).Where(r => r.Comment.Id == comment.Id));
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
          } else {
            throw new ForbiddenHttpException("You're trying to delete comment, that you didn't post");
          }
        }
      } else {
        throw new BadRequestHttpException("Id of comment can't be less than 1");
      }
    }

    public async Task Like(int id, User user) {
      // TODO: Check if user is null
      if (id > 0) {
        var comment = await _context.Comments.Include(c => c.Likes)
            .Include(c => c.Dislikes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null) {
          if (comment.Likes.Any(like => like.UserId == user.Id) ||
              comment.Dislikes.Any(dislike => dislike.UserId == user.Id)) {
            throw new BadRequestHttpException("You already reacted to this comment");
          }

          await _context.CommentLikes.AddAsync(new CommentLike() {UserId = user.Id, CommentId = comment.Id});
          await _context.SaveChangesAsync();
                    
          comment.Likes = await _context.CommentLikes.Select(l => l).Where(l => l.CommentId == comment.Id).ToListAsync();
          comment.LikesCount = comment.Likes.Count;
          await _context.SaveChangesAsync();
        } else {
          throw new NotFoundHttpException("Comment with passed id wasn't found");
        }
      } else {
        throw new BadRequestHttpException("Id of comment can't be less than 1");
      }
    }

    public async Task Unlike(int id, User user) {
      if (id > 0) {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null) {
          var like = await _context.CommentLikes.FirstOrDefaultAsync(
              l => l.CommentId == comment.Id && l.UserId == user.Id);

          if (like != null) {
            _context.CommentLikes.Remove(like);
            await _context.SaveChangesAsync();

            comment.Likes.Remove(like);
            comment.LikesCount = comment.Likes.Count;
            await _context.SaveChangesAsync();
          } else {
            throw new BadRequestHttpException("You didn't like this comment to unlike it");
          }
        } else {
          throw new NotFoundHttpException("Comment with passed id wasn't found");
        }
      } else {
        throw new BadRequestHttpException("Id of comment can't be less than 1");
      }
    }

    public async Task Dislike(int id, User user) {
      if (id > 0) {
        var comment = await _context.Comments.Include(c => c.Likes)
            .Include(c => c.Dislikes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null)
        {
          if (comment.Dislikes.Any(dislike => dislike.UserId == user.Id) ||
              comment.Likes.Any(like => like.UserId == user.Id))
          {
            throw new BadRequestHttpException("You already reacted to this comment");
          }

          await _context.CommentDislikes.AddAsync(new CommentDislike() {UserId = user.Id, CommentId = comment.Id});
          await _context.SaveChangesAsync();
                    
          comment.Dislikes = await _context.CommentDislikes.Select(d => d).Where(d => d.CommentId == comment.Id).ToListAsync();
          comment.DislikesCount = comment.Dislikes.Count;
          await _context.SaveChangesAsync();
        } else {
          throw new NotFoundHttpException("Comment with passed id wasn't found");
        }
      } else {
        throw new BadRequestHttpException("Id of comment can't be less than 1");
      }
    }

    public async Task Undislike(int id, User user) {
      if (id > 0) {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

        if (comment != null) {
          var dislike = await _context.CommentDislikes.FirstOrDefaultAsync(d => d.CommentId == comment.Id && d.UserId == user.Id);

          if (dislike != null) {
            _context.CommentDislikes.Remove(dislike);
            await _context.SaveChangesAsync();

            comment.Dislikes.Remove(dislike);
            comment.DislikesCount = comment.Likes.Count;
            await _context.SaveChangesAsync();
          } else {
            throw new BadRequestHttpException("You didn't dislike this comment to undislike it");
          }
        } else {
          throw new NotFoundHttpException("Comment with passed id wasn't found");
        }
      } else {
        throw new BadRequestHttpException("Id of comment can't be less than 1");
      }
    }
  }
}