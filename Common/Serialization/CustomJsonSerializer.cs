using System.Text.Json;
using Confluent.Kafka;

namespace Common.Serialization;

public class CustomJsonSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}