using Broadcast;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddTransient<IEventBus>(c =>
{
    var eventBus = new EventBus(c.GetService<IEventStore>());
    //eventBus.Subscribe<StartTestEvent>(new TestRunEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    //eventBus.Subscribe<EndTestEvent>(new TestRunEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    //eventBus.Subscribe<ThreadSummaryEvent>(new SummaryEventHandler(c.GetService<IProjectionConnectionBuilder>()));
    //eventBus.Subscribe<TestSummaryEvent>(new SummaryEventHandler(c.GetService<IProjectionConnectionBuilder>()));

    return eventBus;
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
