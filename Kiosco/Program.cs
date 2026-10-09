using Kiosco.Components;
using Kiosco.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// NUEVO: cadena de conexión (en desarrollo viene de User Secrets)
var cadenaConexion = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexión 'Default'. Configurala en User Secrets (desarrollo) o en App Settings (producción).");

// NUEVO: fábrica de contextos de EF Core
builder.Services.AddDbContextFactory<KioscoDbContext>(opciones =>
    opciones.UseSqlServer(cadenaConexion));

var app = builder.Build();

// NUEVO: aplicar las migraciones pendientes al iniciar
var fabrica = app.Services.GetRequiredService<IDbContextFactory<KioscoDbContext>>();
await using (var contexto = await fabrica.CreateDbContextAsync())
{
    await contexto.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();