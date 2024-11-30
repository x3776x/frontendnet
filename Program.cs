using frontendnet.Middlewares;
using frontendnet.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder (args);

// Agregar servicios
builder.Services.AddControllersWithViews ();

// Soporte a consultas API
var UrlWebAPI = builder.Configuration["UrlWebAPI"] ?? throw new InvalidOperationException ("La URL de la API no está configurada.");
builder.Services.AddHttpContextAccessor ();
builder.Services.AddTransient<EnviaBearerDelegatingHandler> ();
builder.Services.AddTransient<RefrescaTokenDelegatingHandler> ();

// Configuración de HttpClient para servicios API
builder.Services.AddHttpClient<AuthClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
});
builder.Services.AddHttpClient<CategoriasClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler> ()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler> ();

builder.Services.AddHttpClient<UsuariosClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler> ()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler> ();

builder.Services.AddHttpClient<RolesClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler> ()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler> ();

builder.Services.AddHttpClient<ProductosClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler> ()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler> ();

builder.Services.AddHttpClient<PerfilClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler> ()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler> ();

builder.Services.AddHttpClient<ArchivosClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler> ()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler> ();

builder.Services.AddHttpClient<BitacoraClientService> (httpClient => {
    httpClient.BaseAddress = new Uri (UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler> ()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler> ();

// Soporte para Cookie Authentication
builder.Services.AddAuthentication (CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie (options => {
        options.Cookie.Name = ".frontendnet";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.LoginPath = "/Auth";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes (20);
    });

var app = builder.Build ();

// Agrega middleware para manejo de errores
app.UseExceptionHandler ("/Home/Error");

app.UseStaticFiles ();
app.UseRouting ();

app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllerRoute ("default", "{controller=Home}/{action=Index}/{id?}");

app.Run ();
