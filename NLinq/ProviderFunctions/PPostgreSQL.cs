﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NLinq.ProviderFunctions
{
    public static class PPostgreSQL
    {
        [DbFunction]
        public static double Random() => throw new NotSupportedException();

    }
}
