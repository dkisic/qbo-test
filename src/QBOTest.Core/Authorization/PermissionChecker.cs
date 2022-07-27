using Abp.Authorization;
using QBOTest.Authorization.Roles;
using QBOTest.Authorization.Users;

namespace QBOTest.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
