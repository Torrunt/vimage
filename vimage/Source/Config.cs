using System;
using System.IO;
using System.Collections.Generic;
using SFML.Window;

namespace vimage
{
    class Config
    {

        public List<int> Control_Drag = new List<int>();
        public List<int> Control_Close = new List<int>();
        public List<int> Control_OpenContextMenu = new List<int>();
        public List<int> Control_PrevImage = new List<int>();
        public List<int> Control_NextImage = new List<int>();
        public List<int> Control_RotateClockwise = new List<int>();
        public List<int> Control_RotateAntiClockwise = new List<int>();
        public List<int> Control_Flip = new List<int>();
        public List<int> Control_FitToMonitorHeight = new List<int>();
        public List<int> Control_FitToMonitorHeightAlternative = new List<int>();
        public List<int> Control_ZoomFaster = new List<int>();
        public List<int> Control_ZoomAlt = new List<int>();
        public List<int> Control_ToggleSmoothing = new List<int>();
        public List<int> Control_ToggleBackgroundForTransparency = new List<int>();
        public List<int> Control_ToggleAlwaysOnTop = new List<int>();
        public List<int> Control_PauseAnimation = new List<int>();
        public List<int> Control_PrevFrame = new List<int>();
        public List<int> Control_NextFrame = new List<int>();
        public List<int> Control_OpenConfig = new List<int>();
        public List<int> Control_ReloadConfig = new List<int>();
        public List<int> Control_ResetImage = new List<int>();
        public List<int> Control_OpenAtLocation = new List<int>();
        public List<int> Control_Delete = new List<int>();
        public List<int> Control_OpenDuplicateImage = new List<int>();

        public bool Setting_OpenAtMousePosition
        {
            get { return (Boolean)Settings["OPENATMOUSEPOSITION"]; }
            set { Settings["OPENATMOUSEPOSITION"] = value; }
        }
        public bool Setting_SmoothingDefault
        {
            get { return (Boolean)Settings["SMOOTHINGDEFAULT"]; }
            set { Settings["SMOOTHINGDEFAULT"] = value; }
        }
        public bool Setting_BackgroundForImagesWithTransparencyDefault
        {
            get { return (Boolean)Settings["BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT"]; }
            set { Settings["BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT"] = value; }
        }
        public bool Setting_LimitImagesToMonitorHeight
        {
            get { return (Boolean)Settings["LIMITIMAGESTOMONITORHEIGHT"]; }
            set { Settings["LIMITIMAGESTOMONITORHEIGHT"] = value; }
        }
        public bool Setting_PositionLargeWideImagesInCorner
        {
            get { return (Boolean)Settings["POSITIONLARGEWIDEIMAGESINCORNER"]; }
            set { Settings["POSITIONLARGEWIDEIMAGESINCORNER"] = value; }
        }
        public bool Setting_PreloadNextImage
        {
            get { return (Boolean)Settings["PRELOADNEXTIMAGE"]; }
            set { Settings["PRELOADNEXTIMAGE"] = value; }
        }
        public bool Setting_OpenSettingsEXE
        {
            get { return (Boolean)Settings["OPENSETTINGSEXE"]; }
            set { Settings["OPENSETTINGSEXE"] = value; }
        }
        public int Setting_MinImageSize
        {
            get { return (int)Settings["MINIMAGESIZE"]; }
            set { Settings["MINIMAGESIZE"] = value; }
        }
        public int Setting_SmoothingMinImageSize
        {
            get { return (int)Settings["SMOOTHINGMINIMAGESIZE"]; }
            set { Settings["SMOOTHINGMINIMAGESIZE"] = value; }
        }

        public List<object> ContextMenu = new List<object>();
        public List<object> ContextMenu_Animation = new List<object>();
        public List<object> ContextMenu_Default = new List<object>();
        public List<object> ContextMenu_Animation_Default = new List<object>();

        public int ContextMenu_Animation_InsertAtIndex
        {
            get { return (int)Settings["CONTEXTMENU_ANIMATION_INSERTATINDEX"]; }
            set { Settings["CONTEXTMENU_ANIMATION_INSERTATINDEX"] = value; }
        }
        public bool ContextMenuShowMargin
        {
            get { return (Boolean)Settings["CONTEXTMENU_SHOWMARGIN"]; }
            set { Settings["CONTEXTMENU_SHOWMARGIN"] = value; }
        }

        private Dictionary<string, object> Settings;

        public const int MouseCodeOffset = 150;

