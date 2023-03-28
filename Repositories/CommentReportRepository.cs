using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class CommentReportRepository : ICommentReportRepository {
    private readonly EFDBContext _context;
    
    public CommentReportRepository(EFDBContext context) {
      _context = context;
    }

    public async Task<CommentReport> Create(CommentReport report) {
      if (report.CommentId <= 0 || report.UserId <= 0) {
        throw new BadRequestHttpException("Passed incorrect report");
      }
      
      if (await _context.CommentReports.AnyAsync(r => r.UserId == report.UserId && r.CommentId == report.CommentId)) {
        throw new BadRequestHttpException("You already reported this comment");
      }

      await _context.CommentReports.AddAsync(report);
      await _context.SaveChangesAsync();

      return report;
    }

    public async Task<CommentReport> Update(CommentReport updatedReport) {
      //TODO: Check if updated report already exists in db
      if (updatedReport == null) {
        throw new BadRequestHttpException("Updated report can't be null");
      }

      var report = await _context.CommentReports.FirstOrDefaultAsync(r => r.Id == updatedReport.Id);

      if (report != null) {
        report.Copy(updatedReport);
        await _context.SaveChangesAsync();

        return report;
      }

      return null;
    }

    public async Task<CommentReport> Get(int id) {
      if (id > 0) {
        var report = await _context.CommentReports.FirstOrDefaultAsync(r => r.Id == id);

        return report;
      }

      return null;
    }

    public async Task<List<CommentReport>> GetAll() {
      var reports = await _context.CommentReports.Select(r => r).ToListAsync();

      return reports;
    }

    public async void Delete(int id) {
      if (id > 0) {
        var report = await _context.CommentReports.FirstOrDefaultAsync(r => r.Id == id);

        if (report != null) {
          _context.CommentReports.Remove(report);
          await _context.SaveChangesAsync();
        } else {
          throw new NotFoundHttpException("Can't delete non-existent report");
        }
      }
    }
  }
}