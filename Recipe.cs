using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    public class Recipe
    {
        public List<string> Ingredients { get; set; }
        public string Title { get; set; }
        public List<string> Instructions { get; set; }
        public List<string> Categories { get; set; }

    }
}
