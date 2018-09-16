﻿using Dawnx.Sequences;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dawnx.Ranges
{
    public class LetterRange : IRange<string>
    {
        public int Start { get; private set; }
        public int End { get; private set; }
        public int Step { get; private set; }

        public LetterRange(string end) : this("A", end, 1) { }
        public LetterRange(string start, string end) : this(start, end, 1) { }
        public LetterRange(string start, string end, int step) : this(LetterSequence.GetNumber(start), LetterSequence.GetNumber(end), 1) { }
        public LetterRange(int start, int end, int step)
        {
            Start = start;
            End = end;
            Step = step;
        }

        public string GetValue(int index) => LetterSequence.GetLetter(index);

        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i <= End; i++)
                yield return GetValue(i);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
