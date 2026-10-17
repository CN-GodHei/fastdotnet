/**
 * @fastdotnet/shared —— Fastdotnet 前端公共层
 *
 * 说明：本包以「源码直连」方式被两个前端消费（无构建产物），
 * 因此这里只做汇总导出，不承载逻辑。
 *
 * 推荐按子路径精确导入，便于摇树：
 *   import { formatTime } from '@fastdotnet/shared/utils/formatTime';
 */

export * from './utils/formatTime';
export * from './utils/arrayOperation';
export * from './utils/commonFunction';
export * from './utils/storage';
export * from './utils/toolsValidate';
export * from './utils/queryBuilder';
export * from './utils/watermark';
export * from './utils/theme';
export * from './utils/loading';
export * from './utils/mitt';
export * from './utils/setIconfont';
export * from './utils/getStyleSheets';
export * from './utils/richTextCommunication';
export * from './utils/crypto-utils';
