using System;
using System.IO;
using System.Collections.Generic;
using SFML.Window;

namespace vimage
{
    class Config
    {
        public const string DEFAULT_CONFIG =
@"OpenAtMousePosition = 1
SmoothingDefault = 1
BackgroundForImagesWithTransparencyDefault = 0
LimitImagesToMonitorHeight = 1
PositionLargeWideImagesInCorner = 1 // ie: Desktop Wallpapers and Screenshots
ContextMenuShowMargin = 0

Drag = MOUSELEFT
Close = ESC, BACKSPACE
ContextMenu = MOUSERIGHT
PrevImage = LEFT, PAGE UP, MOUSE4
NextImage = RIGHT, PAGE DOWN, MOUSE5
RotateClockwise = UP
RotateAntiClockwise = DOWN
Flip = F
FitToMonitorHeight = MOUSEMIDDLE
FitToMonitorHeightAlternative = RSHIFT, LSHIFT
ZoomFaster = RSHIFT, LSHIFT
ZoomAlt = RCONTROL, LCONTROL
ToggleSmoothing = S
ToggleBackgroundForTransparency = T
ToggleAlwaysOnTop = L
PauseAnimation = SPACE
PrevFrame = <
NextFrame = >
OpenConfig = O
ReloadConfig = P
ResetImage = R";

        public List<int> Control_Drag;
        public List<int> Control_Close;
        public List<int> Control_ContextMenu;
        public List<int> Control_PrevImage;
        public List<int> Control_NextImage;
        public List<int> Control_RotateClockwise;
        public List<int> Control_RotateAntiClockwise;
        public List<int> Control_Flip;
        public List<int> Control_FitToMonitorHeight;
        public List<int> Control_FitToMonitorHeightAlternative;
        public List<int> Control_ZoomFaster;
        public List<int> Control_ZoomAlt;
        public List<int> Control_ToggleSmoothing;
        public List<int> Control_ToggleBackgroundForTransparency;
        public List<int> Control_ToggleAlwaysOnTop;
        public List<int> Control_PauseAnimation;
        public List<int> Control_PrevFrame;
        public List<int> Control_NextFrame;
        public List<int> Control_OpenConfig;
        public List<int> Control_ReloadConfig;
		public List<int> Control_ResetImage;

        public bool Setting_OpenAtMousePosition { get { return (Boolean)Settings["OPENATMOUSEPOSITION"]; } }
        public bool Setting_SmoothingDefault { get { return (Boolean)Settings["SMOOTHINGDEFAULT"]; } }
        public bool Setting_BackgroundForImagesWithTransparencyDefault { get { return (Boolean)Settings["BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT"]; } }
        public bool Setting_LimitImagesToMonitorHeight { get { return (Boolean)Settings["LIMITIMAGESTOMONITORHEIGHT"]; } }
        public bool Setting_PositionLargeWideImagesInCorner { get { return (Boolean)Settings["POSITIONLARGEWIDEIMAGESINCORNER"]; } }
        public bool Setting_ContextMenuShowMargin { get { return (Boolean)Settings["CONTEXTMENUSHOWMARGIN"]; } }

        private Dictionary<string, object> Settings;

        public const int MouseCodeOffset = 150;

        public Config()
        {
            Init();
        }
        public void Init()
        {
            Control_Drag = new List<int>();
            Control_Close = new List<int>();
            Control_ContextMenu = new List<int>();
            Control_PrevImage = new List<int>();
            Control_NextImage = new List<int>();
            Control_RotateClockwise = new List<int>();
            Control_RotateAntiClockwise = new List<int>();
            Control_Flip = new List<int>();
            Control_FitToMonitorHeight = new List<int>();
            Control_FitToMonitorHeightAlternative = new List<int>();
            Control_ZoomFaster = new List<int>();
            Control_ZoomAlt = new List<int>();
            Control_ToggleSmoothing = new List<int>();
            Control_ToggleBackgroundForTransparency = new List<int>();
            Control_ToggleAlwaysOnTop = new List<int>();
            Control_PauseAnimation = new List<int>();
            Control_PrevFrame = new List<int>();
            Control_NextFrame = new List<int>();
            Control_OpenConfig = new List<int>();
            Control_ReloadConfig = new List<int>();
			Control_ResetImage = new List<int>();

            Settings = new Dictionary<string, object>()
            {
                { "OPENATMOUSEPOSITION", true },
                { "SMOOTHINGDEFAULT", true },
                { "BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT", false },
                { "LIMITIMAGESTOMONITORHEIGHT", true },
                { "POSITIONLARGEWIDEIMAGESINCORNER", true },
                { "CONTEXTMENUSHOWMARGIN", false},

                { "DRAG", Control_Drag },
                { "CLOSE", Control_Close },
                { "CONTEXTMENU", Control_ContextMenu },
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
				{ "RESETIMAGE", Control_ResetImage }
            };
        }

        /// <summary> Parses and loads a config.txt file. If it doesn't exist, a default one will be made. </summary>
        public void Load(string configFile)
        {
            if (!File.Exists(configFile))
            {
                FileStream fileStream =  File.Create(configFile);
                StreamWriter writer = new StreamWriter(fileStream);

                writer.Write(DEFAULT_CONFIG);

                writer.Close();
            }

            StreamReader reader = File.OpenText(configFile);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Equals(""))
                    continue;

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
                    continue;

                // split values by commas
                string[] values = nameValue[1].Split(',');

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
                else if (Settings[name] is Boolean)
                {
                    // Boolean
                    if (values[0].Equals("1") || values[0].ToUpper().Equals("T") || values[0].ToUpper().Equals("TRUE"))
                        Settings[name] = true;
                    else
                        Settings[name] = false;
                }
            }

            reader.Close();
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
                    return Keyboard.Key.LControl;
                case "LSHIFT":
                    return Keyboard.Key.LShift;
                case "LALT":
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
                case "(":
                    return Keyboard.Key.LBracket;
                case "RBRACKET":
                case ")":
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
                case "+":
                case "=":
                    return Keyboard.Key.Equal;
                case "DASH":
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
                    return Keyboard.Key.PageUp;
                case "PAGEDOWN":
                    return Keyboard.Key.PageDown;
                case "END":
                    return Keyboard.Key.End;
                case "HOME":
                    return Keyboard.Key.Home;
                case "INSERT":
                    return Keyboard.Key.Insert;
                case "DELETE":
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
    }
}