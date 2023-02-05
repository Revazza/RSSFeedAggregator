using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//for project's simplicity I won't use dbContextFactory 
//and I'll hard-code sql connection string
builder.Services.AddDbContext<RSSFeedAggregatorDbContext>(options =>
{
    options.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=RSSFeedAggregator");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
