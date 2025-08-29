// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 生成验证码图片 GET /api/Captcha/generate */
export async function getCaptchaGenerate(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCaptchaGenerateParams,
	options?: { [key: string]: any }
) {
	return request<string>('/api/Captcha/generate', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 验证验证码 (仅供测试使用)
在正常的登录流程中，验证码验证应在后端完成，而不是通过此接口。 POST /api/Captcha/validate */
export async function postCaptchaValidate(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postCaptchaValidateParams,
	options?: { [key: string]: any }
) {
	return request<boolean>('/api/Captcha/validate', {
		method: 'POST',
		params: {
			...params,
		},
		...(options || {}),
	});
}
