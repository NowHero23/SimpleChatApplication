using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data.Domain.Repositories.Abstract;
using DataAccessLayer.Data.Domain.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.Hubs;
using SimpleChatApplication.Data.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContextPool<AppDbContext>(
    options => options.UseNpgsql(
        builder.Configuration
        .GetConnectionString("Postgres_db")
    ));

builder.Services.AddStackExchangeRedisCache(option =>
{
    var connection = builder.Configuration.GetConnectionString("Redis");
    option.Configuration = connection;
});
builder.Services.AddSignalR();


builder.Services.AddScoped<IChatRepository, EFChatRepository>();
builder.Services.AddScoped<IUserConnectionRepository, EFUserConnectionRepository>();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserConnectionService, UserConnectionService>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    /*using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (dbContext.Database.CanConnect())
        {
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }
    }*/

    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("chat-hub"); ;

app.Run();
