using Microsoft.OpenApi.Models;
using NLog.Web;
using NLog;

var builder = WebApplication.CreateBuilder(args);


// Configure NLog
builder.Host.UseNLog();

// Configure Kestrel to listen on port 5000
builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5000);
});

// Register services
builder.Services.AddControllers();
builder.Services.AddSingleton<SwiftParser>();

// Register DatabaseService with connection string
builder.Services.AddSingleton<DatabaseService>(provider =>
	new DatabaseService("Data Source=SwiftMessages.db"));

// Add Swagger generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API", Version = "v1" });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
		c.RoutePrefix = string.Empty;
	});
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

// Ensure NLog shutdown on application exit
LogManager.Shutdown();