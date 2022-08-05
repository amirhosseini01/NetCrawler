using AutoMapper;
using GatheredData.Api.AutoMapperProfiles;
using GatheredData.Api.Models;
using GatheredData.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ProductStoreDatabaseSettings>(
    builder.Configuration.GetSection("ProductStoreDatabaseSettings"));

builder.Services.AddSingleton<ProductsService>();

builder.Services.AddControllers();

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

//todo: move this line to above <if> statement
app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
