using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StubhubNotifierFunction.Extensions
{
    public static class StringExtensions
    {
        public static List<string> FromCSV(this string item, char separator)
        {
            var rVal = new List<string>();
            if(string.IsNullOrWhiteSpace(item) == false)
            {
                rVal = item.Contains(separator) ? item.Split(separator).ToList() : new List<string> { item };
            }
            return rVal;
        }
        public static bool To<T>(this string str, out T o)
        {
            try
            {
                var t = typeof(T);
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    throw new InvalidOperationException("To<T> extension method does not work reliably with nullable types.");
                }

                o = (T)Convert.ChangeType(str, typeof(T));
                return o != null;
            }
            catch
            {
            }

            o = default(T);
            return false;
        }
    }
}
