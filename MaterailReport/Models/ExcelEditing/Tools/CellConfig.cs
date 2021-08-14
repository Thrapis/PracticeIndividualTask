using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelEditing.Tools
{
    public struct CellConfig
    {
        public string FontName;
        public int FontSize;
        public string Format;
        public bool Bold;
        public BorderSide BorderSide;
        public ExcelVerticalAlignment ExcelVerticalAlignment;
        public ExcelHorizontalAlignment ExcelHorizontalAlignment;

        public CellConfig Clone()
        {
            CellConfig cc = new CellConfig();
            cc.FontName = FontName;
            cc.FontSize = FontSize;
            cc.Format = Format;
            cc.Bold = Bold;
            cc.BorderSide = BorderSide;
            cc.ExcelVerticalAlignment = ExcelVerticalAlignment;
            cc.ExcelHorizontalAlignment = ExcelHorizontalAlignment;
            return cc;
        }
    }
}
