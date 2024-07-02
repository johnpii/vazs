using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using vazs.server.Helpers;
using vazs.server.Profiles;
using vazs.server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = ConfigurationHelper.expireTimeCookie;
    });

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(@"firebase_auth.json")
});

builder.Services.AddSingleton<FirebaseClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new FirebaseClient(configuration.GetValue<string>("Firebase_Database_Adress"), new FirebaseOptions
    {
        AuthTokenAsyncFactory = () => Task.FromResult(configuration.GetValue<string>("Firebase_Database_Secret"))
    });
});

builder.Services.AddSingleton<FirebaseStorage>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new FirebaseStorage(configuration.GetValue<string>("Storage_Path"));
});

builder.Services.AddSingleton<FirebaseAuthClient>(provider =>
{
    FirebaseAuthClient client;
    var configuration = provider.GetRequiredService<IConfiguration>();
    FirebaseAuthConfig config = new FirebaseAuthConfig
    {
        ApiKey = configuration.GetValue<string>("Api_Key"),
        AuthDomain = configuration.GetValue<string>("AuthDomain"),
        Providers = new FirebaseAuthProvider[]
                {
                    // Провайдеры
                    new GoogleProvider().AddScopes("email"),
                    new EmailProvider()
                     // ...
                }
    };
    client = new FirebaseAuthClient(config);
    return client;
});

builder.Services.AddAutoMapper(typeof(TSIndexMappingProfile), typeof(AdminDepartmentIndexMappingProfile), typeof(HomeDepartmentIndexMappingProfile));

builder.Services.AddScoped<EmailService>();

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
