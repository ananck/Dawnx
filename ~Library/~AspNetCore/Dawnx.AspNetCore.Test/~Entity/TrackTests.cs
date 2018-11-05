using Xunit;
using System.Linq;

namespace Dawnx.AspNetCore.Test
{
    public class TrackTests
    {
        [Fact]
        public void Test1()
        {
            using (var context = new ApplicationDbContext())
            {
                var origin = new TrackModel
                {
                    ForTrim = "   127.0.0.* ",
                    ForLower = "Dawnx",
                    ForUpper = "Dawnx",
                    ForCondensed = "  Welcome to   Dawnx  ",
                };
                var model = new TrackModel().Accept(origin);
                context.TrackModels.Add(model);
                context.SaveChanges();

                Assert.Equal("127.0.0.*", model.ForTrim);
                Assert.Equal("dawnx", model.ForLower);
                Assert.Equal("DAWNX", model.ForUpper);
                Assert.Equal("Welcome to Dawnx", model.ForCondensed);
                Assert.Equal(@"127\.0\.0\.(?:[1-2]\d(?<!2[6-9])\d(?<!25[6-9])|\d\d|[0-9])", model.Automatic);

                model = context.TrackModels.First();
                model.Accept(origin, x => new { x.ForTrim, x.ForLower, x.ForUpper, x.ForCondensed });

                Assert.Equal(origin.ForTrim, model.ForTrim);
                Assert.Equal(origin.ForLower, model.ForLower);
                Assert.Equal(origin.ForUpper, model.ForUpper);
                Assert.Equal(origin.ForCondensed, model.ForCondensed);

                context.TrackModels.Remove(model);
                context.SaveChanges();

                Assert.False(context.SimpleModels.Any());
            }

        }

    }
}
