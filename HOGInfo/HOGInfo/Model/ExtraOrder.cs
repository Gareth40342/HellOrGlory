using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOGInfo.Model
{
    public class ExtraOrder
    {
        public string Extra { get; set; }
        public int Quantity { get; set; }
        public override string ToString()
        {
            return $"{Extra} {Quantity}";
        }
    }
}