        public Config()
        {
            Init();
        }
        public void Init()
        {
            SetControls(Control_Drag, "MOUSELEFT");
            SetControls(Control_Close, "ESC", "BACKSPACE");
            SetControls(Control_OpenContextMenu, "MOUSERIGHT");
            SetControls(Control_PrevImage, "LEFT", "PAGE UP", "MOUSE4");
            SetControls(Control_NextImage, "RIGHT", "PAGE DOWN", "MOUSE5");
            SetControls(Control_RotateClockwise, "UP");
            SetControls(Control_RotateAntiClockwise, "DOWN");
            SetControls(Control_Flip, "F");
            SetControls(Control_FitToMonitorHeight, "MOUSEMIDDLE");
            SetControls(Control_FitToMonitorHeightAlternative, "RSHIFT", "LSHIFT");
            SetControls(Control_ZoomFaster, "RSHIFT", "LSHIFT");
            SetControls(Control_ZoomAlt, "RCONTROL", "LCONTROL");
            SetControls(Control_ToggleSmoothing, "S");
            SetControls(Control_ToggleBackgroundForTransparency, "T");
            SetControls(Control_ToggleAlwaysOnTop, "L");
            SetControls(Control_PauseAnimation, "SPACE");
            SetControls(Control_PrevFrame, "<");
            SetControls(Control_NextFrame, ">");
            SetControls(Control_OpenConfig, "O");
            SetControls(Control_ReloadConfig, "P");
            SetControls(Control_ResetImage, "R");
            SetControls(Control_OpenAtLocation, "");
            SetControls(Control_Delete, "DELETE");
            SetControls(Control_OpenDuplicateImage, "C");

            ContextMenu.Add(new { name = "Close", func = MenuFuncs.CLOSE });
            ContextMenu.Add(new { name = "-", func = "-" });
            ContextMenu.Add(new { name = "Next Image", func = MenuFuncs.NEXT_IMAGE });
            ContextMenu.Add(new { name = "Prev Image", func = MenuFuncs.PREV_IMAGE });
            ContextMenu.Add("Sort by");
            List<object> SubMenu_SortBy = new List<object>();
            SubMenu_SortBy.Add(new { name = "Name", func = MenuFuncs.SORT_NAME });
            SubMenu_SortBy.Add(new { name = "Date modified", func = MenuFuncs.SORT_DATE_MODIFIED });
            SubMenu_SortBy.Add(new { name = "Date created", func = MenuFuncs.SORT_DATE_CREATED });
            SubMenu_SortBy.Add(new { name = "Size", func = MenuFuncs.SORT_SIZE });
            SubMenu_SortBy.Add(new { name = "-", func = "-" });
            SubMenu_SortBy.Add(new { name = "Ascending", func = MenuFuncs.SORT_ASCENDING });
            SubMenu_SortBy.Add(new { name = "Descending", func = MenuFuncs.SORT_DESCENDING });
            ContextMenu.Add(SubMenu_SortBy);
            ContextMenu.Add(new { name = "-", func = "-" });
            ContextMenu.Add(new { name = "Rotate Clockwise", func = MenuFuncs.ROTATE_CLOCKWISE });
            ContextMenu.Add(new { name = "Rotate Anti-Clockwise", func = MenuFuncs.ROTATE_ANTICLOCKWISE });
            ContextMenu.Add(new { name = "Flip", func = MenuFuncs.FLIP });
            ContextMenu.Add(new { name = "Fit to monitor height", func = MenuFuncs.FIT_TO_HEIGHT });
            ContextMenu.Add(new { name = "Reset Image", func = MenuFuncs.RESET_IMAGE });
            ContextMenu.Add(new { name = "Smoothing", func = MenuFuncs.TOGGLE_SMOOTHING });
            ContextMenu.Add(new { name = "Background", func = MenuFuncs.TOGGLE_BACKGROUND });
            ContextMenu.Add(new { name = "Always on top", func = MenuFuncs.ALWAYS_ON_TOP });
            ContextMenu.Add(new { name = "-", func = "-" });
            ContextMenu.Add(new { name = "Open file location", func = MenuFuncs.OPEN_FILE_LOCATION });
            ContextMenu.Add(new { name = "Delete", func = MenuFuncs.DELETE });
            ContextMenu.Add(new { name = "-", func = "-" });
            ContextMenu.Add(new { name = "Open Settings", func = MenuFuncs.OPEN_SETTINGS });
            ContextMenu.Add(new { name = "Reload Settings", func = MenuFuncs.RELOAD_SETTINGS });
            ContextMenu.Add(new { name = "", func = MenuFuncs.VERSION_NAME });

            ContextMenu_Animation.Add(new { name = "Next Frame", func = MenuFuncs.NEXT_FRAME });
            ContextMenu_Animation.Add(new { name = "Prev Frame", func = MenuFuncs.PREV_FRAME });
            ContextMenu_Animation.Add(new { name = "Pause/Play Animation", func = MenuFuncs.TOGGLE_ANIMATION });
            ContextMenu_Animation.Add(new { name = "-", func = "-" });

            ContextMenu_Default = new List<object>(ContextMenu);
            ContextMenu_Animation_Default = new List<object>(ContextMenu_Animation);

            Settings = new Dictionary<string, object>()
            {
                { "OPENATMOUSEPOSITION", true },
                { "SMOOTHINGDEFAULT", true },
                { "BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT", false },
                { "LIMITIMAGESTOMONITORHEIGHT", true },
                { "POSITIONLARGEWIDEIMAGESINCORNER", true },
                { "PRELOADNEXTIMAGE", true },
                { "OPENSETTINGSEXE", true },
                { "MINIMAGESIZE", 64 },
                { "SMOOTHINGMINIMAGESIZE", 65 },

                { "DRAG", Control_Drag },
                { "CLOSE", Control_Close },
                { "OPENCONTEXTMENU", Control_OpenContextMenu },
                { "PREVIMAGE", Control_PrevImage },
                { "NEXTIMAGE", Control_NextImage },
                { "ROTATECLOCKWISE", Control_RotateClockwise },
                { "ROTATEANTICLOCKWISE", Control_RotateAntiClockwise },
                { "FLIP", Control_Flip },
                { "FITTOMONITORHEIGHT", Control_FitToMonitorHeight },
                { "FITTOMONITORHEIGHTALTERNATIVE", Control_FitToMonitorHeightAlternative },
                { "ZOOMFASTER", Control_ZoomFaster },
                { "ZOOMALT", Control_ZoomAlt },
                { "TOGGLESMOOTHING", Control_ToggleSmoothing },
                { "TOGGLEBACKGROUNDFORTRANSPARENCY", Control_ToggleBackgroundForTransparency },
                { "TOGGLEALWAYSONTOP", Control_ToggleAlwaysOnTop },
                { "PAUSEANIMATION", Control_PauseAnimation },
                { "PREVFRAME", Control_PrevFrame },
                { "NEXTFRAME", Control_NextFrame },
                { "OPENCONFIG", Control_OpenConfig },
                { "RELOADCONFIG", Control_ReloadConfig },
				{ "RESETIMAGE", Control_ResetImage },
                { "OPENATLOCATION", Control_OpenAtLocation },
                { "DELETE", Control_Delete },
                { "OPENDUPLICATEIMAGE", Control_OpenDuplicateImage },

                { "CONTEXTMENU", ContextMenu },
                { "CONTEXTMENU_ANIMATION", ContextMenu_Animation },
                { "CONTEXTMENU_ANIMATION_INSERTATINDEX", 2 },
                { "CONTEXTMENU_SHOWMARGIN", false }
            };
        }

