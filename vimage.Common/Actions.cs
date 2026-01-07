using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace vimage.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Action
    {
        None,

        Drag,
        Close,
        OpenContextMenu,
        PrevImage,
        NextImage,

        RotateClockwise,
        RotateAntiClockwise,
        Flip,
        FitToMonitorHeight,
        FitToMonitorWidth,
        FitToMonitorAuto,
        FitToMonitorAlt,
        ZoomIn,
        ZoomOut,
        ZoomFaster,
        ZoomAlt,
        DragLimitToMonitorBounds,

        ToggleSmoothing,
        ToggleBackground,
        ToggleLock,
        ToggleAlwaysOnTop,
        ToggleClickThroughAble,
        ToggleTitleBar,

        PauseAnimation,
        PrevFrame,
        NextFrame,
        PlaybackSpeedIncrease,
        PlaybackSpeedDecrease,
        PlaybackSpeedReset,

        OpenSettings,
        ResetImage,
        OpenAtLocation,
        Delete,
        Copy,
        CopyAsImage,
        OpenDuplicateImage,
        OpenFullDuplicateImage,
        RandomImage,

        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,

        TransparencyToggle,
        TransparencyIncrease,
        TransparencyDecrease,
        Crop,
        UndoCrop,
        ExitAll,
        RerenderSVG,

        VisitWebsite,

        SortName,
        SortDate,
        SortDateModified,
        SortDateCreated,
        SortSize,
        SortAscending,
        SortDescending,

        Custom,
    }

    public enum MouseWheel
    {
        ScrollUp,
        ScrollDown,
    }

    public static partial class Actions
    {
        /// <summary>List of modifier actions that can be activated simultaneously.</summary>
        public static readonly Action[] ModifierActions =
        [
            Action.Crop,
            Action.Drag,
            Action.DragLimitToMonitorBounds,
            Action.FitToMonitorAlt,
            Action.ZoomAlt,
            Action.ZoomFaster,
        ];

        /// <summary>List of actions that occur while a key/button is down.</summary>
        public static readonly Action[] HoldDownActions =
        [
            Action.NextFrame,
            Action.PrevFrame,
            Action.PlaybackSpeedIncrease,
            Action.PlaybackSpeedDecrease,
            Action.TransparencyIncrease,
            Action.TransparencyDecrease,
            Action.ZoomIn,
            Action.ZoomOut,
            Action.MoveLeft,
            Action.MoveRight,
            Action.MoveUp,
            Action.MoveDown,
        ];

        /// <summary>List of actions that can be used in the Context Menu.</summary>
        public static readonly Action[] MenuActions =
        [
            Action.Close,
            Action.NextImage,
            Action.PrevImage,
            Action.RotateClockwise,
            Action.RotateAntiClockwise,
            Action.Flip,
            Action.FitToMonitorHeight,
            Action.FitToMonitorWidth,
            Action.FitToMonitorAuto,
            Action.ResetImage,
            Action.ToggleSmoothing,
            Action.ToggleBackground,
            Action.TransparencyToggle,
            Action.ToggleLock,
            Action.ToggleAlwaysOnTop,
            Action.ToggleClickThroughAble,
            Action.ToggleTitleBar,
            Action.OpenAtLocation,
            Action.Delete,
            Action.Copy,
            Action.CopyAsImage,
            Action.OpenDuplicateImage,
            Action.OpenFullDuplicateImage,
            Action.RandomImage,
            Action.UndoCrop,
            Action.ExitAll,
            Action.PauseAnimation,
            Action.NextFrame,
            Action.PrevFrame,
            Action.PlaybackSpeedIncrease,
            Action.PlaybackSpeedDecrease,
            Action.PlaybackSpeedReset,
            Action.OpenSettings,
            Action.VisitWebsite,
            Action.SortName,
            Action.SortDate,
            Action.SortDateModified,
            Action.SortDateCreated,
            Action.SortSize,
            Action.SortAscending,
            Action.SortDescending,
        ];

        /// <summary>
        /// Split exe and arguments by the first space (regex to exclude the spaces within the quotes of the exe's path)
        /// </summary>
        [GeneratedRegex("(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")]
        public static partial Regex CustomActionSplitRegex();
    }

    /// <summary>
    /// Either a Action enum or the name of a custom action.
    /// </summary>
    [JsonConverter(typeof(ActionFuncConverter))]
    public abstract record ActionFunc;

    public record CustomAction(string Value) : ActionFunc;

    public record ActionEnum(Action Value) : ActionFunc;

    public sealed class ActionFuncConverter : JsonConverter<ActionFunc>
    {
        public override ActionFunc Read(
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
                return new ActionEnum(action);

            return new CustomAction(value);
        }

        public override void Write(
            Utf8JsonWriter writer,
            ActionFunc value,
            JsonSerializerOptions options
        )
        {
            switch (value)
            {
                case ActionEnum a:
                    writer.WriteStringValue(a.Value.ToString());
                    break;
                case CustomAction s:
                    writer.WriteStringValue(s.Value);
                    break;
                default:
                    throw new JsonException();
            }
        }
    }
}
