using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using WebApi_1;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5000); 
});


builder.Host.UseNLog(); 

builder.Services.AddControllers();
builder.Services.AddSingleton(new DatabaseService("Data Source=SwiftMessages.db"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API", Version = "v1" });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
		c.RoutePrefix = string.Empty; 
	});

	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();


app.MapControllers();


app.Run();

LogManager.Shutdown();
