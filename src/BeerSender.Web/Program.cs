using BeerSender.Domain;
using BeerSender.EventStore;
using BeerSender.Web.EventPublishing;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddSignalR();

builder.Services.RegisterDomain();
builder.Services.RegisterEventStore();
builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddMarten(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("MartenDB");
    opt.Connection(connectionString!);
}).UseLightweightSessions();
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
