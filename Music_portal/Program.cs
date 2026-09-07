using Music_portal.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddPortalServices(builder.Configuration);
var app = builder.Build();
app.UsePortalPipeline();
app.Run();

public partial class Program { }
