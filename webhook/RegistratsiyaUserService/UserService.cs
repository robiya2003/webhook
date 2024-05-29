using Microsoft.EntityFrameworkCore;
using webhook;
using webhook.models;

namespace TelegramBotWithBacgroundService.RegistratsiyaUserService
{
    public class UserService:IUserService
    {
        private readonly AppBotDbContext appDbContext;

        public UserService(AppBotDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task Add(User user)
        {
            var test = await appDbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (test != null)
            {
                return;
            }
            await appDbContext.Users.AddAsync(user);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await appDbContext.Users.ToListAsync();
        }
    }
}
