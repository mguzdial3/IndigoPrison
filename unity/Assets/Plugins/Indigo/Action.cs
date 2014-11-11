using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{
    /// <summary>
    /// Represents an action on the game state, generally instigated by one character towards another.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <param name="instigator">The primary instigator of the action.</param>
    /// <param name="receiver">The primary receiver of the action.</param>
    /// <param name="item">The target item for this action.</param>
    /// <returns>The new game state after applying the effects of the action.</returns>
    public delegate GameState Action(GameState state, Character instigator, Character receiver, Item item);

    /// <summary>
    /// A "container" to match actions with preconditions and an intensity, as well as the characters they act on.
    /// </summary>
    public class ActionAggregate
    {
        /// <summary>
        /// The action.
        /// </summary>
        public Action Action { get; private set; }
        
        /// <summary>
        /// The intensity level associated with this action.
        /// </summary>
        public int Intensity { get; private set; }

        /// <summary>
        /// The preconditions.
        /// </summary>
        public List<ConditionPair> Preconditions { get; private set; }

        /// <summary>
        /// The primary character instigating the action.
        /// </summary>
        public Character Instigator { get; private set; }

        /// <summary>
        /// The character at the receiving end of the action.
        /// </summary>
        public Character Receiver { get; private set; }

        /// <summary>
        /// The target item in this action.
        /// </summary>
        public Item Item { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="intensity">The intensity.</param>
        /// <param name="preconditions">The preconditions.</param>
        /// <param name="instigator">The instigator of the action.</param>
        /// <param name="receiver">The receiver for the action.</param>
        /// <param name="item">The target item for this action.</param>
        public ActionAggregate(Action action, int intensity, List<ConditionPair> preconditions, Character instigator, Character receiver, Item item)
        {
            this.Action = action;
            this.Intensity = intensity;
            this.Preconditions = preconditions;
            this.Instigator = instigator;
            this.Receiver = receiver;
            this.Item = item;
        }

        /// <summary>
        /// Evaluates the action.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="InvalidOperationException">Throws if preconditions are violated.</exception>
        /// <returns></returns>
        public GameState EvaluateAction(GameState state)
        {
            if (!DoPreconditionsHold(state)) {
                throw new InvalidOperationException("Preconditions for this action are not met!");
            }
            return this.Action(state, this.Instigator, this.Receiver, this.Item);
        }

        /// <summary>
        /// Checks if the preconditions for this action hold on a given state.
        /// </summary>
        /// <param name="state">The current game state.</param>
        /// <returns>True if none of the preconditions are violated, false otherwise.</returns>
        public bool DoPreconditionsHold(GameState state)
        {
            return this.Preconditions.Aggregate(true, (value, pc) => value & pc.Condition(state, pc.Target, pc.Item));
        }
    }

    /// <summary>
    /// A container which stores all possible actions and provides access to them.
    /// </summary>
    public class ActionManager
    {
        private List<ActionAggregate> allActions;

        public ActionManager()
        {
            allActions = new List<ActionAggregate>();
            
            // TEMP: Here's an example of how one might add an action to the manager for a pair of characters.
            // THIS SHOULD OBVIOUSLY NOT BE DONE IN THE CONSTRUCTOR. SERIOUSLY.
            var alice = new Character("Alice");
            var bob = new Character("Bob");
            var preconditions = new List<ConditionPair>{ new ConditionPair(ConditionLibrary.IsAlive, alice, null), new ConditionPair(ConditionLibrary.IsAlive, bob, null) };
            var killCharacter = new ActionAggregate(ActionLibrary.KillCharacter, 3, preconditions, alice, bob, null);
            this.AddAction(killCharacter);
        }

        /// <summary>
        /// Adds a new action to the library.
        /// </summary>
        /// <param name="newAction">The new action.</param>
        public void AddAction(ActionAggregate newAction)
        {
            allActions.Add(newAction);
        }

        /// <summary>
        /// Returns a subset of actions with the specified intensity.
        /// </summary>
        /// <param name="intensity">The intensity of the action.</param>
        /// <returns>An enumerable object of actions.</returns>
        public IEnumerable<ActionAggregate> GetActions(int intensity)
        {
            return allActions.Where(action => action.Intensity == intensity);
        }
    }

    /// <summary>
    /// A static class which enumerates all possible actions (delegates).
    /// These are then paired with preconditions to form the actions that the drama manager can act on.
    /// </summary>
    public static class ActionLibrary
    {
        // TEMP: Here's an example of an action that kills a character.
        public static GameState KillCharacter(GameState state, Character instigator, Character receiver, Item item)
        {
            GameState newState = state.Clone();
            var killedIndex = newState.Characters.FindIndex(c => c.Name == receiver.Name);
            // If the character exists...
            if (killedIndex >= 0 && killedIndex < newState.Characters.Count()) {
                var killed = newState.Characters[killedIndex];
                // KILL IT (and overwrite the old character)
                killed.Alive = false;
                newState.Characters[killedIndex] = killed;
            }
			item.SetAlive (false);
			newState.AddLine(instigator.Name, new DialogueLine(instigator.Name,"Haha, now I can kill the "+receiver.Name));

            return newState;
        }

        // One character insults another, lowering the second's trust and like for the first.
        public static GameState Insult(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();

            var jerk = newState.Characters.Find(c => c.Name == instigator.Name);
            var insultee = newState.Characters.Find(c => c.Name == receiver.Name);

            if (jerk != null && insultee != null) {
                // HACK (kasiu): SOMEONE THINK OF A BETTER INSULT TO PUT HERE.
                newState.AddLine(jerk.Name, new DialogueLine(jerk.Name, "You know " + insultee.Name + ", you're an asshole."));
                newState.AddLine(insultee.Name, new DialogueLine(insultee.Name, "What? You try saying that again!"));
                var oldFeelings = insultee.GetRelationship(jerk.Name);
                var newFeelings = new feelingsAboutChar();
                // Lowers trust and like by -1
                newFeelings.Trust = Math.Max(oldFeelings.Trust - 1, -2);
                newFeelings.Like = Math.Max(oldFeelings.Like - 1, -2);
                insultee.ChangeRelationship(jerk.Name, newFeelings);
            }
            
            return newState;
        }

        // Steals an item from another character. One of the preconditions should be that the receiver has the item.
        public static GameState StealItem(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var thief = newState.Characters.Find(c => c.Name == instigator.Name);
            var victim = newState.Characters.Find(c => c.Name == receiver.Name);

            // Nothing should happen if for some reason the game state is mucked up.
            if (thief != null && victim != null && victim.HasItem(item)) {
                victim.Items.Remove(item);
                thief.Items.Add(item);
                newState.AddLine(thief.Name, new DialogueLine(thief.Name, "Success! This " + item.Name + " is mine!"));
                newState.AddLine(victim.Name, new DialogueLine(receiver.Name, "Wait...where did the " + item.Name + " go?"));
            }

            return newState;
        }

		//Introduces a character by dehiding them, and adding their initial item quest to the world state
		public static GameState IntroduceCharacterQuest(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			Location loc = DramaManager.Instance
				.GetNewItemLocation (state,instigator);

			var instigatorIndex = newState.Characters.FindIndex(c => c.Name == instigator.Name);

			newState.Characters[instigatorIndex].Hidden = false;

			Item guardItem = new Item (instigator.Name + "Item", loc.X, loc.Y);
			newState.AddItem (guardItem);
			guardItem.SetHidden (false);

			newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Hey get this!"));

			return newState;
		}
    }
}
