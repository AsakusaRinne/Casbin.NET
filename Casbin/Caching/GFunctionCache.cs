using System.Collections.Generic;
using System.Threading;

namespace Casbin.Caching;

public class GFunctionCache : IGFunctionCache
{
    private readonly Dictionary<string, bool> _cache = new();
    private readonly ReaderWriterLockSlim _lockSlim = new();
    public GFunctionCacheOptions CacheOptions { get; } = new();

    public GFunctionCache(GFunctionCacheOptions options)
    {
        if (options is not null)
        {
            CacheOptions = options;
        }
    }

    public bool TrySet(string name1, string name2, bool result, string domain = null)
    {
        if (_lockSlim.TryEnterWriteLock(CacheOptions.WaitTimeOut) is false)
        {
            return false;
        }
        try
        {
            _cache[Key(name1, name2, domain)] = result;
            return true;
        }
        finally
        {
            _lockSlim.ExitWriteLock();
        }
    }

    public bool TryGet(string name1, string name2, out bool result, string domain = null)
    {
        if (_lockSlim.TryEnterReadLock(CacheOptions.WaitTimeOut) is false)
        {
            result = false;
            return false;
        }

        try
        {
            return _cache.TryGetValue(Key(name1, name2, domain), out result);
        }
        finally
        {
            _lockSlim.ExitReadLock();
        }
    }

    private static string Key(string name1, string name2, string domain = null)
    {
        bool hasDomain = domain is not null;
        return hasDomain
            ? string.Join(":", name1, name2, domain)
            : string.Join(":", name1, name2);
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
