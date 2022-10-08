using Draughts.Common.Events;
using Newtonsoft.Json;
using System;

namespace Draughts.Common.Utilities;

public static class JsonUtils {
    public static string SerializeEvent(DomainEventId id, object? value) {
        try {
            return JsonConvert.SerializeObject(value);
        }
        catch (Exception e) {
            throw new JsonException($"Failure trying to serialize object for event {id}.", e);
        }
    }

    public static T? DeserializeEvent<T>(DomainEventId id, string value) {
        try {
            return JsonConvert.DeserializeObject<T>(value);
        }
        catch (Exception e) {
            throw new JsonException($"Failure trying to deserialize '{value}' for event {id}.", e);
        }
    }
}
