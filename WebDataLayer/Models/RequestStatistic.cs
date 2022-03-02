using System;
using System.Collections.Generic;
using System.Text;

namespace WebDataLayer.Models
{
    public class RequestStatistic
    {
        public string Category { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }

        public RequestStatistic(string type, string category,  string name)
        {
            Type = type;
            Category = category;
            Name = name;
        }
    }
}
