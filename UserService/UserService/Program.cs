using EF_InteractionFrameworkCore;
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

//Named the repo the name I wanted for this (((: 
builder.Services.AddTransient<IUserService, UserServiceProvider>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

builder.Services.AddTransientAsyncRepository<UserContext>();
builder.Services.AddTransient<IRepositoryPatch<UserContext>, RepositoryPatch<UserContext>>();

builder.Services.AddDbContextFactory<UserContext>();

var app = builder.Build();

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

app.MapControllers();

app.Run();
