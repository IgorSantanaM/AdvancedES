using BeerSender.Domain;
using BeerSender.Domain.Boxes;
using BeerSender.Web.EventPublishing;
using BeerSender.Web.Integrations;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Linq.Includes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();

builder.Services.AddSignalR();

builder.Services.RegisterDomain();

builder.Services.AddMarten(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("MartenDB");
    opt.Connection(connectionString!);
    opt.ApplyDomainConfig();
    opt.AddProjections();
})
.AddSubscriptionWithServices<EventHubSubscription>(ServiceLifetime.Singleton, opt =>
{
    opt.FilterIncomingEventsOnStreamType(typeof(Box));
    opt.Options.BatchSize = 10;
})
.AddSubscriptionWithServices<ShipmentNotifier>(ServiceLifetime.Singleton, opt =>
{
    opt.IncludeType<BoxSent>(); 
    opt.Options.BatchSize = 10;
})
.AddAsyncDaemon(DaemonMode.Solo);

builder.Services.AddSwaggerGen();
 
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BeerSender API V1")
    );
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapHub<EventHub>("event-hub");

app.Run();
