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

	type AppMarketplacePluginDto = {
		Id?: string;
		Name?: string;
		Description?: string;
		Author?: string;
		Category?: string;
		SupportedLicenseMode?: SupportedLicenseModeDto;
		Price_SingleServer?: number;
		Price_MultiServer?: number;
		IsFree?: boolean;
		DocumentationUrl?: string;
		PurchaseStatus?: string;
		SubTitle?: string;
	};

	type AppMarketplacePluginDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: AppMarketplacePluginDto[];
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

	type BooleanApiResult = {
		Data?: boolean;
		Code?: number;
		Msg?: string;
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
		Label?: string;
		Value?: string;
		Code?: string;
		Name: string;
		TagType: string;
		StyleSetting: string;
		ClassSetting: string;
		OrderNo?: number;
		Remark: string;
		ExtData: string;
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
		SubTitle?: string;
	};

	type CreateOnlinePaymentDto = {
		PurchaseId: number;
		Channel?: OnlinePaymentChannelDto;
		TransactionId: string;
		ThirdPartyData?: string;
		CallbackData?: string;
		PaidTime?: string;
	};

	type CreateOrderRequest = {
		OrderId?: string;
		Amount?: number;
		Description?: string;
		NotifyUrl?: string;
	};

	type CreatePaymentRequest = {
		OrderId?: string;
		Amount?: number;
		Subject?: string;
		NotifyUrl?: string;
		PaymentMethod?: PaymentMethod;
	};

	type CreatePluginAUserExtensionDto = {
		Preferences?: string;
		Points?: number;
	};

	type CreatePointRedemptionDto = {
		PurchaseId: number;
		PointsUsed?: number;
		UserId: string;
		RedeemedTime?: string;
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

	type deleteApiFdAppUserIdParams = {
		id: string;
	};

	type deleteApiStorage_openAPI_deleteFileNameParams = {
		/** 文件名 */
		fileName: string;
		/** 存储桶名称（可选） */
		bucketName?: string;
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
		CreatedAt?: string;
		Avatar?: string;
		RoleIds?: string[];
		Buttons?: string[];
	};

	type FdAdminUserDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdAdminUserDto[];
	};

	type FdAdminUserRoleBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdAdminUserRoleDto = {
		Id?: string;
		AdminUserId?: string;
		RoleId?: string;
	};

	type FdAdminUserRoleDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdAdminUserRoleDto[];
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

	type FdAppUserDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdAppUserDto[];
	};

	type FdAppUserRoleBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdAppUserRoleDto = {
		Id?: string;
		AppUserId?: string;
		RoleId?: string;
	};

	type FdAppUserRoleDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdAppUserRoleDto[];
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
		CreatedAt?: string;
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

	type FdDictDataBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdDictDataDto = {
		DictTypeId?: string;
		Label?: string;
		Value?: string;
		Code?: string;
		Name?: string;
		TagType?: string;
		StyleSetting?: string;
		ClassSetting?: string;
		OrderNo?: number;
		Remark?: string;
		ExtData?: string;
		Status?: number;
	};

	type FdDictDataDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdDictDataDto[];
	};

	type FdDictTypeBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
	};

	type FdDictTypeDto = {
		Name?: string;
		Code?: string;
		OrderNo?: number;
		Remark?: string;
		Status?: number;
		SysFlag?: number;
		PluginSysFlag?: number;
		PluginId?: string;
	};

	type FdDictTypeDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdDictTypeDto[];
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
		Title?: string;
		SupportWeb?: boolean;
		SupportDesktop?: boolean;
		SupportMobile?: boolean;
		Children?: FdMenuDto[];
		Creator?: UserRefDto;
		Updater?: UserRefDto;
		Deleter?: UserRefDto;
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

	type FdRoleDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdRoleDto[];
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

	type FdSystemInfoConfigDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: FdSystemInfoConfigDto[];
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

	type ForeignKeyConfigModel = {
		FkConfigId?: string;
		FkEntityName?: string;
		FkTableName?: string;
		FkDisplayColumnList?: string[];
		FkLinkColumnName?: string;
		FkColumnNetType?: string;
		CodeGenId?: string;
	};

	type GenerateLicenseRequestDto = {
		PluginId: string;
		MachineFingerprint: string;
	};

	type getApiCaptchaGenerateParams = {
		/** 验证码标识符，通常是用户会话ID或GUID */
		id?: string;
	};

	type getApiFdAppUserIdParams = {
		id: string;
	};

	type getApiFdAppUserPageParams = {
		pageIndex?: number;
		pageSize?: number;
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

	type getUploadsRelativePathParams = {
		/** 文件相对路径 */
		relativePath: string;
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
		SubTitle?: string;
	};

	type MarketplacePluginDtoPageResult = {
		PageInfo?: PageInfo;
		Items?: MarketplacePluginDto[];
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

	type MetricQueryRequest = {
		MetricIds?: string[];
		StartDate?: string;
		EndDate?: string;
		Dimensions?: string[];
		Filters?: Record<string, any>;
		PageNumber?: number;
		PageSize?: number;
	};

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

	enum PaymentMethod {
		0 = '0',
		1 = '1',
	}

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
		CreatedAt?: string;
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

	type PluginAUserExtension = {
		FdAppUserId?: string;
		Preferences?: string;
		Points?: number;
		Id?: string;
		CreatedAt?: string;
		UpdatedAt?: string;
		IsDeleted?: boolean;
		DeletedAt?: string;
	};

	type PluginInfo = {
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

	type postApiCaptchaValidateParams = {
		/** 验证码标识符 */
		id?: string;
		/** 用户输入的验证码 */
		code?: string;
	};

	type postApiStorageUploadParams = {
		/** 存储桶名称（可选） */
		bucketName?: string;
	};

	type PreCreateRequest = {
		OrderId?: string;
		Amount?: number;
		Subject?: string;
		NotifyUrl?: string;
	};

	enum PurchaseStatusDto {
		0 = '0',
		1 = '1',
		2 = '2',
	}

	type putApiFdAppUserIdParams = {
		id: string;
	};

	type QueryByConditionDto = {
		DynamicQuery?: string;
		QueryParameters?: any[];
		SelectFields?: string[];
	};

	type ResetPasswordDto = {
		NewPassword: string;
	};

	type SendMessageRequest = {
		Message?: string;
	};

	type SendRegistrationCodeDto = {
		Email: string;
	};

	type StorageConfigResponse = {
		StorageType?: string;
		DefaultBucket?: string;
		Domain?: string;
		SupportDirectUpload?: boolean;
		ConfigParams?: Record<string, any>;
	};

	enum SupportedLicenseModeDto {
		0 = '0',
		1 = '1',
		2 = '2',
	}

	enum SystemCategory {
		0 = '0',
		1 = '1',
	}

	type SystemInfoConfigBooleanFuncExpression = {
		Type?: string;
		NodeType?: ExpressionType;
		Parameters?: ParameterExpression[];
		Name?: string;
		Body?: Expression;
		ReturnType?: string;
		TailCall?: boolean;
		CanReduce?: boolean;
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

	type UpdateFdAdminUserRoleDto = {
		AdminUserId?: string;
		RoleId?: string;
	};

	type UpdateFdAdminUserRoleDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAdminUserRoleDto;
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

	type UpdateFdAppUserDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAppUserDto;
	};

	type UpdateFdAppUserRoleDto = {
		AppUserId?: string;
		RoleId?: string;
	};

	type UpdateFdAppUserRoleDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAppUserRoleDto;
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

	type UpdateFdCodeGenConfigDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdCodeGenConfigDto;
	};

	type UpdateFdDictDataDto = {
		DictTypeId?: string;
		Label?: string;
		Value?: string;
		Code?: string;
		Name?: string;
		TagType?: string;
		StyleSetting?: string;
		ClassSetting?: string;
		OrderNo?: number;
		Remark?: string;
		ExtData?: string;
		Status?: number;
	};

	type UpdateFdDictDataDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdDictDataDto;
	};

	type UpdateFdDictTypeDto = {
		Name?: string;
		Code?: string;
		OrderNo?: number;
		Remark?: string;
		Status?: number;
		SysFlag?: number;
		PluginSysFlag?: number;
		PluginId?: string;
	};

	type UpdateFdDictTypeDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdDictTypeDto;
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
		Title?: string;
		SupportWeb?: boolean;
		SupportDesktop?: boolean;
		SupportMobile?: boolean;
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

	type UpdateFdRoleDto = {
		Name: string;
		Description?: string;
	};

	type UpdateFdRoleDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdRoleDto;
	};

	type UpdateFdSystemInfoConfigDto = {
		Id?: string;
		Name?: string;
		Code?: string;
		Value?: string;
		Description?: string;
		IsSystem?: boolean;
	};

	type UpdateFdSystemInfoConfigDtoBatchUpdateByConditionDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdSystemInfoConfigDto;
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
		SubTitle?: string;
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

	type UserRefDto = {
		Id?: string;
		Name?: string;
	};
}
