using FinanceControl.Database;
using FinanceControl.Extensions;
using FinanceControl.Mediatr.PaymentRequests;
using FinanceControl.Services;
using FinanceControl.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureAuthorization(builder.Configuration);
builder.Services.AddAuthorization();


builder.Services.AddOpenApi();
builder.Services.ConfigureSwagger();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddHttpClient<CreatePaymentByCheckRequest.CreateByCheckRequestHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("");
    });
});
builder.Services.AddHostedService<DailyUpdateService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();     
    app.UseSwaggerUI();
    app.UseCors("AllowAll");

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();