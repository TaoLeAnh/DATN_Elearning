using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public static class SIMapper
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo[]> PropertyCache
            = new ConcurrentDictionary<string, PropertyInfo[]>();

        private static readonly int CacheLimit = 500; // Giới hạn số loại được cache

        public static TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            TTarget target = new TTarget();
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            // Xóa cache nếu vượt quá giới hạn
            if (PropertyCache.Count >= CacheLimit)
            {
                PropertyCache.Clear(); // Xóa toàn bộ cache nếu quá tải
            }

            var sourceProps = PropertyCache.GetOrAdd(sourceType.FullName!, _ => sourceType.GetProperties());
            var targetProps = PropertyCache.GetOrAdd(targetType.FullName!, _ => targetType.GetProperties());

            foreach (var sourceProp in sourceProps)
            {
                var targetProp = targetProps.FirstOrDefault(p =>
                    p.Name == sourceProp.Name ||
                    p.GetCustomAttribute<MapFromAttribute>()?.SourceName == sourceProp.Name);

                if (targetProp != null && targetProp.CanWrite)
                {
                    try
                    {
                        var value = sourceProp.GetValue(source);
                        if (value == null) continue;

                        if (targetProp.PropertyType == sourceProp.PropertyType)
                        {
                            targetProp.SetValue(target, value);
                        }
                        else
                        {
                            var convertedValue = ConvertValue(value, targetProp.PropertyType);
                            if (convertedValue != null)
                            {
                                targetProp.SetValue(target, convertedValue);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Mapping error: {ex.Message}");
                    }
                }
            }
            return target;
        }



        // Chuyển đổi kiểu dữ liệu (hỗ trợ cả TypeConverter)
        private static object? ConvertValue(object value, Type targetType)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(value.GetType()))
                {
                    return converter.ConvertFrom(value);
                }
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null; // Không thể chuyển đổi, bỏ qua
            }
        }
    }

    // Attribute để ánh xạ tên thuộc tính khác nhau
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapFromAttribute : Attribute
    {
        public string SourceName { get; }
        public MapFromAttribute(string sourceName) => SourceName = sourceName;
    }
}
