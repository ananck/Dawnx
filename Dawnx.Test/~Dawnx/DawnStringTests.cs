using Dawnx.Definition;
using System;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Dawnx.Test
{
    public class DawnStringTests
    {
        [Fact]
        public void Common()
        {
            var ds = "123";

            Assert.Equal("123123123", ds.Repeat(3));
            Assert.Equal("12", ds.Slice(0, -1));
            Assert.Equal("1", ds.Slice(0, 1));
            Assert.Equal("23", ds.Slice(1));
            Assert.Equal("23", ds.Slice(-2));
            Assert.Throws<IndexOutOfRangeException>(() => ds.Slice(3, 2));

            Assert.Equal('1', ds.CharAt(0));
            Assert.Equal('3', ds.CharAt(-1));

            Assert.Equal("zmjack", "zmjack".Center(5));
            Assert.Equal("zmjack", "zmjack".Center(6));
            Assert.Equal(" zmjack", "zmjack".Center(7));
            Assert.Equal(" zmjack ", "zmjack".Center(8));
        }

        [Fact]
        public void GetBytes()
        {
            var str = "����";
            Assert.Equal(Encoding.UTF8.GetBytes(str), str.Bytes(Encoding.UTF8));
            Assert.Equal(Encoding.UTF8.GetBytes(str), str.Bytes("utf-8"));
            Assert.NotEqual(Encoding.ASCII.GetBytes(str), str.Bytes(Encoding.UTF8));
            Assert.NotEqual(Encoding.ASCII.GetBytes(str), str.Bytes("utf-8"));

            var hexString = "0c66182ec710840065ebaa47c5e6ce90";
            var hexString_Base64 = "MGM2NjE4MmVjNzEwODQwMDY1ZWJhYTQ3YzVlNmNlOTA=";
            var hexString_Bytes = new byte[]
            {
                0x0C, 0x66, 0x18, 0x2E, 0xC7, 0x10, 0x84, 0x00, 0x65, 0xEB, 0xAA, 0x47, 0xC5, 0xE6, 0xCE, 0x90
            };
            Assert.Equal(hexString_Bytes, hexString.BytesFromHex());
            Assert.Equal(hexString, hexString_Bytes.HexString());

            Assert.Equal(hexString,
                hexString_Base64.BytesFromBase64().String(Encoding.Default));
        }

        [Fact]
        public void NormalizeNewLine()
        {
            Assert.Equal("123456", "123\n456".NormalizeNewLine().Replace(Environment.NewLine, ""));
            Assert.Equal("123456", "123\r\n456".NormalizeNewLine().Replace(Environment.NewLine, ""));
        }

        [Fact]
        public void GetLines()
        {
            string nullString = null;
            Assert.Equal(new string[0], nullString.GetLines());
            Assert.Equal(new[] { "1", "2" }, $"1{Environment.NewLine}2".GetLines());
            Assert.Equal(new[] { "1", "2" }, $"1{Environment.NewLine}2{Environment.NewLine}".GetLines());
            Assert.Equal(new[] { "1", " " }, $"1{Environment.NewLine} ".GetLines());
            Assert.Equal(new[] { "", "1", " " }, $"{Environment.NewLine}1{Environment.NewLine} ".GetLines());

            Assert.Equal(new string[0], nullString.GetLines());
            Assert.Equal(new[] { "1", "2" }, $"1{ControlChars.Lf}2".GetLines(true));
            Assert.Equal(new[] { "1", "2" }, $"1{ControlChars.CrLf}2".GetLines(true));
        }

        [Fact]
        public void GetPureLines()
        {
            string nullString = null;
            Assert.Equal(new string[0], nullString.GetPureLines());
            Assert.Equal(new[] { "1", "2" }, $"1{Environment.NewLine}2".GetPureLines());
            Assert.Equal(new[] { "1", "2" }, $"1{Environment.NewLine}2{Environment.NewLine}".GetPureLines());
            Assert.Equal(new[] { "1" }, $"1{Environment.NewLine} ".GetPureLines());
            Assert.Equal(new[] { "1" }, $"{Environment.NewLine}1{Environment.NewLine} ".GetPureLines());
        }

        [Fact]
        public void Unique()
        {
            Assert.Equal("123 456 7890", "  123  456    7890 ".Unique());
            Assert.Equal("123 456 7890", "  123  456 7890".Unique());
            Assert.Equal("123 456 7890", "  123\r\n456 7890".Unique());
            Assert.Equal("123 456 7890", "  123\r\n 456 7890".Unique());
        }

        [Fact]
        public void Project1()
        {
            var regex = new Regex(@"(.+?)(?:(?=\()|(?=��)|$)");
            Assert.Equal("zmjack", "zmjack(1)".Project(regex).Trim());
            Assert.Equal("zmjack", "zmjack (1)".Project(regex).Trim());
            Assert.Equal("zmjack", "zmjack (".Project(regex).Trim());
            Assert.Equal("zmjack", "zmjack".Project(regex).Trim());
        }

        [Fact]
        public void Project2()
        {
            Assert.Equal("ja", "zmjack".Project(@"(ja)", "$1"));
        }

        [Fact]
        public void Insert()
        {
            Assert.Equal("", "".UnitInsert(4, " "));
            Assert.Equal("", "".UnitInsert(4, " ", true));
            Assert.Equal("1 2345", "12345".UnitInsert(4, " "));
            Assert.Equal("1234 5", "12345".UnitInsert(4, " ", true));
            Assert.Equal("1234 5678", "12345678".UnitInsert(4, " "));
            Assert.Equal("1234 5678", "12345678".UnitInsert(4, " ", true));
            Assert.Equal("1 2345 6789", "123456789".UnitInsert(4, " "));
            Assert.Equal("1234 5678 9", "123456789".UnitInsert(4, " ", true));
        }

        [Fact]
        public void CapitalizeFirst()
        {
            Assert.Equal("zmjack", "Zmjack".CapitalizeFirst(false));
            Assert.Equal("Zmjack", "Zmjack".CapitalizeFirst());
        }

    }
}
