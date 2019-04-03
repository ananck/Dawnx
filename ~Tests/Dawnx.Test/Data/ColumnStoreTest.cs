﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;

namespace Dawnx.Data.Test
{
    public class RegistryStore : IColumnStore
    {
        [Key]
        public Guid Id { get; set; }
        public string EntityId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Registry : IColumnStorable<RegistryStore>
    {
        [Key]
        public Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Sex { get; set; }

        public IEnumerable<RegistryStore> ColumnStores { get; set; }

        public bool ProxyLoaded { get; set; }

        public static Registry Connect(IEnumerable<RegistryStore> columnStores)
        {
            var proxy = new Registry().Proxy(new RegistryProxy<Registry, RegistryStore>());
            proxy.Load(columnStores);
            return proxy;
        }
    }

    public class Registry2
    {
        [Key]
        public Guid Id { get; set; }

        public string Name
        {
            get => ColumnStores.FirstOrDefault(x => x.Key == "Name").Value;
            set => ColumnStores.FirstOrDefault(x => x.Key == "Name").Value = value;
        }

        public string Sex
        {
            get => ColumnStores.FirstOrDefault(x => x.Key == "Sex").Value;
            set => ColumnStores.FirstOrDefault(x => x.Key == "Sex").Value = value;
        }

        public IEnumerable<RegistryStore> ColumnStores { get; set; }
    }

    public class ColumnStoreTest
    {
        [Fact]
        public void Test1()
        {
            var regs = new RegistryStore[]
            {
                new RegistryStore { EntityId = "0EA3F3B8-291A-4D23-A0DC-4BBC2F2BA9DB", Key = "Name", Value = "zmjack" },
                new RegistryStore { EntityId = "0EA3F3B8-291A-4D23-A0DC-4BBC2F2BA9DB", Key = "Sex", Value = "Male" },
            };

            var item = Registry.Connect(regs);
            item.Name = "999";

            Assert.Equal(Guid.Parse("0EA3F3B8-291A-4D23-A0DC-4BBC2F2BA9DB"), item.Id);
            Assert.Equal("999", regs.First(x => x.Key == "Name").Value);
        }

        [Fact]
        public void Test2()
        {
            var regs = new RegistryStore[]
            {
                new RegistryStore { EntityId = "0EA3F3B8-291A-4D23-A0DC-4BBC2F2BA9DB", Key = "Name", Value = "zmjack" },
                new RegistryStore { EntityId = "0EA3F3B8-291A-4D23-A0DC-4BBC2F2BA9DB", Key = "Sex", Value = "Male" },
            };

            var item = new Registry2();
            item.Id = Guid.Parse(regs[0].EntityId);
            item.ColumnStores = regs;

            item.Name = "999";

            Assert.Equal(Guid.Parse("0EA3F3B8-291A-4D23-A0DC-4BBC2F2BA9DB"), item.Id);
            Assert.Equal("999", regs.First(x => x.Key == "Name").Value);
        }

    }
}