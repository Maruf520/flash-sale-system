namespace FlashSale.Core.Entities
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public ApplicationRole Role { get; set; } = default!;
        public Permission Permission { get; set; } = default!;
    }
}
