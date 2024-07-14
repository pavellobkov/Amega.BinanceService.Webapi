using Amega.BinanceService.Webapi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;



services.AddControllers();
services
    .AddSwaggerGen(options =>
    {
        //options.SwaggerDoc("v1", builder.Environment.GetOpenApiInfo<Program>());
    })
    .AddEndpointsApiExplorer();

//services.AddSingleton<BinanceDataProvider>();

// Build and config app
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseWebSockets();
app.UseWebSocketHandler();

app.UseRouting();
app.UseEndpoints(x => x.MapControllers());

var fLogger = app.Services.GetRequiredService<ILogger<Program>>();

app.Run();









/*

namespace Amega.BinanceService.Webapi
{
   public class Program
   {
       public static void Main(string[] args)
       {
           CreateHostBuilder(args).Build().Run();
       }

       public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
                   webBuilder.UseUrls("https://localhost:7148", "http://localhost:5134");
               });
   }
}
*/

/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
*/