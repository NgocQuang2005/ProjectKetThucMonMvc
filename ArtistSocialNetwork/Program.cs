using ArtistSocialNetwork.Models;
using Business;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Repository;

namespace ArtistSocialNetwork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình xác thực với cookie cho Web
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Forbidden/";
                    options.LoginPath = "/Login/Index"; // Đường dẫn đăng nhập dành cho phần Web
                    options.ReturnUrlParameter = "returnUrl";
                })
                // Cấu hình xác thực với cookie cho Admin
                .AddCookie("Admin", options =>
                {
                    options.LoginPath = new PathString("/Admin/Login/Index");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                });

            // Cấu hình Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session hết hạn
                options.Cookie.HttpOnly = true; // Chỉ truy cập qua HTTP, bảo mật hơn
                options.Cookie.IsEssential = true; // Cần thiết để session hoạt động
            });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Thêm các DAO và repository vào Dependency Injection (DI)
            builder.Services.AddScoped<ApplicationDbContext>();
            builder.Services.AddScoped<AccountDAO>();
            builder.Services.AddScoped<AccountDetailDAO>();
            builder.Services.AddScoped<RoleDAO>();
            builder.Services.AddScoped<ArtworkDAO>();
            builder.Services.AddScoped<ProjectDAO>();
            builder.Services.AddScoped<ProjectParticipantDAO>();
            builder.Services.AddScoped<EventDAO>();
            builder.Services.AddScoped<EventParticipantsDAO>();
            builder.Services.AddScoped<CommentDAO>();
            builder.Services.AddScoped<TypeOfArtworkDAO>();
            builder.Services.AddScoped<ReactionDAO>();
            builder.Services.AddScoped<FollowDAO>();
            builder.Services.AddScoped<DocumentInfoDAO>();

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IAccountDetailRepository, AccountDetailRepository>();
            builder.Services.AddScoped<IArtworkRepository, ArtworkRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IProjectParticipantRepository, ProjectParticipantRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<ITypeOfArtworkRepository, TypeOfArtworkRepository>();
            builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
            builder.Services.AddScoped<IDocumentInfoRepository, DocumentInfoRepository>();
            builder.Services.AddScoped<IFollowRepository, FollowRepository>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // Limit to 100 MB
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload")),
                RequestPath = "/Upload",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
                    ctx.Context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000");
                }
            });

            app.UseRouting();

            // Sử dụng Session
            app.UseSession();

            // Sử dụng Authentication
            app.UseAuthentication();
            app.UseAuthorization();

            // Định nghĩa route cho Admin và Web
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Login}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
