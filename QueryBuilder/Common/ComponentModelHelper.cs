using SqlQueryBuilder.Exceptions;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SqlQueryBuilder.Common;

public static class ComponentModelHelper {
    public static IReadOnlyDictionary<string, object?> ToDictionary(object source) {
        return ToDictionary(source, new IdentityColumnFormat());
    }

    public static IReadOnlyDictionary<string, object?> ToDictionary(object source, IColumnFormat columnFormat) {
        var result = new Dictionary<string, object?>();
        foreach (PropertyDescriptor? property in TypeDescriptor.GetProperties(source)) {
            if (property is not null) {
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

            var parsedValue = Parse(value, property.PropertyType);
            try {
                property.SetValue(result, parsedValue);
            } catch (Exception e) {
                throw new SqlQueryBuilderException($"Error trying to set property '{property.DisplayName}' to value '{value}'", e);
            }
        }
        return result;
    }

    public static object? Parse(object? value, Type type) {
        if (value is string valueStr
                && DateTimeParser.ParseProperty(type, valueStr, out object? parsedValue)) {
            return parsedValue;
        }
        if (value is DateTime systemDateTime
                && DateTimeParser.ParseProperty(type, systemDateTime, out object? parsedDateTimeValue)) {
            return parsedDateTimeValue;
        }
        if (type == typeof(bool) && IsNumber(value)) {
            return Convert.ToBoolean(value);
        }
        return value;
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
