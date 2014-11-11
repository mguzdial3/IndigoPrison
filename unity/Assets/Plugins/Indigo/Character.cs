using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{

    public class Character
    {
		// Smarterish properties.
		public string Name { get; private set; }
		public bool Alive { get; set; }
		public bool Hidden { get; set; }
		//public List<Attribute> attributes; // TODO emotions, trust, etc
		public List<Condition> Goal {get; set;} 
		//Location; 
		public float X{ get; private set; }
		public float Y{ get; private set; }
		//List<Action> plan; TODO;
		public Action nextAction {get; private set;}
		//Inventory; 
		public List<Item> Items;
		// charactername to feelingsAboutChar
		private Dictionary <string, feelingsAboutChar> Relationships {get; set;}

        public Character(string name) 
        {
            this.Name = name;
            this.Alive = true;
			this.Hidden = false;
			this.Relationships = new Dictionary<string, feelingsAboutChar>();
			this.Goal = new List<Condition> ();
			this.Items = new List<Item> ();
        }

		public Character(string name, float x, float y): this(name){
			X = x;
			Y = y;
		}

        public Character Clone(){
			Character clone = new Character (Name, X, Y);
			clone.Alive = Alive;
			clone.Hidden = Hidden;
			foreach (Condition condition in Goal) {
				clone.Goal.Add(condition);			
			}

			clone.SetAction (nextAction);

			foreach (KeyValuePair<string, feelingsAboutChar> kvp in Relationships) {
				clone.AddRelationship(kvp.Key,kvp.Value);			
			}

			foreach (Item item in Items) {
				clone.Items.Add(item.Clone());			
			}

			return clone;
		}

		//TODO; remove when we have this legit planning
		public void SetAction(Action toSet){
			nextAction = toSet;
		}

		//LOCATION STUFF
		public void SetLocation(float x, float y){
			X = x;
			Y = y;
		}

		//RELATIONSHIP STUFF
		public void AddRelationship(string characterName, feelingsAboutChar feeling){
			Relationships.Add (characterName, feeling);
		}

		public void ChangeRelationship(string characterName, feelingsAboutChar feeling){
			if (Relationships.ContainsKey (characterName)) {
				Relationships[characterName] = feeling;				
			}
		}

        public feelingsAboutChar GetRelationship(string characterName) {
            return this.Relationships[characterName];
        }

		//ITEM STUFF
		public bool HasItem(Item item){
			return Items.Contains (item);
		}

		 


    }
	
	/// <summary>
	/// Feelings possessed by one char towards another. Values should be -2 to +2. 
	/// </summary>
	public struct feelingsAboutChar 
	{
		public int Trust {set; get;} // -2 = never believe, +2 = always believe
		public int Like {set; get;} // -2 = hate, +2 = adore
	}
}
