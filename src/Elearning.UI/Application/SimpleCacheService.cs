using Elearning.Shared.Commons.Interfaces.Extentions;
using Elearning.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.Extentions.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elearning.UI.Application
{
    // Very small in-memory/no-op cache implementation for development only
    public class SimpleCacheService : ICacheService
    {
        public string GenerateKey(object input, bool genMD5 = true) => input?.ToString() ?? string.Empty;

        public Task<T?> GetAsync<T>(RedisTypeKey type, string mainKeyword) => Task.FromResult<T?>(default);
        public Task<string?> GetStringAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult<string?>(null);
        public Task<bool> SetAsync<T>(RedisTypeKey type, string mainKeyword, T value, TimeSpan? expiry = null, bool useJitter = true) => Task.FromResult(true);
        public Task<bool> SetExactAsync<T>(RedisTypeKey type, string mainKeyword, T value, TimeSpan expiry) => Task.FromResult(true);

        public Task<double> IncrementAsync(RedisTypeKey type, string mainKeyword, double value = 1) => Task.FromResult(0.0);
        public Task<double> DecrementAsync(RedisTypeKey type, string mainKeyword, double value = 1) => Task.FromResult(0.0);

        public Task<List<T>> ListRangeAsync<T>(RedisTypeKey type, string mainKeyword, int start = 0, int stop = -1) => Task.FromResult(new List<T>());
        public Task<long> ListLeftPushAsync<T>(RedisTypeKey type, string mainKeyword, T item) => Task.FromResult(0L);
        public Task<long> ListRightPushAsync<T>(RedisTypeKey type, string mainKeyword, T item) => Task.FromResult(0L);
        public Task<T?> ListLeftPopAsync<T>(RedisTypeKey type, string mainKeyword) => Task.FromResult<T?>(default);
        public Task<T?> ListRightPopAsync<T>(RedisTypeKey type, string mainKeyword) => Task.FromResult<T?>(default);
        public Task<long> ListLengthAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult(0L);

        public Task<bool> HashSetAsync<T>(RedisTypeKey type, string mainKeyword, string hashField, T value) => Task.FromResult(true);
        public Task<T?> HashGetAsync<T>(RedisTypeKey type, string mainKeyword, string hashField) => Task.FromResult<T?>(default);
        public Task<Dictionary<string, T>> HashGetAllAsync<T>(RedisTypeKey type, string mainKeyword) => Task.FromResult(new Dictionary<string, T>());
        public Task<bool> HashDeleteAsync(RedisTypeKey type, string mainKeyword, string hashField) => Task.FromResult(true);
        public Task<long> HashLengthAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult(0L);
        public Task<string[]> HashKeysAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult(new string[0]);

        public Task<bool> SetAddAsync<T>(RedisTypeKey type, string mainKeyword, T item) => Task.FromResult(true);
        public Task<bool> SetRemoveAsync<T>(RedisTypeKey type, string mainKeyword, T item) => Task.FromResult(true);
        public Task<bool> SetContainsAsync<T>(RedisTypeKey type, string mainKeyword, T item) => Task.FromResult(false);
        public Task<long> SetLengthAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult(0L);
        public Task<HashSet<T>> SetMembersAsync<T>(RedisTypeKey type, string mainKeyword) => Task.FromResult(new HashSet<T>());

        public Task<T?> SetRandomMemberAsync<T>(RedisTypeKey type, string mainKeyword) => Task.FromResult<T?>(default);

        public Task<bool> SortedSetAddAsync<T>(RedisTypeKey type, string mainKeyword, T member, double score) => Task.FromResult(true);
        public Task<double?> SortedSetScoreAsync<T>(RedisTypeKey type, string mainKeyword, T member) => Task.FromResult<double?>(null);
        public Task<List<T>> SortedSetRangeByScoreAsync<T>(RedisTypeKey type, string mainKeyword, double start = double.NegativeInfinity, double stop = double.PositiveInfinity) => Task.FromResult(new List<T>());

        public Task PublishAsync<T>(string channel, T message) { return Task.CompletedTask; }
        public IDisposable Subscribe<T>(string channel, Action<T> handler) => new DummyDisposable();

        public Task<Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons.DataTableJson> GetKeysByPage(string key, int page = 1, int pageSize = 20, string? keyword = null, bool isPrefixKey = false)
            => Task.FromResult(new Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons.DataTableJson());

        public Task<bool> ExtendSessionIfNearExpiryAsync(RedisTypeKey type, string mainKeyword, TimeSpan threshold, TimeSpan extension) => Task.FromResult(true);

        public List<string> GetKeysByPrefix(string prefix, int count = 1000) => new List<string>();

        public Task<bool> FindAndDeleteAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult(true);
        public Task<bool> FullKeyDeleteAsync(string fullKeyword) => Task.FromResult(true);
        public Task<bool> KeyDeleteAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult(true);
        public bool KeyDelete(RedisTypeKey type, string mainKeyword) => true;
        public Task<bool> KeyExistsAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult(false);
        public bool KeyExists(RedisTypeKey type, string mainKeyword) => false;
        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisTypeKey type, string mainKeyword) => Task.FromResult<TimeSpan?>(null);
        public Task<bool> KeyExpireAsync(RedisTypeKey type, string mainKeyword, TimeSpan expiry, bool useJitter = true) => Task.FromResult(true);
        public Task<long> BatchDeleteAsync(IEnumerable<string> keys) => Task.FromResult(0L);
        public Task BatchSetAsync<T>(Dictionary<string, T> keyValues, TimeSpan? expiry = null, bool useJitter = true) => Task.CompletedTask;

        private class DummyDisposable : IDisposable { public void Dispose() { } }
    }
}
