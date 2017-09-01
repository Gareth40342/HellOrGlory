﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOGInfo.Model
{
    public class Coffee
    {
        public string Drink { get; set; }
        public decimal Price { get; set; }
        public override string ToString()
        {
            return $"{Drink} {Price}";
        }
    }
}
