﻿using NPOI.SS.UserModel;
using NStandard;
using System;
using System.Data;
using System.Linq;

namespace Dawnx.NPOI
{
    public partial class SheetCell
    {
        public ExcelSheet Sheet { get; private set; }
        public ICell MapedCell { get; private set; }
        public SheetCell(ExcelSheet sheet, ICell cell)
        {
            Sheet = sheet;
            MapedCell = cell;
        }

        public string CellName => Sheet.GetCellName((RowIndex, ColumnIndex));

        public static SheetCell operator |(SheetCell @this, bool value) => @this.Then(_ => _.SetValue(value));
        public static SheetCell operator |(SheetCell @this, double value) => @this.Then(_ => _.SetValue(value));
        public static SheetCell operator |(SheetCell @this, DateTime value) => @this.Then(_ => _.SetValue(value));
        public static SheetCell operator |(SheetCell @this, IRichTextString value) => @this.Then(_ => _.SetValue(value));
        public static SheetCell operator |(SheetCell @this, string value) => @this.Then(_ => _.SetValue(value));
        public static SheetCell operator |(SheetCell @this, object value) => @this.Then(_ => _.SetValue(value));

        public static implicit operator bool(SheetCell @this) => @this.MapedCell.BooleanCellValue;
        public static implicit operator double(SheetCell @this) => @this.MapedCell.NumericCellValue;
        public static implicit operator DateTime(SheetCell @this) => @this.MapedCell.DateCellValue;
        public static implicit operator string(SheetCell @this)
        {
            if (@this.MapedCell.CellType == CellType.Formula)
                return @this.MapedCell.StringCellValue;
            else return @this.ToString();
        }

        public void SetCellStyle(ICellStyle style) => MapedCell.CellStyle = style;
        public void SetCStyle(CStyle style) => SetCellStyle(style.CellStyle);
        public void SetCStyle(Action<CStyleApplier> initApplier) => SetCellStyle(Sheet.Book.CStyle(initApplier).CellStyle);
        public CStyle GetCStyle() => new CStyle(Sheet.Book, CellStyle);
        public void UpdateCStyle(Action<CStyleApplier> initApplier)
        {
            var applier = GetCStyle().GetApplier().Then(x => initApplier(x));
            SetCellStyle(Sheet.Book.CStyle(applier).CellStyle);
        }

        public void SetValue(object value)
        {
            switch (value)
            {
                case null: String = ""; break;
                case bool v: Boolean = v; break;
                case short v: Number = v; break;
                case ushort v: Number = v; break;
                case int v: Number = v; break;
                case uint v: Number = v; break;
                case long v: Number = v; break;
                case ulong v: Number = v; break;
                case float v: Number = v; break;
                case double v: Number = v; break;
                case DateTime v: DateTime = v; break;
                case IRichTextString v: RichTextString = v; break;
                case string v:
                    if (v.StartsWith("=")) Formula = v.Slice(1);
                    else String = v;
                    break;

                case SheetCell v when v.MapedCell.CellType == CellType.Blank: break;
                case SheetCell v when v.MapedCell.CellType == CellType.Error: break;
                case SheetCell v when v.MapedCell.CellType == CellType.Unknown: break;
                case SheetCell v when v.MapedCell.CellType == CellType.Boolean: Boolean = v.Boolean; break;
                case SheetCell v when v.MapedCell.CellType == CellType.Numeric: Number = v.Number; break;
                case SheetCell v when v.MapedCell.CellType == CellType.String: String = v.String; break;
                case SheetCell v when v.MapedCell.CellType == CellType.Formula: Formula = v.Formula; break;

                case CValue v:
                    SetValue(v.Value);
                    if (!(v.Style is null))
                        SetCStyle(v.Style);

                    break;

                case object v: String = v.ToString(); break;
            }
        }
        public object GetValue()
        {
            switch (MapedCell.CellType)
            {
                case CellType.Boolean: return Boolean;
                case CellType.Numeric: return Number;
                case CellType.String:
                case CellType.Formula: return String;

                default: return null;
            }
        }

        public string Formula
        {
            get => MapedCell.CellFormula;
            set => MapedCell.SetCellFormula(value);
        }
        public bool? Boolean
        {
            get => MapedCell.BooleanCellValue;
            set
            {
                if (value.HasValue)
                    MapedCell.SetCellValue(value.Value);
                else MapedCell.SetCellValue((string)null);
            }
        }
        public DateTime? DateTime
        {
            get => MapedCell.DateCellValue;
            set
            {
                if (value.HasValue)
                {
                    var book = Sheet.Book;
                    MapedCell.SetCellValue(value.Value);
                    SetCStyle(book.CStyle(s => s.DataFormat = "yyyy-M-d"));
                }
                else MapedCell.SetCellValue((string)null);
            }
        }
        public double? Number
        {
            get => MapedCell.NumericCellValue;
            set
            {
                if (value.HasValue)
                    MapedCell.SetCellValue(value.Value);
                else MapedCell.SetCellValue((string)null);
            }
        }
        public IRichTextString RichTextString
        {
            get => MapedCell.RichStringCellValue;
            set => MapedCell.SetCellValue(value);
        }
        public string String
        {
            get => MapedCell.StringCellValue;
            set => MapedCell.SetCellValue(value);
        }

        public ICellStyle CellStyle
        {
            get => MapedCell.CellStyle;
            set => MapedCell.CellStyle = value;
        }

        public SheetRange Print(object[,] values) => Sheet.Print(values, true);
        public SheetRange Print(object[][] values) => Sheet.Print(values, true);
        public SheetRange Print(object[] values) => Sheet.Print(values, true);
        public SheetRange Print(DataTable table) => Sheet.Print(table, true);

        public void SetWidth(double width) => Sheet.SetWidth(ColumnIndex, width);
        public void SetHeight(float height) => Sheet.GetRow(RowIndex).HeightInPoints = height;

        public SheetRange MergedRange
        {
            get
            {
                if (IsMergedCell)
                    return Sheet.MergedRanges.FirstOrDefault(x => x.IsInRange(this));
                else return null;
            }
        }
    }
}
