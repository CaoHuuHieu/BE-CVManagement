using AutoMapper;
using CVManagement.Repositories;
using CVManagement.Repositories.Implements;
using CVManagement.Repositories.Interfaces;
using CVManagement.Services.Implements;
using CVManagement.Services.Interfaces;
using CVManagement.Web.Helper;
using CVManagement.Web.Hubs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserCVService, UserCVService>();
builder.Services.AddTransient<ICVService, CvWithFirebaseService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IStorageService, FirebaseStorageService>();
builder.Services.AddTransient<ICVRepository, CVRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<IUserCVRepository, UserCVRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
var autoMapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = autoMapper.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddDbContext<DataContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"), b => b.MigrationsAssembly("CVManagement.Repositories"));
});

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration["HangFire:ConnectionString"]));
builder.Services.AddHangfireServer();
//jwt
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                }
            },
            new List < string > ()
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")  // Replace with your own domain.
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});*/
builder.Services.AddCors(options => {
    options.AddPolicy("CORSPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditAccount", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim(claim => claim.Type == "Id" && claim.Value == context.Resource.ToString())));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHangfireDashboard();
app.UseHangfireServer();
app.UseCors("CORSPolicy");
app.MapHub<NotificationHub>("/notificationhub");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
