using Microsoft.EntityFrameworkCore;
using AWC.DigitalCommerce.TicketsControllerAPI.Classes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TicketsControllerAPI")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
