﻿using Dawnx.Reflection;
using Linqx;
using System;
using System.Data;
using System.Linq;

namespace Dawnx.Data
{
    public class Csv : Csv<DefaultBasicTypeConverter>
    {
        public Csv(string source, string separator = ",")
            : base(source, separator) { }
    }

    public class Csv<TBasicTypeConverter>
        where TBasicTypeConverter : IBasicTypeConverter, new()
    {
        public string Source { get; private set; }
        public string Separator { get; private set; }
        public string[] Titles { get; private set; }
        public string[][] Values { get; private set; }
        private TBasicTypeConverter _Converter = new TBasicTypeConverter();

        private DataTable _Table = new DataTable();
        public DataTable Table { get => _Table.Clone(); }

        public Csv(string source, string separator = ",")
        {
            Source = source.NormalizeNewLine();
            Separator = separator;

            Titles = Source.GetLines().FirstOrDefault()?
                .For(_ => _.Split(new[] { Separator }, StringSplitOptions.None)) ?? new string[0];
            Values = Source.GetLines().Skip(1)
                .Select(_ => _.Split(new[] { Separator }, StringSplitOptions.None)).ToArray();
        }

        public DataTable ToTable()
        {
            var table = new DataTable();
            Titles.Each(title => table.Columns.Add(new DataColumn(title)));
            Values.Each(row => table.Rows.Add(row));
            return table;
        }

        public TRet[] ToArray<TRet>()
            where TRet : new()
        {
            var ret = new TRet[Values.Length].Self((_, i) => _[i] = new TRet());
            var props = typeof(TRet).GetProperties();

            for (int j = 0; j < Titles.Length; j++)
            {
                var prop = props.FirstOrDefault(
                    p => DataAnnotationUtility.GetDisplayName(p) == Titles[j]);

                if (prop != null)
                {
                    for (int i = 0; i < ret.Length; i++)
                    {
                        prop.SetValue(ret[i],
                            _Converter.Convert(prop, Values[i][j]));
                    }
                }
            }

            return ret;
        }

    }
}