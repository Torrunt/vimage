namespace vimage
{
    class MenuFuncs
    {
        public const string CLOSE = "CLOSE";
        public const string NEXT_IMAGE = "NEXTIMAGE";
        public const string PREV_IMAGE = "PREVIMAGE";
        public const string ROTATE_CLOCKWISE = "ROTATECLOCKWISE";
        public const string ROTATE_ANTICLOCKWISE = "ROTATEANTICLOCKWISE";
        public const string FLIP = "FLIP";
        public const string FIT_TO_HEIGHT = "FITTOHEIGHT";
        public const string FIT_TO_WIDTH = "FITTOWIDTH";
        public const string RESET_IMAGE = "RESETIMAGE";
        public const string TOGGLE_SMOOTHING = "TOGGLESMOOTHING";
        public const string TOGGLE_BACKGROUND = "TOGGLEBACKGROUND";
        public const string ALWAYS_ON_TOP = "ALWAYSONTOP";
        public const string OPEN_FILE_LOCATION = "OPENFILELOCATION";
        public const string DELETE = "DELETE";
        public const string OPEN_SETTINGS = "OPENSETTINGS";
        public const string RELOAD_SETTINGS = "RELOADSETTINGS";
        public const string VERSION_NAME = "VERSIONNAME";

        public const string SORT_NAME = "SORTNAME";
        public const string SORT_DATE_MODIFIED = "SORTDATEMODIFIED";
        public const string SORT_DATE_CREATED = "SORTDATECREATED";
        public const string SORT_SIZE = "SORTSIZE";
        public const string SORT_ASCENDING = "SORTASCENDING";
        public const string SORT_DESCENDING = "SORTDESCENDING";

        public const string NEXT_FRAME = "NEXTFRAME";
        public const string PREV_FRAME = "PREVFRAME";
        public const string TOGGLE_ANIMATION = "TOGGLEANIMATION";

        public static readonly string[] FUNCS =
        {
            CLOSE, NEXT_IMAGE, PREV_IMAGE, ROTATE_CLOCKWISE, ROTATE_ANTICLOCKWISE,
            FLIP, FIT_TO_HEIGHT, FIT_TO_WIDTH, RESET_IMAGE, TOGGLE_SMOOTHING, TOGGLE_BACKGROUND,
            ALWAYS_ON_TOP, OPEN_FILE_LOCATION, DELETE, OPEN_SETTINGS, RELOAD_SETTINGS,
            VERSION_NAME, SORT_NAME, SORT_DATE_MODIFIED, SORT_DATE_CREATED, SORT_SIZE,
            SORT_ASCENDING, SORT_DESCENDING, NEXT_FRAME, PREV_FRAME, TOGGLE_ANIMATION,
        };

        // <summary>Takes a MenuFunc name and adds space between certain words (for ease of reading).</summary>
        public static string WithSpaces(string func)
        {
            // Don't bother if there are already spaces
            if (func.IndexOf(" ") != -1)
                return func;

            func = func.Replace("SORT", "SORT ");
            func = func.Replace("DATE", "DATE ");
            func = func.Replace("FRAME", " FRAME");
            func = func.Replace("TOGGLE", "TOGGLE ");
            func = func.Replace("RESET", "RESET ");
            func = func.Replace("FITTO", "FIT TO ");
            func = func.Replace("IMAGE", " IMAGE");
            func = func.Replace("ROTATE", "ROTATE ");
            func = func.Replace("ALWAYSONTOP", "ALWAYS ON TOP");
            func = func.Replace("VERSIONNAME", "VERSION NAME");
            func = func.Replace("RELOAD", "RELOAD ");
            func = func.Replace("OPEN", "OPEN ");
            func = func.Replace("FILELOCATION", "FILE LOCATION");

            return func;
        }
    }
}
