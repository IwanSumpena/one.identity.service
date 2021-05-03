using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using src.Models;
using src.Models.Entities;
using System;
using System.Text;

namespace src
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //dbcontext
            services.AddDbContext<OneDbContext>(o=>o.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "one.identity.service", Version = "v1" });
            });

            //Identity
            services.AddIdentity<UserOne, RoleOne>(o =>
            {
                o.Password.RequireUppercase = Convert.ToBoolean(Configuration["Identity:Password.RequireUppercase"]);
                o.Password.RequiredLength = Convert.ToInt32(Configuration["Identity:Password.RequiredLength"]);
                o.Password.RequireLowercase = Convert.ToBoolean(Configuration["Identity:Password.RequireLowercase"]);
                o.Password.RequireNonAlphanumeric = Convert.ToBoolean(Configuration["Identity:Password.RequireNonAlphanumeric"]);
                o.Password.RequireDigit = Convert.ToBoolean(Configuration["Identity:Password.RequireDigit"]);
                o.Lockout.MaxFailedAccessAttempts = Convert.ToInt32(Configuration["Identity:Lockout.MaxFailedAccessAttempts"]);
                o.User.RequireUniqueEmail = Convert.ToBoolean(Configuration["Identity:User.RequireUniqueEmail"]);
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(Configuration["Identity:Lockout.DefaultLockoutTimeSpan.FromMinutes"]));

            }).AddEntityFrameworkStores<OneDbContext>();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;

                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                        ValidateLifetime = true
                    };
                });
            services.AddCors(options => {
                options.AddDefaultPolicy(builder => {
                    builder.WithOrigins(Configuration["Cors:AllowedOrigins"]).AllowAnyHeader().AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "one.identity.service v1"));
            }

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
