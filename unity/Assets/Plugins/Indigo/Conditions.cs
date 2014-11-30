using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{
    /// <summary>
    /// Represents a condition on the game state from the context of a given character.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <param name="instigator">The primary instigator of the condition.</param>
    /// <param name="receiver">The primary receiver of the condition.</param>
    /// <param name="item">The name of the item from whose context the state is to be evaluated.</param>
    /// <returns>True, if the given condition holds for the current state from the context of characters or item. False otherwise.</returns>
    public delegate bool Condition(GameState state, Character instigator, Character receiver, Item item);

    /// <summary>
    /// A container to permit Conditions to be used as character goals by pairing them with desired values.
    /// </summary>
    public class CharacterGoal
    {
        /// <summary>
        /// The goal condition.
        /// </summary>
        public Condition Condition { get; private set; }

        /// <summary>
        /// The primary character associated with this goal.
        /// </summary>
        public Character Instigator { get; private set; }

        /// <summary>
        /// The secondary character associated with this goal.
        /// </summary>
        public Character Receiver { get; private set; }

        /// <summary>
        /// The item associated with this goal.
        /// </summary>
        public Item Item { get; private set; }

        /// <summary>
        /// The goal's priority, if necessary.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="instigator">The primary character.</param>
        /// <param name="receiver">The secondary character.</param>
        /// <param name="item">The item.</param>
        /// <param name="priority">The goal priority.</param>
        public CharacterGoal(Condition condition, Character instigator, Character receiver, Item item, int priority) {
            this.Condition = condition;
            this.Instigator = instigator;
            this.Receiver = receiver;
            this.Item = item;
            this.Priority = priority;
        }

        /// <summary>
        /// Evaluates whether or not the goal condition is true for the given state.
        /// </summary>
        /// <param name="state">The game state.</param>
        /// <returns>True if the goal condition is met, false otherwise.</returns>
        public bool IsGoalSatisfied(GameState state) {
            return this.Condition(state, this.Instigator, this.Receiver, this.Item);
        }
    }

    /// <summary>
    /// A static class which enumerates all possible conditions (delegates).
    /// When constructing preconditions, the Condition delegate should be assigned to one of these static functions.
    /// </summary>
    public static class ConditionLibrary
    {
        #region ALIVE-DEAD CONDITIONS
        // NOTE (kasiu): The following four functions use the "IsAlive" method found in Character.cs.
        //               This is a shorthand method due to the fact that we check the "alive" status in 
        //               other modules, but is not something we should do for other character stasuses.
        //               For conditions that rely on other attributes or statuses, please us "HasStatus"
        //               with the appropriate string instead.

        /// <summary>
        /// Returns whether or not the instigating character is alive.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsInstigatorAlive(GameState state, Character instigator, Character receiver = null, Item item = null) {
            var character = state.GetCharacter(instigator.Name);
            return (character != null) ? character.IsAlive() : false;
        }

        /// <summary>
        /// Returns whether or not the receiving character is alive.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsReceiverAlive(GameState state, Character instigator, Character receiver, Item item = null) {
            var character = state.GetCharacter(receiver.Name);
            return(character != null) ? character.IsAlive() : false;
        }

        /// <summary>
        /// Returns whether or not the instigating character is dead.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsInstigatorDead(GameState state, Character instigator, Character receiver = null, Item item = null) {
            var character = state.GetCharacter(instigator.Name);
            return (character != null) ? !character.IsAlive() : false;
        }

        /// <summary>
        /// Returns whether or not the receiving character is dead.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsReceiverDead(GameState state, Character instigator, Character receiver, Item item = null) {
            var character = state.GetCharacter(receiver.Name);
            return (character != null) ? !character.IsAlive() : false;
        }
        #endregion

        /// <summary>
        /// Returns whether or not the instigating character has a "mobile" status (i.e. can move).
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsInstigatorMobile(GameState state, Character instigator, Character receiver = null, Item item = null) {
            var character = state.GetCharacter(instigator.Name);
            return (character != null) ? character.HasStatus("Mobile") : false;
        }

        /// <summary>
        /// Returns whether or not the receiving character has a "mobile" status (i.e. can move).
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsReceiverMobile(GameState state, Character instigator, Character receiver, Item item = null) {
            var character = state.GetCharacter(receiver.Name);
            return (character != null) ? character.HasStatus("Mobile") : false;
        }

        /// <summary>
        /// Returns whether or not the instigating character trusts the receiving character.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool DoesInstigatorTrustReceiver(GameState state, Character instigator, Character receiver, Item item = null) {
            var character = state.GetCharacter(instigator.Name);
            return (character != null) ? (character.GetRelationship(receiver.Name).Trust > 0) : false;
        }

        /// <summary>
        /// Returns whether or not the instigating character like the receiving character.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool DoesInstigatorLikeReceiver(GameState state, Character instigator, Character receiver, Item item = null) {
            var character = state.GetCharacter(instigator.Name);
            return (character != null) ? (character.GetRelationship(receiver.Name).Like > 0) : false;
        }

        #region ITEM CONDITIONS
        /// <summary>
        /// Returns whether or not the instigating character has a particular item.
        /// </summary>
        public static bool DoesInstigatorHaveItem(GameState state, Character instigator, Character receiver, Item item) {
            return state.Characters.Find(c => c.Name == instigator.Name).Items.Contains(item);
        }

        /// <summary>
        /// Returns whether or not the receiving character has a particular item.
        /// </summary>
        public static bool DoesReceiverHaveItem(GameState state, Character instigator, Character receiver, Item item) {
            return state.Characters.Find(c => c.Name == receiver.Name).Items.Contains(item);
        }
        #endregion

        public static bool IsItemLethal(GameState state, Character instigator, Character receiver, Item item) {
            return (item != null) ? item.HasStatus("Lethal") : false;
        }

        public static bool IsItemLiberating(GameState state, Character instigator, Character receiver, Item item) {
            return (item != null) ? item.HasStatus("Liberating") : false;
        }
    }
}
