declare namespace APIModel {
	enum ActivationStatusDto {
		0 = '0',
		1 = '1',
		2 = '2',
	}

	type ApiResult = {
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

	type CodeGenConfigDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: CodeGenConfigDto[];
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

	type CreateFdBlacklistDto = {
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
	};

	type CreateFdCodeGenConfigDto = {
		CodeGenId: number;
		ColumnName: string;
		ColumnKey?: string;
		PropertyName: string;
		ColumnLength?: number;
		ColumnComment?: string;
		DataType?: string;
		NetType?: string;
		DefaultValue?: string;
		EffectType?: string;
		FkConfigId?: string;
		FkEntityName?: string;
		FkTableName?: string;
		FkDisplayColumns?: string;
		FkLinkColumnName?: string;
		FkColumnNetType?: string;
		PidColumn?: string;
		DictTypeCode?: string;
		QueryType?: string;
		WhetherQuery?: string;
		WhetherRetract?: string;
		WhetherRequired?: string;
		WhetherSortable?: string;
		WhetherTable?: string;
		WhetherAddUpdate?: string;
		WhetherImport?: string;
		WhetherCommon?: string;
		OrderNo?: number;
		ShowColumnName?: string;
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

	type CreateGiftRecordDto = {
		PurchaseId: number;
		Reason?: string;
		GivenBy?: string;
		GivenTime?: string;
	};

	type CreateMarketplacePluginDto = {
		PluginId: string;
		Name: string;
		Description?: string;
		Version: string;
		Author?: string;
		Category?: string;
		SupportedLicenseMode?: SupportedLicenseModeDto;
		Price_SingleServer?: number;
		Price_MultiServer?: number;
		IsActive?: boolean;
		IsFree?: boolean;
		DownloadUrl?: string;
		DocumentationUrl?: string;
	};

	type CreateOnlinePaymentDto = {
		PurchaseId: number;
		Channel?: OnlinePaymentChannelDto;
		TransactionId: string;
		ThirdPartyData?: string;
		CallbackData?: string;
		PaidTime?: string;
	};

	type CreatePointRedemptionDto = {
		PurchaseId: number;
		PointsUsed?: number;
		UserId: string;
		RedeemedTime?: string;
	};

	type CreateRoleDto = {
		Name: string;
		Description?: string;
		Category: string;
	};

	type CreateUserPluginActivationDto = {
		PurchaseId: number;
		PluginId: string;
		UserId: string;
		LicenseType: LicenseTypeDto;
		ActivationDate?: string;
		MachineFingerprint: string;
		IpAddress?: string;
		Hostname?: string;
		LicenseFileContent?: string;
		Status?: ActivationStatusDto;
		DeactivationDate?: string;
		RevocationReason?: string;
		Notes?: string;
	};

	type CreateUserPluginPurchaseDto = {
		UserId: string;
		PluginId: string;
		OrderId: string;
		LicenseType: LicenseTypeDto;
		Quantity?: number;
		PurchasePrice?: number;
		Currency?: string;
		PurchaseDate?: string;
		UpdatesUntil?: string;
		IsLifetime?: boolean;
		Status?: PurchaseStatusDto;
		Notes?: string;
	};

	type deleteAdminCacheTestClearByTagParams = {
		tags?: string[];
	};

	type deleteAdminFdAdminUserIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdAdminUserRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdBlacklistsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdBlacklistsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdEmailConfigRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdMenuButtonsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdMenuButtonsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdMenuIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdMenuRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdPermissionsIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdPermissionsRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdRateLimitRulesIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteAdminFdRateLimitRulesRecyclebinIdPermanentParams = {
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

	type deleteCodeGenConfigIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteCodeGenConfigRecyclebinIdPermanentParams = {
		/** 要永久删除的记录的唯一标识符 */
		id: string;
	};

	type deleteCodeGenIdParams = {
		/** 要删除的记录的唯一标识符 */
		id: string;
	};

	type deleteCodeGenRecyclebinIdPermanentParams = {
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

	type FdAdminUserDto = {
		Id?: string;
		Username?: string;
		Name?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
		LastLoginTime?: string;
		LastLoginIp?: string;
		CreateTime?: string;
		Avatar?: string;
		RoleIds?: string[];
		Buttons?: string[];
	};

	type FdAdminUserDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdAdminUserDto[];
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
	};

	type FdBlacklistDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdBlacklistDto[];
	};

	type FdCodeGenBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdCodeGenConfigBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdCodeGenConfigDto = {
		Id?: string;
		CodeGenId?: number;
		ColumnName?: string;
		ColumnKey?: string;
		PropertyName?: string;
		ColumnLength?: number;
		ColumnComment?: string;
		DataType?: string;
		NetType?: string;
		DefaultValue?: string;
		EffectType?: string;
		FkConfigId?: string;
		FkEntityName?: string;
		FkTableName?: string;
		FkDisplayColumns?: string;
		FkLinkColumnName?: string;
		FkColumnNetType?: string;
		PidColumn?: string;
		DictTypeCode?: string;
		QueryType?: string;
		WhetherQuery?: string;
		WhetherRetract?: string;
		WhetherRequired?: string;
		WhetherSortable?: string;
		WhetherTable?: string;
		WhetherAddUpdate?: string;
		WhetherImport?: string;
		WhetherCommon?: string;
		OrderNo?: number;
		ShowColumnName?: string;
	};

	type FdCodeGenConfigDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdCodeGenConfigDto[];
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

	type FdEmailConfigDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdEmailConfigDto[];
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

	type FdMenuButtonDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdMenuButtonDto[];
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
		Children?: FdMenuDto[];
	};

	type FdMenuDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdMenuDto[];
	};

