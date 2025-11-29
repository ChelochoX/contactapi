using GraciaTech.ContactApi.Models;
using GraciaTech.ContactApi.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------
// LOGGING
// -------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// -------------------------------------------------
// CONFIGURACIÓN SMTP
// -------------------------------------------------
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// -------------------------------------------------
// CORS (habilitar requests desde tu web)
// -------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins(
                "https://graciatech.com.py",
                "http://localhost:5500",    // LiveServer, VS Code, etc.
                "http://127.0.0.1:5500",
                "http://localhost:3000",
                "http://localhost:5103",    // Tu API local
                "https://localhost:7177"    // HTTPS API local
            )
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// -------------------------------------------------
// SWAGGER
// -------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------------------------------
// BANNER DE INICIO PROFESIONAL
// -------------------------------------------------
var logger = app.Services.GetRequiredService<ILogger<Program>>();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("===============================================");
Console.WriteLine(" 🚀 GraciaTech Contact API iniciando...");
Console.WriteLine($" 🌎 Entorno: {app.Environment.EnvironmentName}");
Console.WriteLine("===============================================");
Console.ResetColor();

logger.LogInformation("✔ API iniciada correctamente.");

// -------------------------------------------------
// MANEJO GLOBAL DE ERRORES
// -------------------------------------------------
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error no manejado.");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { ok = false, error = "Error interno del servidor" });
    }
});

// -------------------------------------------------
// SWAGGER (solo para Development)
// -------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// -------------------------------------------------
// CORS
// -------------------------------------------------
app.UseCors();

// -------------------------------------------------
// STATUS HTML
// -------------------------------------------------
app.MapGet("/status", () =>
{
    var html = $@"
    <html>
        <head>
            <title>GraciaTech Contact API</title>
            <style>
                body {{ font-family: Arial; background:#eee; text-align:center; padding:40px; }}
                .card {{ background:white; padding:20px 30px; border-radius:10px; display:inline-block; }}
                h1 {{ color:#0077cc; }}
                .ok {{ color:green; font-weight:bold; }}
            </style>
        </head>
        <body>
            <div class='card'>
                <h1>🚀 GraciaTech Contact API</h1>
                <p class='ok'>Estado: OK</p>
                <p>Última actualización: {DateTime.Now}</p>
            </div>
        </body>
    </html>";

    return Results.Content(html, "text/html");
});

// -------------------------------------------------
// HEALTH JSON
// -------------------------------------------------
app.MapGet("/health", () =>
{
    return new
    {
        status = "ok",
        timestamp = DateTime.UtcNow,
        service = "GraciaTech Contact API"
    };
});

// -------------------------------------------------
// ENDPOINT PRINCIPAL: /api/contact
// -------------------------------------------------
app.MapPost("/api/contact", async (ContactRequest req, IEmailSender sender) =>
{
    if (string.IsNullOrWhiteSpace(req.Nombre) ||
        string.IsNullOrWhiteSpace(req.Email) ||
        string.IsNullOrWhiteSpace(req.Mensaje))
    {
        return Results.BadRequest(new { ok = false, error = "Datos incompletos." });
    }

    try
    {
        await sender.SendContactEmailAsync(req);
        return Results.Ok(new { ok = true, message = "Mensaje enviado correctamente." });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error al enviar email.");
        return Results.StatusCode(500);
    }
});

// -------------------------------------------------
// INICIAR SERVIDOR
// -------------------------------------------------
app.Run();
