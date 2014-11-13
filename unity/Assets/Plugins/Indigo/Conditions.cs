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
    /// A static class which enumerates all possible conditions (delegates).
    /// When constructing preconditions, the Condition delegate should be assigned to one of these static functions.
    /// </summary>
    public static class ConditionLibrary
    {
        /// <summary>
        /// Returns whether or not the instigating character is alive.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsInstigatorAlive(GameState state, Character instigator, Character receiver = null, Item item = null)
        {
            return state.Characters.Find(c => c.Name == instigator.Name).HasStatus("Alive");
        }

        /// <summary>
        /// Returns whether or not the receiving character is alive.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsReceiverAlive(GameState state, Character instigator, Character receiver = null, Item item = null) {
            return state.Characters.Find(c => c.Name == receiver.Name).HasStatus("Alive");
        }

        /// <summary>
        /// Returns whether or not the instigating character has a particular item.
        /// </summary>
        public static bool DoesInstigatorHaveItem(GameState state, Character instigator, Character receiver, Item item)
        {
            return state.Characters.Find(c => c.Name == instigator.Name).Items.Contains(item);
        }

        /// <summary>
        /// Returns whether or not the receiving character has a particular item.
        /// </summary>
        public static bool DoesReceiverHaveItem(GameState state, Character instigator, Character receiver, Item item) {
            return state.Characters.Find(c => c.Name == receiver.Name).Items.Contains(item);
        }
    }
}
