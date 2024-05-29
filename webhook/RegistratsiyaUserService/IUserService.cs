using webhook.models;

namespace TelegramBotWithBacgroundService.RegistratsiyaUserService
{
    public interface  IUserService
    {
        public Task Add(User user);
        public Task<List<User>> GetAllUsers();
    }
}
