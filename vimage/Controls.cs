using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using vimage.Common;

namespace vimage
{
    public abstract record ControlInput;

    public record KeyInput(Keyboard.Key Key) : ControlInput;

    public record MouseInput(Mouse.Button Button) : ControlInput;

    public record MouseWheelInput(MouseWheel Wheel) : ControlInput;

    public struct ControlBinding
    {
        public List<(ActionFunc Action, List<ControlInput> ComboKeys)> Actions;
    }

    internal class Controls(Config config)
    {
        private Dictionary<ControlInput, ControlBinding> Bindings = ParseBindings(config);

        public void Update(Config config) => Bindings = ParseBindings(config);

        private static Dictionary<ControlInput, ControlBinding> ParseBindings(Config config)
        {
            var bindings = new Dictionary<ControlInput, ControlBinding>();
            foreach (var c in config.Controls)
                ParseBinding(new ActionEnum(c.Key), c.Value, bindings);
            foreach (var c in config.CustomActionBindings)
                ParseBinding(new CustomAction(c.Key), c.Value, bindings);
            return bindings;
        }

        private static void ParseBinding(
            ActionFunc action,
            List<string> values,
            Dictionary<ControlInput, ControlBinding> bindings
        )
        {
            foreach (var value in values)
            {
                ControlInput? key;
                List<ControlInput> keys = [];
                if (value.Contains('+'))
                {
                    // Combo
                    var v = value.Split('+');
                    foreach (var str in v)
                    {
                        var b = ParseControlInput(str);
                        if (b != null)
                            keys.Add(b);
                    }
                    if (keys.Count <= 0)
                        continue;

                    key = keys.Last();
                    keys.RemoveAt(keys.Count - 1);
                }
                else
                {
                    key = ParseControlInput(value);
                }
                if (key == null)
                    continue;

                if (!bindings.TryAdd(key, new ControlBinding() { Actions = [(action, keys)] }))
                    bindings[key].Actions.Add((action, keys));
            }
        }

        /// <summary>
        /// Returns the action that should happen based on control input
        /// </summary>
        /// <param name="value"></param>
        /// <param name="controlDown">If true; will only return bindings that can occur while a key/button is down.</param>
        public ActionFunc? GetActionFromInput(ControlInput input, bool controlDown = false)
        {
            if (!Bindings.TryGetValue(input, out var binding))
                return null;
            ActionFunc? action = null;
            int comboLength = 0;
            foreach (var b in binding.Actions)
            {
                // Skip if action is not a hold-down action (if controlDown is true)
                if (
                    controlDown
                    && (b.Action is not ActionEnum a || !Actions.HoldDownActions.Contains(a.Value))
                )
                    continue;
                // Skip if action is a modifier action
                else if (b.Action is ActionEnum m && Actions.ModifierActions.Contains(m.Value))
                    continue;

                // Skip if we already have an action unless this one is a longer combo
                if (action != null && comboLength >= b.ComboKeys?.Count)
                    continue;

                if (b.ComboKeys == null || b.ComboKeys.Count <= 0)
                {
                    action = b.Action;
                    continue;
                }

                var comboKeysActive = true;
                foreach (var comboKey in b.ComboKeys)
                {
                    if (comboKey is not KeyInput keyInput)
                        continue;
                    if (Keyboard.IsKeyPressed(keyInput.Key))
                        continue;
                    comboKeysActive = false;
                    break;
                }
                if (!comboKeysActive)
                    continue;

                action = b.Action;
                comboLength = b.ComboKeys.Count;
            }
            return action;
        }

        /// <summary>
        /// Returns the modifier actions that should happen based on control input
        /// </summary>
        /// <param name="value"></param>
        public List<Action> GetModifierActionsFromInput(ControlInput input)
        {
            if (!Bindings.TryGetValue(input, out var binding))
                return [];
            List<Action> actions = [];
            foreach (var b in binding.Actions)
            {
                if (b.Action is not ActionEnum a || !Actions.ModifierActions.Contains(a.Value))
                    continue;
                actions.Add(a.Value);
            }
            return actions;
        }

