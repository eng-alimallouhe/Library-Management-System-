using System.Text.Json.Serialization;
using LMS.Common.Enums;

namespace LMS.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }

        [JsonIgnore]
        public bool IsFailed { get; }
        
        public string StatusKey { get; }

        public static Result Success(string statusKey) => new Result(true, statusKey);
        public static Result Failure(string statusKey) => new Result(false, statusKey);

        protected Result(bool isSuccess, string statusKey)
        {
            IsSuccess = isSuccess;
            IsFailed = !isSuccess;
            StatusKey= statusKey;
        }
    }



    public class Result<TEntity> : Result
    {
        public TEntity? Value { get; }

        public static Result<TEntity> Success(TEntity value, string statusKey) => new Result<TEntity>(true, value, statusKey);
        public static new Result<TEntity> Failure(string statusKey) => new Result<TEntity>(false, default!, statusKey);

        private Result(bool isSuccess, TEntity? value, string statusKey) : base(isSuccess, statusKey)
        {
            Value = value;
        }
    }
}