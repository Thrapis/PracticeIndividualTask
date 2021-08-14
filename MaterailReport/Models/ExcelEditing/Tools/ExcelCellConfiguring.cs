using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelEditing.Tools
{
    public static class ExcelCellConfiguring
    {
		public static void MakeHeader(ExcelRange er, object value = null)
		{
			string simpleFont = "Times New Roman";
			er.Value = value;
			er.Style.Numberformat.Format = "General";
			er.Style.Font.Size = 10;
			er.Style.Font.Name = simpleFont;
			er.Style.Font.Bold = true;
			er.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			er.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
			er.Style.WrapText = true;
			er.Style.Border.BorderAround(ExcelBorderStyle.Thin);
		}

		public static void MakeCustom(ExcelRange er, CellConfig cellConfig, object value = null)
		{
			er.Value = value;
			er.Style.Font.Size = cellConfig.FontSize;
			er.Style.Font.Name = cellConfig.FontName;
			er.Style.HorizontalAlignment = cellConfig.ExcelHorizontalAlignment;
			er.Style.VerticalAlignment = cellConfig.ExcelVerticalAlignment;
			er.Style.Font.Bold = cellConfig.Bold;
			er.Style.WrapText = true;
			switch (cellConfig.BorderSide)
			{
				case BorderSide.None: break;
				case BorderSide.Top: er.Style.Border.Top.Style = ExcelBorderStyle.Thin; break;
				case BorderSide.Right: er.Style.Border.Right.Style = ExcelBorderStyle.Thin; break;
				case BorderSide.Bottom: er.Style.Border.Bottom.Style = ExcelBorderStyle.Thin; break;
				case BorderSide.Left: er.Style.Border.Left.Style = ExcelBorderStyle.Thin; break;
				case BorderSide.Around: er.Style.Border.BorderAround(ExcelBorderStyle.Thin); break;
			}
			er.Style.Numberformat.Format = cellConfig.Format;
		}
	}
}
