using ExcelEditing.Tools;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace MaterailReport.Models.ExcelEditing
{
    public static class ReportExcelFile
    {
		public static string CreateFile(string post, string fullName, string fileName, List<int> idList)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			ReportPackage reportPackage = new ReportPackage(idList);

			if (File.Exists($"C:\\XSLXReports\\{fileName}.xlsx"))
				File.Delete($"C:\\XSLXReports\\{fileName}.xlsx");

			using (ExcelPackage excel = new ExcelPackage())
			{
				ExcelWorksheet ws;
				int rowCount = reportPackage.FieldCount;
				int currentRow = 1;

				ws = excel.Workbook.Worksheets.Add("Основные");
				Configure_First(ws);
				currentRow = 3;
				for (int i = 1; i <= rowCount; i++, currentRow++)
				{
					SetRow_First(ws, currentRow, i, reportPackage.PackageFields[i - 1]);
				}
				MakeSignature(ws, currentRow + 2, post, fullName);


				ws = excel.Workbook.Worksheets.Add("По типу");
				Configure_Second(ws);
				ws.Cells[$"A1:H{rowCount + 1}"].AutoFilter = true;
				currentRow = 2;
				for (int i = 1; i <= rowCount; i++, currentRow++)
				{
					SetRow_Second(ws, currentRow, i, reportPackage.PackageFields[i - 1]);
				}


				ws = excel.Workbook.Worksheets.Add("Составляющие компоненты");
				Configure_Third(ws, reportPackage.ComponentTypes, reportPackage.FieldCount);
				ws.Cells[1, 1, 2 + rowCount, 4 + reportPackage.ComponentTypes.Count].AutoFilter = true;
				currentRow = 3;
				for (int i = 1; i <= rowCount; i++, currentRow++)
				{
					SetRow_Third(ws, reportPackage.ComponentTypes, currentRow, i, reportPackage.PackageFields[i - 1]);
				}

				foreach (string systemType in reportPackage.SystemTypes)
                {
					ws = excel.Workbook.Worksheets.Add(systemType.ToUpper());
					List<ReportPackageField> fields = reportPackage.GetPackageFieldsBySystemType(systemType);
					Configure_Addition(ws);
					ws.Cells[$"A1:H{rowCount + 1}"].AutoFilter = true;
					currentRow = 2;
					rowCount = fields.Count;
					for (int i = 1; i <= rowCount; i++, currentRow++)
					{
						SetRow_Addition(ws, currentRow, i, fields[i - 1]);
					}
				}

				excel.SaveAs(new FileInfo($"C:\\XSLXReports\\{fileName}.xlsx"));
				return $"C:\\XSLXReports\\{fileName}.xlsx";
			}
		}

		private static void SetRow_Addition(ExcelWorksheet ws, int row, int number, ReportPackageField packageField)
		{
			CellConfig cellConfig_gen = new CellConfig();
			cellConfig_gen.FontName = "Times New Roman";
			cellConfig_gen.FontSize = 10;
			cellConfig_gen.ExcelHorizontalAlignment = ExcelHorizontalAlignment.Center;
			cellConfig_gen.ExcelVerticalAlignment = ExcelVerticalAlignment.Center;
			cellConfig_gen.BorderSide = BorderSide.Around;
			cellConfig_gen.Format = "General";
			CellConfig cellConfig_date = cellConfig_gen.Clone();
			cellConfig_date.Format = "dd.mm.yyyy";
			CellConfig cellConfig_double = cellConfig_gen.Clone();
			cellConfig_double.Format = "0.00";

			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 1], cellConfig_gen, number);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 2], cellConfig_gen, packageField.DecommissionedContent.Inventory_Id);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 3], cellConfig_gen, packageField.MaterialObject.System_Type);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 4], cellConfig_gen, packageField.MaterialObject.Name);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 5], cellConfig_date, packageField.DecommissionedContent.Commissioning_Date);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 6], cellConfig_double, packageField.DecommissionedContent.Restoration_Coast);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 7], cellConfig_double, packageField.DecommissionedContent.Deprecation);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 8], cellConfig_gen, packageField.MaterialObject.Comments);
		}

		private static void Configure_Addition(ExcelWorksheet ws)
		{
			ws.View.ShowGridLines = false;
			ws.View.FreezePanes(2, 1);

			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 1], "№");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 2], "Инвент. номер");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 3], "Тип системы");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 4], "Наименование ОС");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 5], "Дата оприходования");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 6], "Восстан. стоимость");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 7], "Износ");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 8], "Примечание");

			ws.Column(1).Width = 3.56;
			ws.Column(2).Width = 14.22;
			ws.Column(3).Width = 8.11;
			ws.Column(4).Width = 32.33;
			ws.Column(5).Width = 12.89;
			ws.Column(6).Width = 10;
			ws.Column(7).Width = 10.67;
			ws.Column(8).Width = 34.78;
		}

		private static void SetRow_Third(ExcelWorksheet ws, Dictionary<int, ConstituentComponentType> cct, int row, int number, ReportPackageField packageField)
		{
			CellConfig cellConfig_gen = new CellConfig();
			cellConfig_gen.FontName = "Arial";
			cellConfig_gen.FontSize = 10;
			cellConfig_gen.ExcelHorizontalAlignment = ExcelHorizontalAlignment.Center;
			cellConfig_gen.ExcelVerticalAlignment = ExcelVerticalAlignment.Center;
			cellConfig_gen.BorderSide = BorderSide.Around;
			cellConfig_gen.Format = "General";
			CellConfig cellConfig_date = cellConfig_gen.Clone();
			cellConfig_date.Format = "dd.mm.yyyy";
			CellConfig cellConfig_double = cellConfig_gen.Clone();
			cellConfig_double.Format = "0.####";
			CellConfig cellConfig_int = cellConfig_gen.Clone();
			cellConfig_int.Format = "0";

			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 1], cellConfig_int, number.ToString());
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 2], cellConfig_int, packageField.DecommissionedContent.Inventory_Id);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 3], cellConfig_date, packageField.DecommissionedContent.Commissioning_Date);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 4], cellConfig_gen, packageField.MaterialObject.Name);

			int col = 5;
			foreach (var kvp in cct)
			{
				ConstituentComponent type = packageField.ConstituentComponents.FirstOrDefault(ct => ct.Constituent_Component_Type_Id == kvp.Key);
				double componentCount = type != null ? type.Count : 0;
				ExcelCellConfiguring.MakeCustom(ws.Cells[row, col], cellConfig_double, componentCount);
				col++;
			}
		}

		private static void Configure_Third(ExcelWorksheet ws, Dictionary<int, ConstituentComponentType> cct, int rowCount)
		{
			ws.View.ShowGridLines = false;
			ws.View.FreezePanes(2, 1);

			CellConfig cc = new CellConfig();
			cc.FontName = "Arial";
			cc.Bold = true;
			cc.FontSize = 10;
			cc.ExcelHorizontalAlignment = ExcelHorizontalAlignment.Center;
			cc.ExcelVerticalAlignment = ExcelVerticalAlignment.Center;
			cc.BorderSide = BorderSide.Around;
			cc.Format = "0.####";
			CellConfig cc_num = cc.Clone();
			cc_num.Format = "0";
			CellConfig cc_s = cc.Clone();
			cc_s.Format = "General";

			ExcelCellConfiguring.MakeCustom(ws.Cells[1, 1], cc_num, "№ п/п");
			ExcelCellConfiguring.MakeCustom(ws.Cells[1, 2], cc_num, "Инв. №");
			ExcelCellConfiguring.MakeCustom(ws.Cells[1, 3], cc, "Дата оприходования");
			ExcelCellConfiguring.MakeCustom(ws.Cells[1, 4], cc, "Название");

			ExcelCellConfiguring.MakeCustom(ws.Cells[2, 1], cc, "");
			ExcelCellConfiguring.MakeCustom(ws.Cells[2, 2], cc, "");
			ExcelCellConfiguring.MakeCustom(ws.Cells[2, 3], cc, "");
			ExcelCellConfiguring.MakeCustom(ws.Cells[2, 4], cc, "");

			ws.Column(1).Width = 7.89;
			ws.Column(2).Width = 7.89;
			ws.Column(3).Width = 11.22;
			ws.Column(4).Width = 49.89;

			int col = 5;
			foreach (var kvp in cct)
			{
				ExcelCellConfiguring.MakeCustom(ws.Cells[1, col], cc, kvp.Value.Name);
				ExcelCellConfiguring.MakeCustom(ws.Cells[2, col], cc_s, "");
				ws.Cells[2, col].Formula = $"IF(ROW() = 2, " +
				$"SUM({ws.Cells[3, col, rowCount + 2, col].GetFixedAddress()}), " +
				$"SUM({ws.Cells[2, col, rowCount + 1, col].GetFixedAddress()}))";
				ws.Column(col).Width = 9;
				col++;
			}

			ws.Cells[1, 1, 1, 4 + cct.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[1, 1, 1, 4 + cct.Count].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 128, 128));
			ws.Cells[2, 1, 2, 4 + cct.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
			ws.Cells[2, 1, 2, 4 + cct.Count].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
		}

		private static void SetRow_Second(ExcelWorksheet ws, int row, int number, ReportPackageField packageField)
		{
			CellConfig cellConfig_gen = new CellConfig();
			cellConfig_gen.FontName = "Times New Roman";
			cellConfig_gen.FontSize = 10;
			cellConfig_gen.ExcelHorizontalAlignment = ExcelHorizontalAlignment.Center;
			cellConfig_gen.ExcelVerticalAlignment = ExcelVerticalAlignment.Center;
			cellConfig_gen.BorderSide = BorderSide.Around;
			cellConfig_gen.Format = "General";
			CellConfig cellConfig_date = cellConfig_gen.Clone();
			cellConfig_date.Format = "dd.mm.yyyy";
			CellConfig cellConfig_double = cellConfig_gen.Clone();
			cellConfig_double.Format = "0.00";
			CellConfig cellConfig_num = cellConfig_gen.Clone();
			cellConfig_double.Format = "0";

			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 1], cellConfig_num, number);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 2], cellConfig_num, packageField.DecommissionedContent.Inventory_Id);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 3], cellConfig_gen, packageField.MaterialObject.System_Type);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 4], cellConfig_gen, packageField.MaterialObject.Name);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 5], cellConfig_date, packageField.DecommissionedContent.Commissioning_Date);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 6], cellConfig_double, packageField.DecommissionedContent.Restoration_Coast);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 7], cellConfig_double, packageField.DecommissionedContent.Deprecation);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 8], cellConfig_gen, packageField.MaterialObject.Comments);
		}

		private static void Configure_Second(ExcelWorksheet ws)
		{
			ws.View.ShowGridLines = false;
			ws.View.FreezePanes(2, 1);

			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 1], "№");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 2], "Инвент. номер");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 3], "Тип системы");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 4], "Наименование ОС");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 5], "Дата оприходования");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 6], "Восстан. стоимость");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 7], "Износ");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 8], "Примечание");

			ws.Column(1).Width = 3.56;
			ws.Column(2).Width = 14.22;
			ws.Column(3).Width = 8.11;
			ws.Column(4).Width = 32.33;
			ws.Column(5).Width = 12.89;
			ws.Column(6).Width = 10;
			ws.Column(7).Width = 10.67;
			ws.Column(8).Width = 34.78;
		}

		private static void SetRow_First(ExcelWorksheet ws, int row, int number, ReportPackageField packageField)
		{
			CellConfig cellConfig_gen = new CellConfig();
			cellConfig_gen.FontName = "Times New Roman";
			cellConfig_gen.FontSize = 10;
			cellConfig_gen.ExcelHorizontalAlignment = ExcelHorizontalAlignment.Center;
			cellConfig_gen.ExcelVerticalAlignment = ExcelVerticalAlignment.Center;
			cellConfig_gen.BorderSide = BorderSide.Around;
			cellConfig_gen.Format = "General";
			CellConfig cellConfig_date = cellConfig_gen.Clone();
			cellConfig_date.Format = "dd.mm.yyyy";
			CellConfig cellConfig_double = cellConfig_gen.Clone();
			cellConfig_double.Format = "0.00";

			ws.Cells[$"C{row}:D{row}"].Merge = true;
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 1], cellConfig_gen, number);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 2], cellConfig_gen, packageField.DecommissionedContent.Inventory_Id);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 3], cellConfig_gen, packageField.MaterialObject.Name);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 4], cellConfig_gen, null);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 5], cellConfig_date, packageField.DecommissionedContent.Commissioning_Date);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 6], cellConfig_double, packageField.DecommissionedContent.Restoration_Coast);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 7], cellConfig_double, packageField.DecommissionedContent.Deprecation);
			ExcelCellConfiguring.MakeCustom(ws.Cells[row, 8], cellConfig_gen, packageField.MaterialObject.Comments);
		}

		private static void Configure_First(ExcelWorksheet ws)
		{
			ws.View.ShowGridLines = false;
			ws.View.FreezePanes(2, 1);

			ws.Cells["C1:D1"].Merge = true;
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 1], "№");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 2], "Инвент. номер");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 3], "Наименование ОС");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 4]);
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 5], "Дата оприходования");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 6], "Восстан. стоимость");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 7], "Износ");
			ExcelCellConfiguring.MakeHeader(ws.Cells[1, 8], "Примечание");
			ws.Cells["C2:D2"].Merge = true;
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 1], 1);
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 2], 2);
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 3], 3);
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 4]);
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 5], 4);
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 6], 5);
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 7], 6);
			ExcelCellConfiguring.MakeHeader(ws.Cells[2, 8], 7);

			ws.Column(1).Width = 3.56;
			ws.Column(2).Width = 14.22;
			ws.Column(3).Width = 18.67;
			ws.Column(4).Width = 12.33;
			ws.Column(5).Width = 14.67;
			ws.Column(6).Width = 10;
			ws.Column(7).Width = 10.67;
			ws.Column(8).Width = 34.78;
		}

		private static void MakeSignature(ExcelWorksheet ws, int fromRow, string post, string fullName)
		{
			CellConfig cellConfig_up = new CellConfig();
			cellConfig_up.FontName = "Times New Roman";
			cellConfig_up.FontSize = 14;
			cellConfig_up.ExcelHorizontalAlignment = ExcelHorizontalAlignment.Center;
			cellConfig_up.ExcelVerticalAlignment = ExcelVerticalAlignment.Bottom;
			cellConfig_up.BorderSide = BorderSide.Around;
			cellConfig_up.Format = "General";
			cellConfig_up.BorderSide = BorderSide.Bottom;
			CellConfig cellConfig_down = cellConfig_up.Clone();
			cellConfig_down.FontSize = 10;
			cellConfig_down.BorderSide = BorderSide.None;


			ws.Cells[$"A{fromRow}:C{fromRow}"].Merge = true;
			ws.Cells[$"A{fromRow + 1}:C{fromRow + 1}"].Merge = true;
			ws.Cells[$"E{fromRow}:F{fromRow}"].Merge = true;
			ws.Cells[$"E{fromRow + 1}:F{fromRow + 1}"].Merge = true;
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow, 1], cellConfig_up, post);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow, 2], cellConfig_up);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow, 3], cellConfig_up);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow, 5], cellConfig_up);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow, 6], cellConfig_up);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow, 8], cellConfig_up, fullName);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow + 1, 1], cellConfig_down, "(должность)");
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow + 1, 2], cellConfig_down);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow + 1, 3], cellConfig_down);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow + 1, 5], cellConfig_down, "(подпись)");
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow + 1, 6], cellConfig_down);
			ExcelCellConfiguring.MakeCustom(ws.Cells[fromRow + 1, 8], cellConfig_down, "(расшифровка подписи)");
		}
	}
}