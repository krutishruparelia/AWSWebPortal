﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWS.Models
{
    public class Weeklygraph
    {
        public string Time { get; set; }
        public string ColumnName { get; set; }
        public string min { get; set; }
        public string max { get; set; }
        public string avg { get; set; }


    }
}