using System.Text;
using Amazon.SimpleEmail;
using api;
using api.Application.Banks;
using api.Application.Emails;
using api.Application.Users;
using api.Application.Users.UseCases;
using api.Domain.Repositories;
using api.Domain.Security;
using api.Domain.Services;
using api.Infrastructure;
using api.Infrastructure.Security;
using api.Infrastructure.AWS.SES;
using api.Infrastructure.Database.Repositories;
using api.Infrastructure.Filters;
using api.Presentation.Controllers.Validators;
using api.Presentation.Controllers.Validators.Users;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using api.Application.Transactions;
using api.Infrastructure.Queues;
using api.Infrastructure.AWS;
using api.Infrastructure.Database;
using System.Net;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, int.Parse(builder.Configuration.GetSection("Port").Value));
});


builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWS"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AuthValidador>();
builder.Services.AddValidatorsFromAssemblyContaining<DepositValidator>();

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddScoped<PostgressDbContext>();

builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<CreateBankAccountUseCase>();
builder.Services.AddScoped<SendWelcomeEmailUseCase>();
builder.Services.AddScoped<SendCompletedDepositEmailUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<GetUserByIdUseCase>();
builder.Services.AddScoped<CreateDepositUseCase>();
builder.Services.AddScoped<ProccessDepositUseCase>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddTransient<IEMailService, SesService>();
builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddAWSService<IAmazonSimpleEmailService>();
builder.Services.AddTransient<IHasher, HasherService>();
builder.Services.AddScoped<IQueueService, SqsService>();


builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddHostedService<DepositbWorkerService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuers = [builder.Configuration["Jwt:Issuer"]],
            ValidAudiences = [builder.Configuration["Jwt:Audience"]],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
        };

        options.IncludeErrorDetails = true;
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                return Task.CompletedTask;

            },
            // OnTokenValidated = context =>
            // {
            //     Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
            //     JsonWebToken? token = context.SecurityToken as JsonWebToken;
            //     return Task.CompletedTask;
            // },
            // OnChallenge = context =>
            // {
            //     Console.WriteLine("OnChallenge: " + context.Error);
            //     return Task.CompletedTask;
            // }

        };


    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


