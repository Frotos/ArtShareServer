using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Models;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ArtShareServer.Repositories {
  public class SessionRepository : ISessionRepository {
    private readonly EFDBContext _context;
    private readonly TokenConfig _tokenConfig;
    private readonly byte[] _secret;
    
    public SessionRepository(EFDBContext context, TokenConfig tokenConfig) {
      _context = context;
      _tokenConfig = tokenConfig;
      _secret = Encoding.ASCII.GetBytes(_tokenConfig.Secret);
    }
    
    public async Task<Session> Create(User user) {
      if (user == null) {
        throw new BadRequestHttpException("User can't be null");
      }

      var created = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); 
      
      var session = new Session() {
        Token = GenerateToken(user),
        Created = created,
        Last = created
      };

      if (await _context.Sessions.FirstOrDefaultAsync(s => s.Token == session.Token) == null) {
        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
      }

      return session;
    }

    public async Task<Session> Update(string id, Session updatedSession) {
      var session = await Get(id);

      if (session != null) {
        session.Copy(updatedSession);
        await _context.SaveChangesAsync();
      } else {
        throw new NotFoundHttpException("Session with passed id doesn't exist");
      }

      return session;
    }

    public async Task<Session> Update(string token, string ip = null, string last = null) {
      var session = await Get(token);

      if (ip != null) {
        session.Ip = ip;
      }

      if (last != null) {
        session.Last = last;
      }

      await _context.SaveChangesAsync();

      return session;
    }

    public async Task Delete(string token) {
      var session = await Get(token);

      if (session != null) {
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
      } else {
        throw new NotFoundHttpException("Session with passed token doesn't exist");
      }
    }

    public async Task<Session> Get(string token) {
      return await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);
    }

    private List<Claim> GenerateClaimsIdentity(User user) {
      var claims = new List<Claim> {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      };

      return claims;
    }

    private string GenerateToken(User user) {
      var token = new JwtSecurityToken(
          issuer: _tokenConfig.Issuer,
          audience: _tokenConfig.Audience,
          claims: GenerateClaimsIdentity(user),
          signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256));
      var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
      
      return encodedToken;
    }
  }
}