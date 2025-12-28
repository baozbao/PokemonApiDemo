using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApi.Service.Common
{
    // Non-Generic Result for no returned enetity methods
    public class Result
    {
        public bool Success { get; }
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }

        protected Result(
            bool success, string? errorCode, string? errorMessage) 
        {
            Success = success;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        // Success factory 
        public static Result OK() => new Result(true, null, null);

        // Failed factory
        public static Result Fail(string errorCode, string errorMessage)
                => new Result(false, errorCode, errorMessage);
    }

    // Non-Generic Result for no returned enetity methods
    public class Result<T> : Result
    {
        public T? Data { get; }

        // 在子类Constructor，调用父类Constructor，并把参数传给它，让父类自己去初始化它那一部分success, errorCode, errorMessage
        // 子类在父类基础上，多了一个 Data 属性 它的构造函数有 4 个参数：success，errorCode，errorMessage， data
        // 前 3 个是父类需要的信息 第 4 个 data 是子类自己用

        private Result(
            bool success, string? errorCode, string? errorMessage, T? data)
                : base(success, errorCode, errorMessage)
        {
            Data = data;
        }

        // Success factory 
        public static Result<T> OK(T data) =>
            new Result<T>(true, null, null, data);

        // Failed factory
        // This method will "Hide" the Result Fail since they have same name, same param
        // That's why it showed a warning
        //     Result Fail(string errorCode, string errorMessage)
        // ex: Result<T> Fail(string errorCode, string errorMessage)
        public static Result<T> Fail(string errorCode, string errorMessage)
                => new Result<T>(false, errorCode, errorMessage,default);
    }
}
