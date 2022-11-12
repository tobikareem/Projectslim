using Slim.Pages.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddCustomServicesExtension(builder);
builder.Services.AddCustomConfiguration(builder);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
//app.MapHealthChecks("/Health"); 

app.Run();
