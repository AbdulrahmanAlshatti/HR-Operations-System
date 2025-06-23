using System.Globalization;
using System.Runtime.CompilerServices;

namespace HR_Operations_System.Business
{
    public static class ExtensionFunctions
    {
        public static string ToMyDate(this DateTime dateTime) => dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }
}
