
using DungeonApi.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DungeonApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddDbContext<AppDbContext>();

            builder.Services.AddIdentityCore<AppUser>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.
                AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.
                AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.
                AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secret = builder.Configuration["JwtConfig:Secret"];
                var issuer = builder.Configuration["JwtConfig:ValidIssuer"];
                var audience = builder.Configuration["JwtConfig:ValidAudiences"];
                if (secret is null || issuer is null || audience is null)
                {
                    throw new ApplicationException("Jwt is not set in the configuration");
                }
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters =
                new
                TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidIssuer = issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.
                UTF8.GetBytes(secret))
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();

                using (var serviceScope = app.Services.CreateScope())
                {
                    var services = serviceScope.ServiceProvider;
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var configuration = services.GetRequiredService<IConfiguration>();
                    var dbContext = services.GetRequiredService<AppDbContext>();

                    await dbContext.Database.EnsureDeletedAsync();
                    await dbContext.Database.EnsureCreatedAsync();



                    if (!await roleManager.RoleExistsAsync(AppRoles.User))
                    {
                        await roleManager.CreateAsync(new IdentityRole(AppRoles.
                        User));
                    }

                    if (!await roleManager.RoleExistsAsync(AppRoles.
                    Admin))
                    {
                        await roleManager.CreateAsync(new IdentityRole(AppRoles.
                        Admin));
                    }

                    var adminUserName = configuration["AdminAccount:UserName"]!;
                    var adminPassword = configuration["AdminAccount:Password"]!;

                    var adminUser = await userManager.FindByNameAsync(adminUserName);
                    if (adminUser == null)
                    {
                        var newAdmin = new AppUser
                        {
                            UserName = adminUserName,
                            Email = "admin@dungeon.com",
                            SecurityStamp = Guid.NewGuid().ToString()
                        };

                        var result = await userManager.CreateAsync(newAdmin, adminPassword);
                        if (result.Succeeded)
                            await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin);
                    }

                }

            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapGet("/", [Authorize] (HttpContext httpContext) =>
            {
                return "Secret";
            })
            .WithName("oke")
            .WithOpenApi();

            app.MapPost("account/register",
                async Task<Results<Ok<object>, BadRequest<string>, ValidationProblem>>
                (UserManager<AppUser> userManager,
                IConfiguration configuration,
                IValidator<AddOrUpdateAppUserModel> validator,
                AddOrUpdateAppUserModel model,
                ITokenService tokenService) =>
                {

                    var validation = await validator.ValidateAsync(model);
                    if (!validation.IsValid)
                        return TypedResults.ValidationProblem(validation.ToDictionary());

                    var existedUser = await userManager.FindByNameAsync(model.UserName);
                    if (existedUser != null)
                        return TypedResults.BadRequest("User name is already taken");

                    var user = new AppUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (!result.Succeeded)
                    {
                        var errors = result.Errors.ToDictionary(
                            e => e.Code,
                            e => new[] { e.Description });

                        return TypedResults.ValidationProblem(errors);
                    }

                    var roleResult = await userManager.AddToRoleAsync(user, AppRoles.User);

                    if (!roleResult.Succeeded)
                    {
                        var errors = roleResult.Errors.ToDictionary(
                            e => e.Code,
                            e => new[] { e.Description });

                        return TypedResults.ValidationProblem(errors);
                    }

                    var token = await tokenService.GenerateTokenAsync(user);
                    // Stuur ook hier direct de profielgegevens mee terug
                    return TypedResults.Ok<object>(new { token, username = user.UserName, role = AppRoles.User });
                })
            .WithName("Register")
            .WithOpenApi();

            app.MapPost("account/login",
                async Task<Results<Ok<object>, UnauthorizedHttpResult, ValidationProblem>> (
                    LoginModel model,
                    IValidator<LoginModel> validator,
                    UserManager<AppUser> userManager,
                    SignInManager<AppUser> signInManager,
                    ITokenService tokenService) =>
                {
                    var validation = await validator.ValidateAsync(model);
                    if (!validation.IsValid)
                        return TypedResults.ValidationProblem(validation.ToDictionary());

                    var user = await userManager.FindByNameAsync(model.UserName);
                    if (user is null)
                        return TypedResults.Unauthorized();

                    var result = await signInManager.CheckPasswordSignInAsync(
                        user, model.Password, lockoutOnFailure: true);

                    if (result.IsLockedOut)
                        return TypedResults.Unauthorized();

                    if (!result.Succeeded)
                        return TypedResults.Unauthorized();

                    // Haal de rollen van de user op uit de database
                    var roles = await userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? AppRoles.User;

                    var token = await tokenService.GenerateTokenAsync(user);

                    // Stuur token, username én role terug zodat de client dit direct in-memory kan opslaan!
                    return TypedResults.Ok<object>(new { token, username = user.UserName, role });
                })
                .WithName("Login")
                .WithOpenApi();

            app.MapGet("api/rooms/{roomId}/keyshare", (string roomId, IConfiguration configuration) =>
            {
                var keyShare = configuration[$"RoomsKeys:{roomId}"];

                if (string.IsNullOrWhiteSpace(keyShare))
                {
                    return Results.NotFound(new
                    {
                        message = $"No keyshare found for room '{roomId}'"
                    });
                }

                return Results.Ok(new
                {
                    roomId,
                    keyShare
                });
            })
            .WithName("GetRoomKeyShare")
            .RequireAuthorization()
            .WithOpenApi();


            app.MapGet("api/auth/me",
            (ClaimsPrincipal user) =>
            {
                var userName = user.FindFirstValue(ClaimTypes.Name);
                var role = user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role");

                return TypedResults.Ok(new { userName, role });
            })
            .WithName("Me")
            .RequireAuthorization()
            .WithOpenApi();


            app.MapGet("api/rooms/keyshare/all",
            (ClaimsPrincipal user, IConfiguration configuration) =>
            {
                var allKeys = configuration.GetSection("RoomsKeys")
                    .GetChildren()
                    .ToDictionary(x => x.Key, x => x.Value);

                if (!allKeys.Any())
                    return Results.NotFound(new { message = "No keyshares found" });

                return Results.Ok(new { allKeys });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithName("GetAllKeyShares")
            .WithOpenApi();


            app.Run();
        }
    }
}
