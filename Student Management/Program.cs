using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Student_Management;
using Student_Management.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<Student_Management.Data.ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("StudentDb"));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Student Management API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    options.OperationFilter<AuthorizeOperationFilter>();
});

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API V1");
    });
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

internal sealed class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
            (context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ?? false);

        var hasAllowAnonymous =
            context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (!hasAuthorize || hasAllowAnonymous)
            return;

        var schemeReference = new OpenApiSecuritySchemeReference("Bearer");

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [schemeReference] = new List<string>()
            }
        };
    }
}
