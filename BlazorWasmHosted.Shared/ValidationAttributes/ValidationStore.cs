namespace BlazorWasmHosted.Shared.ValidationAttributes;

/// <summary>
/// Centralized validation data store
/// Qu?n l? cache cho t?t c? validation attributes
/// </summary>
public static class ValidationStore
{
    private static readonly object _lock = new object();
    
    // Dictionary ð? lýu nhi?u lo?i cache
    // Key: Tên cache (VD: "SupplierIds", "CategoryIds", "ProductCodes", etc.)
    // Value: HashSet ch?a data
    private static readonly Dictionary<string, object> _caches = new Dictionary<string, object>();
    
    /// <summary>
    /// Set cache cho m?t lo?i validation
    /// </summary>
    public static void SetCache<T>(string cacheKey, IEnumerable<T> values)
    {
        lock (_lock)
        {
            _caches[cacheKey] = new HashSet<T>(values);
        }
    }
    
    /// <summary>
    /// Get cache theo key
    /// </summary>
    public static HashSet<T>? GetCache<T>(string cacheKey)
    {
        lock (_lock)
        {
            if (_caches.TryGetValue(cacheKey, out var cache) && cache is HashSet<T> typedCache)
            {
                return typedCache;
            }
            return null;
        }
    }
    
    /// <summary>
    /// Ki?m tra value có t?n t?i trong cache không
    /// </summary>
    public static bool Contains<T>(string cacheKey, T value)
    {
        lock (_lock)
        {
            var cache = GetCache<T>(cacheKey);
            return cache?.Contains(value) ?? false;
        }
    }
    
    /// <summary>
    /// Ki?m tra cache ð? ðý?c load chýa
    /// </summary>
    public static bool IsCacheLoaded(string cacheKey)
    {
        lock (_lock)
        {
            return _caches.ContainsKey(cacheKey);
        }
    }
    
    /// <summary>
    /// L?y s? lý?ng items trong cache
    /// </summary>
    public static int GetCacheCount(string cacheKey)
    {
        lock (_lock)
        {
            if (_caches.TryGetValue(cacheKey, out var cache))
            {
                if (cache is System.Collections.ICollection collection)
                {
                    return collection.Count;
                }
            }
            return 0;
        }
    }
    
    /// <summary>
    /// Thêm 1 value vào cache
    /// </summary>
    public static void AddToCache<T>(string cacheKey, T value)
    {
        lock (_lock)
        {
            var cache = GetCache<T>(cacheKey);
            if (cache != null)
            {
                cache.Add(value);
            }
            else
            {
                _caches[cacheKey] = new HashSet<T> { value };
            }
        }
    }
    
    /// <summary>
    /// Xóa 1 value kh?i cache
    /// </summary>
    public static void RemoveFromCache<T>(string cacheKey, T value)
    {
        lock (_lock)
        {
            var cache = GetCache<T>(cacheKey);
            cache?.Remove(value);
        }
    }
    
    /// <summary>
    /// Clear m?t cache c? th?
    /// </summary>
    public static void ClearCache(string cacheKey)
    {
        lock (_lock)
        {
            _caches.Remove(cacheKey);
        }
    }
    
    /// <summary>
    /// Clear toàn b? cache
    /// </summary>
    public static void ClearAllCaches()
    {
        lock (_lock)
        {
            _caches.Clear();
        }
    }
    
    /// <summary>
    /// L?y danh sách t?t c? cache keys
    /// </summary>
    public static IEnumerable<string> GetAllCacheKeys()
    {
        lock (_lock)
        {
            return _caches.Keys.ToList();
        }
    }
}
