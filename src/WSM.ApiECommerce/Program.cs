using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WSM.ApiECommerce.Context;
using WSM.ApiECommerce.Repositories;

var builder = WebApplication.CreateBuilder(args);

// configura a aplica��o para autenticar os usu�rios usando tokens JWT,
// verificando o emissor, audi�ncia, tempo de vida e chave de assinatura do emissor
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            //define o emissor e a audi�ncia validas para o token JWT obtidos da aplica��o
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            //Define a chave de assinatura usada para assinar e verificar o token JWT.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend App Lanches MAUI", Version = "v1" });

    // Define um esquema securo para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Implementa a autentica��o em todos os endpoints da API
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

//permite injetar a inst�ncia do contexto nos controladores
builder.Services.AddDbContext<AppDbContext>(option =>
                                            option.UseSqlServer(connection));

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

var app = builder.Build();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend App Lanches Maui V1");
});

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.MapControllers();
app.Run();

