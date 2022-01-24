using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public class Node
    {
        public string Name { get; set; }
        public Node(string name) => Name = name;
        public bool IsExit { get; set; } = false;
    }
}
