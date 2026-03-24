using Backend.Persistence.Entities;
using Backend.Repositories;
using Backend.Repository;
using Srbopoly.Services.Messaging;
using Srbopoly.Services.Messaging.ChatHub;
using Backend.Services.GameCode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddCors(options => {
//     options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b =>
        b.SetIsOriginAllowed(_ => true)
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials());
});

var rabbitConnection = new RabbitMqConnection(builder.Configuration);
await rabbitConnection.InitializeAsync();
builder.Services.AddSingleton(rabbitConnection);
builder.Services.AddSingleton<IRabbitMqConnection>(rabbitConnection);
builder.Services.AddScoped<IChatPublisher, RabbitMqChatPublisher>();
builder.Services.AddHostedService<RabbitMqChatConsumer>();

builder.Services.AddSingleton<GameCodeService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<CardRepository>();
builder.Services.AddScoped<LobbyRepository>();
builder.Services.AddScoped<LobbyPlayerRepository>();
builder.Services.AddScoped<BoardRepository>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<PropertyFieldRepository>();

builder.Services.AddSignalR();

builder.Services.AddControllers();

// builder.Services.AddDbContext<SrbopolyContext> (Options => 
// Options.UseSqlServer(builder.Configuration.GetConnectionString("SrbopolyCS")));

builder.Services.AddDbContext<SrbopolyContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("SrbopolyCS")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseCors("AllowAll");

//app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();