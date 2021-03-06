
/****** Object:  Table [dbo].[Sys_Authority]    Script Date: 2018/12/5 22:36:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_Authority](
	[Code] [varchar](50) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[AuthType] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Sort] [int] NOT NULL,
	[AuthGroup] [bigint] NOT NULL,
	[Remark] [varchar](50) NULL,
	[Application] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Sys_Authority_1] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_AuthorityBindOperation]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_AuthorityBindOperation](
	[AuthorithOperation] [bigint] NOT NULL,
	[AuthorityCode] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Sys_AuthorityBindOperation] PRIMARY KEY CLUSTERED 
(
	[AuthorithOperation] ASC,
	[AuthorityCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_AuthorityGroup]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_AuthorityGroup](
	[SysNo] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[SortIndex] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Parent] [bigint] NOT NULL,
	[Level] [int] NOT NULL,
	[Application] [varchar](50) NULL,
	[Remark] [varchar](100) NULL,
 CONSTRAINT [PK_SYS_AUTHORITYGROUP] PRIMARY KEY NONCLUSTERED 
(
	[SysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_AuthorityOperation]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_AuthorityOperation](
	[SysNo] [bigint] NOT NULL,
	[ControllerCode] [varchar](100) NOT NULL,
	[ActionCode] [varchar](100) NOT NULL,
	[Method] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[Sort] [int] NOT NULL,
	[Group] [bigint] NOT NULL,
	[AuthorizeType] [int] NOT NULL,
	[Application] [varchar](50) NOT NULL,
	[Remark] [varchar](50) NULL,
 CONSTRAINT [PK_Sys_AuthorityOperation] PRIMARY KEY CLUSTERED 
(
	[SysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_AuthorityOperationGroup]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_AuthorityOperationGroup](
	[SysNo] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Sort] [int] NOT NULL,
	[Parent] [bigint] NOT NULL,
	[Root] [bigint] NOT NULL,
	[Level] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Application] [varchar](50) NOT NULL,
	[Remark] [varchar](50) NULL,
 CONSTRAINT [PK_Sys_AuthorityOperationGroup] PRIMARY KEY CLUSTERED 
(
	[SysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_Role]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_Role](
	[SysNo] [bigint] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Level] [int] NOT NULL,
	[Parent] [bigint] NOT NULL,
	[SortIndex] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Application] [varchar](50) NOT NULL,
	[Remark] [varchar](50) NULL,
 CONSTRAINT [PK_SYS_ROLE] PRIMARY KEY NONCLUSTERED 
(
	[SysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_RoleAuthorize]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_RoleAuthorize](
	[Role] [bigint] NOT NULL,
	[Authority] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Sys_RoleAuthorize] PRIMARY KEY CLUSTERED 
(
	[Role] ASC,
	[Authority] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_User]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_User](
	[SysNo] [bigint] NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[RealName] [varchar](50) NOT NULL,
	[Pwd] [varchar](50) NOT NULL,
	[UserType] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Mobile] [varchar](15) NULL,
	[Email] [varchar](50) NULL,
	[QQ] [varchar](15) NULL,
	[SuperUser] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
 CONSTRAINT [PK_SYS_USER] PRIMARY KEY NONCLUSTERED 
(
	[SysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_UserAuthorize]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_UserAuthorize](
	[User] [bigint] NOT NULL,
	[Authority] [varchar](50) NOT NULL,
	[Disable] [bit] NOT NULL,
 CONSTRAINT [PK_Sys_UserAuthorize] PRIMARY KEY CLUSTERED 
(
	[User] ASC,
	[Authority] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sys_UserRole]    Script Date: 2018/12/5 22:36:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_UserRole](
	[UserSysNo] [bigint] NOT NULL,
	[RoleSysNo] [bigint] NOT NULL,
 CONSTRAINT [PK_SYS_USERROLE] PRIMARY KEY CLUSTERED 
(
	[UserSysNo] ASC,
	[RoleSysNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sys_Authority] ADD  CONSTRAINT [DF_Sys_Authority_AuthGroup]  DEFAULT ((0)) FOR [AuthGroup]
GO
ALTER TABLE [dbo].[Sys_Authority] ADD  CONSTRAINT [DF_Sys_Authority_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Sys_AuthorityBindOperation] ADD  CONSTRAINT [DF_Sys_AuthorityBindOperation_AuthorithOperation]  DEFAULT ((0)) FOR [AuthorithOperation]
GO
ALTER TABLE [dbo].[Sys_AuthorityGroup] ADD  CONSTRAINT [DF_Sys_AuthorityGroup_Parent]  DEFAULT ((0)) FOR [Parent]
GO
ALTER TABLE [dbo].[Sys_AuthorityGroup] ADD  CONSTRAINT [DF_Sys_AuthorityGroup_Level]  DEFAULT ((0)) FOR [Level]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperation] ADD  CONSTRAINT [DF_Sys_AuthorityOperation_SysNo]  DEFAULT ((0)) FOR [SysNo]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperation] ADD  CONSTRAINT [DF_Sys_AuthorityOperation_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperation] ADD  CONSTRAINT [DF_Sys_AuthorityOperation_Sort]  DEFAULT ((0)) FOR [Sort]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperation] ADD  CONSTRAINT [DF_Sys_AuthorityOperation_Group]  DEFAULT ((0)) FOR [Group]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperation] ADD  CONSTRAINT [DF_Sys_AuthorityOperation_AuthorizeType]  DEFAULT ((0)) FOR [AuthorizeType]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperationGroup] ADD  CONSTRAINT [DF_Sys_AuthorityOperationGroup_SysNo]  DEFAULT ((0)) FOR [SysNo]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperationGroup] ADD  CONSTRAINT [DF_Sys_AuthorityOperationGroup_Sort]  DEFAULT ((0)) FOR [Sort]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperationGroup] ADD  CONSTRAINT [DF_Sys_AuthorityOperationGroup_Parent]  DEFAULT ((0)) FOR [Parent]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperationGroup] ADD  CONSTRAINT [DF_Sys_AuthorityOperationGroup_Root]  DEFAULT ((0)) FOR [Root]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperationGroup] ADD  CONSTRAINT [DF_Sys_AuthorityOperationGroup_Level]  DEFAULT ((0)) FOR [Level]
GO
ALTER TABLE [dbo].[Sys_AuthorityOperationGroup] ADD  CONSTRAINT [DF_Sys_AuthorityOperationGroup_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Sys_Role] ADD  CONSTRAINT [DF_Sys_Role_Application]  DEFAULT ((0)) FOR [Application]
GO
ALTER TABLE [dbo].[Sys_UserAuthorize] ADD  CONSTRAINT [DF_Sys_UserAuthority_Disable]  DEFAULT ((1)) FOR [Disable]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'AuthType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限分组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'AuthGroup'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属应用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'Application'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Authority'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'授权操作' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityBindOperation', @level2type=N'COLUMN',@level2name=N'AuthorithOperation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityBindOperation', @level2type=N'COLUMN',@level2name=N'AuthorityCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限&授权操作关联' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityBindOperation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'SysNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'SortIndex'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级分组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'Parent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组等级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'Level'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属应用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'Application'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限分组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityGroup'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'SysNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'控制器' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'ControllerCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作方法' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'ActionCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'Method'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作分组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'Group'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'授权类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'AuthorizeType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属应用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'Application'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'方法描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'授权操作' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'SysNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Parent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'根组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Root'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'等级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Level'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属应用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Application'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'授权操作组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_AuthorityOperationGroup'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'SysNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'等级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'Level'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'Parent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'SortIndex'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属应用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'Application'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_Role'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_RoleAuthorize', @level2type=N'COLUMN',@level2name=N'Role'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_RoleAuthorize', @level2type=N'COLUMN',@level2name=N'Authority'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色权限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_RoleAuthorize'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'SysNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'真实名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'RealName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'Pwd'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'UserType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'手机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'Mobile'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'QQ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'QQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'超级管理员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'SuperUser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近登录时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'LastLoginDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_UserAuthorize', @level2type=N'COLUMN',@level2name=N'User'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_UserAuthorize', @level2type=N'COLUMN',@level2name=N'Authority'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'禁用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_UserAuthorize', @level2type=N'COLUMN',@level2name=N'Disable'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户授权' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_UserAuthorize'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_UserRole', @level2type=N'COLUMN',@level2name=N'UserSysNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_UserRole', @level2type=N'COLUMN',@level2name=N'RoleSysNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户角色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_UserRole'
GO
