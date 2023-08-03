using Application.Services;
using Application.Ultilities;
using AutoMapper;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.SignalRHub;
using Amazon.S3;
using Application.AWS;
using Amazon.SimpleEmail;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});
builder.Services.AddControllers();
builder.Services.AddSignalR();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultAWSOptions(configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAWSService<IAmazonSimpleEmailService>();

builder.Services.Configure<IConfiguration>(configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkService, WorkService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IAmazonS3Service, AmazonS3Service>();
builder.Services.AddScoped<IAmazonEmailService, AmazonEmailService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IWorkRepo, WorkRepo>();
builder.Services.AddScoped<IProposalRepo, ProposalRepo>();
builder.Services.AddScoped<IMessageRepo, MessageRepo>();
builder.Services.AddScoped<IContractRepo, ContractRepo>();
builder.Services.AddScoped<IAttachmentRepo, AttachmentRepo>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration.GetSection("Jwt:Issuer").Get<string>(),
            ValidAudience = configuration.GetSection("Jwt:Issuer").Get<string>(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Get<string>()))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

app.Run();
