using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace space
{
    class SendOrder
    {
        public Planet from;
        public Planet to;
        public int amount;

        public SendOrder(Planet from, Planet to, int amount) 
        {
            this.from = from;
            this.to = to;
            this.amount = amount;
        }
    }
}
