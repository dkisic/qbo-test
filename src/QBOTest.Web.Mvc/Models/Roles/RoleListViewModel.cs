using System.Collections.Generic;
using QBOTest.Roles.Dto;

namespace QBOTest.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}
