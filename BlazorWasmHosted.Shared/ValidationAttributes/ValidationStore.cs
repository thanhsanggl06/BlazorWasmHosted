//namespace BlazorWasmHosted.Shared.ValidationAttributes;

///// <summary>
///// Centralized validation data store
///// Qu?n l? cache cho t?t c? validation attributes
///// </summary>
//public static class ValidationStore
//{
//    private static readonly object _lock = new object();

//    // Dictionary đ? lưu nhi?u lo?i cache
//    // Key: Tên cache (VD: "SupplierIds", "CategoryIds", "ProductCodes", etc.)
//    // Value: HashSet ch?a data
//    private static readonly Dictionary<string, object> _caches = new Dictionary<string, object>();

//    /// <summary>
//    /// Set cache cho m?t lo?i validation
//    /// </summary>
//    public static void SetCache<T>(string cacheKey, IEnumerable<T> values)
//    {
//        lock (_lock)
//        {
//            _caches[cacheKey] = new HashSet<T>(values);
//        }
//    }

//    /// <summary>
//    /// Get cache theo key
//    /// </summary>
//    public static HashSet<T>? GetCache<T>(string cacheKey)
//    {
//        lock (_lock)
//        {
//            if (_caches.TryGetValue(cacheKey, out var cache) && cache is HashSet<T> typedCache)
//            {
//                return typedCache;
//            }
//            return null;
//        }
//    }

//    /// <summary>
//    /// Ki?m tra value có t?n t?i trong cache không
//    /// </summary>
//    public static bool Contains<T>(string cacheKey, T value)
//    {
//        lock (_lock)
//        {
//            var cache = GetCache<T>(cacheKey);
//            return cache?.Contains(value) ?? false;
//        }
//    }

//    /// <summary>
//    /// Ki?m tra cache đ? đư?c load chưa
//    /// </summary>
//    public static bool IsCacheLoaded(string cacheKey)
//    {
//        lock (_lock)
//        {
//            return _caches.ContainsKey(cacheKey);
//        }
//    }

//    /// <summary>
//    /// L?y s? lư?ng items trong cache
//    /// </summary>
//    public static int GetCacheCount(string cacheKey)
//    {
//        lock (_lock)
//        {
//            if (_caches.TryGetValue(cacheKey, out var cache))
//            {
//                if (cache is System.Collections.ICollection collection)
//                {
//                    return collection.Count;
//                }
//            }
//            return 0;
//        }
//    }

//    /// <summary>
//    /// Thêm 1 value vào cache
//    /// </summary>
//    public static void AddToCache<T>(string cacheKey, T value)
//    {
//        lock (_lock)
//        {
//            var cache = GetCache<T>(cacheKey);
//            if (cache != null)
//            {
//                cache.Add(value);
//            }
//            else
//            {
//                _caches[cacheKey] = new HashSet<T> { value };
//            }
//        }
//    }

//    /// <summary>
//    /// Xóa 1 value kh?i cache
//    /// </summary>
//    public static void RemoveFromCache<T>(string cacheKey, T value)
//    {
//        lock (_lock)
//        {
//            var cache = GetCache<T>(cacheKey);
//            cache?.Remove(value);
//        }
//    }

//    /// <summary>
//    /// Clear m?t cache c? th?
//    /// </summary>
//    public static void ClearCache(string cacheKey)
//    {
//        lock (_lock)
//        {
//            _caches.Remove(cacheKey);
//        }
//    }

//    /// <summary>
//    /// Clear toàn b? cache
//    /// </summary>
//    public static void ClearAllCaches()
//    {
//        lock (_lock)
//        {
//            _caches.Clear();
//        }
//    }

//    /// <summary>
//    /// L?y danh sách t?t c? cache keys
//    /// </summary>
//    public static IEnumerable<string> GetAllCacheKeys()
//    {
//        lock (_lock)
//        {
//            return _caches.Keys.ToList();
//        }
//    }
//}

using System.Collections.Immutable;

namespace BlazorWasmHosted.Shared.ValidationAttributes;

/// <summary>
/// Centralized validation data store (thread-safe, immutable-based)
/// Quản lý cache cho tất cả validation attributes.
/// </summary>
/// 

public record ValidationState(
    bool IsInitialized,
    string? LastReloadSource,
    DateTime LastReloadTime
);
public static class ValidationStore
{
    // Immutable snapshot của toàn bộ cache
    private static ImmutableDictionary<string, object> _caches = ImmutableDictionary<string, object>.Empty;

    private static ValidationState _state = new(false, null, default);

    public static ValidationState State => _state;

    /// <summary>
    /// Lấy cache hiện tại (snapshot)
    /// </summary>
    private static ImmutableDictionary<string, object> Snapshot => _caches;

    /// <summary>
    /// Set cache cho một loại validation (thay thế toàn bộ set cũ)
    /// </summary>
    public static void SetCache<T>(string cacheKey, IEnumerable<T> values)
    {
        var newSnapshot = Snapshot.SetItem(cacheKey, new HashSet<T>(values));
        Interlocked.Exchange(ref _caches, newSnapshot);
    }

