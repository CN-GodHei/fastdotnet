/**
 * 插件加载器
 * 负责从后端API加载插件配置并注册到插件系统
 */

import { PluginMetadata } from './PluginManager';
import { pluginRegistry } from './PluginRegistry';
import { getApiPluginScan } from '@/api/fd-system-api-admin/Plugin';

export interface PluginLoadResult {
  success: boolean;
  message: string;
  pluginId?: string;
}

export class PluginLoader {
  private static instance: PluginLoader;

  private constructor() {}

  public static getInstance(): PluginLoader {
    if (!PluginLoader.instance) {
      PluginLoader.instance = new PluginLoader();
    }
    return PluginLoader.instance;
  }

  /**
   * 从后端API加载插件配置
   */
  public async loadPluginsFromAPI(): Promise<PluginLoadResult[]> {
    try {
      // 从后端获取插件列表
      const response: any = await getApiPluginScan();
      const plugins = response || [];

      console.log(`[PluginLoader] Found ${plugins.length} plugins from API`);

      const results: PluginLoadResult[] = [];

      for (const plugin of plugins) {
        const result = await this.registerPluginFromAPIResponse(plugin);
        results.push(result);
      }

      return results;
    } catch (error) {
      console.error('[PluginLoader] Error loading plugins from API:', error);
      return [{
        success: false,
        message: `Failed to load plugins from API: ${error}`
      }];
    }
  }

  /**
   * 从API响应数据注册插件
   */
  private async registerPluginFromAPIResponse(apiPlugin: any): Promise<PluginLoadResult> {
    try {
      // 转换API响应为插件元数据格式
      const metadata: PluginMetadata = {
        id: apiPlugin.id || apiPlugin.Id || apiPlugin.pluginId,
        name: apiPlugin.name || apiPlugin.Name || apiPlugin.displayName || 'Unknown Plugin',
        description: apiPlugin.description || apiPlugin.Description || '',
        version: apiPlugin.version || apiPlugin.Version || '1.0.0',
        author: apiPlugin.author || apiPlugin.Author || 'Unknown',
        enabled: apiPlugin.enabled ?? apiPlugin.Enabled ?? true,
        entryPoint: apiPlugin.entryPoint || apiPlugin.EntryPoint || 'index.html',
        dependencies: apiPlugin.dependencies || apiPlugin.Dependencies || [],
        permissions: apiPlugin.permissions || apiPlugin.Permissions || [],
        microAppConfig: apiPlugin.microAppConfig || {
          name: apiPlugin.id || apiPlugin.Id || apiPlugin.pluginId,
          entry: `${import.meta.env.VITE_API_URL}/plugins/${apiPlugin.id || apiPlugin.Id || apiPlugin.pluginId}/index.html`,
          container: '#subapp-viewport',
          activeRule: `/micro/${apiPlugin.id || apiPlugin.Id || apiPlugin.pluginId}`,
        }
      };

      // 注册插件
      const registered = pluginRegistry.registerPlugin(metadata);
      
      if (registered) {
        return {
          success: true,
          message: `Plugin ${metadata.name} registered successfully`,
          pluginId: metadata.id
        };
      } else {
        return {
          success: false,
          message: `Failed to register plugin ${metadata.name}`
        };
      }
    } catch (error) {
      console.error('[PluginLoader] Error registering plugin from API:', error);
      return {
        success: false,
        message: `Error processing plugin: ${error}`
      };
    }
  }

  /**
   * 动态加载单个插件
   */
  public async loadPluginById(pluginId: string): Promise<PluginLoadResult> {
    try {
      // 检查插件是否已在注册表中
      if (pluginRegistry.hasPlugin(pluginId)) {
        return {
          success: true,
          message: `Plugin ${pluginId} already registered`
        };
      }

      // 从API获取特定插件信息
      // 注意：这里需要一个获取单个插件的API，如果不存在则获取全部插件并查找
      const allPluginsResult = await this.loadPluginsFromAPI();
      const pluginResult = allPluginsResult.find(r => r.pluginId === pluginId);

      if (pluginResult) {
        return pluginResult;
      } else {
        return {
          success: false,
          message: `Plugin ${pluginId} not found in API response`
        };
      }
    } catch (error) {
      return {
        success: false,
        message: `Error loading plugin ${pluginId}: ${error}`
      };
    }
  }

  /**
   * 从URL加载远程插件配置
   */
  public async loadPluginFromUrl(url: string, pluginId: string): Promise<PluginLoadResult> {
    try {
      // 从指定URL获取插件配置
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const metadata: PluginMetadata = await response.json();

      // 确保ID匹配
      if (metadata.id !== pluginId) {
        console.warn(`[PluginLoader] Plugin ID mismatch: expected ${pluginId}, got ${metadata.id}`);
        metadata.id = pluginId; // 使用传入的ID
      }

      // 注册插件
      const registered = pluginRegistry.registerPlugin(metadata);

      if (registered) {
        return {
          success: true,
          message: `Plugin ${metadata.name} loaded from URL and registered successfully`,
          pluginId: metadata.id
        };
      } else {
        return {
          success: false,
          message: `Failed to register plugin from URL: ${metadata.name}`
        };
      }
    } catch (error) {
      console.error('[PluginLoader] Error loading plugin from URL:', error);
      return {
        success: false,
        message: `Error loading plugin from URL: ${error}`
      };
    }
  }

  /**
   * 从本地配置加载插件
   */
  public loadPluginFromConfig(metadata: PluginMetadata): PluginLoadResult {
    try {
      const registered = pluginRegistry.registerPlugin(metadata);

      if (registered) {
        return {
          success: true,
          message: `Plugin ${metadata.name} loaded from config and registered successfully`,
          pluginId: metadata.id
        };
      } else {
        return {
          success: false,
          message: `Failed to register plugin from config: ${metadata.name}`
        };
      }
    } catch (error) {
      console.error('[PluginLoader] Error loading plugin from config:', error);
      return {
        success: false,
        message: `Error loading plugin from config: ${error}`
      };
    }
  }

  /**
   * 获取插件加载统计
   */
  public getLoadStats() {
    return pluginRegistry.getStats();
  }
}

// 创建全局插件加载器实例
export const pluginLoader = PluginLoader.getInstance();

// 初始化时自动加载插件
export async function initializePlugins() {
  console.log('[PluginLoader] Starting plugin initialization...');
  const results = await pluginLoader.loadPluginsFromAPI();
  
  const successCount = results.filter(r => r.success).length;
  const totalCount = results.length;
  
  console.log(`[PluginLoader] Plugin initialization completed: ${successCount}/${totalCount} plugins loaded successfully`);
  
  // 输出当前激活的插件信息
  pluginRegistry.logActivePlugins();
  
  return results;
}