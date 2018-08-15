using NPOI.SS.UserModel;
using System;
using System.IO;
using Xunit;

namespace Dawnx.NPOI.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var book = new ExcelBook(ExcelVersion.Excel2007);

            var ����style1 = ComparedCellStyle.FullBorder.Self(_ =>
            {
                _.DataFormat = book.GetDataFormat("0.00");
                _.FontIndex = book.GetFont(new ComparedFont
                {
                    FontName = "����",
                    FontHeightInPoints = 20,
                }).Index;
                _.FillForegroundColor = IndexedColors.Blue.Index;
                _.FillPattern = FillPattern.SolidForeground;
            });
            var ����style1 = ComparedCellStyle.FullBorder.Self(_ =>
            {
                _.DataFormat = book.GetDataFormat("0.00%");
                _.FontIndex = book.GetFont(new ComparedFont
                {
                    FontName = "����",
                    FontHeightInPoints = 27,
                    Color = IndexedColors.BlueGrey.Index,
                }).Index;
                _.FillForegroundColor = IndexedColors.Red.Index;
                _.FillPattern = FillPattern.SolidForeground;
            });

            var sampleSheet = book.CreateSheet("Sample");
            sampleSheet.SetColumnWidth("A", 8.5);
            sampleSheet.SetColumnWidth("B", 3);
            sampleSheet.SetColumnWidth("C", 30);

            sampleSheet.SetCursor("C2");

            sampleSheet.Print("Suplier", "Product", "Quantity", "Release Date", "Describe");
            sampleSheet.Print(new object[,]
            {
                { "Company 101", "Ag", 1, DateTime.Parse("2017-1-1"), "[??]" },
                { "Company 101", "Ag", 1, DateTime.Parse("2017-1-1"), "[??]" },
                { "Company 101", "Ag", 2, DateTime.Parse("2017-1-2"), "[??]" },
                { "Company 101", "Ag", 2, DateTime.Parse("2017-1-2"), "[??]" },
                { "Company 102", "Ag", 3, DateTime.Parse("2017-1-3"), "[??]" },
                { "Company 102", "Cu", 3, DateTime.Parse("2017-1-3"), "[??]" },
                { "Company 102", "Cu", 4, DateTime.Parse("2017-1-4"), "[??]" },
            }).Self(_ =>
            {
                _.SetCellStyle(����style1);
                _.Each(row =>
                {
                    if (row[(0, 1)].GetValue().ToString() == "Cu")
                    {
                        row.SelectColunms(1).SetCellStyle(����style1);
                    }
                });
                _.SmartMerge(new[] { 0, 1, 2, 3 });
            });

            sampleSheet["E4"].SetValue(123);

            //var dawnxSheet = book.CloneSheet( sampleSheet;
            //var cell = sheet["C2"];

            //var sheet = book.CloneSheet("Products");
            var dawnxSheet = sampleSheet.Clone("dawnx");

            var memory = new MemoryStream();
            book.Write(memory);



            //sheet["A2", "C8"].SmartMerge(new[] { 0, 2, 1 });
            book.SaveAs(@"result1.xlsx");

            //var book = new XSSFWorkbook(@"C:\Users\19558\Desktop\e1.xlsx");
            //var book2 = new HSSFWorkbook();
            //var sheet = book.GetSheetAt(0).Dawn();
            //var table = sheet.FetchTable("A3", true,
            //    new[] { typeof(int), typeof(int), typeof(string) });

            //sheet["A1"].Cell.SetValue(123);
        }

        [Fact]
        public void Test2()
        {
            var book = new ExcelBook("test.xlsx");
            var models = book[0].Fetch<Model>("A2", m => new { m.Supplier, m.Product, m.ReleaseDate });

            Assert.Equal(10, book[0].LastRowNum);

            Assert.Equal(ExcelVersion.Excel2007, ExcelBook.GetVersion("test2.xlsx"));

            book[0]["A10"].SetValue("123");
            book[0]["B10"].SetValue("123");

            book.SaveAs("result2.xlsx");
        }

        public class Model
        {
            public string Supplier { get; set; }
            public string Product { get; set; }
            public string ReleaseDate { get; set; }
        }

    }
}