﻿var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddApplicationServices();
builder.Services.AddGrpcServices();

builder.Services.Configure<UrlsConfig>(builder.Configuration.GetSection("urls"));

var app = builder.Build();

app.UseServiceDefaults();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers().RequireCors("CorsPolicy");
app.MapReverseProxy();

await app.RunAsync();
