using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(kso =>
{
    kso.ListenAnyIP(23483, l => l.UseMqtt());
    kso.ListenAnyIP(5000);
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddWindowsService();
builder.Services.AddHostedMqttServer(ob =>
{
    ob.WithDefaultEndpoint();
}).AddMqttConnectionHandler().AddConnections();

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

app.UseEndpoints(ep =>
{
    ep.MapConnectionHandler<MqttConnectionHandler>("/mqtt", o =>
    {
        o.WebSockets.SubProtocolSelector = l => l.FirstOrDefault() ?? string.Empty;
    });
});

app.UseMqttServer(s =>
{
    s.StartedAsync += (e) =>
    {
        Console.WriteLine("Server Started");
        return Task.CompletedTask;
    };
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
