namespace Docller.Core.Common
{
    public interface ISecureable
    {
        PermissionFlag CurrentUserPermissions { get; set; }
    }
}