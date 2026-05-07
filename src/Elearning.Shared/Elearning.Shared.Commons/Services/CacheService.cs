using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.Extentions.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ITransaction = Elearning.Shared.Commons.Interfaces.SQL.ITransaction;

namespace Elearning.Shared.Commons.Services
{
    public class CacheService : ICacheService, IDisposable
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _prefixKey;
        private readonly Random _random;

        public CacheService(string connectionString, string prefixKey)
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            options.ConnectRetry = 3;
            options.ConnectTimeout = 5000;
            options.SyncTimeout = 5000;

            _redis = ConnectionMultiplexer.Connect(options);
            _db = _redis.GetDatabase();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _prefixKey = prefixKey;
            _random = new Random();


        }

        /// <summary>
        /// Thêm jitter (0-30s) vào expiry để tránh cache stampede
        /// </summary>
        private TimeSpan AddJitter(TimeSpan expiry, int maxJitterSeconds = 30)
        {
            if (expiry == TimeSpan.Zero || expiry == TimeSpan.MaxValue)
                return expiry;

            var jitterSeconds = _random.Next(0, maxJitterSeconds + 1);
            return expiry.Add(TimeSpan.FromSeconds(jitterSeconds));
        }

        private string GetKey(RedisTypeKey type, string key, bool WithOutPrefixkey = false)
        {
            if (WithOutPrefixkey)
            {
                return $":{type}:{key}";
            }
            else
            {
                return $"{_prefixKey}:{type}:{key}";
            }
        }

        public string GenerateKey(object input, bool genMD5 = true)
        {
            string str = JsonSerializer.Serialize(input, _jsonOptions);
            if (genMD5)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                    return BitConverter.ToString(bHash).Replace("-", "").ToLower();
                }
            }
            return str;
        }

        #region String Operations

        public async Task<T?> GetAsync<T>(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);
            var value = await _db.StringGetAsync(key);
            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
        }

        public async Task<string?> GetStringAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            var value = await _db.StringGetAsync(key);
            return value.ToString();
        }

        public async Task<bool> SetAsync<T>(
            RedisTypeKey type,
            string mainKeyword,
            T value,
            TimeSpan? expiry = null,
            bool useJitter = true)
        {
            string key = GetKey(type, mainKeyword);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);

            var finalExpiry = expiry.HasValue && useJitter
                ? AddJitter(expiry.Value)
                : expiry;

            return await _db.StringSetAsync(key, serializedValue, (Expiration)finalExpiry);
        }
        /// <summary>
        /// Set với expiry cố định (không jitter) - cho các case đặc biệt
        /// </summary>
        public async Task<bool> SetExactAsync<T>(
            RedisTypeKey type,
            string mainKeyword,
            T value,
            TimeSpan expiry)
        {
            return await SetAsync(type, mainKeyword, value, expiry, useJitter: false);
        }

        #endregion

        #region Number Operations

        public async Task<double> IncrementAsync(RedisTypeKey type, string mainKeyword, double value = 1)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.StringIncrementAsync(key, value);
        }

        public async Task<double> DecrementAsync(RedisTypeKey type, string mainKeyword, double value = 1)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.StringDecrementAsync(key, value);
        }

        #endregion

        #region List Operations

        public async Task<List<T>> ListRangeAsync<T>(RedisTypeKey type, string mainKeyword, int start = 0, int stop = -1)
        {
            string key = GetKey(type, mainKeyword);

            var values = await _db.ListRangeAsync(key, start, stop);
            return values.Select(x => JsonSerializer.Deserialize<T>((string)x!, _jsonOptions)!).ToList();
        }

        public async Task<long> ListLeftPushAsync<T>(RedisTypeKey type, string mainKeyword, T item)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(item, _jsonOptions);
            return await _db.ListLeftPushAsync(key, serializedValue);
        }

        public async Task<long> ListRightPushAsync<T>(RedisTypeKey type, string mainKeyword, T item)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(item, _jsonOptions);
            return await _db.ListRightPushAsync(key, serializedValue);
        }

        public async Task<T?> ListLeftPopAsync<T>(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            var value = await _db.ListLeftPopAsync(key);
            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
        }

        public async Task<T?> ListRightPopAsync<T>(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            var value = await _db.ListRightPopAsync(key);
            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
        }

        public async Task<long> ListLengthAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.ListLengthAsync(key);
        }

        #endregion

        #region Hash Operations

        public async Task<bool> HashSetAsync<T>(RedisTypeKey type, string mainKeyword, string hashField, T value)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _db.HashSetAsync(key, hashField, serializedValue);
        }

        public async Task<T?> HashGetAsync<T>(RedisTypeKey type, string mainKeyword, string hashField)
        {
            string key = GetKey(type, mainKeyword);

            var value = await _db.HashGetAsync(key, hashField);
            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
        }

        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            var entries = await _db.HashGetAllAsync(key);
            return entries.ToDictionary(
                x => x.Name.ToString(),
                x => JsonSerializer.Deserialize<T>((string)x.Value!, _jsonOptions)!
            );
        }

        public async Task<bool> HashDeleteAsync(RedisTypeKey type, string mainKeyword, string hashField)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.HashDeleteAsync(key, hashField);
        }

        public async Task<long> HashLengthAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.HashLengthAsync(key);
        }

        public async Task<string[]> HashKeysAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            var values = await _db.HashKeysAsync(key);
            return values.Select(x => x.ToString()).ToArray();
        }

        #endregion

        #region Set Operations

        public async Task<bool> SetAddAsync<T>(RedisTypeKey type, string mainKeyword, T item)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(item, _jsonOptions);
            return await _db.SetAddAsync(key, serializedValue);
        }

        public async Task<bool> SetRemoveAsync<T>(RedisTypeKey type, string mainKeyword, T item)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(item, _jsonOptions);
            return await _db.SetRemoveAsync(key, serializedValue);
        }

        public async Task<bool> SetContainsAsync<T>(RedisTypeKey type, string mainKeyword, T item)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(item, _jsonOptions);
            return await _db.SetContainsAsync(key, serializedValue);
        }

        public async Task<long> SetLengthAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.SetLengthAsync(key);
        }

        public async Task<HashSet<T>> SetMembersAsync<T>(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            var values = await _db.SetMembersAsync(key);
            return values.Select(x => JsonSerializer.Deserialize<T>((string)x!, _jsonOptions)!).ToHashSet();
        }

        public async Task<T?> SetRandomMemberAsync<T>(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            var value = await _db.SetRandomMemberAsync(key);
            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
        }

        #endregion

        #region Sorted Set Operations

        public async Task<bool> SortedSetAddAsync<T>(RedisTypeKey type, string mainKeyword, T member, double score)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(member, _jsonOptions);
            return await _db.SortedSetAddAsync(key, serializedValue, score);
        }

        public async Task<double?> SortedSetScoreAsync<T>(RedisTypeKey type, string mainKeyword, T member)
        {
            string key = GetKey(type, mainKeyword);

            var serializedValue = JsonSerializer.Serialize(member, _jsonOptions);
            return await _db.SortedSetScoreAsync(key, serializedValue);
        }

        public async Task<List<T>> SortedSetRangeByScoreAsync<T>(RedisTypeKey type, string mainKeyword, double start = double.NegativeInfinity, double stop = double.PositiveInfinity)
        {
            string key = GetKey(type, mainKeyword);

            var values = await _db.SortedSetRangeByScoreAsync(key, start, stop);
            return values.Select(x => JsonSerializer.Deserialize<T>((string)x!, _jsonOptions)!).ToList();
        }

        #endregion

        #region Pub/Sub Operations

        public async Task PublishAsync<T>(string channel, T message)
        {
            var subscriber = _redis.GetSubscriber();
            var serializedMessage = JsonSerializer.Serialize(message, _jsonOptions);
            await subscriber.PublishAsync(new RedisChannel(channel, RedisChannel.PatternMode.Literal), serializedMessage);
        }

        public IDisposable Subscribe<T>(string channel, Action<T> handler)
        {
            var subscriber = _redis.GetSubscriber();
            subscriber.Subscribe(new RedisChannel(channel, RedisChannel.PatternMode.Literal), (_, value) =>
            {
                var message = JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
                handler(message!);
            });

            return new SubscriptionDisposable(subscriber, channel);
        }

        #endregion

        #region Key Operations


        /// <summary>
        /// GetKeysByPage - cải thiện performance
        /// </summary>
        public async Task<DataTableJson> GetKeysByPage(
            string key,
            int page = 1,
            int pageSize = 20,
            string? keyword = null,
            bool isPrefixKey = false)
        {
            var dataTableJson = new DataTableJson();
            var pattern = $"{_prefixKey}:{key}*";

            const int ScanPageSize = 100; // Scan từng batch nhỏ
            const int MaxKeysToScan = 5000; // Giảm xuống để tránh timeout

            var allKeys = new HashSet<RedisKey>(); // ⚡ Dùng HashSet tránh duplicate

            var endpoints = _redis.GetEndPoints();
            foreach (var ep in endpoints)
            {
                var server = _redis.GetServer(ep);
                if (!server.IsConnected || server.IsReplica)
                    continue;

                try
                {
                    // ⚡ Sử dụng cursor-based scanning
                    await foreach (var k in server.KeysAsync(
                        database: _db.Database,
                        pattern: pattern,
                        pageSize: ScanPageSize))
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            if (k.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                allKeys.Add(k);
                        }
                        else
                        {
                            allKeys.Add(k);
                        }

                        if (allKeys.Count >= MaxKeysToScan)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Log error nhưng không throw để có thể lấy từ server khác
                    Console.Error.WriteLine($"Error scanning keys from {ep}: {ex.Message}");
                }

                if (allKeys.Count >= MaxKeysToScan)
                    break;
            }

            // ⚡ Pagination trên memory (đã filter)
            var totalCount = allKeys.Count;
            var keysPage = allKeys
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ⚡ Batch get TTL thay vì gọi từng cái
            var ttlDict = await BatchGetTTLAsync(keysPage);

            var result = new List<RedisItemDto>();
            foreach (var k in keysPage)
            {
                var fullKey = k.ToString();
                var rawKey = fullKey.Replace($"{_prefixKey}:", string.Empty);
                var parts = rawKey.Split(':', 2);
                if (parts.Length < 2) continue;

                var prefix = parts[0];
                var nameKey = parts[1];

                if (!Enum.TryParse(prefix, true, out RedisTypeKey typeKey))
                    continue;

                // ⚡ Lấy TTL từ batch result
                var ttl = ttlDict.GetValueOrDefault(fullKey);

                // Filter logic (giữ nguyên)
                bool shouldInclude = false;
                if (key != null && typeKey.ToString() == key)
                {
                    if (isPrefixKey)
                    {
                        var redisKeyDefaults = typeof(RedisKeys)
                            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                            .Where(f => f.IsLiteral && !f.IsInitOnly)
                            .Select(x => x.GetRawConstantValue()?.ToString() ?? "")
                            .ToHashSet(); // ⚡ HashSet cho O(1) lookup

                        shouldInclude = redisKeyDefaults.Contains(nameKey);
                    }
                    else
                    {
                        shouldInclude = true;
                    }
                }

                if (shouldInclude)
                {
                    result.Add(new RedisItemDto
                    {
                        TypeKey = typeKey,
                        Key = nameKey,
                        TTL = ttl ?? TimeSpan.MaxValue
                    });
                }
            }

            dataTableJson.data = result;
            dataTableJson.recordsTotal = totalCount;
            dataTableJson.recordsFiltered = totalCount;
            return dataTableJson;
        }

        public async Task<bool> ExtendSessionIfNearExpiryAsync(RedisTypeKey type, string mainKeyword, TimeSpan threshold, TimeSpan extension)
        {
            string key = GetKey(type, mainKeyword);
            var luaScript = @"
                local ttl = redis.call('TTL', KEYS[1])
                if ttl == -2 then
                    return 0  -- key không tồn tại
                end
                if ttl == -1 then
                    return 0  -- key không có TTL
                end
                if ttl < tonumber(ARGV[1]) then
                    redis.call('EXPIRE', KEYS[1], tonumber(ARGV[2]))
                    return 1  -- đã extend
                end
                return 2  -- không cần extend
            ";

            var result = await _db.ScriptEvaluateAsync(luaScript,
                new RedisKey[] { key },
                new RedisValue[] { (long)threshold.TotalSeconds, (long)extension.TotalSeconds });

            return result.ToString() == "1";
        }

        public List<string> GetKeysByPrefix(string key, int count = 1000)
        {
            var keys = new List<string>();
            var endpoints = _redis.GetEndPoints(); // Lấy danh sách các endpoints của Redis

            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint); // Lấy đối tượng server

                if (server.IsConnected)
                {
                    var pattern = $"{_prefixKey}{key}*"; // Prefix đầy đủ bao gồm _prefixKey
                    var cursor = 0L;

                    do
                    {
                        var scanResult = server.Keys(
                           database: _db.Database,
                           pattern: pattern,
                           pageSize: count);

                        // Thêm tất cả các key từ scanResult vào danh sách keys
                        keys.AddRange(scanResult.Select(k => k.ToString()));

                    } while (cursor != 0);
                }
            }

            return keys;
        }
        public async Task<bool> FullKeyDeleteAsync(string fullKeyword)
        {

            return await _db.KeyDeleteAsync(fullKeyword);
        }

        public async Task<bool> KeyDeleteAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.KeyDeleteAsync(key);
        }

        public async Task<bool> FindAndDeleteAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword, true);
            List<string> keysFound = GetKeysByPrefix(key);

            if (keysFound.Count == 0)
                return false;

            // Chuyển danh sách keys thành mảng RedisKey
            RedisKey[] redisKeys = keysFound.Select(k => (RedisKey)k).ToArray();

            // Thực hiện xóa keys
            long deletedCount = await _db.KeyDeleteAsync(redisKeys);

            return deletedCount > 0;
        }

        public bool KeyDelete(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return _db.KeyDelete(key);
        }

        public async Task<bool> KeyExistsAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.KeyExistsAsync(key);
        }
        public bool KeyExists(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return _db.KeyExists(key);
        }

        public async Task<TimeSpan?> KeyTimeToLiveAsync(RedisTypeKey type, string mainKeyword)
        {
            string key = GetKey(type, mainKeyword);

            return await _db.KeyTimeToLiveAsync(key);
        }

        public async Task<bool> KeyExpireAsync(
            RedisTypeKey type,
            string mainKeyword,
            TimeSpan expiry,
            bool useJitter = true)
        {
            string key = GetKey(type, mainKeyword);
            var finalExpiry = useJitter ? AddJitter(expiry) : expiry;

            return await _db.KeyExpireAsync(key, finalExpiry);
        }

        /// <summary>
        /// Batch get TTL - giải quyết N+1 problem
        /// </summary>
        private async Task<Dictionary<string, TimeSpan?>> BatchGetTTLAsync(IEnumerable<RedisKey> keys)
        {
            var batch = _db.CreateBatch();
            var tasks = keys.Select(key => new
            {
                Key = key.ToString(),
                Task = batch.KeyTimeToLiveAsync(key)
            }).ToList();

            batch.Execute();

            var results = new Dictionary<string, TimeSpan?>();
            foreach (var item in tasks)
            {
                results[item.Key] = await item.Task;
            }

            return results;
        }

        public Task<RedisType> GetRedisTypeKey(RedisTypeKey type, string mainKeyword)
        {
            var key = GetKey(type, mainKeyword);

            return _db.KeyTypeAsync(key);
        }

        #endregion

        #region Transaction Support

        public ITransaction CreateTransaction()
        {
            return (ITransaction)_db.CreateTransaction();
        }

        #endregion

        #region Batch Operations

        public async Task<long> BatchDeleteAsync(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return await _db.KeyDeleteAsync(redisKeys);
        }

        public async Task BatchSetAsync<T>(
                Dictionary<string, T> keyValues,
                TimeSpan? expiry = null,
                bool useJitter = true)
        {
            if (keyValues == null || !keyValues.Any())
                return;

            // ⚡ Sử dụng batch thay vì transaction
            var batch = _db.CreateBatch();
            var tasks = new List<Task>();

            foreach (var kvp in keyValues)
            {
                var serializedValue = JsonSerializer.Serialize(kvp.Value, _jsonOptions);
                var finalExpiry = expiry.HasValue && useJitter
                    ? AddJitter(expiry.Value)
                    : expiry;

                tasks.Add(batch.StringSetAsync(kvp.Key, serializedValue, (Expiration)finalExpiry));
            }

            batch.Execute();
            await Task.WhenAll(tasks);
        }
        #endregion

        public void Dispose()
        {
            _redis?.Dispose();
        }


    }

    internal class SubscriptionDisposable : IDisposable
    {
        private readonly ISubscriber _subscriber;
        private readonly string _channel;

        public SubscriptionDisposable(ISubscriber subscriber, string channel)
        {
            _subscriber = subscriber;
            _channel = channel;
        }

        public void Dispose()
        {
            if (_channel is not null)
                _subscriber.Unsubscribe(new RedisChannel(_channel, RedisChannel.PatternMode.Literal));
        }
    }
}
