using System.Reflection;
using System.Text;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NuGet.Packaging;
using StudyPortal.API.Configs;
using StudyPortal.API.Models;
using StudyPortal.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<StudyPortalDatabaseSettings>(
    builder.Configuration.GetSection( nameof (StudyPortalDatabaseSettings)) );

builder.Services.AddTransient<ISubjectsService<Course>, CourseService>();
builder.Services.AddTransient<ISubjectsService<Article>, ArticleService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc( "v1", new OpenApiInfo
    {
        Title = "Study Portal API",
        Description = "A sample ASP.NET Api that allows to manage a study portal",
        Contact = new OpenApiContact
        {
            Name = "Demba Diop",
            Email = "djoob20@gmail.com",
            Url = new Uri("https://www.twitter.com/djoob20")
        },

        Version = "v1"
    });

    //Include xml documentation.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine( AppContext.BaseDirectory, xmlFile );
    c.IncludeXmlComments( xmlPath );

    //c.CustomOperationIds( apiDescription =>
    //{
    //    return apiDescription.TryGetMethodInfo( out MethodInfo methodInfo ) ? methodInfo.Name  : null;
    //} );

});


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(x =>
{
    x.Cookie.Name = "token";

}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Google:google-secret"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["X-Access-Token"];
            return Task.CompletedTask;
        }
    };

});

//Angular CORS
builder.Services.AddCors( options => options.AddPolicy( name: "FrontendUI", policy =>
{
    policy.WithOrigins( "http://localhost:4200" ).AllowAnyMethod().AllowAnyHeader();
}
) );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI( c => {
        c.SwaggerEndpoint( "/swagger/v1/swagger.json", "StudyPortal v1" );
        c.DisplayOperationId();
    });
}
else
{
    app.UseExceptionHandler( "/error" );
}

app.UseCors( "FrontendUI" );

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
/*
app.MapGet( "/", ( HttpContext ctx ) =>
{
    ctx.GetTokenAsync("access_token");
    return ctx.User.Claims.Select( x => new { x.Type, x.Value } ).ToList();
});
app.MapGet( "/oauth/login", () =>
{
    return Results.Challenge(
        new AuthenticationProperties()
        {
            RedirectUri = "https://localhost:7229/swagger/index.html"
        },
        authenticationSchemes: new List<string>() { "github" } ); ;
} );

app.MapGet( "/login", () =>
{
    return Results.Challenge(
        new AuthenticationProperties()
        {
            RedirectUri = "https://localhost:7229/swagger/index.html"
        },
        authenticationSchemes: new List<string>() { "Google" } );
} );
*/

app.Run();

