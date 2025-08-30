/**
 * 动态查询条件构建工具
 */

/**
 * 查询条件类型
 */
export type QueryCondition = {
  field: string;           // 字段名
  operator: string;        // 操作符
  value: any;              // 值
  prefix?: string;         // 前缀（如 ! 表示取反）
};

/**
 * 构建动态查询条件
 * @param conditions 查询条件数组
 * @returns { dynamicQuery: string, queryParameters: any[] } 动态查询对象
 */
export function buildDynamicQuery(conditions: QueryCondition[]): { 
  dynamicQuery: string; 
  queryParameters: any[] 
} {
  if (!conditions || conditions.length === 0) {
    return { dynamicQuery: '', queryParameters: [] };
  }

  const queryParts: string[] = [];
  const parameters: any[] = [];

  conditions.forEach((condition, index) => {
    const { field, operator, value, prefix = '' } = condition;
    
    // 根据操作符构建查询表达式
    let expression = '';
    switch (operator.toLowerCase()) {
      case 'equal':
      case 'eq':
        expression = `${field} == @${index}`;
        break;
      case 'notequal':
      case 'ne':
        expression = `${field} != @${index}`;
        break;
      case 'greaterthan':
      case 'gt':
        expression = `${field} > @${index}`;
        break;
      case 'greaterthanorequal':
      case 'gte':
        expression = `${field} >= @${index}`;
        break;
      case 'lessthan':
      case 'lt':
        expression = `${field} < @${index}`;
        break;
      case 'lessthanorequal':
      case 'lte':
        expression = `${field} <= @${index}`;
        break;
      case 'contains':
        expression = `${field}.Contains(@${index})`;
        break;
      case 'startswith':
        expression = `${field}.StartsWith(@${index})`;
        break;
      case 'endswith':
        expression = `${field}.EndsWith(@${index})`;
        break;
      default:
        // 支持自定义操作符
        expression = `${field} ${operator} @${index}`;
        break;
    }

    // 添加前缀（如 !）
    queryParts.push(`${prefix}${expression}`);
    parameters.push(value);
  });

  return {
    dynamicQuery: queryParts.join(' and '),
    queryParameters: parameters
  };
}

/**
 * 构建混合查询条件（支持多种类型的条件组合）
 * @param config 查询配置对象
 * @returns { dynamicQuery: string, queryParameters: any[] } 动态查询对象
 */
export function buildMixedQuery(config: {
  equals?: Record<string, any>;        // 等于条件
  contains?: Record<string, any>;      // 包含条件
  ranges?: Record<string, { from?: any, to?: any }>; // 范围条件
  customs?: QueryCondition[];          // 自定义条件
}): { 
  dynamicQuery: string; 
  queryParameters: any[] 
} {
  const conditions: QueryCondition[] = [];
  
  // 处理等于条件
  if (config.equals) {
    Object.keys(config.equals).forEach(field => {
      const value = config.equals![field];
      if (value !== undefined && value !== null && value !== '') {
        conditions.push({ field, operator: 'equal', value });
      }
    });
  }
  
  // 处理包含条件
  if (config.contains) {
    Object.keys(config.contains).forEach(field => {
      const value = config.contains![field];
      if (value !== undefined && value !== null && value !== '') {
        conditions.push({ field, operator: 'contains', value });
      }
    });
  }
  
  // 处理范围条件
  if (config.ranges) {
    Object.keys(config.ranges).forEach(field => {
      const range = config.ranges![field];
      if (range.from !== undefined && range.from !== null) {
        conditions.push({ 
          field, 
          operator: 'greaterthanorequal', 
          value: range.from 
        });
      }
      if (range.to !== undefined && range.to !== null) {
        conditions.push({ 
          field, 
          operator: 'lessthanorequal', 
          value: range.to 
        });
      }
    });
  }
  
  // 处理自定义条件
  if (config.customs) {
    conditions.push(...config.customs);
  }
  
  return buildDynamicQuery(conditions);
}