using Music.API.Extensions;
using Music.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseApplicationPipeline();
UploadsInitializer.Initialize(app);
AdminSeeder.Seed(app);
await SongSeeder.SeedAsync(app);
app.Run();
