using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicBeach.Application.Identity.Auth;
using MicBeach.Application.Identity.User;
using MicBeach.DTO.Sys.Cmd;
using MicBeach.DTO.Sys.Query;
using MicBeach.DTO.Sys.Query.Filter;
using MicBeach.ServiceContract.Sys;
using MicBeach.Util.Extension;
using MicBeach.Util.Paging;
using MicBeach.Util.Response;
using MicBeach.Util.Serialize;
using MicBeach.ViewModel.Common;
using MicBeach.ViewModel.Sys.Filter;
using MicBeach.ViewModel.Sys.Request;
using MicBeach.ViewModel.Sys.Response;
using MicBeach.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Site.Cms.Controllers
{
    public class SysController : WebBaseController
    {
        IUserService userService = null;
        IRoleService roleService = null;
        IAuthService authService = null;

        public SysController(IUserService userService, IRoleService roleService, IAuthService authService)
        {
            this.userService = userService;
            this.roleService = roleService;
            this.authService = authService;
        }

        #region 修改密码

        public IActionResult ModifyPassword(ModifyPasswordViewModel modifyInfo)
        {
            if (IsPost)
            {
                if (!ModelState.IsValid)
                {
                    return Json(Result.FailedResult("提交数据有错误"));
                }
                var modifyInfoDto = modifyInfo.MapTo<ModifyPasswordCmdDto>();
                modifyInfoDto.CheckOldPassword = true;
                modifyInfoDto.SysNo = User.Id;
                var result = AjaxResult.CopyFromResult(userService.ModifyPassword(modifyInfoDto));
                result.SuccessClose = true;
                return Json(result);
            }
            return View("ModifyPassword");
        }

        #endregion

        #region 角色管理

        #region 角色列表

        public ActionResult RoleList()
        {
            return View();
        }

        #endregion

        #region 编辑/添加角色

        public ActionResult EditRole(EditRoleViewModel role)
        {
            if (IsPost)
            {
                SaveRoleCmdDto roleCmd = new SaveRoleCmdDto()
                {
                    Role = role.MapTo<RoleCmdDto>()
                };
                var saveResult = roleService.SaveRole(roleCmd);
                Result result = saveResult.Success ? Result.SuccessResult(saveResult.Message) : Result.FailedResult(saveResult.Message);
                var ajaxResult = AjaxResult.CopyFromResult(result);
                ajaxResult.SuccessClose = true;
                return Json(ajaxResult);
            }
            else if (role.SysNo > 0)
            {
                RoleFilterDto filter = new RoleFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        role.SysNo
                    },
                    LoadParent = true
                };
                role = roleService.GetRole(filter).MapTo<EditRoleViewModel>();
            }
            return View(role);
        }

        #endregion

        #region 删除角色

        [HttpPost]
        public ActionResult DeleteRole(List<long> sysNos)
        {
            Result result = roleService.DeleteRole(new DeleteRoleCmdDto()
            {
                RoleIds = sysNos
            });
            var ajaxResult = AjaxResult.CopyFromResult(result);
            return Json(ajaxResult);
        }

        #endregion

        #region 修改角色排序

        [HttpPost]
        public ActionResult ChangeRoleSortIndex(long sysNo, int sortIndex)
        {
            Result result = null;
            result = roleService.ModifyRoleSortIndex(new ModifyRoleSortCmdDto()
            {
                RoleSysNo = sysNo,
                NewSortIndex = sortIndex
            });
            var ajaxResult = AjaxResult.CopyFromResult(result);
            return Json(ajaxResult);
        }

        #endregion

        #region 获取一级数据

        [HttpPost]
        public IActionResult GetRoleValue(List<long> excludeIds)
        {
            var menuDatas = InitRoleTreeData(0, excludeIds);
            return Json(new
            {
                AllNodes = menuDatas.Item1,
                AllRole = menuDatas.Item3
            });
        }

        #endregion

        #region 获取下级数据

        /// <summary>
        /// 获取下级数据
        /// </summary>
        /// <param name="parentId">上级编号</param>
        /// <returns></returns>
        public ActionResult LoadChildRoles(long parentId, List<long> excludeIds = null)
        {
            List<RoleViewModel> childRoles = null;
            if (parentId > 0)
            {
                RoleFilterDto filter = new RoleFilterDto()
                {
                    Parent = parentId
                };
                if (!excludeIds.IsNullOrEmpty())
                {
                    filter.ExcludeSysNos = excludeIds;
                }
                childRoles = roleService.GetRoleList(filter).Select(c => c.MapTo<RoleViewModel>()).OrderBy(r => r.SortIndex).ToList();
            }
            childRoles = childRoles ?? new List<RoleViewModel>(0);
            List<TreeNode> treeNodeList = childRoles.Select(c => RoleToTreeNode(c)).ToList();
            string nodesString = JsonSerialize.ObjectToJson<List<TreeNode>>(treeNodeList);
            string rolesData = JsonSerialize.ObjectToJson(childRoles.ToDictionary(c => c.SysNo.ToString()));
            return Json(new
            {
                ChildNodes = nodesString,
                RoleData = rolesData
            });
        }

        #endregion

        #region 数据序列化

        string RoleListToJsonString(IEnumerable<RoleViewModel> roleList)
        {
            List<TreeNode> nodeList = RoleListToTreeNodes(roleList.ToList());
            return JsonSerialize.ObjectToJson<List<TreeNode>>(nodeList);
        }

        List<TreeNode> RoleListToTreeNodes(List<RoleViewModel> roleList)
        {
            if (roleList.IsNullOrEmpty())
            {
                return new List<TreeNode>(0);
            }
            List<TreeNode> nodeList = new List<TreeNode>(roleList.Count);
            var levelOneRoles = roleList.Where(c => c.Level == 1).OrderBy(c => c.SortIndex);
            foreach (var role in levelOneRoles)
            {
                TreeNode node = RoleToTreeNode(role);
                AppendChildNodes(node, role.SysNo, roleList);
                nodeList.Add(node);
            }
            return nodeList;
        }

        void AppendChildNodes(TreeNode parentNode, long parentSysNo, IEnumerable<RoleViewModel> allRoles)
        {
            var childRoles = allRoles.Where(c => c.Parent != null && c.Parent.SysNo == parentSysNo && c.SysNo != c.Parent.SysNo && c.Parent.SysNo > 0).OrderBy(c => c.SortIndex);
            if (childRoles.IsNullOrEmpty())
            {
                return;
            }
            foreach (var role in childRoles)
            {
                TreeNode node = RoleToTreeNode(role);
                parentNode.Children.Add(node);
                AppendChildNodes(node, role.SysNo, allRoles);
            }
        }

        TreeNode RoleToTreeNode(RoleViewModel role)
        {
            return new TreeNode()
            {
                Value = role.SysNo.ToString(),
                Text = role.Name,
                Children = new List<TreeNode>(),
                IsParent = true,
                LoadData = false
            };
        }

        Tuple<string, string, string> InitRoleTreeData(long parentId, List<long> excludeIds=null)
        {
            RoleFilterDto filter = new RoleFilterDto();
            if (parentId > 0)
            {
                filter.Parent = parentId;
            }
            else
            {
                filter.Level = 1;
            }
            if (!excludeIds.IsNullOrEmpty())
            {
                filter.ExcludeSysNos = excludeIds;
            }
            List<RoleViewModel> allRoles = roleService.GetRoleList(filter).Select(c => c.MapTo<RoleViewModel>()).ToList();
            string allNodesString = RoleListToJsonString(allRoles);
            RoleViewModel[] copyRoles = new RoleViewModel[allRoles.Count];
            allRoles.CopyTo(copyRoles);
            List<RoleViewModel> selectRoles = copyRoles.ToList();
            selectRoles.Insert(0, new RoleViewModel()
            {
                Name = "一级角色",
                SysNo = 0,
                Level = 1
            });
            string selectNodesString = RoleListToJsonString(selectRoles);
            return new Tuple<string, string, string>(allNodesString, selectNodesString, JsonSerialize.ObjectToJson(allRoles.ToDictionary(c => c.SysNo.ToString())));
        }

        #endregion

        #region 角色多选

        public ActionResult RoleMultipleSelector()
        {
            return View();
        }

        #endregion

        #region 添加角色用户

        [HttpPost]
        public ActionResult AddRoleUser(long roleId, IEnumerable<long> userIds)
        {
            ModifyUserBindRoleCmdDto bindInfo = new ModifyUserBindRoleCmdDto()
            {
                Binds = userIds.Select(c => new Tuple<UserCmdDto, RoleCmdDto>(new UserCmdDto()
                {
                    SysNo = c
                }, new RoleCmdDto()
                {
                    SysNo = roleId
                }))
            };
            return Json(userService.ModifyUserBindRole(bindInfo));
        }

        #endregion

        #region 删除角色用户

        [HttpPost]
        public ActionResult UnBindRoleUser(long roleId, IEnumerable<long> userIds)
        {
            ModifyUserBindRoleCmdDto bindInfo = new ModifyUserBindRoleCmdDto()
            {
                UnBinds = userIds.Select(c => new Tuple<UserCmdDto, RoleCmdDto>(new UserCmdDto()
                {
                    SysNo = c
                }, new RoleCmdDto()
                {
                    SysNo = roleId
                }))
            };
            return Json(userService.ModifyUserBindRole(bindInfo));
        }

        #endregion

        #region 根据角色加载用户

        [HttpPost]
        public ActionResult GetRoleAdminUsers(AdminUserFilterViewModel filter)
        {
            IPaging<UserViewModel> userPager = SearchAdminUserData(filter);
            object objResult = new
            {
                TotalCount = userPager.TotalCount,
                Datas = JsonSerialize.ObjectToJson(userPager.ToList())
            };
            return Json(objResult);
        }

        #endregion

        #region 角色授权

        [HttpPost]
        public ActionResult LoadRoleAuthorityByGroup(long groupId, string key, long roleId)
        {
            AuthorityFilterDto filter = new AuthorityFilterDto()
            {
                AuthGroup = groupId,
                NameCodeMateKey = key
            };
            List<AuthorizationAuthorityViewModel> authorityList = authService.GetAuthorityList(filter).Select(c => c.MapTo<AuthorizationAuthorityViewModel>()).ToList();
            RoleAuthorityFilterDto roleAuthFilter = new RoleAuthorityFilterDto()
            {
                AuthGroup = groupId,
                NameCodeMateKey = key,
                RoleFilter = new RoleFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        roleId
                    }
                }
            };
            var userAllowAuthorityCodeList = authService.GetAuthorityList(roleAuthFilter).Select(c => c.Code).ToList();
            authorityList.ForEach(c => c.AllowUse = userAllowAuthorityCodeList.Contains(c.Code));
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(authorityList)
            };
            return Json(result);
        }

        [HttpPost]
        public ActionResult ModifyRoleAuthority(long roleId, string removeAuthCodes, string newAuthCodes)
        {
            ModifyRoleAuthorizeCmdDto authorizeInfo = new ModifyRoleAuthorizeCmdDto()
            {
                Binds = newAuthCodes.LSplit(",").Select(c => new Tuple<RoleCmdDto, AuthorityCmdDto>(new RoleCmdDto()
                {
                    SysNo = roleId
                }, new AuthorityCmdDto()
                {
                    Code = c
                })),
                UnBinds = removeAuthCodes.LSplit(",").Select(c => new Tuple<RoleCmdDto, AuthorityCmdDto>(new RoleCmdDto()
                {
                    SysNo = roleId
                }, new AuthorityCmdDto()
                {
                    Code = c
                }))
            };
            return Json(authService.ModifyRoleAuthorize(authorizeInfo));
        }

        #endregion

        #region 检查角色名称是否存在

        [HttpPost]
        public ActionResult CheckRoleName(RoleViewModel role)
        {
            bool allowUse = !roleService.ExistRoleName(new ExistRoleNameCmdDto()
            {
                RoleName = role?.Name,
                ExcludeRoleId = role?.SysNo ?? 0
            });
            return Content(allowUse.ToString().ToLower());
        }

        #endregion

        #endregion

        #region 管理用户管理

        #region 管理用户列表

        public ActionResult AdminUserList()
        {
            return View();
        }

        public ActionResult SearchUser(AdminUserFilterViewModel filter)
        {
            var userPager = SearchAdminUserData(filter);
            object objResult = new
            {
                TotalCount = userPager.TotalCount,
                Datas = JsonSerialize.ObjectToJson(userPager.ToList())
            };
            return Json(objResult);
        }

        IPaging<UserViewModel> SearchAdminUserData(AdminUserFilterViewModel filter)
        {
            filter.UserType = UserType.管理账户;
            IPaging<UserViewModel> userPager = userService.GetUserPaging(filter.MapTo<AdminUserFilterDto>()).ConvertTo<UserViewModel>();
            return userPager;
        }

        #endregion

        #region 详情

        public ActionResult AdminUserDetail(long id)
        {
            #region 用户信息

            UserViewModel user = null;
            if (id > 0)
            {
                AdminUserFilterDto filter = new AdminUserFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        id
                    },
                    LoadRole = true
                };
                user = userService.GetUser(filter).MapTo<UserViewModel>();
            }
            if (user == null || !(user is AdminUserViewModel))
            {
                return Content("没有找到用户信息");
            }
            var adminUser = user as AdminUserViewModel;
            ViewBag.Roles = adminUser?.Roles;

            #endregion

            #region 授权信息

            Tuple<string, string, string> authorityGroupDatas = InitAuthorityGroupTreeData(0);
            ViewBag.AllAuthorityGroup = authorityGroupDatas.Item3;
            ViewBag.AllAuthGroupNodes = authorityGroupDatas.Item1;

            #endregion

            return View(adminUser);
        }

        #endregion

        #region 编辑/添加管理用户

        public ActionResult EditUser(EditAdminUserViewModel user, string userRoles)
        {
            if (IsPost)
            {
                user.UserType = UserType.管理账户;
                AdminUserCmdDto adminUser = user.MapTo<AdminUserCmdDto>();

                #region 账户角色

                if (!userRoles.IsNullOrEmpty())
                {
                    List<string> roleIds = userRoles.LSplit(",").ToList();
                    List<RoleCmdDto> roles = new List<RoleCmdDto>();
                    foreach (var roleId in roleIds)
                    {
                        long roleIdVal = 0;
                        if (!long.TryParse(roleId, out roleIdVal))
                        {
                            continue;
                        }
                        if (roleIdVal <= 0)
                        {
                            continue;
                        }
                        roles.Add(new RoleCmdDto()
                        {
                            SysNo = roleIdVal
                        });
                    }
                    adminUser.Roles = roles;
                }

                #endregion

                SaveUserCmdDto saveInfo = new SaveUserCmdDto()
                {
                    User = adminUser
                };
                var result = userService.SaveUser(saveInfo);
                return Content(JsonSerialize.ObjectToJson(result));
            }
            else if (user.SysNo > 0)
            {
                AdminUserFilterDto filter = new AdminUserFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        user.SysNo
                    },
                    LoadRole = true
                };
                user = userService.GetUser(filter).MapTo<EditAdminUserViewModel>();
            }
            return View(user);
        }

        #endregion

        #region 删除管理用户

        public ActionResult DeleteUser(string sysNos)
        {
            IEnumerable<long> sysNoArray = sysNos.LSplit(",").Select(s => long.Parse(s));
            Result result = userService.DeleteUser(new DeleteUserCmdDto()
            {
                UserIds = sysNoArray
            });
            return Json(result);
        }

        #endregion

        #region 验证登陆名是否存在

        [HttpPost]
        public ActionResult CheckUserName(string userName)
        {
            bool allowUse = true;
            if (!userName.IsNullOrEmpty())
            {
                UserFilterDto filter = new UserFilterDto()
                {
                    UserName = userName
                };
                var user = userService.GetUser(filter);
                allowUse = user == null;
            }
            return Content(allowUse ? "true" : "false");
        }

        #endregion

        #region 修改用户状态

        [HttpPost]
        public ActionResult ModifyUserStatus(long id, UserStatus status)
        {
            ModifyUserStatusCmdDto statusInfo = new ModifyUserStatusCmdDto()
            {
                Status = status,
                UserId = id
            };
            return Json(userService.ModifyStatus(statusInfo));
        }

        #endregion

        #region 修改密码

        public ActionResult AdminModifyPassword(ModifyPasswordViewModel modifyInfo)
        {
            if (IsPost)
            {
                ModelState.Remove("NowPassword");
                if (!ModelState.IsValid)
                {
                    return Json(Result.FailedResult("提交数据有错误"));
                }
                var modifyInfoDto = modifyInfo.MapTo<ModifyPasswordCmdDto>();
                modifyInfoDto.CheckOldPassword = false;
                var modifyResult = userService.ModifyPassword(modifyInfoDto);
                var result = AjaxResult.CopyFromResult(modifyResult);
                result.SuccessClose = true;
                return Json(result);
            }
            return View(modifyInfo);
        }

        #endregion

        #region 管理用户多选

        public ActionResult AdminUserMultiSelect()
        {
            return View();
        }

        public ActionResult AdminUserMultiSelectSearch(AdminUserFilterViewModel filter)
        {
            IPaging<UserViewModel> userPager = SearchAdminUserData(filter);
            object objResult = new
            {
                TotalCount = userPager.TotalCount,
                Datas = JsonSerialize.ObjectToJson(userPager.ToList())
            };
            return Json(objResult);
        }

        #endregion

        #region 用户单选

        public ActionResult AdminUserSingleSelect()
        {
            return View();
        }

        #endregion

        #region 移除用户角色

        [HttpPost]
        public ActionResult RemoveUserRole(long userId, IEnumerable<long> roleIds)
        {
            ModifyUserBindRoleCmdDto bindInfo = new ModifyUserBindRoleCmdDto()
            {
                UnBinds = roleIds.Select(c => new Tuple<UserCmdDto, RoleCmdDto>(new UserCmdDto()
                {
                    SysNo = userId
                }, new RoleCmdDto()
                {
                    SysNo = c
                }))
            };
            return Json(userService.ModifyUserBindRole(bindInfo));
        }

        #endregion

        #region 添加用户角色

        [HttpPost]
        public ActionResult BindUserRole(long userId, IEnumerable<long> roleIds)
        {
            ModifyUserBindRoleCmdDto bindInfo = new ModifyUserBindRoleCmdDto()
            {
                Binds = roleIds.Select(c => new Tuple<UserCmdDto, RoleCmdDto>(new UserCmdDto()
                {
                    SysNo = userId
                }, new RoleCmdDto()
                {
                    SysNo = c
                }))
            };
            return Json(userService.ModifyUserBindRole(bindInfo));
        }

        #endregion

        #region 账户授权

        [HttpPost]
        public ActionResult LoadUserAuthorityByGroup(long groupId, string key, long userId)
        {
            AuthorityFilterDto filter = new AuthorityFilterDto()
            {
                AuthGroup = groupId,
                NameCodeMateKey = key,
            };
            UserAuthorityFilterDto userAuthFilter = new UserAuthorityFilterDto()
            {
                AuthGroup = groupId,
                NameCodeMateKey = key,
                UserFilter = new UserFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        userId
                    }
                }
            };
            List<AuthorizationAuthorityViewModel> authorityList = authService.GetAuthorityList(filter).Select(c => c.MapTo<AuthorizationAuthorityViewModel>()).ToList();
            var userAllowAuthorityCodeList = authService.GetAuthorityList(userAuthFilter).Select(c => c.Code).ToList();
            authorityList.ForEach(c => c.AllowUse = userAllowAuthorityCodeList.Contains(c.Code));
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(authorityList)
            };
            return Json(result);
        }

        [HttpPost]
        public ActionResult ModifyUserAuthority(long userId, IEnumerable<string> removeAuthCodes, IEnumerable<string> newAuthCodes)
        {
            List<UserAuthorizeCmdDto> userAuthorizeList = new List<UserAuthorizeCmdDto>();
            var user = new AdminUserCmdDto()
            {
                SysNo = userId,
                UserType = UserType.管理账户
            };
            if (!removeAuthCodes.IsNullOrEmpty())
            {
                userAuthorizeList.AddRange(removeAuthCodes.Select(c => new UserAuthorizeCmdDto()
                {
                    Disable = true,
                    Authority = new AuthorityCmdDto()
                    {
                        Code = c
                    },
                    User = user
                }));
            }
            if (!newAuthCodes.IsNullOrEmpty())
            {
                userAuthorizeList.AddRange(newAuthCodes?.Select(c => new UserAuthorizeCmdDto()
                {
                    Disable = false,
                    Authority = new AuthorityCmdDto()
                    {
                        Code = c
                    },
                    User = user
                }));
            }
            ModifyUserAuthorizeCmdDto userAuthInfo = new ModifyUserAuthorizeCmdDto()
            {
                UserAuthorizes = userAuthorizeList
            };
            return Json(authService.ModifyUserAuthorize(userAuthInfo));
        }

        #endregion

        #endregion

        #region 权限分组管理

        #region 权限分组列表

        public ActionResult AuthorityGroupList()
        {
            Tuple<string, string, string> authorityGroupDatas = InitAuthorityGroupTreeData(0);
            ViewBag.AllAuthorityGroup = authorityGroupDatas.Item3;
            ViewBag.AllNodes = authorityGroupDatas.Item1;
            ViewBag.SelectNodes = authorityGroupDatas.Item2;
            return View();
        }

        #endregion

        #region 编辑/添加权限分组

        public ActionResult EditAuthorityGroup(EditAuthorityGroupViewModel authorityGroup)
        {
            if (IsPost)
            {
                Result<AuthorityGroupDto> saveResult = authService.SaveAuthorityGroup(authorityGroup.MapTo<SaveAuthorityGroupCmdDto>());
                var ajaxResult = AjaxResult.CopyFromResult(saveResult);
                ajaxResult.SuccessClose = true;
                return Json(ajaxResult);
            }
            else if (!(authorityGroup.SysNo <= 0))
            {
                AuthorityGroupFilterDto filter = new AuthorityGroupFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        authorityGroup.SysNo
                    }
                };
                authorityGroup = authService.GetAuthorityGroup(filter).MapTo<EditAuthorityGroupViewModel>();
            }
            return View(authorityGroup);
        }

        #endregion

        #region 删除权限分组

        [HttpPost]
        public ActionResult DeleteAuthorityGroup(List<long> sysNos)
        {
            Result result = authService.DeleteAuthorityGroup(new DeleteAuthorityGroupCmdDto()
            {
                AuthorityGroupIds = sysNos
            });
            var ajaxResult = AjaxResult.CopyFromResult(result);
            return Json(ajaxResult);
        }

        #endregion

        #region 修改权限分组排序

        [HttpPost]
        public ActionResult ChangeAuthorityGroupSortIndex(long sysNo, int sortIndex)
        {
            Result result = null;
            result = authService.ModifyAuthorityGroupSort(new ModifyAuthorityGroupSortCmdDto()
            {
                AuthorityGroupSysNo = sysNo,
                NewSortIndex = sortIndex
            });
            var ajaxResult = AjaxResult.CopyFromResult(result);
            return Json(ajaxResult);
        }

        #endregion

        #region 获取一级数据

        [HttpPost]
        public IActionResult GetAuthorityGroupValue(List<long> excludeIds)
        {
            var authorityGroupDatas = InitAuthorityGroupTreeData(0, excludeIds);
            return Json(new
            {
                AllNodes = authorityGroupDatas.Item1,
                AllGroups = authorityGroupDatas.Item3
            });
        }

        #endregion

        #region 获取下级数据

        /// <summary>
        /// 获取下级数据
        /// </summary>
        /// <param name="parentId">上级编号</param>
        /// <returns></returns>
        public ActionResult LoadChildAuthorityGroups(long parentId, List<long> excludeIds = null)
        {
            List<AuthorityGroupViewModel> childAuthorityGroups = null;
            if (parentId > 0)
            {
                AuthorityGroupFilterDto filter = new AuthorityGroupFilterDto()
                {
                    Parent = parentId
                };
                if (!excludeIds.IsNullOrEmpty())
                {
                    filter.ExcludeSysNos = excludeIds;
                }
                childAuthorityGroups = authService.GetAuthorityGroupList(filter).Select(c => c.MapTo<AuthorityGroupViewModel>()).OrderBy(r => r.SortIndex).ToList();
            }
            childAuthorityGroups = childAuthorityGroups ?? new List<AuthorityGroupViewModel>(0);
            List<TreeNode> treeNodeList = childAuthorityGroups.Select(c => AuthorityGroupToTreeNode(c)).ToList();
            string nodesString = JsonSerialize.ObjectToJson<List<TreeNode>>(treeNodeList);
            string authorityGroupsData = JsonSerialize.ObjectToJson(childAuthorityGroups.ToDictionary(c => c.SysNo.ToString()));
            return Json(new
            {
                ChildNodes = nodesString,
                AuthorityGroupData = authorityGroupsData
            });
        }

        #endregion

        #region 数据序列化

        string AuthorityGroupListToJsonString(IEnumerable<AuthorityGroupViewModel> authorityGroupList)
        {
            List<TreeNode> nodeList = AuthorityGroupListToTreeNodes(authorityGroupList.ToList());
            return JsonSerialize.ObjectToJson<List<TreeNode>>(nodeList);
        }

        List<TreeNode> AuthorityGroupListToTreeNodes(List<AuthorityGroupViewModel> authorityGroupList)
        {
            if (authorityGroupList.IsNullOrEmpty())
            {
                return new List<TreeNode>(0);
            }
            List<TreeNode> nodeList = new List<TreeNode>(authorityGroupList.Count);
            var levelOneAuthorityGroups = authorityGroupList.Where(c => c.Level == 1).OrderBy(c => c.SortIndex);
            foreach (var authorityGroup in levelOneAuthorityGroups)
            {
                TreeNode node = AuthorityGroupToTreeNode(authorityGroup);
                AppendChildNodes(node, authorityGroup.SysNo, authorityGroupList);
                nodeList.Add(node);
            }
            return nodeList;
        }

        void AppendChildNodes(TreeNode parentNode, long parentSysNo, IEnumerable<AuthorityGroupViewModel> allAuthorityGroups)
        {
            var childAuthorityGroups = allAuthorityGroups.Where(c => c.Parent != null && c.Parent.SysNo == parentSysNo && c.SysNo != c.Parent.SysNo && !(c.Parent.SysNo <= 0)).OrderBy(c => c.SortIndex);
            if (childAuthorityGroups.IsNullOrEmpty())
            {
                return;
            }
            foreach (var authorityGroup in childAuthorityGroups)
            {
                TreeNode node = AuthorityGroupToTreeNode(authorityGroup);
                parentNode.Children.Add(node);
                AppendChildNodes(node, authorityGroup.SysNo, allAuthorityGroups);
            }
        }

        TreeNode AuthorityGroupToTreeNode(AuthorityGroupViewModel authorityGroup)
        {
            return new TreeNode()
            {
                Value = authorityGroup.SysNo.ToString(),
                Text = authorityGroup.Name,
                Children = new List<TreeNode>(),
                IsParent = true,
                LoadData = false
            };
        }

        Tuple<string, string, string> InitAuthorityGroupTreeData(long parentId,List<long> excludeIds=null)
        {
            AuthorityGroupFilterDto filter = new AuthorityGroupFilterDto()
            {
                Level = 1
            };
            if (parentId > 0)
            {
                filter.Parent = parentId;
            }
            if (!excludeIds.IsNullOrEmpty())
            {
                filter.ExcludeSysNos = excludeIds;
            }
            List<AuthorityGroupViewModel> allAuthorityGroups = authService.GetAuthorityGroupList(filter).Select(c => c.MapTo<AuthorityGroupViewModel>()).ToList();
            string allNodesString = AuthorityGroupListToJsonString(allAuthorityGroups);
            AuthorityGroupViewModel[] copyAuthorityGroups = new AuthorityGroupViewModel[allAuthorityGroups.Count];
            allAuthorityGroups.CopyTo(copyAuthorityGroups);
            List<AuthorityGroupViewModel> selectAuthorityGroups = copyAuthorityGroups.ToList();
            selectAuthorityGroups.Insert(0, new AuthorityGroupViewModel()
            {
                Name = "一级分组",
                SysNo = 0,
                Level = 1
            });
            string selectNodesString = AuthorityGroupListToJsonString(selectAuthorityGroups);
            return new Tuple<string, string, string>(allNodesString, selectNodesString, JsonSerialize.ObjectToJson(allAuthorityGroups.ToDictionary(c => c.SysNo.ToString())));
        }

        /// <summary>
        /// 初始化权限分组选择数据
        /// </summary>
        /// <returns></returns>
        Tuple<string, string> InitAuthorityGroupSelectTreeData()
        {
            AuthorityGroupFilterDto filter = new AuthorityGroupFilterDto()
            {
                Level = 1
            };
            List<AuthorityGroupViewModel> allAuthorityGroups = authService.GetAuthorityGroupList(filter).Select(c => c.MapTo<AuthorityGroupViewModel>()).ToList();
            string allNodesJson = AuthorityGroupListToJsonString(allAuthorityGroups);
            string groupDataJson = JsonSerialize.ObjectToJson(allAuthorityGroups.ToDictionary(c => c.SysNo.ToString()));
            return new Tuple<string, string>(allNodesJson, groupDataJson);
        }

        #endregion

        #region 验证权限分组名称是否存在

        [HttpPost]
        public ActionResult CheckAuthorityGroupName(AuthorityGroupViewModel group)
        {
            ExistAuthorityGroupNameCmdDto existInfo = new ExistAuthorityGroupNameCmdDto()
            {
                GroupName = group.Name,
                ExcludeGroupId = group.SysNo
            };
            bool allowUse = !authService.ExistAuthorityGroupName(existInfo);
            return Content(allowUse.ToString().ToLower());
        }

        #endregion

        #endregion

        #region 权限管理

        #region 查询权限

        public ActionResult SearchAuthority(AuthorityFilterViewModel filter)
        {
            IPaging<AuthorityViewModel> authorityPager = authService.GetAuthorityPaging(filter.MapTo<AuthorityFilterDto>()).ConvertTo<AuthorityViewModel>(); ;
            string viewContent = string.Empty; //this.RenderViewContent("Partial/_AuthorityList", authorityPager, null, true);
            object objResult = new
            {
                TotalCount = authorityPager.TotalCount,
                View = viewContent
            };
            return Json(objResult);
        }

        #endregion

        #region 编辑/添加权限

        public ActionResult EditAuthority(EditAuthorityViewModel authority, long groupSysNo = 0)
        {
            if (IsPost)
            {
                SaveAuthorityCmdDto saveInfo = new SaveAuthorityCmdDto()
                {
                    Authority = authority.MapTo<AuthorityCmdDto>()
                };
                var result = authService.SaveAuthority(saveInfo);
                var ajaxResult = AjaxResult.CopyFromResult(result);
                ajaxResult.SuccessClose = true;
                return Json(ajaxResult);
            }
            else
            {
                if (!(authority.Code.IsNullOrEmpty()))
                {
                    AuthorityFilterDto filter = new AuthorityFilterDto()
                    {
                        Codes = new List<string>()
                        {
                            authority.Code
                        }
                    };
                    authority = authService.GetAuthority(filter).MapTo<EditAuthorityViewModel>();
                }
                EditAuthorityGroupViewModel group = null;
                if (groupSysNo > 0)
                {
                    AuthorityGroupFilterDto groupFilter = new AuthorityGroupFilterDto()
                    {
                        SysNos = new List<long>()
                        {
                            groupSysNo
                        }
                    };
                    group = authService.GetAuthorityGroup(groupFilter).MapTo<EditAuthorityGroupViewModel>();
                }
                if (group == null)
                {
                    return Content("请指定要设置权限的分组");
                }
                authority.AuthGroup = group;
                return View(authority);
            }
        }

        #endregion

        #region 删除权限

        [HttpPost]
        public ActionResult DeleteAuthority(string codes)
        {
            IEnumerable<string> codeArray = codes.LSplit(",");
            Result result = authService.DeleteAuthority(new DeleteAuthorityCmdDto()
            {
                AuthorityCodes = codeArray
            });
            return Json(result);
        }

        #endregion

        #region 加载分组权限


        [HttpPost]
        public ActionResult GetAuthoritys(long groupSysNo, string key)
        {
            AuthorityFilterDto filter = new AuthorityFilterDto()
            {
                AuthGroup = groupSysNo,
                NameCodeMateKey = key
            };
            List<AuthorityViewModel> authorityList = authService.GetAuthorityList(filter).Select(c => c.MapTo<AuthorityViewModel>()).ToList();
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(authorityList)
            };
            return Json(result);
        }

        #endregion

        #region 启用/关闭权限

        [HttpPost]
        public ActionResult ModifyAuthorityStatus(string codes, AuthorityStatus status)
        {
            IEnumerable<string> codeList = codes.LSplit(",");
            Dictionary<string, AuthorityStatus> statusInfo = new Dictionary<string, AuthorityStatus>();
            foreach (var code in codeList)
            {
                statusInfo.Add(code, status);
            }
            var result = authService.ModifyAuthorityStatus(new ModifyAuthorityStatusCmdDto()
            {
                AuthorityStatusInfo = statusInfo
            });
            return Json(result);
        }

        #endregion

        #region 权限多选

        public ActionResult AuthorityMultiSelect()
        {
            var result = InitAuthorityGroupSelectTreeData();
            ViewBag.AllNodes = result.Item1;
            return View();
        }

        public ActionResult AuthorityMultiSelectSearch(long groupSysNo, string key)
        {
            AuthorityFilterDto filter = new AuthorityFilterDto()
            {
                AuthGroup = groupSysNo,
                NameCodeMateKey = key
            };
            List<AuthorityViewModel> authorityList = authService.GetAuthorityList(filter).Select(c => c.MapTo<AuthorityViewModel>()).ToList();
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(authorityList)
            };
            return Json(result);
        }

        #endregion

        #region 查看权限关联的操作

        public ActionResult AuthorityBindOperationList(string authCode)
        {
            ViewBag.AuthorityCode = authCode;
            return View();
        }

        public ActionResult AuthorityBindOperationSearch(AuthorityBindOperationFilterViewModel filter)
        {
            var filterDto = filter.MapTo<AuthorityBindOperationFilterDto>();
            filterDto.LoadGroup = true;
            List<AuthorityOperationViewModel> operationList = authService.GetAuthorityOperationList(filterDto).Select(c => c.MapTo<AuthorityOperationViewModel>()).ToList();
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(operationList)
            };
            return Json(result);
        }

        #endregion

        #region 权限绑定操作

        [HttpPost]
        public ActionResult AuthorityBindOperation(string authCode, IEnumerable<long> operationIds)
        {
            ModifyAuthorityBindAuthorityOperationCmdDto bindInfo = new ModifyAuthorityBindAuthorityOperationCmdDto()
            {
                Binds = operationIds.Select(c => new Tuple<AuthorityCmdDto, AuthorityOperationCmdDto>(new AuthorityCmdDto()
                {
                    Code = authCode
                },
                  new AuthorityOperationCmdDto()
                  {
                      SysNo = c
                  }))
            };
            return Json(authService.ModifyAuthorityOperationBindAuthority(bindInfo));
        }

        #endregion

        #region 权限解绑操作

        [HttpPost]
        public ActionResult AuthorityUnBindOperation(string authCode, IEnumerable<long> operationIds)
        {
            ModifyAuthorityBindAuthorityOperationCmdDto bindInfo = new ModifyAuthorityBindAuthorityOperationCmdDto()
            {
                UnBinds = operationIds.Select(c => new Tuple<AuthorityCmdDto, AuthorityOperationCmdDto>(new AuthorityCmdDto()
                {
                    Code = authCode
                },
                  new AuthorityOperationCmdDto()
                  {
                      SysNo = c
                  }))
            };
            return Json(authService.ModifyAuthorityOperationBindAuthority(bindInfo));
        }

        #endregion

        #region 检查权限码是否可用

        [HttpPost]
        public ActionResult CheckAuthorityCode(AuthorityViewModel authority)
        {
            bool allowUse = !authService.ExistAuthorityCode(new ExistAuthorityCodeCmdDto()
            {
                AuthCode = authority.Code
            });
            return Content(allowUse.ToString().ToLower());
        }

        #endregion

        #region 检查权限名称是否可用

        [HttpPost]
        public ActionResult CheckAuthorityName(AuthorityViewModel authority)
        {
            bool allowUse = !authService.ExistAuthorityName(new ExistAuthorityNameCmdDto()
            {
                ExcludeCode = authority.Code,
                Name = authority.Name
            });
            return Content(allowUse.ToString().ToLower());
        }

        #endregion

        #endregion

        #region 操作分组

        #region 授权操作组列表

        public ActionResult AuthorityOperationGroupList()
        {
            Tuple<string, string, string> authorityOperationGroupDatas = InitAuthorityOperationGroupTreeData(0);
            ViewBag.AllAuthorityOperationGroup = authorityOperationGroupDatas.Item3;
            ViewBag.AllNodes = authorityOperationGroupDatas.Item1;
            ViewBag.SelectNodes = authorityOperationGroupDatas.Item2;
            return View();
        }

        #endregion

        #region 编辑/添加授权操作组

        public ActionResult EditAuthorityOperationGroup(EditAuthorityOperationGroupViewModel authorityOperationGroup)
        {
            if (IsPost)
            {
                SaveAuthorityOperationGroupCmdDto saveInfo = new SaveAuthorityOperationGroupCmdDto()
                {
                    AuthorityOperationGroup = authorityOperationGroup.MapTo<AuthorityOperationGroupCmdDto>()
                };
                var saveResult = authService.SaveAuthorityOperationGroup(saveInfo);
                var ajaxResult = AjaxResult.CopyFromResult(saveResult);
                ajaxResult.SuccessClose = true;
                return Json(ajaxResult);
            }
            else if (!(authorityOperationGroup.SysNo <= 0))
            {
                AuthorityOperationGroupFilterDto filter = new AuthorityOperationGroupFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        authorityOperationGroup.SysNo
                    }
                };
                authorityOperationGroup = authService.GetAuthorityOperationGroup(filter).MapTo<EditAuthorityOperationGroupViewModel>();
            }
            return View(authorityOperationGroup);
        }

        #endregion

        #region 删除授权操作组

        [HttpPost]
        public ActionResult DeleteAuthorityOperationGroup(List<long> sysNos)
        {
            Result result = authService.DeleteAuthorityOperationGroup(new DeleteAuthorityOperationGroupCmdDto()
            {
                AuthorityOperationGroupIds = sysNos
            });
            var ajaxResult = AjaxResult.CopyFromResult(result);
            return Json(ajaxResult);
        }

        #endregion

        #region 修改授权操作组排序

        [HttpPost]
        public ActionResult ChangeAuthorityOperationGroupSort(long sysNo, int sort)
        {
            Result result = null;
            result = authService.ModifyAuthorityOperationGroupSort(new ModifyAuthorityOperationGroupSortCmdDto()
            {
                AuthorityOperationGroupSysNo = sysNo,
                NewSort = sort
            });
            var ajaxResult = AjaxResult.CopyFromResult(result);
            return Json(ajaxResult);
        }

        #endregion

        #region 获取一级数据

        [HttpPost]
        public IActionResult GetAuthorityOperationGroupValue(List<long> excludeIds)
        {
            var authorityGroupDatas = InitAuthorityOperationGroupTreeData(0, excludeIds);
            return Json(new
            {
                AllNodes = authorityGroupDatas.Item1,
                AllGroups = authorityGroupDatas.Item3
            });
        }

        #endregion

        #region 获取下级数据

        /// <summary>
        /// 获取下级数据
        /// </summary>
        /// <param name="parentId">上级编号</param>
        /// <returns></returns>
        public ActionResult LoadChildAuthorityOperationGroups(long parentId, List<long> excludeIds = null)
        {
            List<AuthorityOperationGroupViewModel> childAuthorityOperationGroups = null;
            if (parentId > 0)
            {
                AuthorityOperationGroupFilterDto filter = new AuthorityOperationGroupFilterDto()
                {
                    Parent = parentId
                };
                if (!excludeIds.IsNullOrEmpty())
                {
                    filter.ExcludeSysNos = excludeIds;
                }
                childAuthorityOperationGroups = authService.GetAuthorityOperationGroupList(filter).Select(c => c.MapTo<AuthorityOperationGroupViewModel>()).OrderBy(r => r.Sort).ToList();
            }
            childAuthorityOperationGroups = childAuthorityOperationGroups ?? new List<AuthorityOperationGroupViewModel>(0);
            List<TreeNode> treeNodeList = childAuthorityOperationGroups.Select(c => AuthorityOperationGroupToTreeNode(c)).ToList();
            string nodesString = JsonSerialize.ObjectToJson<List<TreeNode>>(treeNodeList);
            string authorityOperationGroupsData = JsonSerialize.ObjectToJson(childAuthorityOperationGroups.ToDictionary(c => c.SysNo.ToString()));
            return Json(new
            {
                ChildNodes = nodesString,
                AuthorityOperationGroupData = authorityOperationGroupsData
            });
        }

        #endregion

        #region 数据序列化

        string AuthorityOperationGroupListToJsonString(IEnumerable<AuthorityOperationGroupViewModel> authorityOperationGroupList)
        {
            List<TreeNode> nodeList = AuthorityOperationGroupListToTreeNodes(authorityOperationGroupList.ToList());
            return JsonSerialize.ObjectToJson<List<TreeNode>>(nodeList);
        }

        List<TreeNode> AuthorityOperationGroupListToTreeNodes(List<AuthorityOperationGroupViewModel> authorityOperationGroupList)
        {
            if (authorityOperationGroupList.IsNullOrEmpty())
            {
                return new List<TreeNode>(0);
            }
            List<TreeNode> nodeList = new List<TreeNode>(authorityOperationGroupList.Count);
            var levelOneAuthorityOperationGroups = authorityOperationGroupList.Where(c => c.Level == 1).OrderBy(c => c.Sort);
            foreach (var authorityOperationGroup in levelOneAuthorityOperationGroups)
            {
                TreeNode node = AuthorityOperationGroupToTreeNode(authorityOperationGroup);
                AppendChildNodes(node, authorityOperationGroup.SysNo, authorityOperationGroupList);
                nodeList.Add(node);
            }
            return nodeList;
        }

        void AppendChildNodes(TreeNode parentNode, long parentSysNo, IEnumerable<AuthorityOperationGroupViewModel> allAuthorityOperationGroups)
        {
            var childAuthorityOperationGroups = allAuthorityOperationGroups.Where(c => c.Parent != null && c.Parent.SysNo == parentSysNo && c.SysNo != c.Parent.SysNo && !(c.Parent.SysNo <= 0)).OrderBy(c => c.Sort);
            if (childAuthorityOperationGroups.IsNullOrEmpty())
            {
                return;
            }
            foreach (var authorityOperationGroup in childAuthorityOperationGroups)
            {
                TreeNode node = AuthorityOperationGroupToTreeNode(authorityOperationGroup);
                parentNode.Children.Add(node);
                AppendChildNodes(node, authorityOperationGroup.SysNo, allAuthorityOperationGroups);
            }
        }

        TreeNode AuthorityOperationGroupToTreeNode(AuthorityOperationGroupViewModel authorityOperationGroup)
        {
            return new TreeNode()
            {
                Value = authorityOperationGroup.SysNo.ToString(),
                Text = authorityOperationGroup.Name,
                Children = new List<TreeNode>(),
                IsParent = true,
                LoadData = false
            };
        }

        Tuple<string, string, string> InitAuthorityOperationGroupTreeData(long parentId,List<long> excludeIds=null)
        {
            AuthorityOperationGroupFilterDto filter = new AuthorityOperationGroupFilterDto();
            if (parentId > 0)
            {
                filter.Parent = parentId;
            }
            else
            {
                filter.Level = 1;
            }
            if (!excludeIds.IsNullOrEmpty())
            {
                filter.ExcludeSysNos = excludeIds;
            }
            List<AuthorityOperationGroupViewModel> allAuthorityOperationGroups = authService.GetAuthorityOperationGroupList(filter).Select(c => c.MapTo<AuthorityOperationGroupViewModel>()).ToList();
            string allNodesString = AuthorityOperationGroupListToJsonString(allAuthorityOperationGroups);
            AuthorityOperationGroupViewModel[] copyAuthorityOperationGroups = new AuthorityOperationGroupViewModel[allAuthorityOperationGroups.Count];
            allAuthorityOperationGroups.CopyTo(copyAuthorityOperationGroups);
            List<AuthorityOperationGroupViewModel> selectAuthorityOperationGroups = copyAuthorityOperationGroups.ToList();
            selectAuthorityOperationGroups.Insert(0, new AuthorityOperationGroupViewModel()
            {
                Name = "一级分组",
                SysNo = 0,
                Level = 1
            });
            string selectNodesString = AuthorityOperationGroupListToJsonString(selectAuthorityOperationGroups);
            return new Tuple<string, string, string>(allNodesString, selectNodesString, JsonSerialize.ObjectToJson(allAuthorityOperationGroups.ToDictionary(c => c.SysNo.ToString())));
        }

        /// <summary>
        /// 初始化操作分组选择数据
        /// </summary>
        /// <returns></returns>
        Tuple<string, string> InitAuthorityOperationGroupSelectTreeData()
        {
            AuthorityOperationGroupFilterDto filter = new AuthorityOperationGroupFilterDto()
            {
                Level = 1
            };
            List<AuthorityOperationGroupViewModel> allAuthorityOperationGroups = authService.GetAuthorityOperationGroupList(filter).Select(c => c.MapTo<AuthorityOperationGroupViewModel>()).ToList();
            string allNodesJson = AuthorityOperationGroupListToJsonString(allAuthorityOperationGroups);
            string groupDataJson = JsonSerialize.ObjectToJson(allAuthorityOperationGroups.ToDictionary(c => c.SysNo.ToString()));
            return new Tuple<string, string>(allNodesJson, groupDataJson);
        }

        #endregion

        #region 操作分组单选

        /// <summary>
        /// 操作分组单选
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthorityOperationGroupSingleSelect()
        {
            var result = InitAuthorityOperationGroupSelectTreeData();
            ViewBag.AllAuthorityOperationGroup = result.Item2;
            ViewBag.AllNodes = result.Item1;
            return View();
        }

        #endregion

        #region 检查操作分组名称是否可用

        [HttpPost]
        public ActionResult CheckAuthorityOperationGroupName(AuthorityOperationGroupViewModel group)
        {
            ExistAuthorityOperationGroupNameCmdDto existInfo = new ExistAuthorityOperationGroupNameCmdDto()
            {
                GroupName = group.Name,
                ExcludeGroupId = group.SysNo
            };
            bool allowUser = !authService.ExistAuthorityOperationGroupName(existInfo);
            return Content(allowUser.ToString().ToLower());
        }

        #endregion

        #endregion

        #region 授权操作

        #region 编辑/添加授权操作

        public ActionResult EditAuthorityOperation(EditAuthorityOperationViewModel authorityOperation, long groupSysNo = 0)
        {
            if (IsPost)
            {
                SaveAuthorityOperationCmdDto saveInfo = new SaveAuthorityOperationCmdDto()
                {
                    AuthorityOperation = authorityOperation.MapTo<AuthorityOperationCmdDto>()
                };
                Result<AuthorityOperationDto> result = authService.SaveAuthorityOperation(saveInfo);
                var ajaxResult = AjaxResult.CopyFromResult(result);
                ajaxResult.SuccessClose = true;
                return Json(ajaxResult);
            }
            else if (!(authorityOperation.SysNo <= 0))
            {
                AuthorityOperationFilterDto filter = new AuthorityOperationFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        authorityOperation.SysNo
                    }
                };
                authorityOperation = authService.GetAuthorityOperation(filter).MapTo<EditAuthorityOperationViewModel>();
                if (authorityOperation == null)
                {
                    return Content("没有指定要操作的数据");
                }
            }
            #region 分组

            EditAuthorityOperationGroupViewModel group = null;
            if (groupSysNo > 0)
            {
                AuthorityOperationGroupFilterDto groupFilter = new AuthorityOperationGroupFilterDto()
                {
                    SysNos = new List<long>()
                    {
                        groupSysNo
                    }
                };
                group = authService.GetAuthorityOperationGroup(groupFilter).MapTo<EditAuthorityOperationGroupViewModel>();
            }
            if (group == null)
            {
                return Content("请指定要设置操作的分组");
            }
            authorityOperation.Group = group;

            #endregion

            return View(authorityOperation);
        }

        #endregion

        #region 删除授权操作

        public ActionResult DeleteAuthorityOperation(string sysNos)
        {
            IEnumerable<long> sysNoArray = sysNos.LSplit(",").Select(s => long.Parse(s));
            Result result = authService.DeleteAuthorityOperation(new DeleteAuthorityOperationCmdDto()
            {
                AuthorityOperationIds = sysNoArray
            });
            return Json(result);
        }

        #endregion

        #region 加载分组操作

        [HttpPost]
        public ActionResult GetActions(long groupSysNo, string key)
        {
            AuthorityOperationFilterDto filter = new AuthorityOperationFilterDto()
            {
                Group = groupSysNo,
                OperationMateKey = key
            };
            List<AuthorityOperationViewModel> authorityOperationList = authService.GetAuthorityOperationList(filter).Select(c => c.MapTo<AuthorityOperationViewModel>()).ToList();
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(authorityOperationList)
            };
            return Json(result);
        }

        #endregion

        #region 启用/关闭操作

        [HttpPost]
        public ActionResult ModifyAuthorityOperationStatus(string sysNos, AuthorityOperationStatus status)
        {
            IEnumerable<long> sysNoArray = sysNos.LSplit(",").Select(s => long.Parse(s));
            Dictionary<long, AuthorityOperationStatus> statusInfo = new Dictionary<long, AuthorityOperationStatus>();
            foreach (var sysNo in sysNoArray)
            {
                statusInfo.Add(sysNo, status);
            }
            var result = authService.ModifyAuthorityOperationStatus(new ModifyAuthorityOperationStatusCmdDto()
            {
                StatusInfo = statusInfo
            });
            return Json(result);
        }

        #endregion

        #region 查看授权操作对应的权限

        public ActionResult AuthorityOperationBindAuthorityList(long id)
        {
            ViewBag.OperationId = id;
            return View();
        }

        public ActionResult AuthorityOperationBindAuthoritySearch(AuthorityOperationBindAuthorityFilterViewModel filter)
        {
            var filterDto = filter.MapTo<AuthorityOperationBindAuthorityFilterDto>();
            filterDto.LoadGroup = true;
            List<AuthorityViewModel> authorityList = authService.GetAuthorityList(filterDto).Select(c => c.MapTo<AuthorityViewModel>()).ToList();
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(authorityList)
            };
            return Json(result);
        }

        #endregion

        #region 检查操作名称是否存在

        [HttpPost]
        public ActionResult CheckAuthorityOperationName(AuthorityOperationViewModel operation)
        {
            bool allowUse = true;
            if (operation.Name.IsNullOrEmpty())
            {
                allowUse = false;
            }
            else
            {
                allowUse = !authService.ExistAuthorityOperationName(operation.Name, operation.SysNo);
            }
            return Content(allowUse ? "true" : "false");
        }

        #endregion

        #region 操作绑定权限

        [HttpPost]
        public ActionResult OperationBindAuthority(long operationId, IEnumerable<string> authCodes)
        {
            ModifyAuthorityBindAuthorityOperationCmdDto bindInfo = new ModifyAuthorityBindAuthorityOperationCmdDto()
            {
                Binds = authCodes.Select(c => new Tuple<AuthorityCmdDto, AuthorityOperationCmdDto>(new AuthorityCmdDto()
                {
                    Code = c
                },
                  new AuthorityOperationCmdDto()
                  {
                      SysNo = operationId
                  }))
            };
            return Json(authService.ModifyAuthorityOperationBindAuthority(bindInfo));
        }

        #endregion

        #region 权限解绑操作

        [HttpPost]
        public ActionResult OperationUnBindAuthority(long operationId, IEnumerable<string> authCodes)
        {
            ModifyAuthorityBindAuthorityOperationCmdDto bindInfo = new ModifyAuthorityBindAuthorityOperationCmdDto()
            {
                UnBinds = authCodes.Select(c => new Tuple<AuthorityCmdDto, AuthorityOperationCmdDto>(new AuthorityCmdDto()
                {
                    Code = c
                },
                  new AuthorityOperationCmdDto()
                  {
                      SysNo = operationId
                  }))
            };
            return Json(authService.ModifyAuthorityOperationBindAuthority(bindInfo));
        }

        #endregion

        #region 操作多选

        public ActionResult AuthorityOperationMultiSelect()
        {
            var result = InitAuthorityOperationGroupSelectTreeData();
            ViewBag.AllNodes = result.Item1;
            return View();
        }

        public ActionResult AuthorityOperationMultiSelectSearch(long groupSysNo, string key)
        {
            AuthorityOperationFilterDto filter = new AuthorityOperationFilterDto()
            {
                Group = groupSysNo,
                OperationMateKey = key
            };
            List<AuthorityOperationViewModel> authorityList = authService.GetAuthorityOperationList(filter).Select(c => c.MapTo<AuthorityOperationViewModel>()).ToList();
            var result = new
            {
                Datas = JsonSerialize.ObjectToJson(authorityList)
            };
            return Json(result);
        }

        #endregion

        #endregion
    }
}