        /// <summary> Loads and parses a config txt file. If it doesn't exist, a default one will be made. </summary>
        public void Load(string configFile)
        {
            // If config file doesn't exist, make one
            if (!File.Exists(configFile))
                Save(configFile);
            // Clear default controls and context menu before they are loaded back in
            foreach (var list in Settings)
            {
                if (list.Value is List<int>)
                    ((List<int>)list.Value).Clear();
            }
            ContextMenu.Clear();
            ContextMenu_Animation.Clear();

            
            StreamReader reader = File.OpenText(configFile);
            string line = reader.ReadLine();

            while (line != null)
            {
                // if line is empty of has no '=' symbol, go to next
                if (line.Equals("") || line.IndexOf('=') == -1)
                {
                    line = reader.ReadLine();
                    continue;
                }

                // trim spaces
                line = line.Replace(" ", "");

                // ignore comments
                if (line.IndexOf("//") != -1)
                    line = line.Substring(0, line.IndexOf("//"));

                // split variable name and values
                string[] nameValue = line.Split('=');

                // invalid name?
                string name = nameValue[0].ToUpper();
                if (!Settings.ContainsKey(name))
                {
                    line = reader.ReadLine();
                    continue;
                }

                // nothing after '='?, check next line
                if (nameValue[1].Equals(""))
                {
                    line = reader.ReadLine().Replace(" ", "").Replace("\t", "");

                    // line is empty or is part of another setting, skip
                    if (line.Equals("") || line.IndexOf('=') != -1)
                        continue;
                    // line doesn't have open brace, skip
                    if (!line.Equals("{"))
                        continue;

                    // read section
                    line = ReadSection(reader, Settings[name] as List<object>);
                    continue;
                }


                // split values by commas
                string[] values = nameValue[1].Split(',');

                // Assign Values
                if (Settings[name] is List<int>)
                {
                    // Control
                    List<int> list = (List<int>)Settings[name];
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].ToUpper().StartsWith("MOUSE"))
                        {
                            // Mouse Button
                            int btn = StringToMouseButton(values[i].ToUpper());
                            if (btn != -1)
                                list.Add(btn);
                        }
                        else
                        {
                            // Keyboard Key
                            Keyboard.Key key = StringToKey(values[i].ToUpper());
                            if (key != Keyboard.Key.Unknown)
                                list.Add((int)key);
                        }
                    }
                }
                else if (Settings[name] is int)
                {
                    // Integer
                    int i;
                    if (int.TryParse(values[0], out i))
                        Settings[name] = i;
                }
                else if (Settings[name] is Boolean)
                {
                    // Boolean
                    if (values[0].Equals("1") || values[0].ToUpper().Equals("T") || values[0].ToUpper().Equals("TRUE"))
                        Settings[name] = true;
                    else
                        Settings[name] = false;
                }
                else if (Settings[name] is String)
                {
                    Settings[name] = values[0];
                }

