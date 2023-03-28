using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ArtShareServer.Exceptions;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace ArtShareServer.Infrastructure.Authentication {
  public class AuthHandler : AuthenticationHandler<AuthSchemeOptions> {
    private readonly ISessionRepository _sessionRepository;

    public AuthHandler(IOptionsMonitor<AuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
                       ISystemClock clock, ISessionRepository sessionRepository) :
        base(options, logger, encoder, clock) {
      _sessionRepository = sessionRepository;
    }
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
      if (!Request.Headers.ContainsKey(HeaderNames.Authorization)) {
        return AuthenticateResult.Fail("Header not found");
      }

      var token = Request.Headers[HeaderNames.Authorization].FirstOrDefault();

      if (token == null) {
        throw new ForbiddenHttpException("Not passed auth token");
      }

      var session = await _sessionRepository.Get(token);

      if (session != null) {
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var claims = decodedToken.Claims;
        var claimsIdentity = new ClaimsIdentity(claims, nameof(AuthHandler));
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
      }

      return AuthenticateResult.Fail("Unable to authenticate");
    }
  }
}