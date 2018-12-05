using MicBeach.DataAccessContract.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicBeach.Entity.Sys;
using System.Data;
using MicBeach.Develop.DataAccess;
using MicBeach.Develop.CQuery;

namespace MicBeach.DataAccess.Sys
{
    /// <summary>
    /// 用户角色数据访问
    /// </summary>
    public class UserRoleDataAccess : RdbDataAccess<UserRoleEntity>, IUserRoleDbAccess
    {
        #region 获取添加字段

        /// <summary>
        /// 获取添加字段
        /// </summary>
        /// <returns></returns>
        protected override string[] GetEditFields()
        {
            return new string[] { "UserSysNo", "RoleSysNo" };
        }

        #endregion

        #region 获取查询字段

        /// <summary>
        /// 获取查询字段
        /// </summary>
        /// <returns></returns>
        protected override string[] GetQueryFields()
        {
            return new string[] { "UserSysNo", "RoleSysNo" };
        }

        #endregion
    }
}
