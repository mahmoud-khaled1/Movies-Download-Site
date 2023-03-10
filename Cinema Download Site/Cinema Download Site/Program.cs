using Cinema_Download_Site.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register DbContext
builder.Services.AddDbContext<ApplicationDb>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
 
//register Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDb>().AddDefaultTokenProviders();

// to enable front-end Angular access the api 
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// To use cookies
builder.Services.Configure<CookiePolicyOptions>(options => 
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;

});
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme= CookieAuthenticationDefaults.AuthenticationScheme; ;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
})
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly= true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        //options.LoginPath= "/Account/Login";
        options.LogoutPath= "/Account/Logout";
        //options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

//// if i want custom validation of user table such as if i want accept password RequireNonAlphanumeric etc... 
//builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
//    {
//        option.Password.RequireNonAlphanumeric = true;
//        option.Password.RequireDigit = false;
//        option.Password.RequiredLength = 5;
//        option.Password.RequiredUniqueChars = 1;
//        option.SignIn.RequireConfirmedEmail = true; // checks if the email is confirmed before successfully signing in the user
//        option.Lockout.MaxFailedAccessAttempts = 5;
//        option.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(5);
//    }).AddEntityFrameworkStores<ApplicationDb>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.UseAuthentication();   
app.MapControllers();
app.UseCookiePolicy();
app.UseAuthorization();

app.Run();
