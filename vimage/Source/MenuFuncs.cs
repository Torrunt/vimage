namespace vimage
{
    class MenuFuncs
    {
        public const string CLOSE = "CLOSE";
        public const string NEXT_IMAGE = "NEXT IMAGE";
        public const string PREV_IMAGE = "PREV IMAGE";
        public const string ROTATE_CLOCKWISE = "ROTATE CLOCKWISE";
        public const string ROTATE_ANTICLOCKWISE = "ROTATE ANTICLOCKWISE";
        public const string FLIP = "FLIP";
        public const string FIT_TO_HEIGHT = "FIT TO HEIGHT";
        public const string FIT_TO_WIDTH = "FIT TO WIDTH";
        public const string FIT_TO_AUTO = "FIT TO AUTO";
        public const string RESET_IMAGE = "RESET IMAGE";
        public const string TOGGLE_SMOOTHING = "TOGGLE SMOOTHING";
        public const string TOGGLE_MIPMAPPING = "TOGGLE MIPMAPPING";
        public const string TOGGLE_BACKGROUND = "TOGGLE BACKGROUND";
        public const string TOGGLE_IMAGE_TRANSPARENCY = "TOGGLE IMAGE TRANSPARENCY";
        public const string TOGGLE_LOCK = "TOGGLE LOCK";
        public const string ALWAYS_ON_TOP = "ALWAYS ON TOP";
        public const string OPEN_FILE_LOCATION = "OPEN FILE LOCATION";
        public const string DELETE = "DELETE";
        public const string COPY = "COPY";
        public const string COPY_AS_IMAGE = "COPY AS_IMAGE";
        public const string OPEN_SETTINGS = "OPEN SETTINGS";
        public const string RELOAD_SETTINGS = "RELOAD SETTINGS";
        public const string VERSION_NAME = "VERSION NAME";

        public const string SORT_NAME = "SORT NAME";
        public const string SORT_DATE = "SORT DATE";
        public const string SORT_DATE_MODIFIED = "SORT DATE MODIFIED";
        public const string SORT_DATE_CREATED = "SORT DATE CREATED";
        public const string SORT_SIZE = "SORT SIZE";
        public const string SORT_ASCENDING = "SORT ASCENDING";
        public const string SORT_DESCENDING = "SORT DESCENDING";

        public const string NEXT_FRAME = "NEXT FRAME";
        public const string PREV_FRAME = "PREV FRAME";
        public const string TOGGLE_ANIMATION = "TOGGLE ANIMATION";

        public const string OPEN_DUPLICATE = "OPEN DUPLICATE";
        public const string OPEN_DUPLICATE_FULL = "OPEN DUPLICATE FULL";
        public const string RANDOM_IMAGE = "RANDOM IMAGE"; 
        public const string UNDO_CROP = "UNDO CROP";
        public const string EXIT_ALL_INSTANCES = "EXIT ALL INSTANCES";

        public static readonly string[] FUNCS =
        {
            CLOSE, NEXT_IMAGE, PREV_IMAGE, ROTATE_CLOCKWISE, ROTATE_ANTICLOCKWISE,
            FLIP, FIT_TO_HEIGHT, FIT_TO_WIDTH, FIT_TO_AUTO, RESET_IMAGE, TOGGLE_SMOOTHING, TOGGLE_MIPMAPPING, TOGGLE_BACKGROUND,
            TOGGLE_IMAGE_TRANSPARENCY, TOGGLE_LOCK, ALWAYS_ON_TOP, OPEN_FILE_LOCATION, DELETE, COPY, COPY_AS_IMAGE,
            OPEN_SETTINGS, RELOAD_SETTINGS, VERSION_NAME, SORT_NAME, SORT_DATE, SORT_DATE_MODIFIED, SORT_DATE_CREATED, SORT_SIZE,
            SORT_ASCENDING, SORT_DESCENDING, NEXT_FRAME, PREV_FRAME, TOGGLE_ANIMATION,
            OPEN_DUPLICATE, OPEN_DUPLICATE_FULL, RANDOM_IMAGE, UNDO_CROP, EXIT_ALL_INSTANCES
        };
    }
}
