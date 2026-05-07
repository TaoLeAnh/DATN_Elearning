using Elearning.Shared.Commons.Model.Commons.Service.Shared.Commons.Model.Commons;
using Elearning.Shared.Commons.Model.Extentions.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Commons.Interfaces.Extentions
{
    public interface ICacheService
    {

        string GenerateKey(object input, bool genMD5 = true);
        #region String Operations
        /// <summary>
        /// Lấy giá trị theo key với kiểu dữ liệu tùy chỉnh
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của giá trị cần lấy</typeparam>
        /// <param name="type">Loại Redis key</param>
        /// <param name="mainKeyword">Từ khóa chính của key</param>
        /// <returns>Giá trị được deserialized hoặc null nếu không tồn tại</returns>
        Task<T?> GetAsync<T>(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lấy giá trị string theo key
        /// </summary>
        /// <param name="type">Loại Redis key</param>
        /// <param name="mainKeyword">Từ khóa chính của key</param>
        /// <returns>Giá trị string hoặc null nếu không tồn tại</returns>
        Task<string?> GetStringAsync(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lưu giá trị vào Redis với key xác định kèm khả năng hết hạn
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của giá trị cần lưu</typeparam>
        /// <param name="type">Loại Redis key</param>
        /// <param name="mainKeyword">Từ khóa chính của key</param>
        /// <param name="value">Giá trị cần lưu</param>
        /// <param name="expiry">Thời gian hết hạn (tùy chọn)</param>
        Task<bool> SetAsync<T>(RedisTypeKey type, string mainKeyword, T value, TimeSpan? expiry = null, bool useJitter = true);

        /// <summary>
        /// Lưu giá trị vào Redis với key xác định
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="mainKeyword"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<bool> SetExactAsync<T>(RedisTypeKey type, string mainKeyword, T value, TimeSpan expiry);
        #endregion

        #region Number Operations
        /// <summary>
        /// Tăng giá trị số theo key
        /// </summary>
        /// <param name="type">Loại Redis key</param>
        /// <param name="mainKeyword">Từ khóa chính của key</param>
        /// <param name="value">Giá trị tăng (mặc định là 1)</param>
        Task<double> IncrementAsync(RedisTypeKey type, string mainKeyword, double value = 1);

        /// <summary>
        /// Giảm giá trị số theo key
        /// </summary>
        /// <param name="type">Loại Redis key</param>
        /// <param name="mainKeyword">Từ khóa chính của key</param>
        /// <param name="value">Giá trị giảm (mặc định là 1)</param>
        Task<double> DecrementAsync(RedisTypeKey type, string mainKeyword, double value = 1);
        #endregion

        #region List Operations
        /// <summary>
        /// Lấy danh sách các phần tử trong một khoảng của List
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của các phần tử</typeparam>
        /// <param name="type">Loại Redis key</param>
        /// <param name="mainKeyword">Từ khóa chính của key</param>
        /// <param name="start">Vị trí bắt đầu</param>
        /// <param name="stop">Vị trí kết thúc</param>
        Task<List<T>> ListRangeAsync<T>(RedisTypeKey type, string mainKeyword, int start = 0, int stop = -1);

        /// <summary>
        /// Thêm phần tử vào đầu List
        /// </summary>
        Task<long> ListLeftPushAsync<T>(RedisTypeKey type, string mainKeyword, T item);

        /// <summary>
        /// Thêm phần tử vào cuối List
        /// </summary>
        Task<long> ListRightPushAsync<T>(RedisTypeKey type, string mainKeyword, T item);

        /// <summary>
        /// Lấy và xóa phần tử đầu tiên của List
        /// </summary>
        Task<T?> ListLeftPopAsync<T>(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lấy và xóa phần tử cuối cùng của List
        /// </summary>
        Task<T?> ListRightPopAsync<T>(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lấy độ dài của List
        /// </summary>
        Task<long> ListLengthAsync(RedisTypeKey type, string mainKeyword);
        #endregion

        #region Hash Operations
        /// <summary>
        /// Lưu giá trị vào Hash field
        /// </summary>
        Task<bool> HashSetAsync<T>(RedisTypeKey type, string mainKeyword, string hashField, T value);

        /// <summary>
        /// Lấy giá trị từ Hash field
        /// </summary>
        Task<T?> HashGetAsync<T>(RedisTypeKey type, string mainKeyword, string hashField);

        /// <summary>
        /// Lấy tất cả các cặp key-value trong Hash
        /// </summary>
        Task<Dictionary<string, T>> HashGetAllAsync<T>(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Xóa một field trong Hash
        /// </summary>
        Task<bool> HashDeleteAsync(RedisTypeKey type, string mainKeyword, string hashField);

        /// <summary>
        /// Lấy số lượng field trong Hash
        /// </summary>
        Task<long> HashLengthAsync(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lấy danh sách các field trong Hash
        /// </summary>
        Task<string[]> HashKeysAsync(RedisTypeKey type, string mainKeyword);
        #endregion

        #region Set Operations
        /// <summary>
        /// Thêm phần tử vào Set
        /// </summary>
        Task<bool> SetAddAsync<T>(RedisTypeKey type, string mainKeyword, T item);

        /// <summary>
        /// Xóa phần tử khỏi Set
        /// </summary>
        Task<bool> SetRemoveAsync<T>(RedisTypeKey type, string mainKeyword, T item);

        /// <summary>
        /// Kiểm tra phần tử có tồn tại trong Set
        /// </summary>
        Task<bool> SetContainsAsync<T>(RedisTypeKey type, string mainKeyword, T item);

        /// <summary>
        /// Lấy số lượng phần tử trong Set
        /// </summary>
        Task<long> SetLengthAsync(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lấy tất cả các phần tử trong Set
        /// </summary>
        Task<HashSet<T>> SetMembersAsync<T>(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lấy ngẫu nhiên một phần tử từ Set
        /// </summary>
        Task<T?> SetRandomMemberAsync<T>(RedisTypeKey type, string mainKeyword);
        #endregion

        #region Sorted Set Operations
        /// <summary>
        /// Thêm phần tử vào Sorted Set với điểm số
        /// </summary>
        Task<bool> SortedSetAddAsync<T>(RedisTypeKey type, string mainKeyword, T member, double score);

        /// <summary>
        /// Lấy điểm số của phần tử trong Sorted Set
        /// </summary>
        Task<double?> SortedSetScoreAsync<T>(RedisTypeKey type, string mainKeyword, T member);

        /// <summary>
        /// Lấy danh sách phần tử trong khoảng điểm số
        /// </summary>
        Task<List<T>> SortedSetRangeByScoreAsync<T>(RedisTypeKey type, string mainKeyword, double start = double.NegativeInfinity, double stop = double.PositiveInfinity);
        #endregion

        #region Pub/Sub Operations
        /// <summary>
        /// Gửi thông điệp tới một kênh
        /// </summary>
        Task PublishAsync<T>(string channel, T message);

        /// <summary>
        /// Đăng ký nhận thông điệp từ một kênh
        /// </summary>
        IDisposable Subscribe<T>(string channel, Action<T> handler);
        #endregion

        #region Key Operations

        /// <summary>
        /// Hàm lấy danh sách redis phục vụ có khả năng phân trang
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<DataTableJson> GetKeysByPage(string key, int page = 1, int pageSize = 20, string? keyword = null, bool isPrefixKey = false);

        /// <summary>
        /// Gia hạn thêm TTL cho key. 
        /// Lấy TTL hiện tại, cộng thêm additionalExpiry rồi set lại.
        /// </summary>
        Task<bool> ExtendSessionIfNearExpiryAsync(RedisTypeKey type, string mainKeyword, TimeSpan threshold, TimeSpan extension);

        /// <summary>
        /// Lấy danh sách các key bắt đầu bằng một từ khóa
        /// </summary>
        /// <param name="prefix">Prefix của key</param>
        /// <param name="count">Số lượng key tối đa cần lấy (tùy chọn)</param>
        /// <returns>Danh sách các key</returns>
        List<string> GetKeysByPrefix(string prefix, int count = 1000);

        /// <summary>
        /// Tim kiem và xoa theo tu khoa
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mainKeyword"></param>
        /// <returns></returns>
        Task<bool> FindAndDeleteAsync(RedisTypeKey type, string mainKeyword);
        /// <summary>
        /// Xóa key khỏi Redis
        /// </summary>
        /// <param name="fullKeyword">full key</param>
        /// <returns></returns>
        Task<bool> FullKeyDeleteAsync(string fullKeyword);
        /// <summary>
        /// Xóa key khỏi Redis
        /// </summary>
        Task<bool> KeyDeleteAsync(RedisTypeKey type, string mainKeyword);
        /// <summary>
        /// Xóa key khỏi Redis không bất đồng bộ
        /// </summary>
        bool KeyDelete(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Kiểm tra key có tồn tại trong Redis
        /// </summary>
        Task<bool> KeyExistsAsync(RedisTypeKey type, string mainKeyword);
        bool KeyExists(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Lấy thời gian còn lại trước khi key hết hạn
        /// </summary>
        Task<TimeSpan?> KeyTimeToLiveAsync(RedisTypeKey type, string mainKeyword);

        /// <summary>
        /// Đặt thời gian hết hạn cho key
        /// </summary>
        Task<bool> KeyExpireAsync(RedisTypeKey type, string mainKeyword, TimeSpan expiry, bool useJitter = true);

        #endregion

        #region Batch Operations
        /// <summary>
        /// Xóa nhiều key cùng lúc
        /// </summary>
        Task<long> BatchDeleteAsync(IEnumerable<string> keys);

        /// <summary>
        /// Lưu nhiều cặp key-value cùng lúc
        /// </summary>
        Task BatchSetAsync<T>(Dictionary<string, T> keyValues, TimeSpan? expiry = null, bool useJitter = true);
        #endregion
    }
}
