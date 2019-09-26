﻿using Dawnx;
using Xunit;

namespace TypeSharp.Test
{
    public class TypeScriptModelBuilderTests
    {
        [Fact]
        public void Test1()
        {
            var builder = new TypeScriptModelBuilder();
            builder.CacheType<RootClass>();
            builder.CacheType<JSend>(new TypeScriptModelAttribute { Namespace = "Nx" });

            var tscode = builder.Compile();
            var expectedCode = @"namespace TSNS1 {
    export namespace RootClass {
        export const CONST_STRING : string = 'const_string';
        export const CONST_INTEGER : number = 2147483647;
    }
}

declare namespace TypeSharp.Test {
    export const enum EState {
        Ready = 0,
        Running = 1,
        Complete = 2,
    }
}
declare namespace TSNS2 {
    interface SubClass {
        name? : string;
    }
}
declare namespace TSNS1 {
    interface RootClass {
        state? : TypeSharp.Test.EState;
        bbb? : TSNS2.SubClass;
        str? : string;
        int? : number;
    }
}
declare namespace Nx {
    interface JSend {
        status? : string;
        data? : any;
        code? : string;
        message? : string;
    }
}

";
            Assert.Equal(expectedCode, tscode);
        }

    }
}