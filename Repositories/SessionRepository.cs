using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtShareServer.Repositories {
  public class SessionRepository : ISessionRepository {
    private readonly EFDBContext _context;

    private const int KEY_LENGTH = 64;
    private readonly char[] _encoding = {
      'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
      'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
      '0', '1', '2', '3', '4', '5' };

    public SessionRepository(EFDBContext context) {
      _context = context;
    }
    
    public async Task<Session> Create(int userId) {
      var session = new Session();
      
      do
      {
        session.Id = GenerateSessionId();
      } while (_context.Sessions.FirstOrDefault(s => s.Id == session.Id) != null);
    
      session.UserId = userId;
      session.Created = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
      session.Last = session.Created;
    
      _context.Sessions.Add(session);
      await _context.SaveChangesAsync();

      return session;
    }

    public async Task<Session> Update(string id, Session updatedSession) {
      var session = await Get(id);

      if (session != null) {
        session.Copy(updatedSession);
        await _context.SaveChangesAsync();
      } else {
        throw new SessionNotFoundException("Session with passed id doesn't exist");
      }

      return session;
    }

    public async Task<Session> Update(string id, int userId = 0, User user = null, string ip = null, string last = null) {
      var session = await Get(id);

      if (userId != 0) {
        session.UserId = userId;
      }

      if (user != null) {
        session.User = user;
      }

      if (ip != null) {
        session.Ip = ip;
      }

      if (last != null) {
        session.Last = last;
      }

      await _context.SaveChangesAsync();

      return session;
    }

    public async void Delete(string id, User user) {
      var session = await Get(id);

      if (session != null) {
        if (session.User == user) {
          _context.Sessions.Remove(session);
          await _context.SaveChangesAsync(); 
        } else {
          throw new UnauthorizedAccessException("You can't delete a session that doesn't belong to you");
        }
      } else {
        throw new SessionNotFoundException("Session with passed id doesn't exist");
      }
    }

    public async Task<Session> Get(string id) {
      return await _context.Sessions.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
    }

    private string GenerateSessionId() {
      char[] identifier = new char[KEY_LENGTH];
      byte[] randomData = new byte[KEY_LENGTH];
      using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
      {
        rng.GetBytes(randomData);
      }
      for (int i = 0; i < identifier.Length; i++)
      {
        int pos = randomData[i] % _encoding.Length;
        identifier[i] = _encoding[pos];
      }
      return new string(identifier);
    }
  }
}