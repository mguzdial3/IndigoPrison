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
            return this.Preconditions.Aggregate(true, (value, pc) => value & pc.Condition(state, pc.Target));
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
            var preconditions = new List<ConditionPair>{ new ConditionPair(ConditionLibrary.IsAlive, alice), new ConditionPair(ConditionLibrary.IsAlive, bob) };
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
            return newState;
        }
    }
}
