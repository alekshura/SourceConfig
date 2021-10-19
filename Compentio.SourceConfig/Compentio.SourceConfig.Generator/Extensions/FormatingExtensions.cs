namespace Compentio.SourceConfig.Extensions
{
    static class FormatingExtensions
    {
        public static string FromatClassName(this string input)
        {
            return ToPascalCase(input).Replace("settings", "Settings");
        }

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
