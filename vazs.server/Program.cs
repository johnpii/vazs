using Firebase.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using vazs.server.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = ConfigurationHelper.expireTimeCookie;
    });
// Добавляем сервис FirebaseClient
builder.Services.AddSingleton<FirebaseClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new FirebaseClient(configuration.GetValue<string>("Firebase_Database_Adress"), new FirebaseOptions
    {
        AuthTokenAsyncFactory = () => Task.FromResult(configuration.GetValue<string>("Firebase_Database_Secret"))
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddCors();

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

app.UseAuthorization();

app.UseCors(builder => builder.AllowAnyOrigin());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
