using System;
using System.Collections.Generic;
using System.Linq;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class CommentRepository : ICommentRepository {
    private readonly EFDBContext _context;

    public CommentRepository(EFDBContext context) {
      _context = context;
    }
    
    public Comment Create(Comment comment) {
      if (comment != null) {
        _context.Comments.Add(comment);
        _context.SaveChanges();

        return comment;
      }

      return null;
    }

    public Comment Update(Comment updatedComment) {
      if (updatedComment == null) {
        throw new ArgumentNullException(nameof(updatedComment), "Updated comment can't be null");
      }

      var comment = _context.Comments.FirstOrDefault(c => c.Id == updatedComment.Id);

      if (comment != null) {
        comment.Copy(updatedComment);
        _context.SaveChanges();

        return comment;
      }

      return null;
    }

    public Comment Get(int id) {
      if (id > 0) {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == id);

        return comment;
      }

      return null;
    }

    // TODO: Customize output for user ??
    public List<Comment> GetAll(User user) {
      var comments = _context.Comments.Select(c => c).ToList();

      return comments;
    }

    public List<Comment> GetPaged(int contentId, int page, User user) {
      if (page > 0) {
        // var total = _context.Comments.Select(c => c).Count(c => c.ImageContentId == contentId);
        var pageSize = 10;
        
        // temporary removed
        // var canPage = ((page * pageSize) - total) < 10;

        // if (!canPage) {
        //   return null;
        // }
      
        var comments = _context.Comments.Select(c => c).Where(c => c.ImageContentId == contentId).Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return comments;
      }

      return null;
    }

    public void Delete(int id, User user) {
      if (id > 0) {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == id);

        if (comment != null) {
          if (comment.User == user) {
            _context.CommentLikes.RemoveRange(_context.CommentLikes.Select(l => l).Where(l => l.CommentId == comment.Id));
            _context.CommentDislikes.RemoveRange(_context.CommentDislikes.Select(d => d).Where(d => d.CommentId == comment.Id));
            _context.CommentReports.RemoveRange(_context.CommentReports.Select(r => r).Where(r => r.Comment.Id == comment.Id));
            _context.Comments.Remove(comment);
            _context.SaveChanges();
          } else {
            throw new UnauthorizedAccessException("You're trying to delete comment, that you didn't post");
          }
        }
      } else {
        throw new ArgumentException("Id of comment can't be less than 1");
      }
    }

    public void Like(int id, User user) {
      if (id > 0) {
        var comment = _context.Comments.Include(c => c.Likes)
            .Include(c => c.Dislikes)
            .FirstOrDefault(c => c.Id == id);

        if (comment != null) {
          if (comment.Likes.Any(like => like.UserId == user.Id) ||
              comment.Dislikes.Any(dislike => dislike.UserId == user.Id)) {
            throw new AlreadyReactedException("You already reacted to this comment");
          }

          _context.CommentLikes.Add(new CommentLike() {UserId = user.Id, CommentId = comment.Id});
          _context.SaveChanges();
                    
          comment.Likes = _context.CommentLikes.Select(l => l).Where(l => l.CommentId == comment.Id).ToList();
          comment.LikesCount = comment.Likes.Count;
          _context.SaveChanges();
        } else {
          throw new CommentNotFoundException("Comment with passed id wasn't found");
        }
      } else {
        throw new ArgumentException("Id of comment can't be less than 1");
      }
    }

    public void Unlike(int id, User user) {
      if (id > 0) {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == id);

        if (comment != null) {
          var like = _context.CommentLikes.FirstOrDefault(
              l => l.CommentId == comment.Id && l.UserId == user.Id);

          if (like != null) {
            _context.CommentLikes.Remove(like);
            _context.SaveChanges();

            comment.Likes.Remove(like);
            comment.LikesCount = comment.Likes.Count;
            _context.SaveChanges();
          } else {
            throw new ArgumentException("You didn't like this comment to unlike it");
          }
        } else {
          throw new CommentNotFoundException("Comment with passed id wasn't found");
        }
      } else {
        throw new ArgumentException("Id of comment can't be less than 1");
      }
    }

    public void Dislike(int id, User user) {
      if (id > 0) {
        var comment = _context.Comments.Include(c => c.Likes)
            .Include(c => c.Dislikes)
            .FirstOrDefault(c => c.Id == id);

        if (comment != null)
        {
          if (comment.Dislikes.Any(dislike => dislike.UserId == user.Id) ||
              comment.Likes.Any(like => like.UserId == user.Id))
          {
            throw new AlreadyReactedException("You already reacted to this comment");
          }

          _context.CommentDislikes.Add(new CommentDislike() {UserId = user.Id, CommentId = comment.Id});
          _context.SaveChanges();
                    
          comment.Dislikes = _context.CommentDislikes.Select(d => d).Where(d => d.CommentId == comment.Id).ToList();
          comment.DislikesCount = comment.Dislikes.Count;
          _context.SaveChanges();
        } else {
          throw new CommentNotFoundException("Comment with passed id wasn't found");
        }
      } else {
        throw new ArgumentException("Id of comment can't be less than 1");
      }
    }

    public void Undislike(int id, User user) {
      if (id > 0) {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == id);

        if (comment != null) {
          var dislike = _context.CommentDislikes.FirstOrDefault(d => d.CommentId == comment.Id && d.UserId == user.Id);

          if (dislike != null) {
            _context.CommentDislikes.Remove(dislike);
            _context.SaveChanges();

            comment.Dislikes.Remove(dislike);
            comment.DislikesCount = comment.Likes.Count;
            _context.SaveChanges();
          } else {
            throw new ArgumentException("You didn't dislike this comment to undislike it");
          }
        } else {
          throw new CommentNotFoundException("Comment with passed id wasn't found");
        }
      } else {
        throw new ArgumentException("Id of comment can't be less than 1");
      }
    }
  }
}