using Blazored.LocalStorage;
using HoangThach.AccountShared.Data;
using HoangThach.AccountShared.Services;
using HT.PackingMachine.Components;
using HT.PackingMachine.Data;
using HT.PackingMachine.Data.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddDbContext<XhsContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("XhsDbContext") ?? ""));
builder.Services.AddScoped<XhsSpServices>();

var sp = builder.Services.BuildServiceProvider();
var scope = sp.CreateScope();
var authDB = scope.ServiceProvider.GetRequiredService<XhsContext>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#region SSO
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, HoangThach.AccountShared.Services.AuthenticationService>();
builder.Services.AddScoped<HoangThach.AccountShared.Services.AuthenticationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient();
#endregion

builder.Services.AddAuthorization();
builder.Services.AddScoped<XhsSpServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
