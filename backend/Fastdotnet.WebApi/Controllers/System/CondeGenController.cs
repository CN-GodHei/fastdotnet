using Fastdotnet.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 代码生成相关接口
    /// </summary>
    [Route("api/[controller]")]
    public class CondeGenController : ControllerBase
    {
        protected readonly ISqlSugarClient _db;

        public CondeGenController(ISqlSugarClient db)
        {
            _db = db;
        }
        /// <summary>
        /// 获取库表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettablelist")]
        public async Task<List<DbTableInfo>> GetTableList()
        {
            var tables = _db.DbMaintenance.GetTableInfoList(false);//true 走缓存 false不走缓存
            //foreach (var table in tables)
            //{
            //    Console.WriteLine(table.Description);//输出表信息

            //    //获取列信息
            //    //var columns=db.DbMaintenance.GetColumnInfosByTableName("表名",false);
            //}
            return tables;
        }

        /// <summary>
        /// 获取表的列数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettablecolumnlist")]
        public async Task<List<DbColumnInfo>> GetTableColumnList(string TableName)
        {
            if (string.IsNullOrEmpty(TableName))
            {
                throw new BusinessException("表名不能为空");
            }
            var columns = _db.DbMaintenance.GetColumnInfosByTableName(TableName, false);

            //foreach (var table in columns)
            //{

            //    //获取列信息
            //    //var columns=db.DbMaintenance.GetColumnInfosByTableName("表名",false);
            //}
            return columns;
        }


    }
}
