using System;
using System.Collections.Generic;
using SFML.Window;

namespace vimage
{
    internal class Controls
    {
        private const string COMBO_PLUS = "+";
        public const string MOUSE_SCROLL_UP = "SCROLLUP";
        public const string MOUSE_SCROLL_DOWN = "SCROLLDOWN";

        /// <summary> Returns true if code is one of Control bindings. </summary>
        public static bool Equals(object code, List<string> Control, bool onlyIfKeyCombo = false)
        {
            if (Control.Count == 0)
                return false;

            var codeID = CodeToString(code);
            int index = Control.IndexOf(codeID);
            if (index == -1)
                return false;
            bool value;
            do
            {
                // Check if the current index is a combo modifier (e.g., CTRL, SHIFT)
                bool isComboModifier = index >= 1 && Control[index - 1] == COMBO_PLUS;

                if (isComboModifier)
                {
                    value = false;
                }
                else if (index > 1 && Control[index - 2] == COMBO_PLUS)
                {
                    // This is a key combo, check if the key is pressed
                    if (TryParseKey(Control[index - 1], out var key))
                        value = Keyboard.IsKeyPressed(key);
                    else
                        value = false;
                }
                else
                {
                    value = !onlyIfKeyCombo;
                }

                // loop if there might be second binding using the same keyCode (eg: CTRL+UP and RCTRL+UP)
                index = !value ? Control.IndexOf(codeID, index + 1) : -1;
            } while (index != -1);

            return value;
        }

        public static string CodeToString(object code)
        {
            if (code is Mouse.Button)
            {
                return code switch
                {
                    Mouse.Button.Left => "MOUSELEFT",
                    Mouse.Button.Right => "MOUSERIGHT",
                    Mouse.Button.Middle => "MOUSEMIDDLE",
                    Mouse.Button.XButton1 => "MOUSE4",
                    Mouse.Button.XButton2 => "MOUSE5",
                    _ => "",
                };
            }

            return code.ToString()?.ToUpperInvariant() ?? "";
        }

        public static bool TryParseKey(string value, out Keyboard.Key output)
        {
            var parsed = value switch
            {
                "<" => Keyboard.Key.Comma,
                ">" => Keyboard.Key.Period,
                _ => Keyboard.Key.Unknown,
            };

            if (parsed != Keyboard.Key.Unknown)
            {
                output = parsed;
                return true;
            }

            return Enum.TryParse(value, out output);
        }
    }
}
