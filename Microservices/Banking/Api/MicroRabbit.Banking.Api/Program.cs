using MicroRabbit.Banking.Api;
using MicroRabbit.Banking.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRegisterServices();
builder.Services.AddControllers();
builder.Services.AddDbContext<BankingDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("BankingDbConnection"));
});

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Microservice v1");
        }
    );
}

app.MapControllers();
app.UseHttpsRedirection(); 
app.Run();
