using FioRino_NewProject.AccessAttribute;
using FioRino_NewProject.Controllers;
using FioRino_NewProject.Data;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using FioRino_NewProject.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Text;

namespace FioRino_NewProject
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
            services.AddScoped<FioRinoBaseContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<ISaveRepository, SaveRepository>();
            services.AddScoped<IParsingExcelService, ParsingExcelSevice>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderProductsService, OrderProductsService>();
            services.AddScoped<IOrderProductsRepository, OrderProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStorageRepository, StorageRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUniqueProductsRepository, UniqueProductsRepository>();
            services.AddScoped<ISkuRepository, SkuRepository>();
            services.AddScoped<ParsingProductsService>();
            services.AddScoped<IBrakiService, BrakiService>();
            services.AddScoped<ExcelParsingClass>();
            services.AddTransient<ParsingByDownloadingExcel>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IShoperService, ShoperService>();
            services.AddScoped<ISizeRepository, SizeRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddTransient<IUserAccessRepository, UserAccessRepository>();
            services.AddTransient<IAuthorizationHandler, AccessAttribute.AccessHandler>();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(cfg =>
                {
                    cfg.SaveToken = true;
                    cfg.RequireHttpsMetadata = false;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["JWT:ValidateAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HurtAccess", policy =>
                {
                    policy.Requirements.Add(new UserAccess { Hurt = true });
                    policy.AuthenticationSchemes.Add(
                                        JwtBearerDefaults.AuthenticationScheme);
                });
                options.AddPolicy("MagazynAccess", policy =>
                {
                    policy.Requirements.Add(new UserAccess { Magazyn = true });
                    policy.AuthenticationSchemes.Add(
                                        JwtBearerDefaults.AuthenticationScheme);
                });
                options.AddPolicy("ArchiveAccess", policy =>
                {
                    policy.Requirements.Add(new UserAccess { Magazyn = true });
                    policy.AuthenticationSchemes.Add(
                                        JwtBearerDefaults.AuthenticationScheme);
                });
            });

            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FioRino_NewProject", Version = "v1" });
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FioRino_NewProject v1"));
            //}

            //app.UseHttpsRedirection();
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath/*, "wwwroot"*/)),
            });
            app.UseDefaultFiles();

            app.UseWebSockets();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
