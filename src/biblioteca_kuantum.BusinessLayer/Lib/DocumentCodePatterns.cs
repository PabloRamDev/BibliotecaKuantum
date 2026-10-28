
using System.Globalization;
using System.Text.RegularExpressions;

namespace biblioteca_kuantum.BusinessLayer.Lib
{
    /*Claude Prompt:need this validations with regex for FluentValidation:

Prefijo “ISO-” seguido de un número.EJ: “ISO-27001”
- Prefijo “Ley N° ” seguido de un número con separador de miles. EJ: “Ley
N° 19.628”
- Prefijo "P-” seguido de un texto “XX.[AAAAMMDD]”, donde XX es el
número de la versión (EJ: 01), AAAA un año, MM, meses del 01 al 12 y
DD días del 01 al 31. EJ: (P-01.20250614) */

    public static partial class DocumentCodePatterns
    {
        [GeneratedRegex(@"^ISO-[0-9]+$")]
        public static partial Regex Iso();

        [GeneratedRegex(@"^Ley N\u00B0 [1-9][0-9]{0,2}(\.[0-9]{3})*$")]
        public static partial Regex Ley();

        [GeneratedRegex(@"^P-[0-9]{2}\.[0-9]{4}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])$")]
        public static partial Regex Prefix();

        public static bool HasValidPrefixDate(string code) =>
            DateTime.TryParseExact(
                code.Substring(5, 8),          // "P-01." is 5 chars, then yyyyMMdd
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
    }
}
