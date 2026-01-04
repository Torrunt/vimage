using System.Text.Json;
using System.Text.Json.Serialization;

namespace vimage.Common
{
    [Serializable]
    public struct ContextMenuItem
    {
        public string name;
        public ContextMenuFunc? func;
        public List<ContextMenuItem>? children;
    }

    [JsonConverter(typeof(ContextMenuFuncConverter))]
    public abstract record ContextMenuFunc;

    public record FuncString(string Value) : ContextMenuFunc;

    public record FuncAction(Action Value) : ContextMenuFunc;

    public sealed class ContextMenuFuncConverter : JsonConverter<ContextMenuFunc>
    {
        public override ContextMenuFunc Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();

            var value = reader.GetString()!;

            // Check if Action enum
            if (Enum.TryParse<Action>(value, out var action))
                return new FuncAction(action);

            return new FuncString(value);
        }

        public override void Write(
            Utf8JsonWriter writer,
            ContextMenuFunc value,
            JsonSerializerOptions options
        )
        {
            switch (value)
            {
                case FuncAction a:
                    writer.WriteStringValue(a.Value.ToString());
                    break;
                case FuncString s:
                    writer.WriteStringValue(s.Value);
                    break;
                default:
                    throw new JsonException();
            }
        }
    }
}
