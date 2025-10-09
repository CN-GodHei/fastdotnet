using System;
using System.Collections.Generic;

namespace Fastdotnet.Core.Models
{
    /// <summary>
    /// 通用返回值类型
    /// </summary>
    /// <typeparam name="T">返回数据的类型</typeparam>
    public class ApiResult
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        public ApiResult()
        {
        }

        public ApiResult(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }
    }
    /// <summary>
    /// 不带数据的通用返回值类型
    /// </summary>
    public class ApiResult<T> : ApiResult
    {
        public T Data { get; set; }

        public ApiResult()
        {
        }

        public ApiResult(int code, string msg)
            : base(code, msg)
        {
        }

        //public ApiResult(int code, string msg, T data)
        //    : base(code, msg)
        //{
        //    Data = data;
        //}
        public static ApiResult<T> Success(T data) => new ApiResult<T> { Data = data };
    }
}