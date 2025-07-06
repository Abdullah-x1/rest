using DSAR.Data;
using DSAR.Factories;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.Repositories;
using DSAR.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(option =>
{
    option.Password.RequiredLength = 3;
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireUppercase = false;
    option.Password.RequireNonAlphanumeric = false;
    option.SignIn.RequireConfirmedAccount = false;
    option.SignIn.RequireConfirmedEmail = false;
    option.SignIn.RequireConfirmedPhoneNumber = false;

})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomUserClaimsPrincipalFactory>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddTransient<IRequestActionRepository,RequestActionRepository>();
builder.Services.AddTransient<IRequestRepository, RequestRepository>();
builder.Services.AddTransient<ICaseStudyRepository, CaseStudyRepository>();
//Email


builder.Services.AddScoped<iFormRepository, FormRepository>();


// Add HTTP context accessor (required for the snapshot system)
builder.Services.AddHttpContextAccessor();

// Configure session with optimized settings for form workflow
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.Name = "FormSession";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string[] roleNames = { "Admin", "User", "SectionManager","DepartmentManager", "ITManager", "ApplicationManager", "Analyzer" };

    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123";
    string userId = "412053721";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System Admin",
            UserId = userId,
            CityId = 2, 
        };
        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
    string ManagerEmail = "DepartmentManager@example.com";
    string ManagerPassword = "123";
    string ManagerUserId = "412341";
    var ManagerUser = await userManager.FindByEmailAsync(ManagerEmail);
    if (ManagerUser == null)
    {
        var newManager = new User
        {
            UserName = ManagerEmail,
            Email = ManagerEmail,
            FirstName = "DepartmentManager",
            UserId = ManagerUserId,
            CityId = 2,
        };
        var result = await userManager.CreateAsync(newManager, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newManager, "DepartmentManager");
        }
    }
    string Manager2Email = "SectionManager@example.com";
    string Manager2Password = "123";
    string Manager2UserId = "4123412";
    var Manager2User = await userManager.FindByEmailAsync(Manager2Email);
    if (Manager2User == null)
    {
        var newManager2 = new User
        {
            UserName = Manager2Email,
            Email = Manager2Email,
            FirstName = "SectionManager",
            UserId = Manager2UserId,
            CityId = 2,
        };
        var result = await userManager.CreateAsync(newManager2, Manager2Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newManager2, "SectionManager");
        }
    }
    string ITManagerEmail = "ITManager@example.com";
    string ITManagerPassword = "123";
    string ITManagerUserId = "412342121";
    var ITManagerUser = await userManager.FindByEmailAsync(ITManagerEmail);
    if (ITManagerUser == null)
    {
        var newITManager = new User
        {
            UserName = ITManagerEmail,
            Email = ITManagerEmail,
            FirstName = "ITManager",
            UserId = ITManagerUserId,
            CityId = 2,
        };
        var result = await userManager.CreateAsync(newITManager, ITManagerPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newITManager, "ITManagerManager");
        }
    }
    string ApplicationEmail = "ApplicationManager@example.com";
    string ApplicationPassword = "123";
    string ApplicationUserId = "41231412";
    var ApplicationUser = await userManager.FindByEmailAsync(ApplicationEmail);
    if (ApplicationUser == null)
    {
        var newApplication = new User
        {
            UserName = ApplicationEmail,
            Email = ApplicationEmail,
            FirstName = "ApplicationManager",
            UserId = ApplicationUserId,
            CityId = 2,
        };
        var result = await userManager.CreateAsync(newApplication, ApplicationPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newApplication, "ApplicationManager");

        }
    }
    string AnalyzerEmail = "Analyzer@example.com";
    string AnalyzerPassword = "123";
    string AnalyzerUserId = "41294234";
    var AnalyzerUser = await userManager.FindByEmailAsync(AnalyzerEmail);
    if (AnalyzerUser == null)
    {
        var newAnalyzer = new User
        {
            UserName = AnalyzerEmail,
            Email = AnalyzerEmail,
            FirstName = "Analyzer",
            UserId = AnalyzerUserId,
            CityId = 2,
        };
        var result = await userManager.CreateAsync(newAnalyzer, AnalyzerPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAnalyzer, "Analyzer");
           

        }
    }
    string Analyzer2Email = "Analyzer2@example.com";
    string Analyzer2Password = "123";
    string Analyzer2UserId = "412934234";
    var Analyzer2User = await userManager.FindByEmailAsync(Analyzer2Email);
    if (Analyzer2User == null)
    {
        var newAnalyzer2 = new User
        {
            UserName = Analyzer2Email,
            Email = Analyzer2Email,
            FirstName = "Analyzer2",
            UserId = Analyzer2UserId,
            CityId = 2,
        };
        var result = await userManager.CreateAsync(newAnalyzer2, Analyzer2Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAnalyzer2, "Analyzer");


        }
    }
}
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
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
