
using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Services;
using HomemadeGit.Infrastructure.Data;
using HomemadeGit.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace HomemadeGit.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRepositoryService, RepositoryService>();
            builder.Services.AddScoped<ICommitService, CommitService>();
            builder.Services.AddScoped<IBranchService, BranchService>();

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IObjectHasher, Sha256ObjectHasher>();
            
            builder.Services.AddScoped<IUserStore, UserStore>();
            builder.Services.AddScoped<IBranchStore, BranchStore>();
            builder.Services.AddScoped<IRepositoryStore, RepositoryStore>();
            builder.Services.AddScoped<ICommitStore, CommitStore>();
            builder.Services.AddScoped<IBlobStore, BlobStore>();


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection"));
            });

            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.MapGet("/", () => "Hello world!");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
