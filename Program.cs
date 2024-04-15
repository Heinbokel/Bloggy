using Bloggy.Exceptions;
using Bloggy.Repositories;
using Bloggy.Services;

var builder = WebApplication.CreateBuilder(args);

// Adds our Controllers to our container and configures our [ApiController] behavior.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        // Don't automatically validate request bodies, as we will do our own validation.
        options.SuppressModelStateInvalidFilter = true;
    }
);

// Register our classes that we want to provide via constructor dependency injection.
builder.Services.AddScoped<BloggyDbContext>();
builder.Services.AddScoped<UserService>();

// Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
