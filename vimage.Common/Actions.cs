using System.Text.RegularExpressions;

namespace vimage.Common
{
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
        TransparencyInc,
        TransparencyDec,
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

    public static partial class Actions
    {
        public static List<string> Names =
        [
            "",
            "DRAG",
            "CLOSE",
            "OPEN CONTEXT MENU",
            "PREV IMAGE",
            "NEXT IMAGE",
            "ROTATE CLOCKWISE",
            "ROTATE ANTICLOCKWISE",
            "FLIP",
            "FIT TO HEIGHT",
            "FIT TO WIDTH",
            "FIT TO AUTO",
            "FIT TO ALT",
            "ZOOM IN",
            "ZOOM OUT",
            "ZOOM FASTER",
            "ZOOM ALT",
            "DRAG LIMIT TO MONITOR BOUNDS",
            "TOGGLE SMOOTHING",
            "TOGGLE BACKGROUND",
            "TOGGLE LOCK",
            "ALWAYS ON TOP",
            "CLICK-THROUGH_ABLE",
            "TOGGLE TITLE BAR",
            "TOGGLE ANIMATION",
            "PREV FRAME",
            "NEXT FRAME",
            "OPEN SETTINGS",
            "RESET IMAGE",
            "OPEN FILE LOCATION",
            "DELETE",
            "COPY",
            "COPY AS IMAGE",
            "OPEN DUPLICATE",
            "OPEN DUPLICATE FULL",
            "RANDOM IMAGE",
            "MOVE LEFT",
            "MOVE RIGHT",
            "MOVE UP",
            "MOVE DOWN",
            "TOGGLE IMAGE TRANSPARENCY",
            "TRANSPARENCY INC",
            "TRANSPARENCY DEC",
            "CROP",
            "UNDO CROP",
            "EXIT ALL INSTANCES",
            "RERENDER SVG",
            "VISIT WEBSITE",
            "SORT NAME",
            "SORT DATE",
            "SORT DATE MODIFIED",
            "SORT DATE CREATED",
            "SORT SIZE",
            "SORT ASCENDING",
            "SORT DESCENDING",
        ];

        public static string ToNameString(this Action action)
        {
            return Names[(int)action];
        }

        public static Action StringToAction(string action)
        {
            return (Action)Names.IndexOf(action);
        }

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
}
