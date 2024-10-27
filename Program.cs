using Fair_Recruitment_Web_Result.Auth;
using Fair_Recruitment_Web_Result.Data;
using Fair_Recruitment_Web_Result.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = builder.Configuration;
/*builder.Services.AddHostedService<Worker>();*/

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/ExamListPage");
    options.Conventions.AuthorizePage("/ExamResultPage");
    options.Conventions.AllowAnonymousToPage("/LoginPage");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWindowsService();
builder.Services.AddRazorPages(); //add  razor pages
builder.Services.AddScoped<DbHandler>();
builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fair Recruitment", Version = "v1" });
});

#region DB Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
    });
}, ServiceLifetime.Scoped);

// Configure SQL Server logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.None);
});
#endregion

#region Authentication and Aurthorization
builder.Services.AddScoped<CustomAuthentication>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "JWT_OR_COOKIE";
    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Auth:Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Auth:Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AshikInovaceTechnologiesLimited2023")),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = false,
        ClockSkew = TimeSpan.Zero
    };
})
.AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string authorization = context.Request.Headers[HeaderNames.Authorization];
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            return JwtBearerDefaults.AuthenticationScheme;

        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
});

builder.Services.AddAuthorization();
#endregion

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fair Recruitment");

    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseRouting();
app.MapRazorPages();
app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}"
//);


app.Lifetime.ApplicationStarted.Register(() =>
{
    var kestrelUrl = builder.Configuration["Kestrel:Endpoints:Http:Url"];
    if (!string.IsNullOrEmpty(kestrelUrl))
    {
        var swaggerURL = $"{kestrelUrl}/swagger";
        LogHandler.WriteLog(swaggerURL);
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = swaggerURL,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            LogHandler.WriteErrorLog(ex);
        }
    }
});

await app.RunAsync();
