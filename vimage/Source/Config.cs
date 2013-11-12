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

Close = ESC, BACKSPACE
PrevImage = LEFT, PAGE UP
NextImage = RIGHT, PAGE DOWN
RotateClockwise = UP
RotateAntiClockwise = DOWN
Flip = F
ZoomFaster = RSHIFT, LSHIFT
ZoomInOnCenter = RCONTROL, LCONTROL
ToggleSmoothing = S
ToggleBackgroundForTransparency = T
PauseAnimation = SPACE
PrevFrame = <
NextFrame = >";

        public List<Keyboard.Key> Control_Close = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_PrevImage = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_NextImage = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_RotateClockwise = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_RotateAntiClockwise = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_Flip = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_ZoomFaster = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_ZoomInOnCenter = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_ToggleSmoothing = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_ToggleBackgroundForTransparency = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_PauseAnimation = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_PrevFrame = new List<Keyboard.Key>();
        public List<Keyboard.Key> Control_NextFrame = new List<Keyboard.Key>();

        public bool Setting_OpenAtMousePosition { get { return (Boolean)Settings["OPENATMOUSEPOSITION"]; } }
        public bool Setting_SmoothingDefault { get { return (Boolean)Settings["SMOOTHINGDEFAULT"]; } }
        public bool Setting_BackgroundForImagesWithTransparencyDefault { get { return (Boolean)Settings["BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT"]; } }

        private Dictionary<string, object> Settings;

        public Config()
        {
            Settings = new Dictionary<string, object>()
            {
                { "OPENATMOUSEPOSITION", true },
                { "SMOOTHINGDEFAULT", true },
                { "BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT", false },
                { "CLOSE", Control_Close },
                { "PREVIMAGE", Control_PrevImage },
                { "NEXTIMAGE", Control_NextImage },
                { "ROTATECLOCKWISE", Control_RotateClockwise },
                { "ROTATEANTICLOCKWISE", Control_RotateAntiClockwise },
                { "FLIP", Control_Flip },
                { "ZOOMFASTER", Control_ZoomFaster },
                { "ZOOMINONCENTER", Control_ZoomInOnCenter },
                { "TOGGLESMOOTHING", Control_ToggleSmoothing },
                { "TOGGLEBACKGROUNDFORTRANSPARENCY", Control_ToggleBackgroundForTransparency },
                { "PAUSEANIMATION", Control_PauseAnimation },
                { "PREVFRAME", Control_PrevFrame },
                { "NEXTFRAME", Control_NextFrame },
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

                if (Settings[name] is List<Keyboard.Key>)
                {
                    // Control
                    List<Keyboard.Key> list = (List<Keyboard.Key>)Settings[name];
                    for (int i = 0; i < values.Length; i++)
                    {
                        Keyboard.Key key = StringToKey(values[i].ToUpper());
                        if (key != Keyboard.Key.Unknown)
                            list.Add(key);
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
        }

        /// <summary> Returns true if keyCode is one of Control bindings. </summary>
        public static bool IsControl(Keyboard.Key keyCode, List<Keyboard.Key> Control)
        {
            foreach (Keyboard.Key key in Control)
            {
                if (keyCode == key)
                    return true;
            }
            return false;
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