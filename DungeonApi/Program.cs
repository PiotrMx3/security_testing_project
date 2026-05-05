
using DungeonApi.Authentication;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DungeonApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddIdentityCore<AppUser>()
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

                    var token = tokenService.GenerateToken(model.UserName);
                    return TypedResults.Ok<object>(new { token });
                })
            .WithName("Register")
            .WithOpenApi();

            app.MapPost("account/login", 
                async Task<Results<Ok<object>, UnauthorizedHttpResult, ValidationProblem>> (
                LoginModel model,
                IValidator<LoginModel> validator,
                UserManager<AppUser> userManager,
                ITokenService tokenService) =>
                {
                    var validation = await validator.ValidateAsync(model);
                    if (!validation.IsValid)
                        return TypedResults.ValidationProblem(validation.ToDictionary());

                    var user = await userManager.FindByNameAsync(model.UserName);
                    if (user is null)
                        return TypedResults.Unauthorized();

                    var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);
                    if (!passwordValid)
                        return TypedResults.Unauthorized();

                    var token = tokenService.GenerateToken(model.UserName);
                    return TypedResults.Ok<object>(new { token });
            })
            .WithName("Login")
            .WithOpenApi();



            app.Run();
        }
    }
}
