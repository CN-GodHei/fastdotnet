/**
 * 应用级依赖注入缝（Dependency Seam）
 *
 * 背景：公共层里有些工具需要「获取加密公钥」这类能力，而它的实现依赖各应用自己的
 * API 客户端（`api/fd-system-api-admin` 与 `api/fd-system-api-app` 是两套生成代码）。
 *
 * 若公共层直接 `import('@/utils/encryption')`，就会把应用专属路径带进包里，
 * 「一份代码」的前提立刻被破坏。因此这里只暴露一个注册点：
 *
 *   - 公共层：调用 `getPublicKeyFetcher()` 拿能力，不知道实现是谁
 *   - 应用层：在入口（main.ts）调用 `registerPublicKeyFetcher()` 注入自己的实现
 *
 * 这样依赖方向是「应用 → 公共层」，公共层对应用零依赖。
 */

export type PublicKeyFetcher = (forceRefresh?: boolean) => Promise<string | null>;

let _publicKeyFetcher: PublicKeyFetcher | undefined;

/** 由应用入口注入公钥获取实现 */
export function registerPublicKeyFetcher(fetcher: PublicKeyFetcher): void {
	_publicKeyFetcher = fetcher;
}

/** 公共层内部使用：获取公钥获取器；未注册时给出明确指引而非静默失败 */
export function getPublicKeyFetcher(): PublicKeyFetcher {
	if (!_publicKeyFetcher) {
		throw new Error(
			'[@fastdotnet/shared] 尚未注册公钥获取器。请在应用入口（main.ts）中调用：\n' +
				"  import { registerPublicKeyFetcher } from '@fastdotnet/shared/runtime/publicKey';\n" +
				"  import { getEncryptionPublicKey } from '@/utils/encryption';\n" +
				'  registerPublicKeyFetcher(getEncryptionPublicKey);'
		);
	}
	return _publicKeyFetcher;
}
