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

            // 2. Enables serving static files (CSS, JS, Images, HTML) from wwwroot folder
            app.UseStaticFiles();

            app.UseAuthorization();

            // 3. API Controllers mapping (/api/...)
            app.MapControllers();

            // 4. SPA Fallback: Any route not handled by API or static files will serve index.html
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
