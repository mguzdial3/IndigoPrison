using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{
    /// <summary>
    /// A dummy placeholder for characters. Replace with Tory's implementation.
    /// (Although please consider keeping them a struct or else changes will need to be made to everything else.)
    /// </summary>
    public struct Character
    {
        // Dummy constructor.
        public Character(string name) : this()
        {
            this.Name = name;
            this.Alive = true;
        }

        // Dummy properties.
        public string Name { get; private set; }
        public bool Alive { get; set; }
    }
}
