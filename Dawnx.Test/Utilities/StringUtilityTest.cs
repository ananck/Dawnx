﻿using Dawnx.Utilities;
using Xunit;

namespace Dawnx.Test.Utilities
{
    public class StringUtilityTest
    {
        [Fact]
        public void CommonStartsTest()
        {
            Assert.Equal("AB", StringUtility.CommonStarts("ABC", "AB123", "ABC23"));
        }
        
    }
}
