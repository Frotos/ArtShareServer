using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class ContentReportRepository : IContentReportRepository {
    private readonly EFDBContext _context;

    public ContentReportRepository(EFDBContext context) {
      _context = context;
    }
    
    public async Task<ContentReport> Create(ContentReport report) {
      if (report.ContentId <= 0 || report.UserId <= 0) {
        throw new BadRequestHttpException("Passed incorrect report");
      }
      
      if (await _context.ContentReports.AnyAsync(r => r.UserId == report.UserId && r.ContentId == report.ContentId)) {
        throw new BadRequestHttpException("You already reported this content");
      }
          
      // TODO: use try
      await _context.ContentReports.AddAsync(report);
      await _context.SaveChangesAsync();

      return report;
    }

    public async Task<ContentReport> Update(ContentReport updatedReport) {
      //TODO: Check if updated report already exists in db
      if (updatedReport == null) {
        throw new BadRequestHttpException("Updated report can't be null");
      }

      var report = await _context.ContentReports.FirstOrDefaultAsync(r => r.Id == updatedReport.Id);

      if (report != null) {
        report.Copy(updatedReport);
        await _context.SaveChangesAsync();

        return report;
      }

      return null;
    }

    public async Task<ContentReport> Get(int id) {
      if (id > 0) {
        var report = await _context.ContentReports.FirstOrDefaultAsync(r => r.Id == id);

        return report;
      }

      return null;
    }

    public async Task<List<ContentReport>> GetAll() {
      var reports = await _context.ContentReports.Select(r => r).ToListAsync();

      return reports;
    }

    public async void Delete(int id) {
      if (id > 0) {
        var report = await _context.ContentReports.FirstOrDefaultAsync(r => r.Id == id);

        if (report != null) {
          _context.ContentReports.Remove(report);
          await _context.SaveChangesAsync();
        } else {
          throw new NotFoundHttpException("Can't delete non-existent report");
        }
      }
    }
  }
}