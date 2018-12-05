using MicBeach.Application.Identity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicBeach.Domain.Sys.Service.Request
{
    /// <summary>
    /// 修改用户状态信息
    /// </summary>
    public class UserStatusInfo
    {
        #region 属性

        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserId
        {
            get;set;
        }

        /// <summary>
        /// 状态信息
        /// </summary>
        public UserStatus Status
        {
            get;set;
        }

        #endregion
    }
}
