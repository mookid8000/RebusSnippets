using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Snippets.Messages;

namespace Snippets
{
    [TestFixture]
    public class MessageExample
    {
        [Test]
        public void PlaceOrderExample()
        {
            var order = new PlaceOrder(new []
            {
                new Item(23, 1),
                new Item(8923, 2),
                new Item(1255, 1),
            });

            Console.WriteLine(JsonConvert.SerializeObject(order, Formatting.Indented));
        }

    }

    namespace Messages
    {
        public class PlaceOrder
        {
            public PlaceOrder(IEnumerable<Item> items)
            {
                Items = items.ToArray();
            }

            public IReadOnlyCollection<Item> Items { get; }
        }

        public class Item
        {
            public Item(int itemId, int quantity)
            {
                ItemId = itemId;
                Quantity = quantity;
            }

            public int ItemId { get; }
            public int Quantity { get; }
        }
    }


}