var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(GenericRepository<>));

var app = builder.Build();

app.MapControllers();
app.Run();
