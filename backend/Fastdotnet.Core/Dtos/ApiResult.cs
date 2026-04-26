namespace Fastdotnet.Core.Dtos
{
    /// <summary>
    /// 通用返回值类型（基类）
    /// </summary>
    public class ApiResult
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        /// <summary>
        /// 默认构造函数
        /// 修改为 protected：允许子类 ApiResult<T> 访问，但禁止外部直接 new ApiResult()
        /// </summary>
        protected ApiResult()
        {
        }

        /// <summary>
        /// 带参构造函数
        /// 同样修改为 protected
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        protected ApiResult(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }

        /// <summary>
        /// 成功返回（不带数据）
        /// </summary>
        /// <param name="msg">成功消息</param>
        /// <returns>ApiResult</returns>
        public static ApiResult Success(string msg = "success") => new ApiResult { Code = 200, Msg = msg };

        /// <summary>
        /// 失败返回
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="msg">错误信息</param>
        /// <returns>ApiResult</returns>
        public static ApiResult Fail(int code, string msg) => new ApiResult { Code = code, Msg = msg };
    }

    /// <summary>
    /// 不带数据的通用返回值类型
    /// 继承自 ApiResult
    /// </summary>
    public class ApiResult<T> : ApiResult
    {
        public T Data { get; set; }

        /// <summary>
        /// 默认构造函数
        /// 现在可以正常调用 base() 了
        /// </summary>
        public ApiResult()
        {
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        public ApiResult(int code, string msg)
            : base(code, msg)
        {
        }

        /// <summary>
        /// 成功返回（带数据）
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <returns>ApiResult<T></returns>
        public static ApiResult<T> Success(T data) => new ApiResult<T> { Code = 200, Msg = "success", Data = data };

        /// <summary>
        /// 成功返回（带数据和自定义消息）
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="msg">成功消息</param>
        /// <returns>ApiResult<T></returns>
        public static ApiResult<T> Success(T data, string msg) => new ApiResult<T> { Code = 200, Msg = msg, Data = data };

        /// <summary>
        /// 失败返回
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="msg">错误信息</param>
        /// <returns>ApiResult<T></returns>
        public static ApiResult<T> Fail(int code, string msg) => new ApiResult<T> { Code = code, Msg = msg, Data = default };
    }
}