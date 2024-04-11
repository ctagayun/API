using ConfArch.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//ApiExplorer and SwaggerGen are added to DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  //this enable swagger to scan for all endpoints
                                   //Swagger middleware writes documentation
                                   //builder.Services.AddCors(); implCookie
builder.Services.AddControllersWithViews();

//implCookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
      o.Cookie.Name = "__Host-spa";
      o.Cookie.SameSite = SameSiteMode.Strict;
      o.Events.OnRedirectToLogin = (context) =>
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
      };
    });

//implCookie - this declares the policy of "admin"
builder.Services.AddAuthorization(o =>
    o.AddPolicy("admin", p => p.RequireClaim("role", "Admin"))
);

//Now let register the DbContext in the DI container. I am turning off 
//tracking because it is more efficient and performant because we are 
//the context is recreated everytime anyway.
builder.Services.AddDbContext<HouseDbContext>(o =>
  o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddScoped<IHouseRepository, HouseRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>(); //added for bidder stuff
builder.Services.AddScoped<IUserRepository, UserRepository>(); //implCookie

//Now run the WebApplication.CreateBuilder which configures HTTP pipeline and routes and
//initializes an instance of the WebApplicationBuilder class with configured defaults
var app = builder.Build();

/*
   Application Middleware
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  //Swagger is only added when we are in DEV environment
  app.UseSwagger();  //this creates standard definition of our API in JSON format
  app.UseSwaggerUI(); //this transforms JSON into a UI. will continue to work with cookiehosting
  app.UseStaticFiles(); //cookiehosting - will enable app to serve contents in wwwwroot
  app.UseAuthentication(); //implCookie - activates authentication. call it before endpoint declarations 
}

//Allow api port 4000 to call port 3000 where the react app is running - ge trid because we are now calling cross-site: cookie hosting
//app.UseCors(p => p.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());

app.MapHouseEndpoints(); //simply call the extension method which contains the HTTP calls.will continue to work with cookiehosting
app.MapBidEndpoints();   //Simply call the extension method which contains the HTTP calls. will continue to work with cookiehosting
//app.UseHttpsRedirection();
app.UseRouting();  //cookiehosting 
app.UseAuthorization();//implCookie 
app.MapDefaultControllerRoute(); //cookiehosting 
app.MapFallbackToFile("index.html"); //cookiehosting before we call run we tell her to fallback to index.html if there's no endpoint match

app.Run();  //Finally the app is commanded to run and then we will 
            //an active endpoint.    
            //However before we run, open launchSettings.JSON and 
            //delete IAS settings and IAS EXPRESS profile
            //as well as the "//localhost:5151" in "applicationUrl"
            //and change the port to https://localhost:4000"

//So our API will run in port 4000 and frontend on 3000

