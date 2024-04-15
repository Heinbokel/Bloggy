using Bloggy.Exceptions;
using Bloggy.Repositories;
using Bloggy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Adds our Controllers to our container and configures our [ApiController] behavior.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        // Don't automatically validate request bodies, as we will do our own validation.
        options.SuppressModelStateInvalidFilter = true;
    }
);

// Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<BloggyDbContext>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// This tells .NET to use our exception handler defined in the builder.Services.AddExceptionHandler code above. 
// We use (_ => {}) to provide it with the default configurations.
app.UseExceptionHandler(_ => { });

app.Run();
