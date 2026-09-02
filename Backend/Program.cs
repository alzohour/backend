namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Enable CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            // 1. Enables default file mapping (e.g. '/' automatically maps to '/index.html' in wwwroot)
            app.UseDefaultFiles();

            // 2. Enables serving static files (CSS, JS, Images, HTML) from wwwroot folder with optimal caching
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var path = ctx.File.PhysicalPath ?? "";
                    // Cache Next.js hashed assets, WebP images, fonts, and PNGs for 1 year immutable
                    if (path.Contains("_next") || path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
                    }
                    else
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400");
                    }
                }
            });

            app.UseAuthorization();

            // 3. API Controllers mapping (/api/...)
            app.MapControllers();

            // 4. SPA Fallback: Any route not handled by API or static files will serve index.html
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
