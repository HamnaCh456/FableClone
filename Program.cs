using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Data;
using Supabase;  
using MyMvcAuthProject.Repositories;  
using DotNetEnv;
using Microsoft.AspNetCore.Http;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); 
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }
        return System.Threading.Tasks.Task.CompletedTask;
    };
    
    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }
        return System.Threading.Tasks.Task.CompletedTask;
    };
});

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.Identity.IsAuthenticated &&
            context.User.Identity.Name == "admin@gmail.com"));
});

// Add SignalR
builder.Services.AddSignalR();

// Register Supabase Client as a singleton service
var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
var options = new SupabaseOptions
{
    AutoConnectRealtime = true
};
var supabaseClient = new Supabase.Client(url, key, options);
await supabaseClient.InitializeAsync();
builder.Services.AddSingleton(supabaseClient);

// Register Repositories
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<AuthorRepository>();
builder.Services.AddScoped<PostRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<MyMvcAuthProject.Hubs.NotificationHub>("/notificationHub");

app.Run();
