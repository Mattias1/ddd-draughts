using SqlQueryBuilder.Exceptions;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SqlQueryBuilder.Common {
    public static class ComponentModelHelper {
        public static IReadOnlyDictionary<string, object?> ToDictionary(object source) {
            return ToDictionary(source, new IdentityColumnFormat());
        }

        public static IReadOnlyDictionary<string, object?> ToDictionary(object source, IColumnFormat columnFormat) {
            var result = new Dictionary<string, object?>();
            foreach (PropertyDescriptor? property in TypeDescriptor.GetProperties(source)) {
                if (property != null) {
                    result.Add(columnFormat.Format(property.Name), property.GetValue(source));
                }
            }
            return result;
        }

        public static T ToObject<T>(IReadOnlyDictionary<string, object?> source) where T : new() {
            return ToObject<T>(source, new IdentityColumnFormat());
        }

        public static T ToObject<T>(IReadOnlyDictionary<string, object?> source, IColumnFormat columnFormat) where T : new() {
            var result = new T();
            foreach (PropertyDescriptor? property in TypeDescriptor.GetProperties(result)) {
                if (property is null || !source.TryGetValue(columnFormat.Format(property.Name), out object? value)) {
                    continue;
                }

                if (value is null || value == DBNull.Value) {
                    SetProperty(result, property, null);
                }
                else if (value is string valueStr && DateTimeParser.ParseProperty(property.PropertyType, valueStr, out object? parsedValue)) {
                    SetProperty(result, property, parsedValue);
                }
                else if (value is DateTime systemDateTime
                        && DateTimeParser.ParseProperty(property.PropertyType, systemDateTime, out object? parsedDateTimeValue)) {
                    SetProperty(result, property, parsedDateTimeValue);
                }
                else if (property.PropertyType == typeof(bool) && IsNumber(value)) {
                    SetProperty(result, property, Convert.ToBoolean(value));
                }
                else {
                    SetProperty(result, property, value);
                }
            }
            return result;
        }

        private static void SetProperty<T>(T result, PropertyDescriptor property, object? value) {
            try {
                property.SetValue(result, value);
            }
            catch (Exception e) {
                throw new SqlQueryBuilderException($"Error trying to set property '{property.DisplayName}' to value '{value}'", e);
            }
        }

        private static bool IsNumber(object? value) {
            return value is long
                || value is ulong
                || value is int
                || value is uint
                || value is short
                || value is ushort
                || value is byte
                || value is sbyte
                || value is float
                || value is double
                || value is decimal;
        }
    }
}
