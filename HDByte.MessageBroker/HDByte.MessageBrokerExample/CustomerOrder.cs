using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDByte.MessageBrokerExample
{
    public class CustomerOrder
    {
        public string Name { get; set; }
        public string ItemName { get; set; }
        public double ItemCost { get; set; }
        public int ItemQuantity { get; set; }

        public double GetTotalCost()
        {
            return ItemCost * ItemQuantity;
        }
    }
}
