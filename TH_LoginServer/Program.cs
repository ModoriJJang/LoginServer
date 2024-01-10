var builder = WebApplication.CreateBuilder( args );

// Add services to the container.
builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>( c =>
{
	var opt = StackExchange.Redis.ConfigurationOptions.Parse( builder.Configuration.GetConnectionString( "RedisConnection" ) );
	return StackExchange.Redis.ConnectionMultiplexer.Connect( opt );
} );

builder.Services.AddSingleton( serviceProvider => ( new RedisDB(serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>()) ) );

builder.Services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
builder.Services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
