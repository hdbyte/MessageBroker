using HDByte.MessageBroker;
using HDByte.MessageBroker.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDByte.MessageBrokerExample
{
    public partial class Form1 : Form
    {
        Broker Broker;
        public Form1()
        {
            InitializeComponent();

            Broker = BrokerManager.GetBroker();
            Broker.Subscribe<CustomerOrder>(OnNewOrder, ActionThread.UI);

        }

        private void OnNewOrder(CustomerOrder order)
        {
            textBox1.Text = order.Name;
            Debug.WriteLine(order.GetTotalCost());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var broker = BrokerManager.GetBroker();
            var order = new CustomerOrder() { Name = "John Smith", ItemName = "Mousepad", ItemCost = 10.99, ItemQuantity = 3 };
            broker.Publish(order);
        }
    }
}
