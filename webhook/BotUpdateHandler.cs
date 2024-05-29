using Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using TelegramBotWithBacgroundService.RegistratsiyaUserService;

namespace TelegramBotWithBacgroundService
{
    public class BotUpdateHandler: IUpdateHandler

    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BotUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Echo received message text
            await HandleMessageAsync(botClient, message, cancellationToken);
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var user = new UserModel()
                    {
                        Id = message.Chat.Id,
                        UserName = message.From.Username
                    };

                    await userRepository.Add(user);

                    try
                    {

                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: $"You said:\n<i>{message.Text}</i>",
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

    }
}
