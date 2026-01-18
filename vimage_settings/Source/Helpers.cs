using System.Text.RegularExpressions;

namespace vimage_settings
{
    public static partial class Helpers
    {
        public static string SplitCamelCase(this string str)
        {
            return SplitCamelCaseRegexFind()
                .Replace(SplitCamelCaseRegexReplace().Replace(str, "$1 $2"), "$1 $2");
        }

        [GeneratedRegex(@"(\p{Ll})(\P{Ll})")]
        private static partial Regex SplitCamelCaseRegexFind();

        [GeneratedRegex(@"(\P{Ll})(\P{Ll}\p{Ll})")]
        private static partial Regex SplitCamelCaseRegexReplace();
    }
}
