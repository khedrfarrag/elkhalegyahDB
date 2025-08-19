using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Services.Auth;
using Alkhaligya.DAL.Data.DbHelper;
using Alkhaligya.DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Alkhaligya.BLL.Services.OrderServices;
using Alkhaligya.BLL.AutoMapper;
using Alkhaligya.DAL.UnitOfWork;
using Alkhaligya.BLL.Services.Cart;
using Alkhaligya.BLL.Services.Contact;
using Alkhaligya.BLL.Services.ProductServices;
using Alkhaligya.BLL.Services.CategoryServices;
using Alkhaligya.BLL.Services.ProductFeedbackServices;
using Alkhaligya.BLL.Services.SiteFeedbackServices;
using Alkhaligya.BLL.Services.Cashing;
using Microsoft.OpenApi.Models;
using Alkhaligya.DAL.Models.PayMob;
using Alkhaligya.BLL.Services.PayMob;
using Hangfire;
using System.Text.Json.Serialization;
using Alkhaligya.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



//QuicContext
builder.Services.AddDbContext<AlkhligyaContext>(options =>
{
    // options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));

    //For Docker
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

////For Docker 
builder.WebHost.ConfigureKestrel(options =>
{
    // Listen on port 80 (HTTP) inside container
    options.ListenAnyIP(80);

    // Listen on port 443 (HTTPS) inside container
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps("/https/devcert.pfx", "SSSherif123!!!");
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
// Specify URLs explicitly
//builder.WebHost.UseUrls("https://localhost:7243;http://localhost:5011");


// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//Paymob
builder.Services.Configure<PaymobSettings>(builder.Configuration.GetSection("PaymobSettings"));

//Hangfire
builder.Services.AddHangfire(config =>
    // config.UseSqlServerStorage(builder.Configuration.GetConnectionString("cs")));
    //For Docker 
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();





builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });




//Identity
builder.Services.AddIdentity<ApplicationUser, CustomRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 5;
    options.SignIn.RequireConfirmedEmail = true;
    //options.User.AllowedUserNameCharacters =
    //  "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ءآأإئابةتثجحخدذرزسشصضطظعغفقكلمنهوي ";
})
   .AddEntityFrameworkStores<AlkhligyaContext>()
   .AddDefaultTokenProviders();



// Add JWT Authentication
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(Option =>
{
    #region SecurityKey
    var SecretKeyString = builder.Configuration.GetSection("SecretKey").Value;
    var SecretKeyByte = Encoding.ASCII.GetBytes(SecretKeyString);
    SecurityKey securityKey = new SymmetricSecurityKey(SecretKeyByte);
    #endregion
    Option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey,

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();




//builder.Services.AddHttpContextAccessor();

// Services Registeration
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartShopService, CartShopService>();
builder.Services.AddScoped<IContactMessageService, ContactMessageService>();



builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductFeedbackService, ProductFeedbackService>();
builder.Services.AddScoped<ISiteFeedbackService, SiteFeedbackService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();




//AutoMapper
builder.Services.AddAutoMapper(typeof(MyProfile));

//UnitOfwork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // ����� JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token",
    });

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
            new string[] {}
        }
    });


});

var app = builder.Build();

// Call the SeedRoles method
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<CustomRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await SeedRolesDtocs.SeedRoles(roleManager);
    await SeedRolesDtocs.SeedSuperAdminAsync(userManager, roleManager);
}

//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<CustomRole>>();
//    await SeedRolesDtocs.SeedRoles(roleManager);
//}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");    // dashboard ���� �� ���� 

    //app.MapOpenApi();
}


app.UseCors("AllowAll");

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication(); // Add Authentication before Authorization
app.UseAuthorization();
app.MapControllers();

app.Run();