using PawsitiveHaven.Web.Components;
using PawsitiveHaven.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HTTP client for API calls
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5052/");
});

// Register services as singletons to maintain state across the session
builder.Services.AddSingleton<AuthStateService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<FaqService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<AdminService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
