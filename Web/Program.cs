using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Service;
using Web.Service.IService;
using Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
//builder.Services.AddHttpClient<ICouponService, CouponService>();
/*builder.Services.AddHttpClient("MangoApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponApi"]);
});*/ // maybe equivalent


StaticDetails.CouponApiBase = builder.Configuration["ServiceUrls:CouponApi"];
StaticDetails.AuthApiBase = builder.Configuration["ServiceUrls:AuthApi"];

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/accessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
