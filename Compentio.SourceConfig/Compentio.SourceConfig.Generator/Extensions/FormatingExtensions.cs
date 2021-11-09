namespace Compentio.SourceConfig.Extensions
{
    /// <summary>
    /// Helper extensions used for objects formattings
    /// </summary>
    static class FormatingExtensions
    {
        /// <summary>
        /// Builds class name with using of Pascal case for settings file name.
        /// </summary>
        /// <param name="input">File name</param>
        /// <returns></returns>
        public static string FromatClassName(this string input)
        {
            return ToPascalCase(input).Replace("settings", "Settings");
        }

        /// <summary>
        /// Formats property name from settings file. It generates Pascal case names and replaces '.' and '$' characters from input string.  
        /// </summary>
        /// <param name="input">Property name to be formatted</param>
        /// <returns></returns>
        public static string FromatPropertyName(this string input)
        {
            var underscore = "_";
            var tmp = input
                .Replace(".", underscore)
                .Replace("$", underscore);

            if (tmp[0].Equals(underscore))
                tmp.Replace(underscore, string.Empty);

            return ToPascalCase(tmp);
        }

        private static string ToPascalCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            if (input.Length == 1)
                return input.ToUpper();

            return string.Concat(input[0].ToString().ToUpper(), input.Substring(1));
        }
    }
}
