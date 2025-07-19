using ChopperStoreAngularTest.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ChopperStoreAngularTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ChopperStoreContext>(options =>
{
    options.UseSqlServer("Data Source=LEITOPC;Initial Catalog=ChopperStore;Integrated Security=True;Trust Server Certificate=True");

});

builder.Services.AddScoped<IUserService, UserService>();
// Agregar el servicio para verificar el token de Google
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: "AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
