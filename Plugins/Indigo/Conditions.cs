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
    /// <param name="name">The name of the character from whose context the state is to be evaluated.</param>
    /// <returns>True, if the given condition holds for the current state from the context of the checker. False otherwise.</returns>
    public delegate bool Condition(GameState state, Character target);

    /// <summary>
    /// Holds a condition and the character it applies to (if applicable, as the character could be null).
    /// Used when specifying actions.
    /// </summary>
    public struct ConditionPair
    {
        public Condition Condition { get; set; }
        public Character Target { get; set; }

        public ConditionPair(Condition condition, Character target) : this()
        {
            Condition = condition;
            Target = target;
        }
    }

    /// <summary>
    /// A static class which enumerates all possible conditions (delegates).
    /// When constructing preconditions, the Condition delegate should be assigned to one of these static functions.
    /// </summary>
    public static class ConditionLibrary
    {
        // TEMP: This is an example of a condition.
        public static bool IsAlive(GameState state, Character target)
        {
            return state.Characters.Find(c => c.Name == target.Name).Alive;
        }
    }
}
