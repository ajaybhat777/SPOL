using System;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelPivotAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Report.xlsx";     // <-- Change
            string sheetName = "Sheet1";             // <-- Change
            string pivotTableName = "PivotTable1";   // <-- Change
            string slicerCacheName = "Slicer_BU";    // <-- Change
            string slicerValue = "Tech";             // <-- Change

            var data = ApplySlicerExpandAndRead(
                filePath,
                sheetName,
                pivotTableName,
                slicerCacheName,
                slicerValue);

            // Print pivot table
            if (data != null)
            {
                for (int i = 1; i <= data.GetLength(0); i++)
                {
                    for (int j = 1; j <= data.GetLength(1); j++)
                    {
                        Console.Write((data[i, j]?.ToString() ?? "") + "\t");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        public static object[,] ApplySlicerExpandAndRead(
            string filePath,
            string sheetName,
            string pivotTableName,
            string slicerCacheName,
            string slicerValue)
        {
            Excel.Application xlApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            Excel.PivotTable pt = null;
            Excel.SlicerCache slicerCache = null;

            try
            {
                xlApp = new Excel.Application
                {
                    Visible = false,
                    ScreenUpdating = false,
                    DisplayAlerts = false,
                    Calculation = Excel.XlCalculation.xlCalculationManual
                };

                wb = xlApp.Workbooks.Open(filePath);
                ws = wb.Sheets[sheetName] as Excel.Worksheet;

                // Apply slicer filter
                slicerCache = wb.SlicerCaches[slicerCacheName];
                slicerCache.ClearManualFilter();

                foreach (Excel.SlicerItem item in slicerCache.SlicerItems)
                {
                    item.Selected = item.Name.Equals(
                        slicerValue,
                        StringComparison.OrdinalIgnoreCase);
                }

                // Get PivotTable
                pt = ws.PivotTables(pivotTableName) as Excel.PivotTable;

                pt.ManualUpdate = true;

                // Expand all row fields
                foreach (Excel.PivotField field in pt.RowFields)
                {
                    try
                    {
                        field.ExpandAll();
                    }
                    catch
                    {
                        foreach (Excel.PivotItem item in field.PivotItems())
                        {
                            item.ShowDetail = true;
                            Marshal.ReleaseComObject(item);
                        }
                    }

                    Marshal.ReleaseComObject(field);
                }

                pt.ManualUpdate = false;
                pt.RefreshTable();
                xlApp.CalculateUntilAsyncQueriesDone();

                // Read entire pivot table
                Excel.Range range = pt.TableRange2;
                object[,] data = range.Value2;

                Marshal.ReleaseComObject(range);

                return data;
            }
            finally
            {
                if (pt != null) Marshal.ReleaseComObject(pt);
                if (slicerCache != null) Marshal.ReleaseComObject(slicerCache);
                if (ws != null) Marshal.ReleaseComObject(ws);

                if (wb != null)
                {
                    wb.Close(false);
                    Marshal.ReleaseComObject(wb);
                }

                if (xlApp != null)
                {
                    xlApp.Quit();
                    Marshal.ReleaseComObject(xlApp);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
