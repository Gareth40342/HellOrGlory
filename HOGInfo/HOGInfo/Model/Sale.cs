using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOGInfo.Model
{
    public class Sale
    {
        public Coffee Coffee { get; set; }
        public string Roast { get; set; }
        public int Quantity { get; set; }
        public string Extras { get; set; }
        public string Blend
        {
            get
            {
                return $"{Coffee.Drink} - {Roast}";
            }
        }
    }
}
