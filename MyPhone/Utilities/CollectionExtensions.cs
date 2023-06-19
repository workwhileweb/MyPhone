using System.Collections.Generic;
using System.Text;

namespace GoodTimeStudio.MyPhone.Utilities
{
    public static class CollectionExtensions
    {
        public static string ContentToString<T>(this List<T> list)
        {
            var sb = new StringBuilder();
            sb.Append("[ ");
            foreach (var item in list)
            {
                sb.Append(item);
                sb.Append(", ");
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
