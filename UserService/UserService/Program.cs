using EF_InteractionFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using UserService.Context;
using UserService.Interfaces;
using UserService.Repository;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks().AddCheck<ReadinessHealthCheck>("readiness");

//Named the repo the name I wanted for this (((: 
builder.Services.AddTransient<IUserService, UserServiceProvider>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

builder.Services.AddTransientAsyncRepository<UserContext>();
builder.Services.AddTransient<IRepositoryPatch<UserContext>, RepositoryPatch<UserContext>>();

builder.Services.AddDbContextFactory<UserContext>();


builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter("fixedLogin", config =>
	{
		config.PermitLimit = 6;  
		config.Window = TimeSpan.FromSeconds(30);
		config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		
	});
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.OnRejected = async (context, _) =>
	{
		await context.HttpContext.Response.WriteAsync("too many login attempts, please try again later");
	};
});

builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter("fixedUsercreation", config =>
	{
		config.PermitLimit = 1;
		config.Window = TimeSpan.FromMinutes(60);
		config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

	});
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.OnRejected = async (context, _) =>
	{
		await context.HttpContext.Response.WriteAsync("too many login attempts, please try again later");
	};
});

var app = builder.Build();

app.MapHealthChecks("/healthz/readiness", new HealthCheckOptions
{
    Predicate = (check) => check.Name == "readiness"
});

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<UserContext>();

	await context.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
	app.UseSwagger();
	app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
