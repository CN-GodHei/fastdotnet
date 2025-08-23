import { registerMicroApps, start } from 'qiankun';

console.log('[Base App] Starting...');

registerMicroApps([
  {
    name: 'pluginc', // 微应用的唯一名称
    entry: '//localhost:9001', // 微应用的入口地址
    container: '#micro-container', // 容器选择器
    activeRule: '/', // 当URL为 / 时激活
  },
]);

start();

console.log('[Base App] Qiankun started.');
