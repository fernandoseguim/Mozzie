using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozzie.Domain
{
    public static class DocumentTypes
    {
        private static IEnumerable<string> Labels => new List<string> { "document", "drivinglicense", "idcards", "license" };

        public static bool IsValidDocument(string name)
        {
            return Labels.Any(label => label.Equals(name.WithoutWhiteSpaces(), StringComparison.InvariantCultureIgnoreCase));
        }
    }

    internal static class StringExtensions
    {
        public static string WithoutWhiteSpaces(this string label)
        {
            return label.Trim().Replace(" ", "");
        }
    }
}
