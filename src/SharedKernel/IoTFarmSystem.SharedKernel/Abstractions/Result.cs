using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.SharedKernel.Abstractions
{
    public record Result(bool Success, string? Error)
    {
        public static Result Ok() => new(true, null);
        public static Result Fail(string error) => new(false, error);
    }

    public record Result<T>(bool Success, T? Value, string? Error) : Result(Success, Error)
    {
        public static Result<T> Ok(T value) => new(true, value, null);
        public static new Result<T> Fail(string error) => new(false, default, error);
    }
}
