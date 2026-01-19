/**
 * 插件注册中心
 * 负责插件的注册、发现和元数据管理
 */

import { PluginMetadata } from './PluginManager';

export interface PluginRegistrationOptions {
  autoEnable?: boolean;
  validateDependencies?: boolean;
  requirePermissions?: string[];
}

export class PluginRegistry {
  private static instance: PluginRegistry;
  private plugins: Map<string, PluginMetadata> = new Map();
  private pluginSources: Map<string, string> = new Map(); // pluginId -> sourceUrl
  private categories: Map<string, string[]> = new Map(); // category -> pluginIds
  private dependencies: Map<string, string[]> = new Map(); // pluginId -> dependencyIds

  private constructor() {}

  public static getInstance(): PluginRegistry {
    if (!PluginRegistry.instance) {
      PluginRegistry.instance = new PluginRegistry();
    }
    return PluginRegistry.instance;
  }

  /**
   * 注册插件
   */
  public registerPlugin(
    metadata: PluginMetadata, 
    sourceUrl?: string,
    options: PluginRegistrationOptions = {}
  ): boolean {
    const { autoEnable = true, validateDependencies = true } = options;

    // 验证插件元数据
    if (!this.validatePluginMetadata(metadata)) {
      console.error(`[PluginRegistry] Invalid plugin metadata for ${metadata.id}`);
      return false;
    }

    // 验证依赖
    if (validateDependencies && !this.validateDependencies(metadata)) {
      console.error(`[PluginRegistry] Dependency validation failed for ${metadata.id}`);
      return false;
    }

    // 设置启用状态
    metadata.enabled = autoEnable;

    // 保存插件元数据
    this.plugins.set(metadata.id, metadata);

    // 保存来源URL
    if (sourceUrl) {
      this.pluginSources.set(metadata.id, sourceUrl);
    }

    // 注册分类
    this.registerToCategory(metadata);

    // 注册依赖关系
    this.registerDependencies(metadata);

    console.log(`[PluginRegistry] Plugin registered: ${metadata.name} (${metadata.id})`);
    return true;
  }

  /**
   * 批量注册插件
   */
  public registerMultiplePlugins(
    plugins: PluginMetadata[], 
    sourceUrls?: Record<string, string>,
    options: PluginRegistrationOptions = {}
  ): { success: string[], failed: string[] } {
    const result = { success: [] as string[], failed: [] as string[] };

    for (const metadata of plugins) {
      const sourceUrl = sourceUrls?.[metadata.id];
      const success = this.registerPlugin(metadata, sourceUrl, options);
      
      if (success) {
        result.success.push(metadata.id);
      } else {
        result.failed.push(metadata.id);
      }
    }

    return result;
  }

  /**
   * 验证插件元数据
   */
  private validatePluginMetadata(metadata: PluginMetadata): boolean {
    if (!metadata.id || !metadata.name || !metadata.version) {
      console.error('[PluginRegistry] Missing required plugin metadata fields');
      return false;
    }

    // 检查ID格式
    if (!/^[a-zA-Z0-9_-]+$/.test(metadata.id)) {
      console.error('[PluginRegistry] Invalid plugin ID format');
      return false;
    }

    // 检查版本号格式 (简单验证)
    if (!/^\d+\.\d+\.\d+$/.test(metadata.version)) {
      console.warn(`[PluginRegistry] Version format for ${metadata.id} might be invalid: ${metadata.version}`);
    }

    // 检查是否已存在同名插件
    if (this.plugins.has(metadata.id)) {
      console.warn(`[PluginRegistry] Plugin with ID ${metadata.id} already exists`);
      return false;
    }

    return true;
  }

  /**
   * 验证插件依赖
   */
  private validateDependencies(metadata: PluginMetadata): boolean {
    if (!metadata.dependencies || metadata.dependencies.length === 0) {
      return true; // 没有依赖，验证通过
    }

    for (const depId of metadata.dependencies) {
      if (!this.plugins.has(depId)) {
        console.error(`[PluginRegistry] Dependency not found: ${depId} for plugin ${metadata.id}`);
        return false;
      }

      const depMeta = this.plugins.get(depId)!;
      if (!depMeta.enabled) {
        console.error(`[PluginRegistry] Dependency is disabled: ${depId} for plugin ${metadata.id}`);
        return false;
      }
    }

    return true;
  }

  /**
   * 注册到分类
   */
  private registerToCategory(metadata: PluginMetadata): void {
    // 这里可以根据插件的特性自动分类，或使用插件提供的分类信息
    const category = metadata.description.includes('editor') ? 'editor' : 'general';
    
    if (!this.categories.has(category)) {
      this.categories.set(category, []);
    }
    
    const categoryList = this.categories.get(category)!;
    if (!categoryList.includes(metadata.id)) {
      categoryList.push(metadata.id);
    }
  }

  /**
   * 注册依赖关系
   */
  private registerDependencies(metadata: PluginMetadata): void {
    if (metadata.dependencies && metadata.dependencies.length > 0) {
      this.dependencies.set(metadata.id, [...metadata.dependencies]);
    }
  }

  /**
   * 获取插件元数据
   */
  public getPluginMetadata(pluginId: string): PluginMetadata | undefined {
    return this.plugins.get(pluginId);
  }

  /**
   * 获取插件来源URL
   */
  public getPluginSource(pluginId: string): string | undefined {
    return this.pluginSources.get(pluginId);
  }

  /**
   * 获取所有插件
   */
  public getAllPlugins(): PluginMetadata[] {
    return Array.from(this.plugins.values());
  }

  /**
   * 根据条件搜索插件
   */
  public searchPlugins(
    filterFn: (plugin: PluginMetadata) => boolean
  ): PluginMetadata[] {
    return this.getAllPlugins().filter(filterFn);
  }

