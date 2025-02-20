namespace Fastdotnet.Core.Models
{
    /// <summary>
    /// 通用返回值类型
    /// </summary>
    /// <typeparam name="T">返回数据的类型</typeparam>
    public class CommonResult<T>
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 返回的消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回的数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 创建一个成功的返回结果
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <param name="msg">返回的消息</param>
        /// <returns>成功的返回结果</returns>
        public static CommonResult<T> Success(T data = default, string msg = "操作成功")
        {
            return new CommonResult<T>
            {
                Result = true,
                Msg = msg,
                Data = data
            };
        }

        /// <summary>
        /// 创建一个失败的返回结果
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <returns>失败的返回结果</returns>
        public static CommonResult<T> Error(string msg = "操作失败")
        {
            return new CommonResult<T>
            {
                Result = false,
                Msg = msg,
                Data = default
            };
        }
    }

    /// <summary>
    /// 不带数据的通用返回值类型
    /// </summary>
    public class CommonResult : CommonResult<object>
    {
        /// <summary>
        /// 创建一个成功的返回结果
        /// </summary>
        /// <param name="msg">返回的消息</param>
        /// <returns>成功的返回结果</returns>
        public static new CommonResult Success(string msg = "操作成功")
        {
            return new CommonResult
            {
                Result = true,
                Msg = msg
            };
        }

        /// <summary>
        /// 创建一个失败的返回结果
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <returns>失败的返回结果</returns>
        public static new CommonResult Error(string msg = "操作失败")
        {
            return new CommonResult
            {
                Result = false,
                Msg = msg
            };
        }
    }
}