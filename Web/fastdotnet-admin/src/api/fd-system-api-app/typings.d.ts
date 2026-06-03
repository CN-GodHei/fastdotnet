declare namespace APIModel {
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

	type BatchUpdateByConditionDto1UpdateFdAppUserDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdAppUserDto;
	};

	type BatchUpdateByConditionDto1UpdateFdDictDataDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdDictDataDto;
	};

	type BatchUpdateByConditionDto1UpdateFdNoticeDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdNoticeDto;
	};

	type BatchUpdateByConditionDto1UpdateFdTodoTaskDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdTodoTaskDto;
	};

	type BatchUpdateByConditionDto1UpdateFdUserLayoutDto = {
		Query?: PageQueryByConditionDto;
		Dto?: UpdateFdUserLayoutDto;
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

	type CreateFdAppUserDto = {
		Username: string;
		Password: string;
		Email: string;
		PhoneNumber: string;
		Nickname?: string;
		AvatarUrl: string;
		Status?: number;
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

	type CreateFdNoticeDto = {
		ReceiverId: string;
		Title: string;
		Content?: string;
	};

	type CreateFdTodoTaskDto = {
		Title: string;
		BusinessId?: string;
		Route?: string;
		AssigneeId: string;
		WorkflowInstanceId?: string;
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

	type deleteAppGenericDtoControllerBase5DeleteParams = {
		id: string;
	};

	type deleteStorageDeleteParams = {
		/** 文件完整路径(支持相对路径,如: plugin-icons/20260425/xxx.png) */
		filePath?: string;
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

	type FdNoticeDto = {
		Id?: string;
		ReceiverId?: string;
		Title?: string;
		Content?: string;
		IsRead?: number;
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
		filePath: string;
	};

	type getStorageGetFileByPathParams = {
		/** 文件相对路径 */
		relativePath: string;
	};

	type getStorageGetFileUrlParams = {
		/** 文件完整路径 */
		filePath: string;
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

	type LoginDto = {
		Username: string;
		Password: string;
		CaptchaId?: string;
		CaptchaCode?: string;
	};

	type LoginResultDto = {
		Token?: string;
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

	type PageResult1FdAppUserDto = {
		PageInfo?: PageInfo;
		Items?: FdAppUserDto[];
	};

	type PageResult1FdDictDataDto = {
		PageInfo?: PageInfo;
		Items?: FdDictDataDto[];
	};

	type PageResult1FdNoticeDto = {
		PageInfo?: PageInfo;
		Items?: FdNoticeDto[];
	};

	type PageResult1FdTodoTaskDto = {
		PageInfo?: PageInfo;
		Items?: FdTodoTaskDto[];
	};

	type PageResult1FdUserLayoutDto = {
		PageInfo?: PageInfo;
		Items?: FdUserLayoutDto[];
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
		/** 存储路径前缀（可选），如：plugin-icons/、user-avatars/2024/01/ */
		pathPrefix?: string;
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

	type StorageConfigResponse = {
		StorageType?: string;
		DefaultBucket?: string;
		Domain?: string;
		SupportDirectUpload?: boolean;
		ConfigParams?: Record<string, any>;
	};

	type UnlockDto = {
		Password: string;
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

	type UpdateFdNoticeDto = {
		Id: string;
		IsRead?: number;
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

	type UploadCredentialRequest = {
		FileName: string;
		FileSize?: number;
		ContentType?: string;
		PathPrefix?: string;
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