    /// <summary>
    /// Get cache theo key
    /// </summary>
    public static HashSet<T>? GetCache<T>(string cacheKey)
    {
        if (Snapshot.TryGetValue(cacheKey, out var cache) && cache is HashSet<T> typedCache)
        {
            return typedCache;
        }
        return null;
    }

    /// <summary>
    /// Kiểm tra value có tồn tại trong cache không
    /// </summary>
    public static bool Contains<T>(string cacheKey, T value)
    {
        var cache = GetCache<T>(cacheKey);
        return cache?.Contains(value) ?? false;
    }

    /// <summary>
    /// Kiểm tra cache đã được load chưa
    /// </summary>
    public static bool IsCacheLoaded(string cacheKey)
    {
        return Snapshot.ContainsKey(cacheKey);
    }

    /// <summary>
    /// Lấy số lượng items trong cache
    /// </summary>
    public static int GetCacheCount(string cacheKey)
    {
        if (Snapshot.TryGetValue(cacheKey, out var cache))
        {
            if (cache is System.Collections.ICollection collection)
            {
                return collection.Count;
            }
        }
        return 0;
    }

    /// <summary>
    /// Thêm 1 value vào cache (tạo bản mới, không mutate cache cũ)
    /// </summary>
    public static void AddToCache<T>(string cacheKey, T value)
    {
        if (Snapshot.TryGetValue(cacheKey, out var cache) && cache is HashSet<T> typedCache)
        {
            var newSet = new HashSet<T>(typedCache) { value };
            var newSnapshot = Snapshot.SetItem(cacheKey, newSet);
            Interlocked.Exchange(ref _caches, newSnapshot);
        }
        else
        {
            var newSnapshot = Snapshot.SetItem(cacheKey, new HashSet<T> { value });
            Interlocked.Exchange(ref _caches, newSnapshot);
        }
    }

    /// <summary>
    /// Xóa 1 value khỏi cache (tạo bản mới)
    /// </summary>
    public static void RemoveFromCache<T>(string cacheKey, T value)
    {
        if (Snapshot.TryGetValue(cacheKey, out var cache) && cache is HashSet<T> typedCache)
        {
            if (typedCache.Contains(value))
            {
                var newSet = new HashSet<T>(typedCache);
                newSet.Remove(value);
                var newSnapshot = Snapshot.SetItem(cacheKey, newSet);
                Interlocked.Exchange(ref _caches, newSnapshot);
            }
        }
    }

    /// <summary>
    /// Clear một cache cụ thể
    /// </summary>
    public static void ClearCache(string cacheKey)
    {
        var newSnapshot = Snapshot.Remove(cacheKey);
        Interlocked.Exchange(ref _caches, newSnapshot);
    }

    /// <summary>
    /// Clear toàn bộ cache
    /// </summary>
    public static void ClearAllCaches()
    {
        Interlocked.Exchange(ref _caches, ImmutableDictionary<string, object>.Empty);
    }

    /// <summary>
    /// Lấy danh sách tất cả cache keys
    /// </summary>
    public static IEnumerable<string> GetAllCacheKeys()
    {
        return Snapshot.Keys.ToList();
    }


    /// <summary>
    /// Đánh dấu store đã được khởi tạo, cập nhật thông tin trạng thái đồng thời.
    /// </summary>
    /// <param name="source">Nguồn reload (ví dụ "Startup", "BackgroundJob")</param>
    public static void MarkInitialized(string? source = null)
    {
        var newState = new ValidationState(
            IsInitialized: true,
            LastReloadSource: source,
            LastReloadTime: DateTime.UtcNow
        );
        Interlocked.Exchange(ref _state, newState);
    }

    /// <summary>
    /// Đặt lại trạng thái khởi tạo về mặc định (chưa khởi tạo)
    /// </summary>
    public static void ResetInitialization()
    {
        var newState = new ValidationState(false, null, default);
        Interlocked.Exchange(ref _state, newState);
    }

    public static void UpdateIsInitialized(bool isInitialized)
    {
        ValidationState oldState, newState;
        do
        {
            oldState = _state;
            newState = oldState with { IsInitialized = isInitialized };
            // Cố gắng thay thế, nếu _state chưa đổi thì thành công
        } while (Interlocked.CompareExchange(ref _state, newState, oldState) != oldState);
    }

    public static void UpdateLastReloadSource(string? source)
    {
        ValidationState oldState, newState;
        do
        {
            oldState = _state;
            newState = oldState with { LastReloadSource = source };
        } while (Interlocked.CompareExchange(ref _state, newState, oldState) != oldState);
    }

    public static void UpdateLastReloadTime(DateTime time)
    {
        ValidationState oldState, newState;
        do
        {
            oldState = _state;
            newState = oldState with { LastReloadTime = time };
        } while (Interlocked.CompareExchange(ref _state, newState, oldState) != oldState);
    }
}


