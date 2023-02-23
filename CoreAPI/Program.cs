using System;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using DAL;
using Microsoft.Extensions.Configuration;
using BL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                     .AddJwtBearer(options =>
                     {
                         options.RequireHttpsMetadata = false;
                         options.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateIssuer = true,

                             ValidIssuer = AuthorizeOptions.ISSUER,

                             ValidateAudience = true,

                             ValidAudience = AuthorizeOptions.AUDIENCE,

                             ValidateLifetime = true,

                             IssuerSigningKey = AuthorizeOptions.GetSymmetricSecurityKey(),

                             ValidateIssuerSigningKey = true,
                         };
                     });

builder.Services.AddControllers();

builder.Services.AddDbContext<ScheduleHighSchoolDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnectionStringDocker"), b => b.MigrationsAssembly("DAL")));

//builder.Services.AddDbContext<ScheduleHighSchoolDb>(options => options.UseSqlServer("TimetableHighSchoolDbConnectionMSSqlServerDocker"), ServiceLifetime.Transient);

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

CustomServices.Configure(builder.Services);


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var importProgress = new ImportProgress
{
    TotalLessonCount = 0,
    CheckedLessonCount = 0,
    ImportError = false,
    ErrorMessage = null,
    ImportFinished = false,
    InProcess = false
};


builder.Services.AddSingleton(importProgress);

var generateScheduleProgress = new ScheduleGeneratedProgress
{
    Start = false,
    End = false,
    Generation = 0,
    Message = "",
    SpentTime = "",
    SaveWithMistakes = false
};

builder.Services.AddSingleton(generateScheduleProgress);

var app = builder.Build();



// Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ScheduleHighSchoolDb>();
    db.Database.Migrate();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