  /**
   * 根据分类获取插件
   */
  public getPluginsByCategory(category: string): PluginMetadata[] {
    const pluginIds = this.categories.get(category) || [];
    return pluginIds
      .map(id => this.plugins.get(id))
      .filter(Boolean) as PluginMetadata[];
  }

  /**
   * 获取所有分类
   */
  public getAllCategories(): string[] {
    return Array.from(this.categories.keys());
  }

  /**
   * 获取插件的依赖项
   */
  public getDependencies(pluginId: string): string[] {
    return this.dependencies.get(pluginId) || [];
  }

  /**
   * 获取依赖于指定插件的插件列表
   */
  public getDependents(pluginId: string): string[] {
    const dependents: string[] = [];
    
    for (const [depId, deps] of this.dependencies.entries()) {
      if (deps.includes(pluginId)) {
        dependents.push(depId);
      }
    }
    
    return dependents;
  }

  /**
   * 检查插件是否存在
   */
  public hasPlugin(pluginId: string): boolean {
    return this.plugins.has(pluginId);
  }

  /**
   * 检查插件是否启用
   */
  public isPluginEnabled(pluginId: string): boolean {
    const metadata = this.plugins.get(pluginId);
    return !!metadata && metadata.enabled;
  }

  /**
   * 启用插件
   */
  public enablePlugin(pluginId: string): boolean {
    const metadata = this.plugins.get(pluginId);
    if (!metadata) {
      return false;
    }

    metadata.enabled = true;
    return true;
  }

  /**
   * 禁用插件
   */
  public disablePlugin(pluginId: string): boolean {
    const metadata = this.plugins.get(pluginId);
    if (!metadata) {
      return false;
    }

    // 检查是否有其他插件依赖于此插件
    const dependents = this.getDependents(pluginId);
    if (dependents.length > 0) {
      console.warn(`[PluginRegistry] Plugin ${pluginId} has dependents: ${dependents.join(', ')}`);
      // 可以选择抛出错误或允许禁用
      // throw new Error(`Cannot disable plugin ${pluginId} as it has dependents: ${dependents.join(', ')}`);
    }

    metadata.enabled = false;
    return true;
  }

  /**
   * 卸载插件（从注册表中移除）
   */
  public unregisterPlugin(pluginId: string): boolean {
    // 检查是否有其他插件依赖于此插件
    const dependents = this.getDependents(pluginId);
    if (dependents.length > 0) {
      console.error(`[PluginRegistry] Cannot unregister plugin ${pluginId} as it has dependents: ${dependents.join(', ')}`);
      return false;
    }

    const success = this.plugins.delete(pluginId);
    if (success) {
      this.pluginSources.delete(pluginId);
      
      // 从分类中移除
      for (const [category, pluginIds] of this.categories.entries()) {
        const index = pluginIds.indexOf(pluginId);
        if (index > -1) {
          pluginIds.splice(index, 1);
        }
      }

      // 移除依赖关系
      this.dependencies.delete(pluginId);

      console.log(`[PluginRegistry] Plugin unregistered: ${pluginId}`);
    }

    return success;
  }

  /**
   * 更新插件元数据
   */
  public updatePluginMetadata(pluginId: string, updates: Partial<PluginMetadata>): boolean {
    const metadata = this.plugins.get(pluginId);
    if (!metadata) {
      return false;
    }

    // 不能更新ID
    if (updates.id && updates.id !== pluginId) {
      console.error('[PluginRegistry] Cannot update plugin ID');
      return false;
    }

    Object.assign(metadata, updates);
    return true;
  }

  /**
   * 获取插件统计信息
   */
  public getStats(): {
    total: number;
    enabled: number;
    disabled: number;
    categories: number;
  } {
    const allPlugins = this.getAllPlugins();
    const enabled = allPlugins.filter(p => p.enabled).length;
    
    return {
      total: allPlugins.length,
      enabled,
      disabled: allPlugins.length - enabled,
      categories: this.getAllCategories().length
    };
  }

  /**
   * 获取并打印所有激活的插件信息
   */
  public logActivePlugins(): void {
    const allPlugins = this.getAllPlugins();
    const activePlugins = allPlugins.filter(p => p.enabled);
    console.log(activePlugins)
    const inactivePlugins = allPlugins.filter(p => !p.enabled);
    
    console.group('[PluginRegistry] Active Plugins Information');
    console.log(`Total plugins: ${allPlugins.length}`);
    console.log(`Enabled plugins: ${activePlugins.length}`);
    console.log(`Disabled plugins: ${inactivePlugins.length}`);
    
    console.group('Enabled Plugins:');
    activePlugins.forEach(plugin => {
      console.log(`- ${plugin.name} (${plugin.id}): v${plugin.version}`);
      if (plugin.description) {
        console.log(`  Description: ${plugin.description}`);
      }
      if (plugin.dependencies && plugin.dependencies.length > 0) {
        console.log(`  Dependencies: ${plugin.dependencies.join(', ')}`);
      }
      if (plugin.microAppConfig) {
        console.log(`  Entry: ${plugin.microAppConfig.entry}`);
        console.log(`  Active Rule: ${plugin.microAppConfig.activeRule}`);
      }
      console.log('');
    });
    console.groupEnd();
    
    if (inactivePlugins.length > 0) {
      console.group('Disabled Plugins:');
      inactivePlugins.forEach(plugin => {
        console.log(`- ${plugin.name} (${plugin.id}): v${plugin.version}`);
      });
      console.groupEnd();
    }
    
    console.groupEnd();
  }
}

// 创建全局插件注册中心实例
export const pluginRegistry = PluginRegistry.getInstance();