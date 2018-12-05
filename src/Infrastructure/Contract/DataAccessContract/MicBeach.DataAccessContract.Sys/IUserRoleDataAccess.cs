using MicBeach.Develop.DataAccess;
using MicBeach.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicBeach.DataAccessContract.Sys
{
    /// <summary>
    /// 用户角色数据访问
    /// </summary>
    public interface IUserRoleDataAccess : IDataAccess<UserRoleEntity>
    {
    }

    public interface IUserRoleDbAccess : IUserRoleDataAccess
    { }
}
