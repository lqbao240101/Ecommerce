using Ecommerce.Application.Extensions;
using Ecommerce.Data;
using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
//using Ecommerce.Hubs;

namespace Ecommerce.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("./Application/appsettings.json", false, true);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddOptions();
            //builder.Services.AddSignalR();

            //builder.Services.AddControllers(options =>
            //{
            //    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = false;
            //})
            //.AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //});

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["AuthSettings:Audience"],
                    ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = true; // Bắt phải có số
                options.Password.RequireLowercase = true; // Bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = true; // Bắt ký tự đặc biệt
                options.Password.RequiredLength = 5; // Số ký tự tối thiểu của password
                options.Password.RequireUppercase = true; // Bắt buộc chữ in

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.RequireUniqueEmail = true; // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddDependencies();
            builder.Services.AddMappingProfile();

            builder.Services.AddRazorPages();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "dotnetClaimAuthorization", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            AppDbInitializer.SeedAdminAndRolesAsync(app).Wait();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapRazorPages();
            app.MapGraphQL("/graphql");
            //app.MapHub<ChatHub>("/chatHub");
            app.Run();
        }
    }
}