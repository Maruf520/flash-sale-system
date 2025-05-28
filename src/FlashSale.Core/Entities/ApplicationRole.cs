namespace FlashSale.Core.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    }
}