                // next line
                line = reader.ReadLine();
            }

            reader.Close();
        }
        private string ReadSection(StreamReader reader, List<object> setting)
        {
            string line = reader.ReadLine();
            string trimedLine = line.Replace(" ", "").Replace("\t", "");
            string[] splitValues;

            while (line != null)
            {
                if (!trimedLine.Equals("-") && trimedLine.IndexOf(':') == -1)
                {
                    // Subsection
                    string subSectionName = line.Replace("\t", "");

                    line = reader.ReadLine().Replace(" ", "").Replace("\t", "");

                    // line is empty or is part of another setting, skip
                    if (line.Equals("") || line.IndexOf('=') != -1)
                        continue;
                    // line doesn't have open brace, skip
                    if (!line.Equals("{"))
                        continue;

                    setting.Add(subSectionName);
                    List<object> subSetting = new List<object>();
                    setting.Add(subSetting);

                    line = ReadSection(reader, subSetting);
                    trimedLine = line;
                }
                else
                {
                    // Item
                    if (trimedLine.Equals("-"))
                        splitValues = new[] { "-", "-" }; // line break
                    else
                        splitValues = line.Split(':');

                    // trim tabs from name, spaces from map name
                    splitValues[0] = splitValues[0].Replace("\t", "");
                    splitValues[1] = splitValues[1].Replace(" ", "");

                    // assign Values
                    setting.Add(new { name = splitValues[0].Trim(), func = splitValues[1].Trim() });

                    // next line
                    line = reader.ReadLine();
                    trimedLine = line.Replace(" ", "").Replace("\t", "");
                }

                // break if end brace
                if (trimedLine.Equals("}"))
                {
                    line = reader.ReadLine().Replace(" ", "").Replace("\t", "");
                    break;
                }
            }

            return line;
        }


        /// <summary> Saves settings to config txt file. </summary>
        public void Save(string configFile)
        {
            // Open
            FileStream fileStream;
            if (File.Exists(configFile))
            {
                // Clear if file already exists
                File.WriteAllText(configFile, String.Empty);
                fileStream = File.Open(configFile, FileMode.Open, FileAccess.Write);
            }
            else
                fileStream = File.Create(configFile);
            StreamWriter writer = new StreamWriter(fileStream);

            // Write
            writer.Write("// General Settings" + Environment.NewLine);

            WriteSetting(writer, "OpenAtMousePosition", Setting_OpenAtMousePosition);
            WriteSetting(writer, "SmoothingDefault", Setting_SmoothingDefault);
            WriteSetting(writer, "BackgroundForImagesWithTransparencyDefault", Setting_BackgroundForImagesWithTransparencyDefault);
            WriteSetting(writer, "LimitImagesToMonitorHeight", Setting_LimitImagesToMonitorHeight);
            WriteSetting(writer, "PositionLargeWideImagesInCorner", Setting_PositionLargeWideImagesInCorner, 
                "ie: Desktop Wallpapers and Screenshots");
            WriteSetting(writer, "PreloadNextImage", Setting_PreloadNextImage, 
                "when using the next/prev image buttons, the image after the one just loaded will be loaded as well");
            WriteSetting(writer, "OpenSettingsEXE", Setting_OpenSettingsEXE, "if false, will open config.txt instead");
            WriteSetting(writer, "MinImageSize", Setting_MinImageSize, 
                "if an image is smaller than this (in width or height) it will scaled up to it automatically");
            WriteSetting(writer, "SmoothingMinImageSize", Setting_SmoothingMinImageSize, 
                "images smaller than this will not have smoothing turned on (if 0, all images with use smoothing)");

            writer.Write(Environment.NewLine);
            writer.Write("// Bindings" + Environment.NewLine);

            WriteControl(writer, "Drag", Control_Drag);
            WriteControl(writer, "Close", Control_Close);
            WriteControl(writer, "OpenContextMenu", Control_OpenContextMenu);
            WriteControl(writer, "PrevImage", Control_PrevImage);
            WriteControl(writer, "NextImage", Control_NextImage);
            WriteControl(writer, "RotateClockwise", Control_RotateClockwise);
            WriteControl(writer, "RotateAntiClockwise", Control_RotateAntiClockwise);
            WriteControl(writer, "Flip", Control_Flip);
            WriteControl(writer, "FitToMonitorHeight", Control_FitToMonitorHeight);
            WriteControl(writer, "FitToMonitorHeightAlternative", Control_FitToMonitorHeightAlternative);
            WriteControl(writer, "ZoomFaster", Control_ZoomFaster);
            WriteControl(writer, "ZoomAlt", Control_ZoomAlt);
            WriteControl(writer, "ToggleSmoothing", Control_ToggleSmoothing);
            WriteControl(writer, "ToggleBackgroundForTransparency", Control_ToggleBackgroundForTransparency);
            WriteControl(writer, "ToggleAlwaysOnTop", Control_ToggleAlwaysOnTop);
            WriteControl(writer, "PauseAnimation", Control_PauseAnimation);
            WriteControl(writer, "PrevFrame", Control_PrevFrame);
            WriteControl(writer, "NextFrame", Control_NextFrame);
            WriteControl(writer, "OpenConfig", Control_OpenConfig);
            WriteControl(writer, "ReloadConfig", Control_ReloadConfig);
            WriteControl(writer, "ResetImage", Control_ResetImage);
            WriteControl(writer, "OpenAtLocation", Control_OpenAtLocation);
            WriteControl(writer, "Delete", Control_Delete);
            WriteControl(writer, "OpenDuplicateImage", Control_OpenDuplicateImage);

            writer.Write(Environment.NewLine);
            writer.Write("// Context Menu" + Environment.NewLine);

            WriteContextMenuSetup(writer, "ContextMenu", ContextMenu);
            WriteContextMenuSetup(writer, "ContextMenu_Animation", ContextMenu_Animation);

            WriteSetting(writer, "ContextMenu_Animation_InsertAtIndex", ContextMenu_Animation_InsertAtIndex);
            WriteSetting(writer, "ContextMenu_ShowMargin", ContextMenuShowMargin, "shows checkboxes for certain menu items");

            // Close
            writer.Close();
        }
        private void WriteSetting(StreamWriter writer, string name, bool value, string comment = "")
        {
            writer.Write(name + " = " + (value ? 1 : 0));
            WriteComment(writer, comment);
        }
        private void WriteSetting(StreamWriter writer, string name, int value, string comment = "")
        {
            writer.Write(name + " = " + value.ToString());
            WriteComment(writer, comment);
        }
        private void WriteComment(StreamWriter writer, string comment = "")
        {
            if (!comment.Equals(""))
                writer.Write(" // " + comment);
            writer.Write(Environment.NewLine);
        }
        private void WriteControl(StreamWriter writer, string name, List<int> controls)
        {
            writer.Write(name + " = " + ControlsToString(controls) + Environment.NewLine);
        }
        private void WriteContextMenuSetup(StreamWriter writer, string name, List<object> contextMenu)
        {
            writer.Write(name + " =" + Environment.NewLine + "{" + Environment.NewLine);
            WriteContextMenuItems(writer, contextMenu, 1);
            writer.Write("}" + Environment.NewLine);
        }
        private void WriteContextMenuItems(StreamWriter writer, List<object> items, int depth = 1)
        {
            for (int i = 0; i < items.Count; i++)
            {
                writer.Write(VariableAmountOfStrings(depth, "\t"));

                if (items[i] is string)
                {
                    // Submenu
                    writer.Write((items[i] as string) + Environment.NewLine + VariableAmountOfStrings(depth, "\t") + "{" + Environment.NewLine);
                    i++;
                    WriteContextMenuItems(writer, items[i] as List<object>, depth + 1);
                    writer.Write(VariableAmountOfStrings(depth, "\t") + "}" + Environment.NewLine);
                }
                else
                {
                    // Item
                    string itemName = (items[i] as dynamic).name as string;
                    string itemFunc = MenuFuncs.WithSpaces((items[i] as dynamic).func as string);
                    if (itemFunc.Equals("-"))
                        writer.Write("-" + Environment.NewLine);
                    else if (itemName.Equals(""))
                        writer.Write(": " + itemFunc + Environment.NewLine);
                    else
                        writer.Write(itemName + " : " + itemFunc + Environment.NewLine);
                }
            }
        }


        /// <summary> Converts list of controls (Keyboard.Key and Mouse.Button) to their string names seperated by commas. </summary>
        public static string ControlsToString(List<int> controls)
        {
            string str = "";

            for (int i = 0; i < controls.Count; i++)
            {
                str += ControlToString(controls[i]);

                if (i != controls.Count - 1)
                    str += ", ";
            }

            return str;
        }
        /// <summary> Converts Keyboard.Key or Mouse.Button to their string name. </summary>
        public static string ControlToString(object code)
        {
            if ((int)code >= MouseCodeOffset)
                return MouseButtonToString((int)code);
            else
                return KeyToString((Keyboard.Key)code);
        }


        /// <summary> Returns true if code is one of Control bindings. </summary>
        public static bool IsControl(object code, List<int> Control)
        {
            if (code is Keyboard.Key)
                return IsControl((Keyboard.Key)code, Control);
            else if (code is Mouse.Button)
                return IsControl((Mouse.Button)code, Control);

            return false;
        }
        /// <summary> Returns true if Keyboard.Key is one of Control bindings. </summary>
        public static bool IsControl(Keyboard.Key keyCode, List<int> Control)
        {
            // Keyboard key?
            foreach (Keyboard.Key key in Control)
            {
                if (keyCode == key)
                    return true;
            }
            return false;
        }
        /// <summary> Returns true if Mouse.Button is one of Control bindings. </summary>
        public static bool IsControl(Mouse.Button code, List<int> Control)
        {
            // Mouse key?
            foreach (Mouse.Button key in Control)
            {
                if (code == (Mouse.Button)(key - MouseCodeOffset))
                    return true;
            }
            return false;
        }

        public static void SetControls(List<int> controls, params string[] bindings)
        {
            foreach (string str in bindings)
            {
                if (str.Equals(""))
                    continue;

                if (str.StartsWith("MOUSE"))
                    controls.Add(StringToMouseButton(str));
                else
                    controls.Add((int)StringToKey(str.Replace(" ", "")));
            }
        }

        public List<int> UpdateControl(string name, int bind)
        {
            name = name.ToUpper();
            if (!Settings.ContainsKey(name))
                return null;

            List<int> Control = (List<int>)Settings[name];

            if (bind == -1)
                Control.Clear();
            else if (Control.IndexOf(bind) == -1)
                Control.Add(bind);

            return Control;
        }

        /// <summary> Converts upper-case string to SFML Mouse.Button (as an int + offset). </summary>
        public static int StringToMouseButton(string str)
        {
            switch (str)
            {
                case "MOUSELEFT":
                case "MOUSE1":
                    return (int)Mouse.Button.Left + MouseCodeOffset;
                case "MOUSERIGHT":
                case "MOUSE2":
                    return (int)Mouse.Button.Right + MouseCodeOffset;
                case "MOUSEMIDDLE":
                case "MOUSE3":
                    return (int)Mouse.Button.Middle + MouseCodeOffset;
                case "MOUSEX1":
                case "MOUSEXBUTTON1":
                case "MOUSE4":
                    return (int)Mouse.Button.XButton1 + MouseCodeOffset;
                case "MOUSEX2":
                case "MOUSEXBUTTON2":
                case "MOUSE5":
                    return (int)Mouse.Button.XButton2 + MouseCodeOffset;
            }

            return -1;
        }
        public static string MouseButtonToString(int code)
        {
            switch ((Mouse.Button)(code - MouseCodeOffset))
            {
                case Mouse.Button.Left: return "MOUSELEFT";
                case Mouse.Button.Right: return "MOUSERIGHT";
                case Mouse.Button.Middle: return "MOUSEMIDDLE";
                case Mouse.Button.XButton1: return "MOUSE4";
                case Mouse.Button.XButton2: return "MOUSE5";
            }

            return "";
        }

        /// <summary> Converts upper-case string to SFML Keyboard.Key. </summary>
        public static Keyboard.Key StringToKey(string str)
        {
            switch (str)
            {
                case "A":
                    return Keyboard.Key.A;
                case "B":
                    return Keyboard.Key.B;
                case "C":
                    return Keyboard.Key.C;
                case "D":
                    return Keyboard.Key.D;
                case "E":
                    return Keyboard.Key.E;
                case "F":
                    return Keyboard.Key.F;
                case "G":
                    return Keyboard.Key.G;
                case "H":
                    return Keyboard.Key.H;
                case "I":
                    return Keyboard.Key.I;
                case "J":
                    return Keyboard.Key.J;
                case "K":
                    return Keyboard.Key.K;
                case "L":
                    return Keyboard.Key.L;
                case "M":
                    return Keyboard.Key.M;
                case "N":
                    return Keyboard.Key.N;
                case "O":
                    return Keyboard.Key.O;
                case "P":
                    return Keyboard.Key.P;
                case "Q":
                    return Keyboard.Key.Q;
                case "R":
                    return Keyboard.Key.R;
                case "S":
                    return Keyboard.Key.S;
                case "T":
                    return Keyboard.Key.T;
                case "U":
                    return Keyboard.Key.U;
                case "V":
                    return Keyboard.Key.V;
                case "W":
                    return Keyboard.Key.W;
                case "X":
                    return Keyboard.Key.X;
                case "Y":
                    return Keyboard.Key.Y;
                case "Z":
                    return Keyboard.Key.Z;
                case "0":
                case "NUM0":
                    return Keyboard.Key.Num0;
                case "1":
                case "NUM1":
                    return Keyboard.Key.Num1;
                case "2":
                case "NUM2":
                    return Keyboard.Key.Num2;
                case "3":
                case "NUM3":
                    return Keyboard.Key.Num3;
                case "4":
                case "NUM4":
                    return Keyboard.Key.Num4;
                case "5":
                case "NUM5":
                    return Keyboard.Key.Num5;
                case "6":
                case "NUM6":
                    return Keyboard.Key.Num6;
                case "7":
                case "NUM7":
                    return Keyboard.Key.Num7;
                case "8":
                case "NUM8":
                    return Keyboard.Key.Num8;
                case "9":
                case "NUM9":
                    return Keyboard.Key.Num9;
                case "ESCAPE":
                case "ESC":
                    return Keyboard.Key.Escape;
                case "LCONTROL":
                case "CONTROL":
                    return Keyboard.Key.LControl;
                case "LSHIFT":
                case "SHIFT":
                    return Keyboard.Key.LShift;
                case "LALT":
                case "ALT":
                    return Keyboard.Key.LAlt;
                case "LSYSTEM":
                    return Keyboard.Key.LSystem;
                case "RCONTROL":
                    return Keyboard.Key.RControl;
                case "RSHIFT":
                    return Keyboard.Key.RShift;
                case "RALT":
                    return Keyboard.Key.RAlt;
                case "RSYSTEM":
                    return Keyboard.Key.RSystem;
                case "MENU":
                    return Keyboard.Key.Menu;
                case "LBRACKET":
                case "[":
                case "{":
                    return Keyboard.Key.LBracket;
                case "RBRACKET":
                case "]":
                case "}":
                    return Keyboard.Key.RBracket;
                case "SEMICOLON":
                case ";":
                case ":":
                    return Keyboard.Key.SemiColon;
                case "COMMA":
                case "<":
                    return Keyboard.Key.Comma;
                case "PERIOD":
                case ">":
                case ".":
                    return Keyboard.Key.Period;
                case "QUOTE":
                case "\"":
                case "'":
                    return Keyboard.Key.Quote;
                case "SLASH":
                case "?":
                case "/":
                case "QUESTION":
                    return Keyboard.Key.Slash;
                case "BACKSLASH":
                case "|":
                case "\\":
                    return Keyboard.Key.BackSlash;
                case "TILDE":
                case "~":
                case "`":
                    return Keyboard.Key.Tilde;
                case "EQUAL":
                case "PLUS":
                case "+":
                case "=":
                    return Keyboard.Key.Equal;
                case "DASH":
                case "MINUS":
                case "_":
                case "-":
                    return Keyboard.Key.Dash;
                case "SPACE":
                    return Keyboard.Key.Space;
                case "RETURN":
                    return Keyboard.Key.Return;
                case "BACK":
                case "BACKSPACE":
                    return Keyboard.Key.Back;
                case "TAB":
                    return Keyboard.Key.Tab;
                case "PAGEUP":
                case "PGUP":
                    return Keyboard.Key.PageUp;
                case "PAGEDOWN":
                case "PGDOWN":
                case "NEXT":
                    return Keyboard.Key.PageDown;
                case "END":
                    return Keyboard.Key.End;
                case "HOME":
                    return Keyboard.Key.Home;
                case "INSERT":
                case "INS":
                    return Keyboard.Key.Insert;
                case "DELETE":
                case "DEL":
                case "DECIMAL":
                    return Keyboard.Key.Delete;
                case "ADD":
                    return Keyboard.Key.Add;
                case "SUBTRACT":
                    return Keyboard.Key.Subtract;
                case "MULTIPLY":
                    return Keyboard.Key.Multiply;
                case "DIVIDE":
                    return Keyboard.Key.Divide;
                case "LEFT":
                    return Keyboard.Key.Left;
                case "RIGHT":
                    return Keyboard.Key.Right;
                case "UP":
                    return Keyboard.Key.Up;
                case "DOWN":
                    return Keyboard.Key.Down;
                case "NUMPAD0":
                    return Keyboard.Key.Numpad0;
                case "NUMPAD1":
                    return Keyboard.Key.Numpad1;
                case "NUMPAD2":
                    return Keyboard.Key.Numpad2;
                case "NUMPAD3":
                    return Keyboard.Key.Numpad3;
                case "NUMPAD4":
                    return Keyboard.Key.Numpad4;
                case "NUMPAD5":
                    return Keyboard.Key.Numpad5;
                case "NUMPAD6":
                    return Keyboard.Key.Numpad6;
                case "NUMPAD7":
                    return Keyboard.Key.Numpad7;
                case "NUMPAD8":
                    return Keyboard.Key.Numpad8;
                case "NUMPAD9":
                    return Keyboard.Key.Numpad9;
                case "F1":
                    return Keyboard.Key.F1;
                case "F2":
                    return Keyboard.Key.F2;
                case "F3":
                    return Keyboard.Key.F3;
                case "F4":
                    return Keyboard.Key.F4;
                case "F5":
                    return Keyboard.Key.F5;
                case "F6":
                    return Keyboard.Key.F6;
                case "F7":
                    return Keyboard.Key.F7;
                case "F8":
                    return Keyboard.Key.F8;
                case "F9":
                    return Keyboard.Key.F9;
                case "F10":
                    return Keyboard.Key.F10;
                case "F11":
                    return Keyboard.Key.F11;
                case "F12":
                    return Keyboard.Key.F12;
                case "F13":
                    return Keyboard.Key.F13;
                case "F14":
                    return Keyboard.Key.F14;
                case "F15":
                    return Keyboard.Key.F15;
                case "PAUSE":
                    return Keyboard.Key.Pause;
            }
            return Keyboard.Key.Unknown;
        }
        /// <summary> Converts SFML Keyboard.Key to upper-case string. </summary>
        public static string KeyToString(Keyboard.Key key)
        {
            switch (key)
            {
                case Keyboard.Key.A:
	                return "A";
                case Keyboard.Key.B:
	                return "B";
                case Keyboard.Key.C:
	                return "C";
                case Keyboard.Key.D:
	                return "D";
                case Keyboard.Key.E:
	                return "E";
                case Keyboard.Key.F:
	                return "F";
                case Keyboard.Key.G:
	                return "G";
                case Keyboard.Key.H:
	                return "H";
                case Keyboard.Key.I:
	                return "I";
                case Keyboard.Key.J:
	                return "J";
                case Keyboard.Key.K:
	                return "K";
                case Keyboard.Key.L:
	                return "L";
                case Keyboard.Key.M:
	                return "M";
                case Keyboard.Key.N:
	                return "N";
                case Keyboard.Key.O:
	                return "O";
                case Keyboard.Key.P:
	                return "P";
                case Keyboard.Key.Q:
	                return "Q";
                case Keyboard.Key.R:
	                return "R";
                case Keyboard.Key.S:
	                return "S";
                case Keyboard.Key.T:
	                return "T";
                case Keyboard.Key.U:
	                return "U";
                case Keyboard.Key.V:
	                return "V";
                case Keyboard.Key.W:
	                return "W";
                case Keyboard.Key.X:
	                return "X";
                case Keyboard.Key.Y:
	                return "Y";
                case Keyboard.Key.Z:
	                return "Z";
                case Keyboard.Key.Num0:
	                return "0";
                case Keyboard.Key.Num1:
	                return "1";
                case Keyboard.Key.Num2:
	                return "2";
                case Keyboard.Key.Num3:
	                return "3";
                case Keyboard.Key.Num4:
	                return "4";
                case Keyboard.Key.Num5:
	                return "5";
                case Keyboard.Key.Num6:
	                return "6";
                case Keyboard.Key.Num7:
	                return "7";
                case Keyboard.Key.Num8:
	                return "8";
                case Keyboard.Key.Num9:
	                return "9";
                case Keyboard.Key.Escape:
	                return "ESC";
                case Keyboard.Key.LControl:
	                return "LCONTROL";
                case Keyboard.Key.LShift:
	                return "LSHIFT";
                case Keyboard.Key.LAlt:
	                return "LALT";
                case Keyboard.Key.LSystem:
	                return "LSYSTEM";
                case Keyboard.Key.RControl:
	                return "RCONTROL";
                case Keyboard.Key.RShift:
	                return "RSHIFT";
                case Keyboard.Key.RAlt:
	                return "RALT";
                case Keyboard.Key.RSystem:
	                return "RSYSTEM";
                case Keyboard.Key.Menu:
	                return "MENU";
                case Keyboard.Key.LBracket:
	                return "[";
                case Keyboard.Key.RBracket:
	                return "]";
                case Keyboard.Key.SemiColon:
	                return ";";
                case Keyboard.Key.Comma:
	                return "<";
                case Keyboard.Key.Period:
	                return ">";
                case Keyboard.Key.Quote:
	                return "\"";
                case Keyboard.Key.Slash:
	                return "/";
                case Keyboard.Key.BackSlash:
	                return "\\";
                case Keyboard.Key.Tilde:
	                return "~";
                case Keyboard.Key.Equal:
	                return "=";
                case Keyboard.Key.Dash:
	                return "-";
                case Keyboard.Key.Space:
	                return "SPACE";
                case Keyboard.Key.Return:
	                return "RETURN";
                case Keyboard.Key.Back:
	                return "BACK";
                case Keyboard.Key.Tab:
	                return "TAB";
                case Keyboard.Key.PageUp:
	                return "PGUP";
                case Keyboard.Key.PageDown:
	                return "PGDOWN";
                case Keyboard.Key.End:
	                return "END";
                case Keyboard.Key.Home:
	                return "HOME";
                case Keyboard.Key.Insert:
	                return "INSERT";
                case Keyboard.Key.Delete:
	                return "DELETE";
                case Keyboard.Key.Add:
	                return "ADD";
                case Keyboard.Key.Subtract:
	                return "SUBTRACT";
                case Keyboard.Key.Multiply:
	                return "MULTIPLY";
                case Keyboard.Key.Divide:
	                return "DIVIDE";
                case Keyboard.Key.Left:
	                return "LEFT";
                case Keyboard.Key.Right:
	                return "RIGHT";
                case Keyboard.Key.Up:
	                return "UP";
                case Keyboard.Key.Down:
	                return "DOWN";
                case Keyboard.Key.Numpad0:
	                return "NUMPAD0";
                case Keyboard.Key.Numpad1:
	                return "NUMPAD1";
                case Keyboard.Key.Numpad2:
	                return "NUMPAD2";
                case Keyboard.Key.Numpad3:
	                return "NUMPAD3";
                case Keyboard.Key.Numpad4:
	                return "NUMPAD4";
                case Keyboard.Key.Numpad5:
	                return "NUMPAD5";
                case Keyboard.Key.Numpad6:
	                return "NUMPAD6";
                case Keyboard.Key.Numpad7:
	                return "NUMPAD7";
                case Keyboard.Key.Numpad8:
	                return "NUMPAD8";
                case Keyboard.Key.Numpad9:
	                return "NUMPAD9";
                case Keyboard.Key.F1:
	                return "F1";
                case Keyboard.Key.F2:
	                return "F2";
                case Keyboard.Key.F3:
	                return "F3";
                case Keyboard.Key.F4:
	                return "F4";
                case Keyboard.Key.F5:
	                return "F5";
                case Keyboard.Key.F6:
	                return "F6";
                case Keyboard.Key.F7:
	                return "F7";
                case Keyboard.Key.F8:
	                return "F8";
                case Keyboard.Key.F9:
	                return "F9";
                case Keyboard.Key.F10:
	                return "F10";
                case Keyboard.Key.F11:
	                return "F11";
                case Keyboard.Key.F12:
	                return "F12";
                case Keyboard.Key.F13:
	                return "F13";
                case Keyboard.Key.F14:
	                return "F14";
                case Keyboard.Key.F15:
	                return "F15";
                case Keyboard.Key.Pause:
	                return "PAUSE";
            }

            return "";
        }


        private static string VariableAmountOfStrings(int amount, string s)
        {
            if (amount == 0)
                return "";

            string str = "";
            for (int i = 0; i < amount; i++)
                str += s;
            return str;
        }
    }
}