using Business;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCustomServices(builder.Configuration);

//httpclient baðlanmaya çalýþacak.bunun için kullanýlýr
builder.Services.AddHttpClient("payment", cfg =>
{
    var paymentUri = builder.Configuration["BaseUrls:payment"];
    if (!string.IsNullOrEmpty(paymentUri))
    {
        cfg.BaseAddress = new Uri(paymentUri);
    }
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
