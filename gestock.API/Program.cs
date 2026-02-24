using gestock.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// On récupère la chaine de connexion du fichier json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// On injecte le DbContext avec SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// On ajoute la gestion des cycles JSON pour les relations (Catégorie <-> Produit)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// ------------------------

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Si tu veux Swagger UI classique (la page bleue), ajoute souvent ça aussi :
    // app.UseSwagger(); 
    // app.UseSwaggerUI(); 
    // Mais si tu utilises "MapOpenApi", c'est peut-être différent selon ta version .NET 9
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();