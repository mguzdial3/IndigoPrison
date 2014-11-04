using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{

    public struct Character
    {

        public Character(string name) : this()
        {
            this.Name = name;
            this.Alive = true;
			this.Relationships = new Dictionary<Character, feelingsAboutChar>();
			//this.nextAction = new Action();
			this.Goal = new List<Condition> ();
        }

        // Dummy properties.
        public string Name { get; private set; }
        public bool Alive { get; set; }
	    //public List<Attribute> attributes; // TODO emotions, trust, etc
	    public List<Condition> Goal {get; set;} 
	    //Location; TODO 
	    //List<Action> plan; TODO;
	    public Action nextAction {get; private set;}
	    //Inventory; // TODO codes/objects/artifacts acquired from the player?
	    private Dictionary <Character, feelingsAboutChar> Relationships {get; set;}
    }
	
	/// <summary>
	/// Feelings possessed by one char towards another. Values should be -2 to +2. 
	/// </summary>
	struct feelingsAboutChar 
	{
		int Trust {set; get;} // -2 = never believe, +2 = always believe
		int Like {set; get;} // -2 = hate, +2 = adore
	}
}
