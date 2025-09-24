declare namespace APIModel {
	type AdminUserDto = {
		Id?: string;
		Username?: string;
		FullName?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
		LastLoginTime?: string;
		LastLoginIp?: string;
		CreateTime?: string;
	};

	type AdminUserDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: AdminUserDto[];
	};

	type AppRegisterDto = {
		Username: string;
		Password: string;
		Email: string;
		VerificationCode: string;
	};

	type AssignPermissionsDto = {
		PermissionIds?: string[];
	};

	type CreateAdminUserDto = {
		Username: string;
		Password: string;
		FullName?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
	};

	type CreateFdBlacklistDto = {
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
	};

	type CreateFdRateLimitRuleDto = {
		Type: string;
		Key: string;
		PermitLimit: number;
		WindowSeconds: number;
		Description?: string;
	};

	type CreateMenuButtonDto = {
		Name?: string;
		Code?: string;
		Description?: string;
		MenuId?: string;
		Module?: string;
		Category?: string;
		Sort?: number;
		PermissionCode?: string;
		IsEnabled?: boolean;
	};

	type CreateMenuDto = {
		Name?: string;
		Path?: string;
		Icon?: string;
		ParentCode?: string;
		Sort?: number;
		Type?: MenuType;
		Module?: string;
		Category?: string;
		IsExternal?: boolean;
		ExternalUrl?: string;
		IsEnabled?: boolean;
		PermissionCode?: string;
		Component?: string;
		IsHide?: boolean;
		IsKeepAlive?: boolean;
		IsAffix?: boolean;
		IsIframe?: boolean;
		IsFdMicroApp?: boolean;
		isLink?: boolean;
	};

	type CreateRoleDto = {
		Name: string;
		Description?: string;
		Category: string;
	};

	type deleteAdminBlacklistsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminBlacklistsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminCacheTestClearByTagParams = {
		tags?: string[];
	};

	type deleteAdminEmailConfigRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminMenuButtonsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminMenuButtonsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminMenusIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminMenusRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminRateLimitRulesIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminRateLimitRulesRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminRolesIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminRolesRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminSystemConfigIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminSystemConfigRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminUsersIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminUsersRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type EmailConfigBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type EmailConfigDto = {
		Id?: string;
		Host?: string;
		Port?: number;
		Username?: string;
		Password?: string;
		SenderEmail?: string;
		SenderName?: string;
		EnableSsl?: boolean;
	};

	type EmailConfigDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: EmailConfigDto[];
	};

	type Expression = {
		NodeType?: ExpressionType;
		Type?: string;
		CanReduce?: boolean;
	};

	enum ExpressionType {
		0 = '0',
		1 = '1',
		2 = '2',
		3 = '3',
		4 = '4',
		5 = '5',
		6 = '6',
		7 = '7',
		8 = '8',
		9 = '9',
		10 = '10',
		11 = '11',
		12 = '12',
		13 = '13',
		14 = '14',
		15 = '15',
		16 = '16',
		17 = '17',
		18 = '18',
		19 = '19',
		20 = '20',
		21 = '21',
		22 = '22',
		23 = '23',
		24 = '24',
		25 = '25',
		26 = '26',
		27 = '27',
		28 = '28',
		29 = '29',
		30 = '30',
		31 = '31',
		32 = '32',
		33 = '33',
		34 = '34',
		35 = '35',
		36 = '36',
		37 = '37',
		38 = '38',
		39 = '39',
		40 = '40',
		41 = '41',
		42 = '42',
		43 = '43',
		44 = '44',
		45 = '45',
		46 = '46',
		47 = '47',
		48 = '48',
		49 = '49',
		50 = '50',
		51 = '51',
		52 = '52',
		53 = '53',
		54 = '54',
		55 = '55',
		56 = '56',
		57 = '57',
		58 = '58',
		59 = '59',
		60 = '60',
		61 = '61',
		62 = '62',
		63 = '63',
		64 = '64',
		65 = '65',
		66 = '66',
		67 = '67',
		68 = '68',
		69 = '69',
		70 = '70',
		71 = '71',
		72 = '72',
		73 = '73',
		74 = '74',
		75 = '75',
		76 = '76',
		77 = '77',
		78 = '78',
		79 = '79',
		80 = '80',
		81 = '81',
		82 = '82',
		83 = '83',
		84 = '84',
	}

	type FdAdminUserBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdBlacklistBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdBlacklistDto = {
		Id?: number;
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
		IsSystem?: boolean;
		CreateTime?: string;
		UpdateTime?: string;
		IsDeleted?: boolean;
	};

	type FdBlacklistDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdBlacklistDto[];
	};

	type FdMenuBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdMenuButtonBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdRateLimitRuleBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdRateLimitRuleDto = {
		Id?: number;
		Type: string;
		Key: string;
		PermitLimit: number;
		WindowSeconds: number;
		Description?: string;
		IsSystem?: boolean;
		CreatedAt?: string;
		UpdatedAt?: string;
		IsDeleted?: boolean;
	};

	type FdRateLimitRuleDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdRateLimitRuleDto[];
	};

	type FdRoleBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type getAdminBlacklistsCheckParams = {
		type?: string;
		value?: string;
	};

	type getAdminBlacklistsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminBlacklistsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminBlacklistsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminCacheTestGetParams = {
		/** 缓存键 */
		key?: string;
	};

	type getAdminCacheTestProductsParams = {
		/** 分类ID */
		categoryId?: number;
		/** 页码 */
		page?: number;
		/** 每页大小 */
		pageSize?: number;
	};

	type getAdminCacheTestUserIdParams = {
		/** 用户ID */
		id: number;
	};

	type getAdminEmailConfigPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminEmailConfigRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminMenuButtonsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminMenuButtonsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminMenuButtonsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminMenusIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminMenusPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminMenusRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminRateLimitRulesByTypeAndKeyParams = {
		type?: string;
		key?: string;
	};

	type getAdminRateLimitRulesCheckParams = {
		type?: string;
		key?: string;
	};

	type getAdminRateLimitRulesIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminRateLimitRulesPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminRateLimitRulesRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminRolesIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminRolesIdPermissionsParams = {
		id: string;
	};

	type getAdminRolesPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminRolesRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminSystemConfigIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminSystemConfigPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminSystemConfigRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminUsersIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminUsersPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminUsersRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getCaptchaGenerateParams = {
		/** 验证码标识符，通常是用户会话ID或GUID */
		id?: string;
	};

	type getPluginActivePluginIdParams = {
		pluginId: string;
	};

	type LoginDto = {
		Username: string;
		Password: string;
		CaptchaId?: string;
		CaptchaCode?: string;
	};

	type MenuButtonDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Description?: string;
		MenuId?: string;
		Module?: string;
		Category?: string;
		Sort?: number;
		PermissionCode?: string;
		IsEnabled?: boolean;
	};

	type MenuButtonDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: MenuButtonDto[];
	};

	type MenuDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Path?: string;
		Icon?: string;
		ParentCode?: string;
		Sort?: number;
		Type?: MenuType;
		Module?: string;
		Category?: string;
		IsExternal?: boolean;
		ExternalUrl?: string;
		IsEnabled?: boolean;
		PermissionCode?: string;
		Component?: string;
		PluginId?: string;
		IsHide?: boolean;
		IsKeepAlive?: boolean;
		IsAffix?: boolean;
		IsIframe?: boolean;
		IsFdMicroApp?: boolean;
		isLink?: boolean;
		Children?: MenuDto[];
	};

	type MenuDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: MenuDto[];
	};

	enum MenuType {
		0 = '0',
		1 = '1',
	}

	type PageInfo = {
		Total?: number;
		TotalPages?: number;
		HasPreviousPage?: boolean;
		HasNextPage?: boolean;
		Page?: number;
		PageSize?: number;
	};

	type PageQueryByConditionDto = {
		PageIndex?: number;
		PageSize?: number;
		Keyword?: string;
		DynamicQuery?: string;
		QueryParameters?: any[];
	};

	type ParameterExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Name?: string;
		IsByRef?: boolean;
		CanReduce?: boolean;
	};

	type postAdminCacheTestSetParams = {
		/** 缓存键 */
		key?: string;
		/** 缓存值 */
		value?: string;
	};

	type postAdminRolesIdPermissionsParams = {
		id: string;
	};

	type postAdminUsersIdResetPasswordParams = {
		id: string;
	};

	type postCaptchaValidateParams = {
		/** 验证码标识符 */
		id?: string;
		/** 用户输入的验证码 */
		code?: string;
	};

	type postPluginDisablePluginIdParams = {
		pluginId: string;
	};

	type postPluginEnablePluginIdParams = {
		pluginId: string;
	};

	type postPluginUninstallPluginIdParams = {
		pluginId: string;
	};

	type putAdminBlacklistsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminBlacklistsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminEmailConfigIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminEmailConfigRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminMenuButtonsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminMenuButtonsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminMenusIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminMenusRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminRateLimitRulesIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminRateLimitRulesRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminRolesIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminRolesRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminSystemConfigIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminSystemConfigRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminUsersIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminUsersRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type ResetPasswordDto = {
		NewPassword: string;
	};

	type RoleDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Description?: string;
		Category?: string;
		IsSystem?: boolean;
		CreateTime?: string;
	};

	type RoleDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: RoleDto[];
	};

	type SendRegistrationCodeDto = {
		Email: string;
	};

	type SystemConfigBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type SystemConfigDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Value?: any;
		Description?: string;
		IsSystem?: boolean;
	};

	type SystemConfigDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: SystemConfigDto[];
	};

	type UnlockDto = {
		Password: string;
	};

	type UpdateAdminUserDto = {
		FullName?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
	};

	type UpdateEmailConfigDto = {
		Host: string;
		Port: number;
		Username: string;
		Password: string;
		SenderEmail: string;
		SenderName: string;
		EnableSsl?: boolean;
	};

	type UpdateFdBlacklistDto = {
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
	};

	type UpdateFdRateLimitRuleDto = {
		Type: string;
		Key: string;
		PermitLimit: number;
		WindowSeconds: number;
		Description?: string;
	};

	type UpdateMenuButtonDto = {
		Name?: string;
		Code?: string;
		Description?: string;
		MenuId?: string;
		Module?: string;
		Category?: string;
		Sort?: number;
		PermissionCode?: string;
		IsEnabled?: boolean;
	};

	type UpdateMenuDto = {
		Name?: string;
		Code?: string;
		Path?: string;
		Icon?: string;
		ParentCode?: string;
		Sort?: number;
		Type?: MenuType;
		Module?: string;
		Category?: string;
		IsExternal?: boolean;
		ExternalUrl?: string;
		IsEnabled?: boolean;
		PermissionCode?: string;
		Component?: string;
		IsHide?: boolean;
		IsKeepAlive?: boolean;
		IsAffix?: boolean;
		IsFdMicroApp?: boolean;
		IsIframe?: boolean;
		isLink?: boolean;
	};

	type UpdateRoleDto = {
		Name: string;
		Description?: string;
	};
}
