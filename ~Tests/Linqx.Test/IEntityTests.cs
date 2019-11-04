using Dawnx;
using NStandard;
using System;
using Xunit;

namespace NLinq.Test
{
    public class IEntityTests
    {
        public class Entity : IEntity
        {
            public string String { get; set; }
            public int Int { get; set; }
        }

        [Fact]
        public void Test1()
        {
            var a = new Entity() { String = "123", Int = 1 };
            var b = new Entity().For(x => x.Accept(a));
            var c = new Entity().For(x => x.Accept(a, m => new { m.String }));
            var d = new Entity().For(x => x.AcceptBut(a, m => new { m.String }));

            Assert.Equal("123", b.String);
            Assert.Equal(1, b.Int);

            Assert.Equal("123", c.String);
            Assert.Equal(0, c.Int);

            Assert.Null(d.String);
            Assert.Equal(1, d.Int);
        }

        public class MyEntity : IEntity<MyEntity>
        {
            public string Class { get; set; }
            public string Name { get; set; }
            public string Comment { get; set; }
        }

        [Fact]
        public void Test2()
        {
            var entity1 = new MyEntity
            {
                Class = "123",
                Name = "aaa",
            };
            var entity2 = new MyEntity();
            entity2.Accept(entity1, m => new { m.Class });
            entity2.Accept(entity1, m => m.Comment);

            Assert.Equal(entity2.Class, entity1.Class);
            Assert.Equal(entity2.Comment, entity1.Comment);
            Assert.NotEqual(entity2.Name, entity1.Name);
        }

        [Fact]
        public void Test3()
        {
            var entity1 = new MyEntity
            {
                Class = "123",
                Name = "aaa",
            };

            var dict = entity1.ToDisplayDictionary(x => x.Name);
        }


    }
}
