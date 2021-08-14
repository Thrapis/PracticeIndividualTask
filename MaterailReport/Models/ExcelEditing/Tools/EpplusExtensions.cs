using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelEditing.Tools
{
    public static class EpplusExtensions
    {
        public static string GetFixedAddress(this ExcelRange rgn, bool fixedRow = true, bool fixedColumn = true)
        {
            string address = rgn.Address.ToString();

            if (fixedRow)
            {
                var matches = Regex.Matches(address, @"([A-Z]+)").Cast<Match>().Select(m => m.Index).ToArray().Reverse();
                foreach (var match in matches)
                {
                    address = address.Insert(match, "$");
                }
            }

            if (fixedColumn)
            {
                var matches = Regex.Matches(address, @"([0-9]+)").Cast<Match>().Select(m => m.Index).ToArray().Reverse();
                foreach (var match in matches)
                {
                    address = address.Insert(match, "$");
                }
            }
                
            return address;
        }
    }
}
