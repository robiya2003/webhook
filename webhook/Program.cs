
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TelegramBotWithBacgroundService;
using TelegramBotWithBacgroundService.RegistratsiyaUserService;

namespace webhook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<HostOptions>(options =>
            {
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddDbContext<AppBotDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=BackgroundDb;User Id=postgres;Password=<your password>;");
            });
            builder.Services.AddScoped<BotUpdateHandler>();

            var botConfig = builder.Configuration.GetSection("BotConfiguration")
    .Get<BotConfiguration>();

            builder.Services.AddHttpClient("webhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(botConfig.Token, httpClient));

            builder.Services.AddHostedService<ConfigureWebhook>();
            builder.Services.AddHostedService<TelegramBotWithBacgroundService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
