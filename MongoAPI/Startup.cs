using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoAPI.Infrastructure;
using MongoAPI.Options;
using MongoAPI.Services;

namespace MongoAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.ConfigurationRoot = builder.Build();
        }

        public IConfigurationRoot ConfigurationRoot { get; set; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.Configure<Settings>(ConfigurationRoot.GetSection("Settings"));
            services.Configure<PersonDatabaseSettings>(ConfigurationRoot.GetSection("PersonDatabaseSettings"));
            services.Configure<HotelDatabaseSettings>(ConfigurationRoot.GetSection("HotelDatabaseSettings"));
            services.Configure<PersonInRoomDbSettings>(ConfigurationRoot.GetSection("PersonInRoomDbSettings"));
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
            
            // services.AddAuthentication(x=>
            //     {
            //         x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //         x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //     })
            //     .AddJwtBearer(options =>
            //     {
            //         options.IncludeErrorDetails = true;
            //         options.RequireHttpsMetadata = false;
            //
            //         options.TokenValidationParameters = new TokenValidationParameters
            //         {
            //             ValidateIssuer = true,
            //             ValidIssuer = AuthOptions.Issuer,
            //             ValidateAudience = true,
            //             ValidAudience = AuthOptions.Audience,
            //             ValidateLifetime = true,
            //             ClockSkew = TimeSpan.Zero, 
            //             IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            //             ValidateIssuerSigningKey = true
            //         };
            //         options.Events = new JwtBearerEvents()
            //         {
            //             OnChallenge = async context =>
            //             {
            //                 object response;
            //                 if (context.AuthenticateFailure == null)
            //                 {
            //                     response = new
            //                     {
            //                         Exception = "",
            //                         Error = "no header",
            //                         ErrorDescription = "Authorization not set",
            //                         ErrorUri = ""
            //                     };
            //                 }
            //                 else
            //                 {
            //                     response = new
            //                     {
            //                         //Exception = context.AuthenticateFailure,
            //                         Error = context.Error,
            //                         ErrorDescription = context.ErrorDescription,
            //                         ErrorUri = context.ErrorUri
            //                     };
            //                 }
            //                 context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //                 await context.Response.WriteAsync(JsonSerializer.Serialize(response,
            //                     new  JsonSerializerOptions() {WriteIndented = true}));
            //                 context.HandleResponse();
            //             }
            //             
            //         };
            //     });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Mongo API",
                    Description = "Описание методов взаимодействия с MongoDB через API",
                    //TermsOfService = "None",
                    Contact = new OpenApiContact
                    {
                        Name = "Danil Grigorev",
                        Email = "shieffmail@gmail.com"/*,
                        Url =  new Uri("https://uni-systems.ru")*/
                    },
                    License = new OpenApiLicense() {Name = "Proprietary"}
                });
                /*c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.ApiKey,  
                    Scheme = "Bearer",  
                    BearerFormat = "JWT",  
                    In = ParameterLocation.Header,  
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",  
                });*/
                // c.AddSecurityRequirement(new OpenApiSecurityRequirement  
                // {  
                //     {  
                //         new OpenApiSecurityScheme  
                //         {  
                //             Reference = new OpenApiReference  
                //             {  
                //                 Type = ReferenceType.SecurityScheme,  
                //                 Id = "Bearer"  
                //             }  
                //         },  
                //         new string[] {}
                //     }  
                // }); 
            });
            // services.AddDbContext<ApiDbContext>(options =>
            //     options.UseSqlServer(ConfigurationRoot.GetConnectionString("cs1")));
            //
            // services.AddScoped<IAuthContext>(provider => provider.GetService<ApiDbContext>());
            // services.AddScoped<IStudentContext>(provider => provider.GetService<ApiDbContext>());
            // services.AddScoped<IPortfolioContext>(provider => provider.GetService<ApiDbContext>());
            // services.AddScoped<IDepartmentContext>(provider => provider.GetService<ApiDbContext>());
            // services.AddScoped<IPersonContext>(provider => provider.GetService<ApiDbContext>());
            // services.AddScoped<IFilesContext>(provider => provider.GetService<ApiDbContext>());
            // services.AddScoped<IGrantContext>(provider => provider.GetService<ApiDbContext>());
            // services.AddScoped<ITargetStudyContext>(provider => provider.GetService<ApiDbContext>());

            var mvcCoreBuilder = services.AddMvcCore();

            // mvcCoreBuilder.AddMvcOptions(options =>
            //     {
            //         options.Filters.Add(new GlobalStudentIdCheckFilterAttribute());
            //     }
            // );
            var corsAllowed = ConfigurationRoot.GetValue<string>("CorsAllowed");  
            mvcCoreBuilder.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins(corsAllowed)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true));
                options.DefaultPolicyName = "CorsPolicy";
            });
            mvcCoreBuilder.AddApiExplorer();
            //mvcCoreBuilder.AddAuthorization();
            mvcCoreBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            
            services.AddOptions();
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.Register(c => new MapperConfiguration(AutoMapperConfig.Register).CreateMapper()).As<IMapper>().SingleInstance();
            builder.RegisterType<PersonService>();
            builder.RegisterType<HotelService>();
            builder.RegisterType<PersonInRoomService>();
            builder.RegisterType<DictService>();
            builder.RegisterType<MongoService>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            //app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            
            // if (env.IsDevelopment())
            // {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mongo API"));
            // }
            
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}