using Microsoft.EntityFrameworkCore;
using RealEstate.Api.Security;
using RealEstate.Api.Setup;
using RealEstate.Application;
using RealEstate.Infrastructure;
using RealEstate.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppSwagger();        // <— Swagger
builder.Services.AddJwtAuth(builder.Configuration); // <— JWT (y Authorization)
builder.Services.AddDbContext<RealEstateDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("RealEstate")));
var app = builder.Build();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.UseAppSwaggerUI(app.Environment); // <— Swagger UI en dev
app.MapControllers();

app.Run();
