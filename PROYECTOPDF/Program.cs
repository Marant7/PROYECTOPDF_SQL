using Microsoft.AspNetCore.Authentication.Cookies;
using NegocioPDF.Repositories;
using Microsoft.Data.SqlClient; // Agregamos esta referencia para SQL Server

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde appsettings.json
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Verificar que la cadena de conexión no sea nula o vacía
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encontró o es nula.");
}

// Verificar la conexión a SQL Server al inicio
try
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
    }
}
catch (Exception ex)
{
    throw new InvalidOperationException($"No se pudo conectar a SQL Server: {ex.Message}");
}

// Registrar servicios - Cambiados a Scoped para mejor manejo de recursos
builder.Services.AddScoped<UsuarioRepository>(provider => new UsuarioRepository(connectionString));
builder.Services.AddScoped<DetalleSuscripcionRepository>(provider => new DetalleSuscripcionRepository(connectionString));
builder.Services.AddScoped<OperacionesPDFRepository>(provider => new OperacionesPDFRepository(connectionString));

// Configurar autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

// Agregar política CORS si es necesaria
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy => policy
            .WithOrigins("http://localhost:8080")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Usar CORS si es necesario
// app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();