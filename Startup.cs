using System;
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
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      string connection = Configuration.GetConnectionString("DefaultConnection");
      services.AddMvc();
      services.AddDbContext<EFDBContext>(options => options.UseSqlServer(connection));
      
      services.AddControllersWithViews()
        .AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

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
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "ArtShareServer v1");
          // c.RoutePrefix = String.Empty;
        });
      }

      // app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}