	type FdPermissionBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
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

	type FdPermissionDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdPermissionDto[];
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

	type FdUpdateEmailConfigDto = {
		Host: string;
		Port: number;
		Username: string;
		Password: string;
		SenderEmail: string;
		SenderName: string;
		EnableSsl?: boolean;
	};

	type FdUpdateEmailConfigDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: FdUpdateEmailConfigDto;
	};

	type GenerateLicenseRequestDto = {
		PluginId: string;
		MachineFingerprint: string;
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

	type getAdminFdAdminUserIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminFdAdminUserPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdAdminUserRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdBlacklistsCheckParams = {
		type?: string;
		value?: string;
	};

	type getAdminFdBlacklistsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminFdBlacklistsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdBlacklistsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdEmailConfigPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdEmailConfigRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdMenuButtonsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminFdMenuButtonsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdMenuButtonsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdMenuIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminFdMenuPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdMenuRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdPermissionsIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminFdPermissionsPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdPermissionsRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdRateLimitRulesByTypeAndKeyParams = {
		type?: string;
		key?: string;
	};

	type getAdminFdRateLimitRulesCheckParams = {
		type?: string;
		key?: string;
	};

	type getAdminFdRateLimitRulesIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getAdminFdRateLimitRulesPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getAdminFdRateLimitRulesRecyclebinParams = {
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

	type getCaptchaGenerateParams = {
		/** 验证码标识符，通常是用户会话ID或GUID */
		id?: string;
	};

	type getCodeGenColumnlistTableNameConfigIdParams = {
		/** 表名 */
		tableName: string;
		/** 配置ID */
		configId: string;
	};

	type getCodeGenConfigIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getCodeGenConfigPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getCodeGenConfigRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getCodeGenDownloadParams = {
		/** 文件路径 */
		filePath?: string;
	};

	type getCodeGenGetentitynameParams = {
		tableName?: string;
	};

	type getCodeGenGettablecolumnlistParams = {
		TableName?: string;
	};

	type getCodeGenIdParams = {
		/** 记录的唯一标识符 */
		id: string;
	};

	type getCodeGenPageParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getCodeGenPreviewConfigIdParams = {
		/** 代码生成配置ID */
		configId: string;
		/** 代码类型：entity, dto, service, controller, frontend */
		type?: string;
	};

	type getCodeGenRecyclebinParams = {
		/** 页码 (从1开始) */
		pageIndex?: number;
		/** 页面大小 */
		pageSize?: number;
	};

	type getCodeGenTablelistConfigIdParams = {
		/** 配置ID */
		configId: string;
	};

	type getPluginActivePluginIdParams = {
		pluginId: string;
	};

	type GiftRecordBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type GiftRecordDto = {
		Id?: string;
		PurchaseId?: number;
		Reason?: string;
		GivenBy?: string;
		GivenTime?: string;
		CreateTime?: string;
		UpdateTime?: string;
	};

	type GiftRecordDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: GiftRecordDto[];
	};

	enum LicenseTypeDto {
		0 = '0',
		1 = '1',
	}

	type LoginDto = {
		Username: string;
		Password: string;
		CaptchaId?: string;
		CaptchaCode?: string;
	};

	type LoginResultDto = {
		Token?: string;
	};

	type MarketplacePluginBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type MarketplacePluginDto = {
		Id?: string;
		PluginId: string;
		Name: string;
		Description?: string;
		Version: string;
		Author?: string;
		Category?: string;
		SupportedLicenseMode?: SupportedLicenseModeDto;
		Price_SingleServer?: number;
		Price_MultiServer?: number;
		IsActive?: boolean;
		IsFree?: boolean;
		DownloadUrl?: string;
		DocumentationUrl?: string;
		CreateTime?: string;
		UpdateTime?: string;
	};

	type MarketplacePluginDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: MarketplacePluginDto[];
	};

	enum MenuType {
		0 = '0',
		1 = '1',
	}

	type OfflineActivationRequestDto = {
		PluginId: string;
		MachineFingerprint: string;
		RequestTimestamp?: string;
		ClientInfo?: string;
		Nonce?: string;
	};

	type OnlinePaymentBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	enum OnlinePaymentChannelDto {
		0 = '0',
		1 = '1',
		2 = '2',
		3 = '3',
		4 = '4',
	}

	type OnlinePaymentDto = {
		Id?: string;
		PurchaseId?: number;
		Channel?: OnlinePaymentChannelDto;
		TransactionId?: string;
		ThirdPartyData?: string;
		CallbackData?: string;
		PaidTime?: string;
		CreateTime?: string;
		UpdateTime?: string;
	};

	type OnlinePaymentDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: OnlinePaymentDto[];
	};

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

	type PluginATestBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
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
		CreateTime?: string;
		UpdateTime?: string;
	};

	type PluginATestDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: PluginATestDto[];
	};

	type PluginATestUpdateDto = {
		Id?: string;
		Name?: string;
		Description?: string;
		TestValue?: number;
		IsEnabled?: boolean;
		Creator?: string;
	};

	type PluginATestUpdateDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: PluginATestUpdateDto;
	};

	type PluginConfig = {
		id?: string;
		name?: string;
		description?: string;
		version?: string;
		enabled?: boolean;
		author?: string;
		dependencies?: string[];
		tags?: string[];
		entryPoint?: string;
	};

	type PointRedemptionBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type PointRedemptionDto = {
		Id?: string;
		PurchaseId?: number;
		PointsUsed?: number;
		UserId?: string;
		RedeemedTime?: string;
	};

	type PointRedemptionDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: PointRedemptionDto[];
	};

	type postAdminCacheTestSetParams = {
		/** 缓存键 */
		key?: string;
		/** 缓存值 */
		value?: string;
	};

	type postAdminFdAdminUserIdResetPasswordParams = {
		id: string;
	};

	type postAdminRolesIdPermissionsParams = {
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

	enum PurchaseStatusDto {
		0 = '0',
		1 = '1',
		2 = '2',
	}

	type putAdminFdAdminUserIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdAdminUserRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdBlacklistsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdBlacklistsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdEmailConfigIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdEmailConfigRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdMenuButtonsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdMenuButtonsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdMenuIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdMenuRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdPermissionsIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdPermissionsRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdRateLimitRulesIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putAdminFdRateLimitRulesRecyclebinIdRestoreParams = {
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

	type putCodeGenConfigIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putCodeGenConfigRecyclebinIdRestoreParams = {
		/** 要恢复的记录的唯一标识符 */
		id: string;
	};

	type putCodeGenIdParams = {
		/** 要更新的记录的唯一标识符 */
		id: string;
	};

	type putCodeGenRecyclebinIdRestoreParams = {
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

	type SendMessageRequest = {
		Message?: string;
	};

	type SendRegistrationCodeDto = {
		Email: string;
	};

	enum SupportedLicenseModeDto {
		0 = '0',
		1 = '1',
		2 = '2',
	}

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

	type SystemConfigDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: SystemConfigDto;
	};

	type SystemConfigDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: SystemConfigDto[];
	};

	type TableInfoDto = {
		TableName?: string;
		EntityName?: string;
		TableComment?: string;
	};

	type TableUniqueConfigDto = {
		Columns: string[];
		Message?: string;
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

	type UpdateCodeGenDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateCodeGenDto;
	};

	type UpdateFdAdminUserDto = {
		Name?: string;
		Email?: string;
		Phone?: string;
		IsActive?: boolean;
		Avatar?: string;
	};

	type UpdateFdAdminUserDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAdminUserDto;
	};

	type UpdateFdBlacklistDto = {
		Type: string;
		Value: string;
		Reason?: string;
		ExpiredAt?: string;
	};

	type UpdateFdBlacklistDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdBlacklistDto;
	};

	type UpdateFdCodeGenConfigDto = {
		Id: number;
		CodeGenId: number;
		ColumnName: string;
		ColumnKey?: string;
		PropertyName: string;
		ColumnLength?: number;
		ColumnComment?: string;
		DataType?: string;
		NetType?: string;
		DefaultValue?: string;
		EffectType?: string;
		FkConfigId?: string;
		FkEntityName?: string;
		FkTableName?: string;
		FkDisplayColumns?: string;
		FkLinkColumnName?: string;
		FkColumnNetType?: string;
		PidColumn?: string;
		DictTypeCode?: string;
		QueryType?: string;
		WhetherQuery?: string;
		WhetherRetract?: string;
		WhetherRequired?: string;
		WhetherSortable?: string;
		WhetherTable?: string;
		WhetherAddUpdate?: string;
		WhetherImport?: string;
		WhetherCommon?: string;
		OrderNo?: number;
		ShowColumnName?: string;
	};

	type UpdateFdCodeGenConfigDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdCodeGenConfigDto;
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

	type UpdateFdFdMenuButtonDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdFdMenuButtonDto;
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
	};

	type UpdateFdMenuDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdMenuDto;
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

	type UpdateFdPermissionDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdPermissionDto;
	};

	type UpdateFdRateLimitRuleDto = {
		Type: string;
		Key: string;
		PermitLimit: number;
		WindowSeconds: number;
		Description?: string;
	};

	type UpdateFdRateLimitRuleDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdRateLimitRuleDto;
	};

	type UpdateGiftRecordDto = {
		Id?: string;
		PurchaseId?: number;
		Reason?: string;
		GivenBy?: string;
		GivenTime?: string;
	};

	type UpdateGiftRecordDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateGiftRecordDto;
	};

	type UpdateMarketplacePluginDto = {
		Id?: string;
		PluginId: string;
		Name: string;
		Description?: string;
		Version: string;
		Author?: string;
		Category?: string;
		SupportedLicenseMode?: SupportedLicenseModeDto;
		Price_SingleServer?: number;
		Price_MultiServer?: number;
		IsActive?: boolean;
		IsFree?: boolean;
		DownloadUrl?: string;
		DocumentationUrl?: string;
	};

	type UpdateMarketplacePluginDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateMarketplacePluginDto;
	};

	type UpdateOnlinePaymentDto = {
		Id?: string;
		PurchaseId?: number;
		Channel?: OnlinePaymentChannelDto;
		TransactionId?: string;
		ThirdPartyData?: string;
		CallbackData?: string;
		PaidTime?: string;
	};

	type UpdateOnlinePaymentDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateOnlinePaymentDto;
	};

	type UpdatePointRedemptionDto = {
		Id?: string;
		PurchaseId?: number;
		PointsUsed?: number;
		UserId?: string;
		RedeemedTime?: string;
	};

	type UpdatePointRedemptionDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdatePointRedemptionDto;
	};

	type UpdateRoleDto = {
		Name: string;
		Description?: string;
	};

	type UpdateRoleDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateRoleDto;
	};

	type UpdateUserPluginActivationDto = {
		Id?: string;
		PurchaseId?: number;
		PluginId?: string;
		UserId?: string;
		LicenseType?: LicenseTypeDto;
		ActivationDate?: string;
		MachineFingerprint?: string;
		IpAddress?: string;
		Hostname?: string;
		LicenseFileContent?: string;
		Status?: ActivationStatusDto;
		DeactivationDate?: string;
		RevocationReason?: string;
		Notes?: string;
	};

	type UpdateUserPluginActivationDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateUserPluginActivationDto;
	};

	type UpdateUserPluginPurchaseDto = {
		Id?: string;
		UserId?: string;
		PluginId?: string;
		OrderId?: string;
		LicenseType?: LicenseTypeDto;
		Quantity?: number;
		PurchasePrice?: number;
		Currency?: string;
		PurchaseDate?: string;
		UpdatesUntil?: string;
		IsLifetime?: boolean;
		Status?: PurchaseStatusDto;
		Notes?: string;
	};

	type UpdateUserPluginPurchaseDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateUserPluginPurchaseDto;
	};

	type UserPluginActivationBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type UserPluginActivationDto = {
		Id?: string;
		PurchaseId?: number;
		PluginId?: string;
		UserId?: string;
		LicenseType?: LicenseTypeDto;
		ActivationDate?: string;
		MachineFingerprint?: string;
		IpAddress?: string;
		Hostname?: string;
		LicenseFileContent?: string;
		Status?: ActivationStatusDto;
		DeactivationDate?: string;
		RevocationReason?: string;
		Notes?: string;
		CreateTime?: string;
		UpdateTime?: string;
	};

	type UserPluginActivationDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: UserPluginActivationDto[];
	};

	type UserPluginPurchaseBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type UserPluginPurchaseDto = {
		Id?: string;
		UserId?: string;
		PluginId?: string;
		OrderId?: string;
		LicenseType?: LicenseTypeDto;
		Quantity?: number;
		PurchasePrice?: number;
		Currency?: string;
		PurchaseDate?: string;
		UpdatesUntil?: string;
		IsLifetime?: boolean;
		Status?: PurchaseStatusDto;
		Notes?: string;
		CreateTime?: string;
		UpdateTime?: string;
	};

	type UserPluginPurchaseDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: UserPluginPurchaseDto[];
	};
}
