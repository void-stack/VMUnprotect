using System;
using System.Collections;
using System.Linq;

namespace VMUnprotect.Core.Helpers {
    public static class Formatter {
        /// <summary>
        ///     Formats code to more reliable format
        /// </summary>
        public static string FormatObject(object obj) {
            try {
                return obj switch {
                    null                   => "null",
                    string x               => $"\"{x}\"",
                    IEnumerable enumerable => $"{obj.GetType().Name} {{{string.Join(", ", enumerable.Cast<object>().Select(FormatObject))}}}",
                    var _                  => obj.ToString()
                };
            }
            catch (Exception) {
                return "???";
            }
        }
    }
}