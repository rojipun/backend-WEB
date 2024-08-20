using AirbnbProject.Models;
using AirbnbProject.Data;
using Microsoft.AspNetCore.Authentication;
using AirbnbProject;


namespace Camping
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policyBuilder =>
                {
                    policyBuilder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                });
            });

            // Register logging and configuration services
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            // Register DataBase as IDataContext
            builder.Services.AddSingleton<IDataContext, DataBase>();

            // Register authentication and authorization services
            builder.Services.AddAuthentication("BasicAuthenticationScheme")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthenticationScheme", options => { });

            
            builder.Services.AddAuthorization(options =>
            {
                // Define an authorization policy requiring the "admin" role
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
            });

            builder.Services.AddControllers(); // Add controllers to handle HTTP requests
            builder.Services.AddEndpointsApiExplorer(); // API explorer for Swagger
            builder.Services.AddSwaggerGen(); //Generate Swagger documentation

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Detailed exception information
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); // Custom error handling
            }

            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthentication(); // Ensure authentication middleware 
            app.UseAuthorization();  // Ensure authorization middleware 

            app.MapControllers(); //Map Routes

            app.Run();
        }
    }
}
