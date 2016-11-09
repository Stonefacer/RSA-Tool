using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;

namespace AbvMathLib {
    public static class StringExtends {
        public static string TrimByRegExp(this string str, string regexp) {
            var mt = Regex.Match(str, regexp);
            if (!mt.Success || mt.Groups.Count < 2)
                return "";
            return mt.Groups[1].Value;
        }

        public static string ToStringWithBytesCount(this string str) {
            BigInteger bi = BigInteger.Parse(str), ts = BigInteger.Pow(1024, 4);
            string [] res = new string[]{" ТБ", " ГБ", " МБ", " КБ", " Б"};
            for (int i = 0; i < 5; i++) {
                if (bi > ts) {
                    bi = bi / ts;
                    if (bi > 1024)
                        return ">1000"+res[i];
                    else
                        return bi.ToString() + res[i];
                }
                ts >>= 10;
            }
            return "ХЗ";
        }
    }
}
