using MicBeach.ServiceContract.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicBeach.Util;
using MicBeach.BusinessContract.Sys;
using MicBeach.Util.Response;
using MicBeach.DTO.Sys.Query;
using MicBeach.DTO.Sys.Cmd;
using MicBeach.DTO.Sys.Query.Filter;
using MicBeach.Util.Paging;

namespace MicBeach.Service.Sys
{
    /// <summary>
    /// 角色服务
    /// </summary>
    public class RoleService : IRoleService
    {
        IRoleBusiness roleBusiness = null;

        public RoleService(IRoleBusiness roleBusiness)
        {
            this.roleBusiness = roleBusiness;
        }

        #region 保存角色

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="saveInfo">保存信息</param>
        /// <returns></returns>
        public Result<RoleDto> SaveRole(SaveRoleCmdDto saveInfo)
        {
            return roleBusiness.SaveRole(saveInfo);
        }

        #endregion

        #region 获取角色

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="filter">查询对象</param>
        /// <returns></returns>
        public RoleDto GetRole(RoleFilterDto filter)
        {
            return roleBusiness.GetRole(filter);
        }

        #endregion

        #region 删除角色

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="deleteInfo">删除信息</param>
        /// <returns>执行结果</returns>
        public Result DeleteRole(DeleteRoleCmdDto deleteInfo)
        {
            return roleBusiness.DeleteRole(deleteInfo);
        }

        #endregion

        #region 获取角色列表

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="filter">查询对象</param>
        /// <returns></returns>
        public List<RoleDto> GetRoleList(RoleFilterDto filter)
        {
            return roleBusiness.GetRoleList(filter);
        }

        #endregion

        #region 获取角色分页

        /// <summary>
        /// 获取角色分页
        /// </summary>
        /// <param name="filter">查询对象</param>
        /// <returns></returns>
        public IPaging<RoleDto> GetRolePaging(RoleFilterDto filter)
        {
            return roleBusiness.GetRolePaging(filter);
        }

        #endregion

        #region 修改角色排序

        /// <summary>
        /// 修改角色排序
        /// </summary>
        /// <param name="sortIndexInfo">排序修改信息</param>
        /// <returns></returns>
        public Result ModifyRoleSortIndex(ModifyRoleSortCmdDto sortIndexInfo)
        {
            return roleBusiness.ModifyRoleSortIndex(sortIndexInfo);
        }

        #endregion

        #region 验证角色名称是否存在

        /// <summary>
        /// 验证角色名称是否存在
        /// </summary>
        /// <param name="existInfo">验证信息</param>
        /// <returns></returns>
        public bool ExistRoleName(ExistRoleNameCmdDto existInfo)
        {
            return roleBusiness.ExistRoleName(existInfo);
        }

        #endregion
    }
}
