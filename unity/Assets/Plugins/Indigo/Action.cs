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
        public List<Condition> Preconditions { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="intensity">The intensity.</param>
        /// <param name="preconditions">The preconditions.</param>
        public ActionAggregate(Action action, int intensity, List<Condition> preconditions)
        {
            this.Action = action;
            this.Intensity = intensity;
            this.Preconditions = preconditions;
        }

        /// <summary>
        /// Evaluates the action.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="InvalidOperationException">Throws if preconditions are violated.</exception>
        /// <returns></returns>
        public GameState EvaluateAction(GameState state, Character instigator, Character receiver, Item item)
        {
            if (!DoPreconditionsHold(state, instigator, receiver, item)) {
                throw new InvalidOperationException("Preconditions for this action are not met!");
            }
            return this.Action(state, instigator, receiver, item);
        }

        /// <summary>
        /// Checks if the preconditions for this action hold on a given state.
        /// </summary>
        /// <param name="state">The current game state.</param>
        /// <returns>True if none of the preconditions are violated, false otherwise.</returns>
        public bool DoPreconditionsHold(GameState state, Character instigator, Character receiver, Item item)
        {
            return this.Preconditions.Aggregate(true, (value, pc) => value & pc(state, instigator, receiver, item));
        }
    } // END ActionAggregate

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
            var preconditions = new List<Condition>{ ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive };
            var killCharacter = new ActionAggregate(ActionLibrary.KillCharacter, 3, preconditions);
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
}
