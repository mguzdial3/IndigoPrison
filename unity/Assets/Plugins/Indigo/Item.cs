using System.Collections;
using System.Collections.Generic;

namespace Indigo{
  public class Item  {
    public string Name { get; private set; }
    public List<string> ITEM_STATUSES = new List<string> { "Lethal", "Liberating" }; // Why this way?
    public List<string> Statuses { get; private set; }
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
      this.Statuses = new List<string>();
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
      foreach (var status in this.Statuses) {
		clone.AddStatus(status);
      }
      

      return clone;
    }

#region STATUS STUFF
    /// <summary>
    /// Adds a new status to the item.
    /// </summary>
    /// <param name="statusName">The name of a status (e.g. "Lethal").</param>
    /// <returns>True, if the status was a valid status and added to the item, false otherwise.</returns>
    public bool AddStatus(string statusName) {
      if ( !this.Statuses.Contains(statusName)) {
	this.Statuses.Add(statusName);
	return true;
      }
      return false;
    }

    /// <summary>
    /// Checks if the item has a particular status.
    /// </summary>
    /// <param name="statusName">The name of a status (e.g. "Lethal").</param>
    /// <returns>True, if the item has this status, false otherwise.</returns>
    public bool HasStatus(string statusName) {
      return this.Statuses.Contains(statusName);
    }

    /// <summary>
    /// Removes a status from a item.
    /// </summary>
    /// <param name="statusName">The name of a status (e.g. "Lethal").</param>
    /// <returns>True, if the character has this status and it is removed successfully, false otherwise.</returns>
    public bool RemoveStatus(string statusName) {
      if (ITEM_STATUSES.Contains(statusName)) {
	// List.Remove should handle the contains check.
	return this.Statuses.Remove(statusName);
      }
      return false;
    }

    /// <summary>
    /// A shorthand function for checking aliveness/deadness of the item. Is it a fresh banana? 
    /// </summary>
    /// <returns>True, if the character has the "Alive" status, false otherwise.</returns>
    public bool IsAlive() {
      return this.HasStatus("Alive");
    }
#endregion

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
