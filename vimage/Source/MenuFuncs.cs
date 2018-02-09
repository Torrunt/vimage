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
        public const string FIT_TO_AUTO = "FITTOAUTO";
        public const string RESET_IMAGE = "RESETIMAGE";
        public const string TOGGLE_SMOOTHING = "TOGGLESMOOTHING";
        public const string TOGGLE_MIPMAPPING = "TOGGLEMIPMAPPING";
        public const string TOGGLE_BACKGROUND = "TOGGLEBACKGROUND";
        public const string TOGGLE_IMAGE_TRANSPARENCY = "TOGGLEIMAGETRANSPARENCY";
        public const string TOGGLE_LOCK = "TOGGLELOCK";
        public const string ALWAYS_ON_TOP = "ALWAYSONTOP";
        public const string OPEN_FILE_LOCATION = "OPENFILELOCATION";
        public const string DELETE = "DELETE";
        public const string COPY = "COPY";
        public const string COPY_AS_IMAGE = "COPYASIMAGE";
        public const string OPEN_SETTINGS = "OPENSETTINGS";
        public const string RELOAD_SETTINGS = "RELOADSETTINGS";
        public const string VERSION_NAME = "VERSIONNAME";

        public const string SORT_NAME = "SORTNAME";
        public const string SORT_DATE = "SORTDATE";
        public const string SORT_DATE_MODIFIED = "SORTDATEMODIFIED";
        public const string SORT_DATE_CREATED = "SORTDATECREATED";
        public const string SORT_SIZE = "SORTSIZE";
        public const string SORT_ASCENDING = "SORTASCENDING";
        public const string SORT_DESCENDING = "SORTDESCENDING";

        public const string NEXT_FRAME = "NEXTFRAME";
        public const string PREV_FRAME = "PREVFRAME";
        public const string TOGGLE_ANIMATION = "TOGGLEANIMATION";

        public const string OPEN_DUPLICATE = "OPENDUPLICATE";
        public const string OPEN_DUPLICATE_FULL = "OPENDUPLICATEFULL";
        public const string RANDOM_IMAGE = "RANDOMIMAGE"; 
        public const string UNDO_CROP = "UNDOCROP";
        public const string EXIT_ALL_INSTANCES = "EXITALLINSTANCES";

        public static readonly string[] FUNCS =
        {
            CLOSE, NEXT_IMAGE, PREV_IMAGE, ROTATE_CLOCKWISE, ROTATE_ANTICLOCKWISE,
            FLIP, FIT_TO_HEIGHT, FIT_TO_WIDTH, FIT_TO_AUTO, RESET_IMAGE, TOGGLE_SMOOTHING, TOGGLE_MIPMAPPING, TOGGLE_BACKGROUND,
            TOGGLE_IMAGE_TRANSPARENCY, TOGGLE_LOCK, ALWAYS_ON_TOP, OPEN_FILE_LOCATION, DELETE, COPY, COPY_AS_IMAGE,
            OPEN_SETTINGS, RELOAD_SETTINGS, VERSION_NAME, SORT_NAME, SORT_DATE, SORT_DATE_MODIFIED, SORT_DATE_CREATED, SORT_SIZE,
            SORT_ASCENDING, SORT_DESCENDING, NEXT_FRAME, PREV_FRAME, TOGGLE_ANIMATION,
            OPEN_DUPLICATE, OPEN_DUPLICATE_FULL, RANDOM_IMAGE, UNDO_CROP, EXIT_ALL_INSTANCES
        };

        // <summary>Takes a MenuFunc name and adds space between certain words (for ease of reading).</summary>
        public static string WithSpaces(string func)
        {
            // Don't bother if there are already spaces
            if (func.IndexOf(" ") != -1)
                return func;

            switch (func)
            {
                case ALWAYS_ON_TOP: return "ALWAYS ON TOP";
                case VERSION_NAME: return "VERSION NAME";
                case TOGGLE_IMAGE_TRANSPARENCY: return "TOGGLE IMAGE TRANSPARENCY";
                case RANDOM_IMAGE: return "RANDOM IMAGE";
                case UNDO_CROP: return "UNDO CROP";
                case EXIT_ALL_INSTANCES: return "EXIT ALL INSTANCES";
            }
            func = func.Replace("SORT", "SORT ");
            func = func.Replace("DATE", "DATE ");
            func = func.Replace("FRAME", " FRAME");
            func = func.Replace("TOGGLE", "TOGGLE ");
            func = func.Replace("FITTO", "FIT TO ");
            func = func.Replace("ASIMAGE", " ASIMAGE");
            func = func.Replace("IMAGE", " IMAGE");
            func = func.Replace("ROTATE", "ROTATE ");
            func = func.Replace("RELOAD", "RELOAD ");
            func = func.Replace("OPEN", "OPEN ");
            func = func.Replace("FILELOCATION", "FILE LOCATION");
            func = func.Replace("DUPLICATEFULL", "DUPLICATE FULL");

            return func;
        }
    }
}
