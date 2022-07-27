using System.Collections.Generic;
using QBOTest.Roles.Dto;

namespace QBOTest.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }
    }
}