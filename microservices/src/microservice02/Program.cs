using Service02;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment}.json", optional: true, reloadOnChange: true);

#region Redis config

var appServicesSettings = new AppServicesSettings();
builder.Configuration.GetSection("AppServicesSettings").Bind(appServicesSettings);
builder.Services.AddSingleton(appServicesSettings);

builder.Services.AddMemoryCache();


var redisOptions = new ConfigurationOptions
{
    EndPoints = { { appServicesSettings.RedisHost, appServicesSettings.RedisPort } },
    AbortOnConnectFail = false,
    DefaultDatabase = 0,
    ConnectTimeout = 15000,
    SyncTimeout = 15000


};

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = appServicesSettings.Redis;
    options.InstanceName = "SampleInstance";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));

#endregion

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
