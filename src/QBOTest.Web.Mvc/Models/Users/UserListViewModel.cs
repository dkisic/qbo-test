using System.Collections.Generic;
using QBOTest.Roles.Dto;

namespace QBOTest.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
