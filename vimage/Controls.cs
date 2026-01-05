using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using vimage.Common;

namespace vimage
{
    public abstract record Binding;

    public record KeyBinding(Keyboard.Key Key) : Binding;

    public record MouseBinding(Mouse.Button Button) : Binding;

    public record MouseWheelBinding(MouseWheel Wheel) : Binding;

    public struct ControlBinding
    {
        public List<(ActionFunc Action, List<Binding> ComboKeys)> Actions;
    }

    internal class Controls(Config config)
    {
        private Dictionary<Binding, ControlBinding> Bindings = ParseBindings(config);

        public void Update(Config config) => Bindings = ParseBindings(config);

        private static Dictionary<Binding, ControlBinding> ParseBindings(Config config)
        {
            var bindings = new Dictionary<Binding, ControlBinding>();

            foreach (var c in config.Controls)
            {
                foreach (var value in c.Value)
                {
                    Binding? key;
                    List<Binding> keys = [];
                    if (value.Contains('+'))
                    {
                        // Combo
                        var v = value.Split('+');
                        foreach (var str in v)
                        {
                            var b = ParseBinding(str);
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
                        key = ParseBinding(value);
                    }
                    if (key == null)
                        continue;

                    var newAction = (new ActionEnum(c.Key), keys);
                    if (!bindings.TryAdd(key, new ControlBinding() { Actions = [newAction] }))
                        bindings[key].Actions.Add(newAction);
                }
            }

            return bindings;
        }

        /// <summary>
        /// Returns all the actions that should happen based on control bindings
        /// </summary>
        /// <param name="value"></param>
        /// <param name="controlDown">If true; will only return bindings that can occur while a key/button is down as well as any modifier keys.</param>
        public List<ActionFunc> GetActionsFromBinding(Binding value, bool controlDown = false)
        {
            if (!Bindings.TryGetValue(value, out var binding))
                return [];
            List<ActionFunc> actions = [];
            foreach (var b in binding.Actions)
            {
                if (controlDown)
                {
                    // Skip if action is not a hold-down or modifier action
                    if (b.Action is not ActionEnum a)
                        continue;
                    if (
                        !(
                            Actions.HoldDownActions.Contains(a.Value)
                            || Actions.ModiferActions.Contains(a.Value)
                        )
                    )
                    {
                        continue;
                    }
                }

                if (b.ComboKeys == null || b.ComboKeys.Count <= 0)
                {
                    actions.Add(b.Action);
                    continue;
                }

                var comboKeysActive = true;
                foreach (var comboKey in b.ComboKeys)
                {
                    if (comboKey is not KeyBinding keyBinding)
                        continue;
                    if (Keyboard.IsKeyPressed(keyBinding.Key))
                        continue;
                    comboKeysActive = false;
                    break;
                }
                if (!comboKeysActive)
                    continue;

                actions.Add(b.Action);
            }
            return actions;
        }

        public static Binding? ParseBinding(string value)
        {
            Binding? parsed = value switch
            {
                "MOUSELEFT" or "MOUSE1" => new MouseBinding(Mouse.Button.Left),
                "MOUSERIGHT" or "MOUSE2" => new MouseBinding(Mouse.Button.Right),
                "MOUSEMIDDLE" or "MOUSE3" => new MouseBinding(Mouse.Button.Middle),
                "MOUSEX1" or "MOUSEXBUTTON1" or "MOUSE4" => new MouseBinding(Mouse.Button.XButton1),
                "MOUSEX2" or "MOUSEXBUTTON2" or "MOUSE5" => new MouseBinding(Mouse.Button.XButton2),
                "SCROLLUP" => new MouseWheelBinding(MouseWheel.ScrollUp),
                "SCROLLDOWN" => new MouseWheelBinding(MouseWheel.ScrollDown),

                "0" or "NUM0" => new KeyBinding(Keyboard.Key.Num0),
                "1" or "NUM1" => new KeyBinding(Keyboard.Key.Num1),
                "2" or "NUM2" => new KeyBinding(Keyboard.Key.Num2),
                "3" or "NUM3" => new KeyBinding(Keyboard.Key.Num3),
                "4" or "NUM4" => new KeyBinding(Keyboard.Key.Num4),
                "5" or "NUM5" => new KeyBinding(Keyboard.Key.Num5),
                "6" or "NUM6" => new KeyBinding(Keyboard.Key.Num6),
                "7" or "NUM7" => new KeyBinding(Keyboard.Key.Num7),
                "8" or "NUM8" => new KeyBinding(Keyboard.Key.Num8),
                "9" or "NUM9" => new KeyBinding(Keyboard.Key.Num9),
                "ESCAPE" or "ESC" => new KeyBinding(Keyboard.Key.Escape),
                "CTRL" or "CONTROL" or "LCTRL" or "LEFTCTRL" or "LCONTROL" or "LEFTCONTROL" =>
                    new KeyBinding(Keyboard.Key.LControl),
                "SHIFT" or "LSHIFT" or "LEFTSHIFT" => new KeyBinding(Keyboard.Key.LShift),
                "ALT" or "LALT" or "LEFTALT" => new KeyBinding(Keyboard.Key.LAlt),
                "LSYSTEM" or "LEFTSYSTEM" => new KeyBinding(Keyboard.Key.LSystem),
                "RCTRL" or "RIGHTCTRL" or "RCONTROL" or "RIGHTCONTROL" => new KeyBinding(
                    Keyboard.Key.RControl
                ),
                "RSHIFT" or "RIGHTSHIFT" => new KeyBinding(Keyboard.Key.RShift),
                "RALT" or "RIGHTALT" => new KeyBinding(Keyboard.Key.RAlt),
                "RSYSTEM" or "RIGHTSYSTEM" => new KeyBinding(Keyboard.Key.RSystem),
                "LBRACKET" or "[" or "{" => new KeyBinding(Keyboard.Key.LBracket),
                "RBRACKET" or "]" or "}" => new KeyBinding(Keyboard.Key.RBracket),
                "SEMICOLON" or ";" or ":" => new KeyBinding(Keyboard.Key.Semicolon),
                "COMMA" or "<" => new KeyBinding(Keyboard.Key.Comma),
                "PERIOD" or ">" or "." => new KeyBinding(Keyboard.Key.Period),
                "QUOTE" or "APOSTROPHE" or "\"" or "'" => new KeyBinding(Keyboard.Key.Apostrophe),
                "SLASH" or "?" or "/" or "QUESTION" => new KeyBinding(Keyboard.Key.Slash),
                "BACKSLASH" or "|" or "\\" => new KeyBinding(Keyboard.Key.Backslash),
                "TILDE" or "GRAVE" or "~" or "`" => new KeyBinding(Keyboard.Key.Grave),
                "EQUAL" or "PLUS" or "=" => new KeyBinding(Keyboard.Key.Equal),
                "DASH" or "MINUS" or "_" or "-" => new KeyBinding(Keyboard.Key.Hyphen),
                "RETURN" or "ENTER" => new KeyBinding(Keyboard.Key.Enter),
                "BACK" or "BACKSPACE" => new KeyBinding(Keyboard.Key.Backspace),
                "PAGEUP" or "PGUP" => new KeyBinding(Keyboard.Key.PageUp),
                "PAGEDOWN" or "PGDOWN" or "NEXT" => new KeyBinding(Keyboard.Key.PageDown),
                "INSERT" or "INS" => new KeyBinding(Keyboard.Key.Insert),
                "DELETE" or "DEL" or "DECIMAL" => new KeyBinding(Keyboard.Key.Delete),
                _ => null,
            };
            if (parsed != null)
                return parsed;

            // Fallback to parsed keyboard key enum name
            if (System.Enum.TryParse<Keyboard.Key>(value, true, out var key))
                return new KeyBinding(key);

            return null;
        }
    }
}
