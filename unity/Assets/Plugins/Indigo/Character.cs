using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{
  public class Character
  {
    /// <summary>
    /// A list of statuses a character may have.
    /// </summary>
    public List<string> CHARACTER_STATUSES = new List<string> { "Alive", "Mobile" };

    // Smarterish properties.
    public string Name { get; private set; }
    public bool Hidden { get; set; }
    //public List<Attribute> attributes; // TODO emotions, trust, etc

    public List<string> Statuses { get; private set; }

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
      this.Hidden = false;
      this.Relationships = new Dictionary<string, feelingsAboutChar>();
      this.Goal = new List<Condition> ();
      this.Items = new List<Item> ();
      this.Statuses = new List<string>();
    }

    public Character(string name, float x, float y): this(name){
      X = x;
      Y = y;
    }

    public Character Clone(){
      Character clone = new Character (Name, X, Y);
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

      foreach (var status in this.Statuses) {
	clone.AddStatus(status);
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

#region STATUS STUFF
    /// <summary>
    /// Adds a new status to the character.
    /// </summary>
    /// <param name="statusName">The name of a status (e.g. "Alive").</param>
    /// <returns>True, if the status was a valid status and added to the character, false otherwise.</returns>
    public bool AddStatus(string statusName) {
      if (CHARACTER_STATUSES.Contains(statusName) && !this.Statuses.Contains(statusName)) {
	this.Statuses.Add(statusName);
	return true;
      }
      return false;
    }

    /// <summary>
    /// Checks if the character has a particular status.
    /// </summary>
    /// <param name="statusName">The name of a status (e.g. "Alive").</param>
    /// <returns>True, if the character has this status, false otherwise.</returns>
    public bool HasStatus(string statusName) {
      return this.Statuses.Contains(statusName);
    }

    /// <summary>
    /// Removes a status from a character.
    /// </summary>
    /// <param name="statusName">The name of a status (e.g. "Alive").</param>
    /// <returns>True, if the characrer has this status and it is removed successfully, false otherwise.</returns>
    public bool RemoveStatus(string statusName) {
      if (CHARACTER_STATUSES.Contains(statusName)) {
	// List.Remove should handle the contains check.
	return this.Statuses.Remove(statusName);
      }
      return false;
    }

    /// <summary>
    /// A shorthand function for checking aliveness/deadness.
    /// </summary>
    /// <returns>True, if the character has the "Alive" status, false otherwise.</returns>
    public bool IsAlive() {
      return this.HasStatus("Alive");
    }
#endregion

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

    // ACTION STUFF


    /// <summary>
    /// Score the results of an action against the goal preconditions, counting the size of the overlap; no normalization needed
    /// </summary>
    private int scoreForGoals (GameState actionResult, Character recipient, Item itm){
      return this.Goal.Aggregate(0, (score, gc) => score + (gc(actionResult, this, recipient, itm) ? 1 : 0)); // xxx doesn't really make sense with the current condition implementation
    }

    /// <summary>
    /// Return the state after evaluating the preferred action given the character's goals. 
    /// </summary>
    /// <param name="actions"> List of action aggregates that may be possible right now given the intensity </param>
    /// <param name="currentState"> Current world state </param>
    /// <returns> The GameState after evaluating the best action, or the current gamestate if waiting is best </returns>
    public GameState evaluateBestAction (List <ActionAggregate> actions, GameState currentState) {  
      GameState bestActionState = currentState;
      int bestActionScore = scoreForGoals(currentState, null, null); // xxx state design doesn't seem to support this well
      foreach (ActionAggregate act in actions){
	foreach (Character person in currentState.Characters){
	  foreach (Item itm in this.Items) {
	    try {
	      GameState actionState = act.EvaluateAction(currentState, this, person, itm);
	      int score = scoreForGoals(actionState,
					person,
					itm);
	      if (score > bestActionScore) {
		bestActionState = actionState;
		bestActionScore = score; 
	      }
	    } catch (InvalidOperationException e) { 
	      // do nothing; preconditions failed
	    } // catch
	  } // items
	} // characters
      } // actions
      return bestActionState;
    } // END chooseBestAction
  } // END Character
	
  /// <summary>
  /// Feelings possessed by one char towards another. Values should be -2 to +2. 
  /// </summary>
  public struct feelingsAboutChar 
  {
    public int Trust {set; get;} // -2 = never believe, +2 = always believe
    public int Like {set; get;} // -2 = hate, +2 = adore
  }
}
