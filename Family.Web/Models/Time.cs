using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Family.Web.Models
{
    public class Time
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        /// <summary>
        /// Retrieves the string value between a start and end value in a string
        /// </summary>
        /// <param name="strSource">The original string</param>
        /// <param name="strStart">The start of the value to return</param>
        /// <param name="strEnd">The end of the value to return</param>
        /// <returns></returns>
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
    }
}