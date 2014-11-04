using System.Collections;

namespace Indigo{
	public class Item  {
		public string Name { get; private set; }
		//Location; 
		public float X{ get; private set; }
		public float Y{ get; private set; }
		public bool Hidden{ get; private set; }
		public bool Alive{ get; private set; }

		public Item(string name, float x, float y){
			Name = name;
			X = x;
			Y = y;

			Hidden = true;
			Alive = true;
		}

		public void SetHidden(bool hidden){
			Hidden = hidden;
		}

		public void SetAlive(bool alive){
			Alive = alive;
		}

		public Item Clone(){
			Item clone = new Item (Name, X, Y);
			clone.SetHidden (Hidden);
			clone.SetAlive (Alive);

			return clone;
		}

		public override bool Equals (object obj){
			Item other = (Item)obj;
			return obj!=null && other.Name.Equals(Name);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
}