        public static ControlInput? ParseControlInput(string value)
        {
            ControlInput? parsed = value switch
            {
                "MOUSELEFT" or "MOUSE1" => new MouseInput(Mouse.Button.Left),
                "MOUSERIGHT" or "MOUSE2" => new MouseInput(Mouse.Button.Right),
                "MOUSEMIDDLE" or "MOUSE3" => new MouseInput(Mouse.Button.Middle),
                "MOUSEX1" or "MOUSEXBUTTON1" or "MOUSE4" => new MouseInput(Mouse.Button.XButton1),
                "MOUSEX2" or "MOUSEXBUTTON2" or "MOUSE5" => new MouseInput(Mouse.Button.XButton2),
                "SCROLLUP" => new MouseWheelInput(MouseWheel.ScrollUp),
                "SCROLLDOWN" => new MouseWheelInput(MouseWheel.ScrollDown),

                "0" or "NUM0" => new KeyInput(Keyboard.Key.Num0),
                "1" or "NUM1" => new KeyInput(Keyboard.Key.Num1),
                "2" or "NUM2" => new KeyInput(Keyboard.Key.Num2),
                "3" or "NUM3" => new KeyInput(Keyboard.Key.Num3),
                "4" or "NUM4" => new KeyInput(Keyboard.Key.Num4),
                "5" or "NUM5" => new KeyInput(Keyboard.Key.Num5),
                "6" or "NUM6" => new KeyInput(Keyboard.Key.Num6),
                "7" or "NUM7" => new KeyInput(Keyboard.Key.Num7),
                "8" or "NUM8" => new KeyInput(Keyboard.Key.Num8),
                "9" or "NUM9" => new KeyInput(Keyboard.Key.Num9),
                "ESCAPE" or "ESC" => new KeyInput(Keyboard.Key.Escape),
                "CTRL" or "CONTROL" or "LCTRL" or "LEFTCTRL" or "LCONTROL" or "LEFTCONTROL" =>
                    new KeyInput(Keyboard.Key.LControl),
                "SHIFT" or "LSHIFT" or "LEFTSHIFT" => new KeyInput(Keyboard.Key.LShift),
                "ALT" or "LALT" or "LEFTALT" => new KeyInput(Keyboard.Key.LAlt),
                "LSYSTEM" or "LEFTSYSTEM" => new KeyInput(Keyboard.Key.LSystem),
                "RCTRL" or "RIGHTCTRL" or "RCONTROL" or "RIGHTCONTROL" => new KeyInput(
                    Keyboard.Key.RControl
                ),
                "RSHIFT" or "RIGHTSHIFT" => new KeyInput(Keyboard.Key.RShift),
                "RALT" or "RIGHTALT" => new KeyInput(Keyboard.Key.RAlt),
                "RSYSTEM" or "RIGHTSYSTEM" => new KeyInput(Keyboard.Key.RSystem),
                "LBRACKET" or "[" or "{" => new KeyInput(Keyboard.Key.LBracket),
                "RBRACKET" or "]" or "}" => new KeyInput(Keyboard.Key.RBracket),
                "SEMICOLON" or ";" or ":" => new KeyInput(Keyboard.Key.Semicolon),
                "COMMA" or "<" => new KeyInput(Keyboard.Key.Comma),
                "PERIOD" or ">" or "." => new KeyInput(Keyboard.Key.Period),
                "QUOTE" or "APOSTROPHE" or "\"" or "'" => new KeyInput(Keyboard.Key.Apostrophe),
                "SLASH" or "?" or "/" or "QUESTION" => new KeyInput(Keyboard.Key.Slash),
                "BACKSLASH" or "|" or "\\" => new KeyInput(Keyboard.Key.Backslash),
                "TILDE" or "GRAVE" or "~" or "`" => new KeyInput(Keyboard.Key.Grave),
                "EQUAL" or "PLUS" or "=" => new KeyInput(Keyboard.Key.Equal),
                "DASH" or "MINUS" or "_" or "-" => new KeyInput(Keyboard.Key.Hyphen),
                "RETURN" or "ENTER" => new KeyInput(Keyboard.Key.Enter),
                "BACK" or "BACKSPACE" => new KeyInput(Keyboard.Key.Backspace),
                "PAGEUP" or "PGUP" => new KeyInput(Keyboard.Key.PageUp),
                "PAGEDOWN" or "PGDOWN" or "NEXT" => new KeyInput(Keyboard.Key.PageDown),
                "INSERT" or "INS" => new KeyInput(Keyboard.Key.Insert),
                "DELETE" or "DEL" or "DECIMAL" => new KeyInput(Keyboard.Key.Delete),
                _ => null,
            };
            if (parsed != null)
                return parsed;

            // Fallback to parsed keyboard key enum name
            if (System.Enum.TryParse<Keyboard.Key>(value, true, out var key))
                return new KeyInput(key);

            return null;
        }
    }
}
