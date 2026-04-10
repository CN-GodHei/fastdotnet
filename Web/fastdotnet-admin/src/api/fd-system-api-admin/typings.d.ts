declare namespace APIModel {
	type ApiResult = {
		Code?: number;
		Msg?: string;
	};

	type ApiResult1Boolean = {
		Data?: boolean;
		Code?: number;
		Msg?: string;
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

	type AssignUserRolesDto = {
		UserId: string;
		RoleIds?: string[];
	};

	type BatchUpdateByConditionDto1FdUpdateEmailConfigDto = {
		Query?: PageQueryByConditionDto;
		Dto?: FdUpdateEmailConfigDto;
	};

	type BatchUpdateByConditionDto1UpdateCodeGenDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateCodeGenDto;
	};

	type BatchUpdateByConditionDto1UpdateFdAdminUserDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAdminUserDto;
	};

	type BatchUpdateByConditionDto1UpdateFdAdminUserRoleDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAdminUserRoleDto;
	};

	type BatchUpdateByConditionDto1UpdateFdAppUserDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAppUserDto;
	};

	type BatchUpdateByConditionDto1UpdateFdAppUserRoleDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAppUserRoleDto;
	};

	type BatchUpdateByConditionDto1UpdateFdBlacklistDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdBlacklistDto;
	};

	type BatchUpdateByConditionDto1UpdateFdCodeGenConfigDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdCodeGenConfigDto;
	};

	type BatchUpdateByConditionDto1UpdateFdDictDataDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdDictDataDto;
	};

	type BatchUpdateByConditionDto1UpdateFdDictTypeDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdDictTypeDto;
	};

	type BatchUpdateByConditionDto1UpdateFdFdMenuButtonDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdFdMenuButtonDto;
	};

	type BatchUpdateByConditionDto1UpdateFdMenuDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdMenuDto;
	};

	type BatchUpdateByConditionDto1UpdateFdNationalStandardDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdNationalStandardDto;
	};

	type BatchUpdateByConditionDto1UpdateFdNationalStandardItemDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdNationalStandardItemDto;
	};

	type BatchUpdateByConditionDto1UpdateFdPermissionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdPermissionDto;
	};

	type BatchUpdateByConditionDto1UpdateFdRateLimitRuleDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdRateLimitRuleDto;
	};

	type BatchUpdateByConditionDto1UpdateFdRoleDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdRoleDto;
	};

	type BatchUpdateByConditionDto1UpdateFdSystemInfoConfigDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdSystemInfoConfigDto;
	};

	type CheckRegistrUserNameDto = {
		Username: string;
	};

	type CodeGenConfigDto = {
		Id?: string;
		TableName?: string;
		TableComment?: string;
		EntityName?: string;
		NameSpace?: string;
		GenerateType?: string;
		GenerateMenu?: boolean;
		MenuIcon?: string;
		MenuPid?: string;
		PagePath?: string;
		PrintType?: string;
		PrintName?: string;
		TableUniqueList?: TableUniqueConfigDto[];
	};

	type ColumnInfoDto = {
		ColumnName?: string;
		PropertyName?: string;
		DataType?: string;
		NetType?: string;
		IsPrimarykey?: boolean;
		IsIdentity?: boolean;
		IsNullable?: boolean;
		Length?: number;
		Scale?: number;
		DefaultValue?: string;
		ColumnComment?: string;
		IsIgnore?: boolean;
		ShowColumnName?: string;
	};

	type CreateCodeGenDto = {
		TableName: string;
		TableComment?: string;
		NameSpace: string;
		GenerateType: string;
		GenerateMenu?: boolean;
		MenuIcon?: string;
		MenuPid?: string;
		PagePath?: string;
		PrintType?: string;
		PrintName?: string;
		TableUniqueList?: TableUniqueConfigDto[];
	};

	type CreateFdAdminUserDto = {
		Username: string;
		Password: string;
		Name?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
		Avatar?: string;
	};

	type CreateFdAdminUserRoleDto = {
		AdminUserId?: string;
		RoleId?: string;
	};

	type CreateFdAppUserDto = {
		Username: string;
		Password: string;
		Email: string;
		PhoneNumber: string;
		Nickname?: string;
		AvatarUrl: string;
		Status?: number;
	};

	type CreateFdAppUserRoleDto = {
		AppUserId?: string;
		RoleId?: string;
	};

	type CreateFdBlacklistDto = {
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
	};

	type CreateFdCodeGenConfigDto = {
		CodeGenId: string;
		ColumnName: string;
		ColumnKey?: boolean;
		PropertyName: string;
		ColumnLength?: number;
		ColumnComment?: string;
		DataType?: string;
		NetType?: string;
		DefaultValue?: string;
		EffectType?: string;
		PidColumn?: string;
		ForeignKeyConfig?: ForeignKeyConfigModel;
		DictTypeCode?: string;
		QueryType?: string;
		WhetherQuery?: boolean;
		WhetherRetract?: boolean;
		WhetherRequired?: boolean;
		WhetherSortable?: boolean;
		WhetherTable?: boolean;
		WhetherAdd?: boolean;
		WhetherUpdate?: boolean;
		WhetherImport?: boolean;
		WhetherCommon?: boolean;
		OrderNo?: number;
		ShowColumnName?: string;
		MaskConfig?: MaskConfigModel;
		EnableMask?: boolean;
	};

	type CreateFdDictDataDto = {
		DictTypeId?: string;
		DictTypeCode?: string;
		Label?: string;
		Value?: string;
		ValueType?: number;
		Code?: string;
		ParentId?: string;
		Level?: number;
		OrderNo?: number;
		Remark?: string;
		TagType?: string;
		CssClass?: string;
		ListClass?: string;
		IsDefault?: number;
		ExtData?: string;
		Status?: number;
	};

	type CreateFdDictTypeDto = {
		Name?: string;
		Code?: string;
		OrderNo?: number;
		Remark?: string;
		Status?: number;
		SysFlag?: number;
		PluginSysFlag?: number;
		PluginId: string;
	};

	type CreateFdFdMenuButtonDto = {
		Name?: string;
		Code?: string;
		Description?: string;
		MenuCode?: string;
		Module?: string;
		Category?: string;
		Sort?: number;
		PermissionCode?: string;
		IsEnabled?: boolean;
	};

	type CreateFdMenuDto = {
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
		Title?: string;
		SupportWeb?: boolean;
		SupportDesktop?: boolean;
		SupportMobile?: boolean;
	};

	type CreateFdNationalStandardDto = {
		StandardCode?: string;
		StandardName?: string;
		StandardNameEn: string;
		StandardType?: string;
		PublishDepartment: string;
		PublishDate: string;
		ImplementDate: string;
		CurrentVersion?: string;
		Status?: boolean;
		TotalItems?: number;
	};

	type CreateFdNationalStandardItemDto = {
		StandardId?: string;
		ItemCode?: string;
		ItemName?: string;
		ItemNameEn: string;
		ParentCode: string;
		Level: number;
		Sort?: number;
		Status?: boolean;
	};

	type CreateFdPermissionDto = {
		Name: string;
		Code: string;
		Description?: string;
		Module?: string;
		Type: number;
		Category: string;
	};

	type CreateFdRateLimitRuleDto = {
		Type: string;
		Key: string;
		PermitLimit: number;
		WindowSeconds: number;
		Description?: string;
	};

	type CreateFdRoleDto = {
		Name: string;
		Description?: string;
		Belong: SystemCategory;
	};

	type CreateFdSystemInfoConfigDto = {
		Name: string;
		Code: string;
		Value?: string;
		Description?: string;
		IsSystem: boolean;
	};

	type CreateOidcApplicationRequest = {
		DisplayName?: string;
		ClientType?: string;
		RedirectUris?: string[];
		PostLogoutRedirectUris?: string[];
		GrantTypes?: string[];
		Scopes?: string[];
	};

	type DataStatus = 0 | 1 | 2 | 3;

	type deleteApiAdminFdAdminUserIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdAdminUserRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdBlacklistsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdBlacklistsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdDictDataIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdDictDataRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdDictTypeIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdDictTypeRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdEmailConfigRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdMenuButtonsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdMenuButtonsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdMenuIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdMenuRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdPermissionsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdPermissionsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdRatelimitRuleIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdRatelimitRuleRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdRoleIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdRoleRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdSystemInfoConfigIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiAdminFdSystemInfoConfigRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiCodeGenConfigIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiCodeGenConfigRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiCodeGenIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiCodeGenRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdAdminUserRoleIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdAdminUserRoleRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdAppUserIdParams = {
		id: string;
	};

	type deleteApiFdAppUserRoleIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdAppUserRoleRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdNationalStandardIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdNationalStandardItemIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdNationalStandardItemRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiFdNationalStandardRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteApiOidcAppClientIdParams = {
		clientId: string;
	};

	type deleteApiStorage_openAPI_deleteFileNameParams = {
		/** 文件名 */
		fileName: string;
		/** 存储桶名称（可选） */
		bucketName?: string;
	};

	type DownloadPluginDto = {
		/** Token */
		Token: string;
		/** 插件Id */
		PluginId: string;
		/** 版本 */
		Version: string;
	};

	type ExampleRequest = {
		Data?: string;
		Token?: string;
	};

	type ExampleResponse = {
		Data?: string;
		Timestamp?: string;
		Success?: boolean;
	};

	type Expression = {
		NodeType?: ExpressionType;
		Type?: string;
		CanReduce?: boolean;
	};

	type Expression1Func2EmailConfig_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdAdminUser_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdAdminUserRole_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdAppUserRole_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdBlacklist_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdCodeGen_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdCodeGenConfig_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdDictData_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdDictType_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdMenu_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdMenuButton_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdNationalStandard_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdNationalStandardItem_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdPermission_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdRateLimitRule_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdRole_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2SystemInfoConfig_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type ExpressionType =
		| 0
		| 1
		| 2
		| 3
		| 4
		| 5
		| 6
		| 7
		| 8
		| 9
		| 10
		| 11
		| 12
		| 13
		| 14
		| 15
		| 16
		| 17
		| 18
		| 19
		| 20
		| 21
		| 22
		| 23
		| 24
		| 25
		| 26
		| 27
		| 28
		| 29
		| 30
		| 31
		| 32
		| 33
		| 34
		| 35
		| 36
		| 37
		| 38
		| 39
		| 40
		| 41
		| 42
		| 43
		| 44
		| 45
		| 46
		| 47
		| 48
		| 49
		| 50
		| 51
		| 52
		| 53
		| 54
		| 55
		| 56
		| 57
		| 58
		| 59
		| 60
		| 61
		| 62
		| 63
		| 64
		| 65
		| 66
		| 67
		| 68
		| 69
		| 70
		| 71
		| 72
		| 73
		| 74
		| 75
		| 76
		| 77
		| 78
		| 79
		| 80
		| 81
		| 82
		| 83
		| 84;

	type FdAdminUserDto = {
		Id?: string;
		Username?: string;
		Name?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
		LastLoginTime?: string;
		LastLoginIp?: string;
		CreatedAt?: string;
		Avatar?: string;
		RoleIds?: string[];
		Buttons?: string[];
	};

	type FdAdminUserRoleDto = {
		Id?: string;
		AdminUserId?: string;
		RoleId?: string;
	};

	type FdAppUserDto = {
		Id?: string;
		Username?: string;
		Email?: string;
		PhoneNumber?: string;
		Nickname?: string;
		AvatarUrl?: string;
		Status?: number;
		RoleIds?: string[];
		Buttons?: string[];
	};

	type FdAppUserRoleDto = {
		Id?: string;
		AppUserId?: string;
		RoleId?: string;
	};

	type FdBlacklistDto = {
		Id?: number;
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
		IsSystem?: boolean;
		CreatedAt?: string;
		UpdateTime?: string;
	};

	type FdCodeGenConfigDto = {
		Id?: string;
		CodeGenId?: string;
		ColumnName?: string;
		ColumnKey?: boolean;
		PropertyName?: string;
		ColumnLength?: number;
		ColumnComment?: string;
		DataType?: string;
		NetType?: string;
		DefaultValue?: string;
		EffectType?: string;
		FkConfigId?: string;
		FkEntityName?: string;
		DictTypeCode?: string;
		QueryType?: string;
		WhetherQuery?: boolean;
		WhetherRetract?: boolean;
		WhetherRequired?: boolean;
		WhetherSortable?: boolean;
		WhetherTable?: boolean;
		WhetherAddUpdate?: boolean;
		WhetherAdd?: boolean;
		WhetherUpdate?: boolean;
		WhetherImport?: boolean;
		WhetherCommon?: boolean;
		OrderNo?: number;
		ShowColumnName?: string;
		MaskConfig?: MaskConfigModel;
		EnableMask?: boolean;
		ForeignKeyConfig?: ForeignKeyConfigModel;
	};

	type FdCreateEmailConfigDto = {
		Host: string;
		Port: number;
		Username: string;
		Password: string;
		SenderEmail: string;
		SenderName: string;
		EnableSsl?: boolean;
	};

	type FdDictDataDto = {
		Id?: string;
		DictTypeId?: string;
		DictTypeCode?: string;
		Label?: string;
		Value?: string;
		ValueType?: number;
		Code?: string;
		ParentId?: string;
		Level?: number;
		OrderNo?: number;
		Remark?: string;
		TagType?: string;
		CssClass?: string;
		ListClass?: string;
		IsDefault?: number;
		ExtData?: string;
		Status?: number;
		Children?: FdDictDataDto[];
	};

	type FdDictTypeDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		OrderNo?: number;
		Remark?: string;
		Status?: number;
		SysFlag?: number;
		PluginSysFlag?: number;
		PluginId?: string;
	};

	type FdEmailConfigDto = {
		Id?: string;
		Host?: string;
		Port?: number;
		Username?: string;
		Password?: string;
		SenderEmail?: string;
		SenderName?: string;
		EnableSsl?: boolean;
	};

	type FdMenuButtonDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Description?: string;
		MenuCode?: string;
		Module?: string;
		Category?: string;
		Sort?: number;
		PermissionCode?: string;
		IsEnabled?: boolean;
	};

	type FdMenuDto = {
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
		Title?: string;
		SupportWeb?: boolean;
		SupportDesktop?: boolean;
		SupportMobile?: boolean;
		Children?: FdMenuDto[];
		Creator?: UserRefDto;
		Updater?: UserRefDto;
		Deleter?: UserRefDto;
	};

	type FdNationalStandard = {
		StandardCode?: string;
		StandardName?: string;
		StandardNameEn?: string;
		StandardType?: string;
		PublishDepartment?: string;
		PublishDate?: string;
		ImplementDate?: string;
		CurrentVersion?: string;
		Status?: boolean;
		TotalItems?: number;
		Extra?: string;
		Id?: string;
		CreatedAt?: string;
		UpdatedAt?: string;
		IsDeleted?: boolean;
		DeletedAt?: string;
	};

	type FdNationalStandardDetailDto = {
		Id?: string;
		StandardCode?: string;
		StandardName?: string;
		StandardType?: string;
		CurrentVersion?: string;
		Status?: boolean;
		TotalItems?: number;
		PublishDate?: string;
		ImplementDate?: string;
	};

	type FdNationalStandardDto = {
		Id?: string;
		StandardCode?: string;
		StandardName?: string;
		StandardNameEn?: string;
		StandardType?: string;
		PublishDepartment?: string;
		PublishDate?: string;
		ImplementDate?: string;
		CurrentVersion?: string;
		Status?: boolean;
		TotalItems?: number;
	};

	type FdNationalStandardItem = {
		StandardId?: string;
		ItemCode?: string;
		ItemName?: string;
		ItemNameEn?: string;
		ParentCode?: string;
		Level?: number;
		Sort?: number;
		Status?: boolean;
		Extra?: string;
		Id?: string;
		CreatedAt?: string;
		UpdatedAt?: string;
		IsDeleted?: boolean;
		DeletedAt?: string;
	};

	type FdNationalStandardItemDto = {
		Id?: string;
		StandardId?: string;
		ItemCode?: string;
		ItemName?: string;
		ItemNameEn?: string;
		ParentCode?: string;
		Level?: number;
		Sort?: number;
		Status?: boolean;
		Children?: FdNationalStandardItemDto[];
	};

	type FdPermissionDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Description?: string;
		Module?: string;
		Type?: number;
		Category?: string;
		CreatedAt?: string;
		UpdatedAt?: string;
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

	type FdRoleDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Description?: string;
		Belong?: SystemCategory;
		IsSystem?: boolean;
		CreatedAt?: string;
		IsDefault?: boolean;
	};

	type FdSystemInfoConfigDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Value?: string;
		Description?: string;
		IsSystem?: boolean;
		CreatedAt?: string;
		UpdatedAt?: string;
	};

	type FdUpdateEmailConfigDto = {
		Host: string;
		Port: number;
		Username: string;
		Password: string;
		SenderEmail: string;
		SenderName: string;
		EnableSsl?: boolean;
	};

	type ForeignKeyConfigModel = {
		FkConfigId?: string;
		FkEntityName?: string;
		FkTableName?: string;
		FkDisplayColumnList?: string[];
		FkLinkColumnName?: string;
		FkColumnNetType?: string;
		CodeGenId?: string;
	};

	type getApiAdminFdAdminUserIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdAdminUserPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdAdminUserRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdBlacklistsCheckParams = {
		type?: string;
		value?: string;
	};

	type getApiAdminFdBlacklistsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdBlacklistsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdBlacklistsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdDictDataIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdDictDataPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdDictDataRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdDictTypeIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdDictTypePageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdDictTypeRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdEmailConfigPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdEmailConfigRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdMenuButtonsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdMenuButtonsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdMenuButtonsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdMenuIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdMenuMenuBtnsParams = {
		Belong?: number;
		RoleId?: string;
	};

	type getApiAdminFdMenuPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdMenuRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdPermissionsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdPermissionsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdPermissionsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdRatelimitRuleByTypeAndKeyParams = {
		type?: string;
		key?: string;
	};

	type getApiAdminFdRatelimitRuleCheckParams = {
		type?: string;
		key?: string;
	};

	type getApiAdminFdRatelimitRuleIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdRatelimitRulePageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdRatelimitRuleRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdRoleIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdRoleIdPermissionsParams = {
		id: string;
	};

	type getApiAdminFdRolePageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdRoleRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdSystemInfoConfigIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiAdminFdSystemInfoConfigPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiAdminFdSystemInfoConfigRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiCaptchaGenerateParams = {
		/** 验证码标识符，通常是用户会话ID或GUID */
		id?: string;
	};

	type getApiCodeGenColumnlistTableNameConfigIdParams = {
		/** 表名 */
		tableName: string;
		/** 配置ID */
		configId: string;
	};

	type getApiCodeGenConfigIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiCodeGenConfigPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiCodeGenConfigRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiCodeGenDownloadParams = {
		/** 文件路径 */
		filePath?: string;
	};

	type getApiCodeGenGetentitynameParams = {
		tableName?: string;
	};

	type getApiCodeGenGettablecolumnlistParams = {
		TableName?: string;
	};

	type getApiCodeGenIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiCodeGenPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiCodeGenPreviewConfigIdParams = {
		/** 代码生成配置ID */
		configId: string;
		/** 代码类型：entity, dto, service, controller, frontend */
		type?: string;
		apiscop?: string;
	};

	type getApiCodeGenRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiCodeGenTablelistConfigIdParams = {
		/** 配置ID */
		configId: string;
	};

	type getApiEncryptionKeyPrivateAlgorithmParams = {
		/** 加密算法类型 */
		algorithm: string;
	};

	type getApiEncryptionKeyPublicAlgorithmParams = {
		/** 加密算法类型 */
		algorithm: string;
	};

	type getApiFdAdminUserRoleIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiFdAdminUserRolePageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdAdminUserRoleRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdAdminUserRoleUserUserIdRolesParams = {
		userId: string;
	};

	type getApiFdAppUserIdParams = {
		id: string;
	};

	type getApiFdAppUserPageParams = {
		pageIndex?: number;
		pageSize?: number;
	};

	type getApiFdAppUserRoleIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiFdAppUserRolePageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdAppUserRoleRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdAppUserRoleUserUserIdRolesParams = {
		userId: string;
	};

	type getApiFdNationalStandardIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiFdNationalStandardItemIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getApiFdNationalStandardItemPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdNationalStandardItemRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdNationalStandardPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdNationalStandardRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getApiFdNationalStandardStandardCodeDetailParams = {
		standardCode: string;
	};

	type getApiFdNationalStandardStandardCodeTreeParams = {
		standardCode: string;
	};

	type getApiOidcAppClientIdParams = {
		clientId: string;
	};

	type getApiPluginActivePluginIdParams = {
		pluginId: string;
	};

	type getApiPluginConfigurationGetPluginConfigurationByPluginIdParams = {
		PluginId: string;
	};

	type getApiStorageDownloadFileNameParams = {
		/** 文件名 */
		fileName: string;
		/** 存储桶名称（可选） */
		bucketName?: string;
	};

	type getApiStorageUrlFileNameParams = {
		/** 文件名 */
		fileName: string;
		/** 存储桶名称（可选） */
		bucketName?: string;
	};

	type getOidcLoginParams = {
		returnUrl?: string;
		error?: string;
	};

	type getUploadsRelativePathParams = {
		/** 文件相对路径 */
		relativePath: string;
	};

	type HealthStatus = {
		/** 健康状态（Healthy/Unhealthy） */
		Status?: string;
		/** 检查时间戳 */
		Timestamp?: string;
		/** 应用版本 */
		Version?: string;
		/** 运行环境 */
		Environment?: string;
	};

	type ImportStandardRequest = {
		Standard?: FdNationalStandard;
		/** 标准条目列表 */
		Items?: FdNationalStandardItem[];
	};

	type LoginDto = {
		Username: string;
		Password: string;
		CaptchaId?: string;
		CaptchaCode?: string;
	};

	type LoginRequest = {
		Username?: string;
		Password?: string;
	};

	type LoginResultDto = {
		Token?: string;
	};

	type MaskConfigModel = {
		Type?: string;
		PrefixKeep?: number;
		SuffixKeep?: number;
		MaskChar?: string;
		MaskLength?: number;
		CustomPattern?: string;
		CustomReplacement?: string;
	};

	type MenuBtnRe = {
		Id?: string;
		Name?: string;
		Title?: string;
		DataStatus?: DataStatus;
		Exist?: boolean;
		Children?: MenuBtnRe[];
		BtnList?: MenuBtnReStatusDto[];
	};

	type MenuBtnReStatusDto = {
		Id?: string;
		Name?: string;
		DataStatus?: DataStatus;
		Exist?: boolean;
	};

	type MenuType = 0 | 1;

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
		DynamicQuery?: string;
		QueryParameters?: any[];
	};

	type PageResult1CodeGenConfigDto = {
		PageInfo?: PageInfo;
		Items?: CodeGenConfigDto[];
	};

	type PageResult1FdAdminUserDto = {
		PageInfo?: PageInfo;
		Items?: FdAdminUserDto[];
	};

	type PageResult1FdAdminUserRoleDto = {
		PageInfo?: PageInfo;
		Items?: FdAdminUserRoleDto[];
	};

	type PageResult1FdAppUserDto = {
		PageInfo?: PageInfo;
		Items?: FdAppUserDto[];
	};

	type PageResult1FdAppUserRoleDto = {
		PageInfo?: PageInfo;
		Items?: FdAppUserRoleDto[];
	};

	type PageResult1FdBlacklistDto = {
		PageInfo?: PageInfo;
		Items?: FdBlacklistDto[];
	};

	type PageResult1FdCodeGenConfigDto = {
		PageInfo?: PageInfo;
		Items?: FdCodeGenConfigDto[];
	};

	type PageResult1FdDictDataDto = {
		PageInfo?: PageInfo;
		Items?: FdDictDataDto[];
	};

	type PageResult1FdDictTypeDto = {
		PageInfo?: PageInfo;
		Items?: FdDictTypeDto[];
	};

	type PageResult1FdEmailConfigDto = {
		PageInfo?: PageInfo;
		Items?: FdEmailConfigDto[];
	};

	type PageResult1FdMenuButtonDto = {
		PageInfo?: PageInfo;
		Items?: FdMenuButtonDto[];
	};

	type PageResult1FdMenuDto = {
		PageInfo?: PageInfo;
		Items?: FdMenuDto[];
	};

	type PageResult1FdNationalStandardDto = {
		PageInfo?: PageInfo;
		Items?: FdNationalStandardDto[];
	};

	type PageResult1FdNationalStandardItemDto = {
		PageInfo?: PageInfo;
		Items?: FdNationalStandardItemDto[];
	};

	type PageResult1FdPermissionDto = {
		PageInfo?: PageInfo;
		Items?: FdPermissionDto[];
	};

	type PageResult1FdRateLimitRuleDto = {
		PageInfo?: PageInfo;
		Items?: FdRateLimitRuleDto[];
	};

	type PageResult1FdRoleDto = {
		PageInfo?: PageInfo;
		Items?: FdRoleDto[];
	};

	type PageResult1FdSystemInfoConfigDto = {
		PageInfo?: PageInfo;
		Items?: FdSystemInfoConfigDto[];
	};

	type ParameterExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Name?: string;
		IsByRef?: boolean;
		CanReduce?: boolean;
	};

	type PluginConfigurationGetRawJsonDto = {
		ExistRocord?: boolean;
		RawJson?: string;
	};

	type PluginInfo = {
		id?: string;
		name?: string;
		description?: string;
		version?: string;
		enabled?: boolean;
		ManualStop?: boolean;
		author?: string;
		dependencies?: string[];
		tags?: string[];
		entryPoint?: string;
	};

	type postApiAdminFdAdminUserIdResetPasswordParams = {
		id: string;
	};

	type postApiAdminFdRoleIdMenuBtnsParams = {
		id: string;
	};

	type postApiAdminFdRoleIdPermissionsParams = {
		id: string;
	};

	type postApiCaptchaValidateParams = {
		/** 验证码标识符 */
		id?: string;
		/** 用户输入的验证码 */
		code?: string;
	};

	type postApiFdNationalStandardStandardCodeVersionParams = {
		standardCode: string;
	};

	type postApiOidcAppClientIdResetSecretParams = {
		clientId: string;
	};

	type postApiPluginDisablePluginIdParams = {
		pluginId: string;
	};

	type postApiPluginEnablePluginIdParams = {
		pluginId: string;
	};

	type postApiPluginUninstallPluginIdParams = {
		pluginId: string;
	};

	type postApiStorageUploadParams = {
		/** 存储桶名称（可选） */
		bucketName?: string;
	};

	type postOidcLoginParams = {
		returnUrl?: string;
	};

	type putApiAdminFdAdminUserIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdAdminUserRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdBlacklistsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdBlacklistsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdDictDataIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdDictDataRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdDictTypeIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdDictTypeRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdEmailConfigIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdEmailConfigRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdMenuButtonsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdMenuButtonsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdMenuIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdMenuRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdPermissionsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdPermissionsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdRatelimitRuleIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdRatelimitRuleRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdRoleIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdRoleRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdSystemInfoConfigIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiAdminFdSystemInfoConfigRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiCodeGenConfigIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiCodeGenConfigRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiCodeGenIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiCodeGenRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiFdAdminUserRoleIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiFdAdminUserRoleRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiFdAppUserIdParams = {
		id: string;
	};

	type putApiFdAppUserRoleIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiFdAppUserRoleRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiFdNationalStandardIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiFdNationalStandardItemIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putApiFdNationalStandardItemRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiFdNationalStandardRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putApiPluginConfigurationPluginIdParams = {
		PluginId: string;
	};

	type QueryByConditionDto = {
		DynamicQuery?: string;
		QueryParameters?: any[];
		SelectFields?: string[];
	};

	type ResetPasswordDto = {
		NewPassword: string;
	};

	type SendRegistrationCodeDto = {
		Email: string;
	};

	type SetAuthCodeDto = {
		/** 用户授权码 */
		AuthCode: string;
	};

	type SetPluginLicenseDto = {
		Type: string;
		LicenseStr: string;
	};

	type StorageConfigResponse = {
		StorageType?: string;
		DefaultBucket?: string;
		Domain?: string;
		SupportDirectUpload?: boolean;
		ConfigParams?: Record<string, any>;
	};

	type SystemCategory = 0 | 1;

	type TableInfoDto = {
		TableName?: string;
		EntityName?: string;
		TableComment?: string;
	};

	type TableUniqueConfigDto = {
		Columns: string[];
		Message?: string;
	};

	type TestSendEmailDto = {
		ToEmail: string;
		Subject?: string;
		Body?: string;
	};

	type TreeModel1FdNationalStandardItemDto = {
		TreeData?: FdNationalStandardItemDto[];
		Total?: number;
	};

	type UninstallResDto = {
		Result?: boolean;
		Offline?: boolean;
		UninstallCode?: string;
	};

	type UnlockDto = {
		Password: string;
	};

	type UpdateCodeGenDto = {
		TableName: string;
		TableComment?: string;
		EntityName: string;
		NameSpace: string;
		GenerateType: string;
		GenerateMenu?: boolean;
		MenuIcon?: string;
		MenuPid?: string;
		PagePath?: string;
		PrintType?: string;
		PrintName?: string;
		TableUniqueList?: TableUniqueConfigDto[];
	};

	type UpdateFdAdminUserDto = {
		Name?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
		Avatar?: string;
	};

	type UpdateFdAdminUserRoleDto = {
		AdminUserId?: string;
		RoleId?: string;
	};

	type UpdateFdAppUserDto = {
		Id?: string;
		Username?: string;
		Email?: string;
		PhoneNumber?: string;
		Nickname?: string;
		AvatarUrl?: string;
		Status?: number;
	};

	type UpdateFdAppUserRoleDto = {
		AppUserId?: string;
		RoleId?: string;
	};

	type UpdateFdBlacklistDto = {
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
	};

	type UpdateFdCodeGenConfigDto = {
		Id: string;
		CodeGenId: string;
		ColumnName: string;
		ColumnKey?: boolean;
		PropertyName: string;
		ColumnLength?: number;
		ColumnComment?: string;
		DataType?: string;
		NetType?: string;
		DefaultValue?: string;
		EffectType?: string;
		PidColumn?: string;
		ForeignKeyConfig?: ForeignKeyConfigModel;
		DictTypeCode?: string;
		QueryType?: string;
		WhetherQuery?: boolean;
		WhetherRetract?: boolean;
		WhetherRequired?: boolean;
		WhetherSortable?: boolean;
		WhetherTable?: boolean;
		WhetherAdd?: boolean;
		WhetherUpdate?: boolean;
		WhetherImport?: boolean;
		WhetherCommon?: boolean;
		OrderNo?: number;
		ShowColumnName?: string;
		MaskConfig?: MaskConfigModel;
		EnableMask?: boolean;
	};

	type UpdateFdDictDataDto = {
		Id: string;
		DictTypeId?: string;
		DictTypeCode?: string;
		Label?: string;
		Value?: string;
		ValueType?: number;
		Code?: string;
		ParentId?: string;
		Level?: number;
		OrderNo?: number;
		Remark?: string;
		TagType?: string;
		CssClass?: string;
		ListClass?: string;
		IsDefault?: number;
		ExtData?: string;
		Status?: number;
	};

	type UpdateFdDictTypeDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		OrderNo?: number;
		Remark?: string;
		Status?: number;
		SysFlag?: number;
		PluginSysFlag?: number;
		PluginId?: string;
	};

	type UpdateFdFdMenuButtonDto = {
		Name?: string;
		Code?: string;
		Description?: string;
		MenuCode?: string;
		Module?: string;
		Category?: string;
		Sort?: number;
		PermissionCode?: string;
		IsEnabled?: boolean;
	};

	type UpdateFdMenuDto = {
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
		Title?: string;
		SupportWeb?: boolean;
		SupportDesktop?: boolean;
		SupportMobile?: boolean;
	};

	type UpdateFdNationalStandardDto = {
		StandardCode?: string;
		StandardName?: string;
		StandardNameEn?: string;
		StandardType?: string;
		PublishDepartment?: string;
		PublishDate?: string;
		ImplementDate?: string;
		CurrentVersion?: string;
		Status?: boolean;
		TotalItems?: number;
	};

	type UpdateFdNationalStandardItemDto = {
		StandardId?: string;
		ItemCode?: string;
		ItemName?: string;
		ItemNameEn?: string;
		ParentCode?: string;
		Level?: number;
		Sort?: number;
		Status?: boolean;
	};

	type UpdateFdPermissionDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Description?: string;
		Module?: string;
		Type?: number;
		Category?: string;
	};

	type UpdateFdRateLimitRuleDto = {
		Type: string;
		Key: string;
		PermitLimit: number;
		WindowSeconds: number;
		Description?: string;
	};

	type UpdateFdRoleDto = {
		Name: string;
		Description?: string;
	};

	type UpdateFdSystemInfoConfigDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Value?: string;
		Description?: string;
		IsSystem?: boolean;
	};

	type UpdatePluginLicenseOnlineDto = {
		Token: string;
		PluginId: string;
	};

	type UpdateVersionRequest = {
		/** 新版本号 */
		NewVersion?: string;
		/** 新版本的条目数据 */
		NewItems?: FdNationalStandardItem[];
	};

	type UploadCredentialRequest = {
		FileName: string;
		FileSize?: number;
		ContentType?: string;
		BucketName?: string;
		OssType?: string;
	};

	type UploadCredentialResponse = {
		CredentialType?: string;
		UploadUrl?: string;
		UploadParams?: Record<string, any>;
		UploadHeaders?: Record<string, any>;
		ExpiresAt?: string;
		FileUrlTemplate?: string;
		SupportDirectUpload?: boolean;
	};

	type UserRefDto = {
		Id?: string;
		Name?: string;
	};
}
