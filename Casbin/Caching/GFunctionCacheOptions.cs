using System;

namespace Casbin.Caching
{
    public class GFunctionCacheOptions
    {
        public TimeSpan WaitTimeOut { get; set; } = TimeSpan.FromMilliseconds(50);
    }
}
