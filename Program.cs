// Matthew Bowes
// St10444262
// Group 3

// References: 
//  https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor
//  https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview
//  https://www.youtube.com/watch?v=fmvcAzHpsk8
//  https://getbootstrap.com
//  https://learn.microsoft.com/en-us/aspnet/core/client-side/bootstrap
//  https://tailwindcss.com/docs/guides/aspnet-core

using ST10444262_CLDV6212_POE.Services;
using System.Collections;


namespace ST10444262_CLDV6212_POE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<TableStorageService>();
            builder.Services.AddSingleton<QueueStorageService>();
            builder.Services.AddSingleton<FileShareService>();
            builder.Services.AddSingleton<BlobStorageService>();


            var app = builder.Build();

            // Ensure Azure Tables exist
            var tableService = app.Services.GetRequiredService<TableStorageService>();
            tableService.InitializeAsync().GetAwaiter().GetResult();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
//---------------------END OF FILE------------------------------------------------------------------//