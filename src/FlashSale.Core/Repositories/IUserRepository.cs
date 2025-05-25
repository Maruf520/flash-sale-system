namespace FlashSale.Core.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
    }
}
