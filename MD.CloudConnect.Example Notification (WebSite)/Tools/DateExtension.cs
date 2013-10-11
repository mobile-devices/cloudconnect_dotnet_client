using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Tools
{
    public static class DateExtension
    {
        public static int GenerateKey(this DateTime date)
        {
            return (int)(date.Year * 10000 + date.Month * 100 + date.Day);
        }
    }
}