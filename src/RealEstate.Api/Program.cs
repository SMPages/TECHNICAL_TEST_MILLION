using RealEstate.Api.Security;
using RealEstate.Api.Setup;
using RealEstate.Application;
using RealEstate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddAppSwagger();
builder.Services.AddJwtAuth(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAppSwaggerUI(app.Environment);
app.MapControllers();

app.Run();

namespace RealEstate.Api { public partial class Program { } }
