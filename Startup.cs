using ArtShareServer.Infrastructure;
using ArtShareServer.Infrastructure.Authentication;
using ArtShareServer.Infrastructure.Authentication.Models;
using ArtShareServer.Infrastructure.Middlewares;
using ArtShareServer.Repositories;
using ArtShareServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ArtShareServer {
  public class Startup {
    private IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services) {
      string connection = Configuration.GetConnectionString("DefaultConnection");
      var jwtTokenConfig = Configuration.GetSection("JwtTokenConfig").Get<TokenConfig>();
      services.AddMvc();
      services.AddDbContext<EFDBContext>(options => options.UseSqlServer(connection));
      
      services.AddControllersWithViews()
        .AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
      
      services.AddSingleton(jwtTokenConfig);
      services.AddSingleton<Mapper>();
      services.AddScoped<ICommentRepository, CommentRepository>();
      services.AddScoped<IContentRepository, ContentRepository>();
      services.AddScoped<ISessionRepository, SessionRepository>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IContentReportRepository, ContentReportRepository>();
      services.AddScoped<ICommentReportRepository, CommentReportRepository>();
      services.AddScoped<IFollowingRepository, FollowingRepository>();

      services.AddSwaggerGen(c => {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "ArtShareServer", Version = "v1" });
      });

      services.AddAuthentication(options => options.DefaultScheme = AuthSchemeConstants.AuthSchemeName)
          .AddScheme<AuthSchemeOptions, AuthHandler>(
              AuthSchemeConstants.AuthSchemeName, options => {});
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "ArtShareServer v1");
        });
      }

      // app.UseHttpsRedirection();

      app.UseRouting();
      app.UseMiddleware<ExceptionMiddleware>();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}