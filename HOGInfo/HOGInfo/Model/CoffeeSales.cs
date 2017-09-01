using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOGInfo.Model
{
    public class CoffeeSales
    {
        public Coffee Coffee { get; set; }
        public string Roast { get; set; }
        public int Sold { get; set; }
        public override string ToString()
        {
            return $"{Coffee.Drink} {Sold}";
        }
    }
}
