using GithubSearchAPI.DATA.MIGRATIONS;
using GithubSearchAPI.Repositoreis;
using GithubSearchAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GithubSearchAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // // Connection string to master database
            // var connectionString = builder.Configuration.GetConnectionString("MasterConnection");

            // // Run database migration
            // string migrationScriptPath = Path.Combine(AppContext.BaseDirectory, "Data", "Migrations", "SetupDatabase.sql");
            // Console.WriteLine("Enter a secure password for the database login:");
            // string password = Console.ReadLine();

            // var migrationExecutor = new MigrationExecutor(connectionString);
            // migrationExecutor.RunMigration(migrationScriptPath, password);


            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins("https://localhost:4200", "http://localhost:4200")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
                });

            });


            // Configure JWT Authentication
            var key = Convert.FromBase64String(builder.Configuration["Jwt:SecretKey"]);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = true,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Extract token from the secured cookie
                            context.Token = context.HttpContext.Request.Cookies["auth-token"];
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddHttpClient();

            // Configure Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IAuthRepository, AuthRepository>();
            builder.Services.AddSingleton<IGithubService, GithubService>();
            builder.Services.AddSingleton<IGithubRepository, GithubRepository>();

            builder.Services.AddControllers();

            builder.Services.AddMemoryCache();

            //swagger
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication(); 
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}