using MicBeach.Develop.CQuery;
using MicBeach.Util.Paging;
using MicBeach.DTO.Sys.Cmd;
using MicBeach.DTO.Sys.Query;
using MicBeach.DTO.Sys.Query.Filter;
using MicBeach.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicBeach.Util.Response;

namespace MicBeach.ServiceContract.Sys
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        #region 保存用户

        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="saveInfo">保存信息</param>
        /// <returns></returns>
        Result<UserDto> SaveUser(SaveUserCmdDto saveInfo);

        #endregion

        #region 获取用户

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="filter">筛选信息</param>
        /// <returns></returns>
        UserDto GetUser(UserFilterDto filter);

        #endregion

        #region 获取用户列表

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="filter">筛选信息</param>
        /// <returns></returns>
        List<UserDto> GetUserList(UserFilterDto filter);

        #endregion

        #region 获取用户分页

        /// <summary>
        /// 获取用户分页
        /// </summary>
        /// <param name="filter">筛选信息</param>
        /// <returns></returns>
        IPaging<UserDto> GetUserPaging(UserFilterDto filter);

        #endregion

        #region 删除用户

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="deleteInfo">删除信息</param>
        /// <returns></returns>
        Result DeleteUser(DeleteUserCmdDto deleteInfo);

        #endregion

        #region 用户登录

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userDto">登录用户信息</param>
        /// <returns></returns>
        Result<UserDto> Login(UserDto userDto);

        #endregion

        #region 修改密码

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="modifyInfo">修改信息</param>
        /// <returns></returns>
        Result ModifyPassword(ModifyPasswordCmdDto modifyInfo);

        #endregion

        #region 修改用户状态

        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="statusInfo">状态信息</param>
        /// <returns>执行结果</returns>
        Result ModifyStatus(ModifyUserStatusCmdDto statusInfo);

        #endregion

        #region 修改用户绑定角色

        /// <summary>
        /// 修改用户绑定角色
        /// </summary>
        /// <param name="bindInfo">绑定信息</param>
        Result ModifyUserBindRole(ModifyUserBindRoleCmdDto bindInfo);

        #endregion
    }
}
