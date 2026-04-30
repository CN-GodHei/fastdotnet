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

	type BatchUpdateByConditionDto1PluginATestUpdateDto = {
		Query?: PageQueryByConditionDto;
		Dto?: PluginATestUpdateDto;
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

	type BatchUpdateByConditionDto1UpdateFdNoticeDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdNoticeDto;
	};

	type BatchUpdateByConditionDto1UpdateFdNoticeDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdNoticeDto;
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

	type BatchUpdateByConditionDto1UpdateFdTodoTaskDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdTodoTaskDto;
	};

	type BatchUpdateByConditionDto1UpdateFdTodoTaskDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdTodoTaskDto;
	};

	type BatchUpdateByConditionDto1UpdateFdUserLayoutDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdUserLayoutDto;
	};

	type BatchUpdateByConditionDto1UpdateFdWorkbenchCardDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdWorkbenchCardDto;
	};

	type ChangeEmailDto = {
		NewEmail: string;
		VerificationCode: string;
	};

	type ChangePasswordDto = {
		CurrentPassword: string;
		NewPassword: string;
		ConfirmPassword: string;
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

	type CreateFdNoticeDto = {
		ReceiverId: string;
		Title: string;
		Content?: string;
	};

	type CreateFdNoticeDto = {
		ReceiverId: string;
		Title: string;
		Content?: string;
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

	type CreateFdTodoTaskDto = {
		Title: string;
		BusinessId?: string;
		Route?: string;
		AssigneeId: string;
		WorkflowInstanceId?: string;
	};

	type CreateFdTodoTaskDto = {
		Title: string;
		BusinessId?: string;
		Route?: string;
		AssigneeId: string;
		WorkflowInstanceId?: string;
	};

	type CreateFdWorkbenchCardDto = {
		Name?: string;
		Description?: string;
		Icon?: string;
		Type?: string;
		DataSourceUrl?: string;
		ConfigJson?: string;
		DefaultWidth?: number;
		DefaultHeight?: number;
	};

	type CreateOidcApplicationRequest = {
		DisplayName?: string;
		ClientType?: string;
		RedirectUris?: string[];
		PostLogoutRedirectUris?: string[];
		GrantTypes?: string[];
		Scopes?: string[];
	};

	type CreateOrderRequest = {
		UserId?: string;
		TotalAmount?: number;
		ItemCount?: number;
	};

	type CreatePluginAUserExtensionDto = {
		Preferences?: string;
		Points?: number;
	};

	type CreateUserWithExtensionRequest = {
		ExtensionData?: CreatePluginAUserExtensionDto;
		Username: string;
		Password: string;
		Email: string;
		PhoneNumber: string;
		Nickname?: string;
		AvatarUrl: string;
		Status?: number;
	};

	enum DataStatus {
		0 = '0',
		1 = '1',
		2 = '2',
		3 = '3',
	}

	type deleteAppGenericDtoControllerBase5DeleteParams = {
		id: string;
	};

	type deleteAppGenericDtoControllerBase5DeleteParams = {
		id: string;
	};

	type deleteAppGenericDtoControllerBase5DeleteParams = {
		id: string;
	};

	type deleteAppGenericDtoControllerBase5DeleteParams = {
		id: string;
	};

	type deleteAppGenericDtoControllerBase5DeleteParams = {
		id: string;
	};

	type deleteStorageDeleteParams = {
		/** 文件路径(支持相对路径,如: 20260425/xxx.png) */
		filePath?: string;
		/** 存储桶名称(可选) */
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

	type Expression1Func2FdNotice_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
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

	type Expression1Func2FdTodoTask_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2FdWorkbenchCard_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type Expression1Func2PluginATest_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral = {
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
		LastLoginTime?: string;
		RegistrationDate?: string;
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

	type FdNoticeDto = {
		Id?: string;
		ReceiverId?: string;
		Title?: string;
		Content?: string;
		IsRead?: number;
		CreatedAt?: string;
	};

	type FdNoticeDto = {
		Id?: string;
		ReceiverId?: string;
		Title?: string;
		Content?: string;
		IsRead?: number;
		CreatedAt?: string;
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

	type FdTodoTaskDto = {
		Id?: string;
		Title?: string;
		BusinessId?: string;
		Route?: string;
		AssigneeId?: string;
		Status?: number;
		WorkflowInstanceId?: string;
		CreatedAt?: string;
	};

	type FdTodoTaskDto = {
		Id?: string;
		Title?: string;
		BusinessId?: string;
		Route?: string;
		AssigneeId?: string;
		Status?: number;
		WorkflowInstanceId?: string;
		CreatedAt?: string;
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

	type FdUserLayoutDto = {
		Id?: string;
		UserId?: string;
		LayoutData?: string;
		Name?: string;
		IsActive?: number;
	};

	type FdWorkbenchCardDto = {
		Id?: string;
		Name?: string;
		Description?: string;
		Icon?: string;
		Type?: string;
		DataSourceUrl?: string;
		ConfigJson?: string;
		DefaultWidth?: number;
		DefaultHeight?: number;
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

	type getAppGenericDtoControllerBase5GetByIdParams = {
		id: string;
	};

	type getAppGenericDtoControllerBase5GetByIdParams = {
		id: string;
	};

	type getAppGenericDtoControllerBase5GetByIdParams = {
		id: string;
	};

	type getAppGenericDtoControllerBase5GetByIdParams = {
		id: string;
	};

	type getAppGenericDtoControllerBase5GetByIdParams = {
		id: string;
	};

	type getAppGenericDtoControllerBase5GetPageParams = {
		pageIndex?: number;
		pageSize?: number;
	};

	type getAppGenericDtoControllerBase5GetPageParams = {
		pageIndex?: number;
		pageSize?: number;
	};

	type getAppGenericDtoControllerBase5GetPageParams = {
		pageIndex?: number;
		pageSize?: number;
	};

	type getAppGenericDtoControllerBase5GetPageParams = {
		pageIndex?: number;
		pageSize?: number;
	};

	type getAppGenericDtoControllerBase5GetPageParams = {
		pageIndex?: number;
		pageSize?: number;
	};

	type getCaptchaGenerateParams = {
		/** 验证码标识符，通常是用户会话ID或GUID */
		id?: string;
	};

	type getFdAppUserWorkbenchGetMyLayoutParams = {
		name?: string;
	};

	type getOidcLoginLoginParams = {
		returnUrl?: string;
		error?: string;
	};

	type getStorageDownloadParams = {
		/** 文件名 */
		fileName: string;
		/** 存储桶名称（可选） */
		bucketName?: string;
	};

	type getStorageGetFileByPathParams = {
		/** 文件相对路径 */
		relativePath: string;
	};

	type getStorageGetFileUrlParams = {
		/** 文件名 */
		fileName: string;
		/** 存储桶名称（可选） */
		bucketName?: string;
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

	type PageResult1FdNoticeDto = {
		PageInfo?: PageInfo;
		Items?: FdNoticeDto[];
	};

	type PageResult1FdNoticeDto = {
		PageInfo?: PageInfo;
		Items?: FdNoticeDto[];
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

	type PageResult1FdTodoTaskDto = {
		PageInfo?: PageInfo;
		Items?: FdTodoTaskDto[];
	};

	type PageResult1FdTodoTaskDto = {
		PageInfo?: PageInfo;
		Items?: FdTodoTaskDto[];
	};

	type PageResult1FdUserLayoutDto = {
		PageInfo?: PageInfo;
		Items?: FdUserLayoutDto[];
	};

	type PageResult1FdWorkbenchCardDto = {
		PageInfo?: PageInfo;
		Items?: FdWorkbenchCardDto[];
	};

	type PageResult1PluginATestDto = {
		PageInfo?: PageInfo;
		Items?: PluginATestDto[];
	};

	type ParameterExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Name?: string;
		IsByRef?: boolean;
		CanReduce?: boolean;
	};

	type PluginATestCreateDto = {
		Name?: string;
		Description?: string;
		TestValue?: number;
		IsEnabled?: boolean;
		Creator?: string;
	};

	type PluginATestDto = {
		Id?: string;
		Name?: string;
		Description?: string;
		TestValue?: number;
		IsEnabled?: boolean;
		Creator?: string;
		CreatedAt?: string;
		UpdateTime?: string;
	};

	type PluginATestUpdateDto = {
		Id?: string;
		Name?: string;
		Description?: string;
		TestValue?: number;
		IsEnabled?: boolean;
		Creator?: string;
	};

	type PluginAUserExtension = {
		FdAppUserId?: string;
		Preferences?: string;
		Points?: number;
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

	type postCaptchaValidateParams = {
		/** 验证码标识符 */
		id?: string;
		/** 用户输入的验证码 */
		code?: string;
	};

	type postFdAppNoticeMarkAsReadParams = {
		id: string;
	};

	type postFdAppTodoTaskCompleteParams = {
		id: string;
	};

	type postFdAppUserResetPasswordParams = {
		/** 用户ID */
		id: string;
	};

	type postOidcLoginLoginParams = {
		returnUrl?: string;
	};

	type postStorageUploadParams = {
		/** 存储桶名称（可选） */
		bucketName?: string;
	};

	type putAppGenericDtoControllerBase5UpdateParams = {
		id: string;
	};

	type putAppGenericDtoControllerBase5UpdateParams = {
		id: string;
	};

	type putAppGenericDtoControllerBase5UpdateParams = {
		id: string;
	};

	type putAppGenericDtoControllerBase5UpdateParams = {
		id: string;
	};

	type putAppGenericDtoControllerBase5UpdateParams = {
		id: string;
	};

	type QueryByConditionDto = {
		DynamicQuery?: string;
		QueryParameters?: any[];
		SelectFields?: string[];
	};

	type SaveFdUserLayoutDto = {
		LayoutData?: string;
		Name?: string;
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

	enum SystemCategory {
		0 = '0',
		1 = '1',
	}

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

	type UpdateFdNoticeDto = {
		Id: string;
		IsRead?: number;
	};

	type UpdateFdNoticeDto = {
		Id: string;
		IsRead?: number;
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

	type UpdateFdTodoTaskDto = {
		Id: string;
		Title?: string;
		Status?: number;
	};

	type UpdateFdTodoTaskDto = {
		Id: string;
		Title?: string;
		Status?: number;
	};

	type UpdateFdUserLayoutDto = {
		Id?: string;
		LayoutData?: string;
		Name?: string;
	};

	type UpdateFdWorkbenchCardDto = {
		Id?: string;
		Name?: string;
		Description?: string;
		Icon?: string;
		Type?: string;
		DataSourceUrl?: string;
		ConfigJson?: string;
		DefaultWidth?: number;
		DefaultHeight?: number;
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
		RequestMethod?: string;
	};

	type UserRefDto = {
		Id?: string;
		Name?: string;
	};
}
