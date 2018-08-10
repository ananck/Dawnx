﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dawnx
{
    public class PagedEnumerable<T> : IPageable<T>
    {
        public T[] Items;
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int PageCount { get; private set; }
        public bool IsFristPage => PageNumber == 1;
        public bool IsLastPage => PageNumber == PageCount;

        public PagedEnumerable(IEnumerable<T> source, int page, int pageSize, int pageCount)
        {
            PageNumber = page;
            PageSize = pageSize;
            PageCount = pageCount;
            Items = source.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in Items)
                yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}