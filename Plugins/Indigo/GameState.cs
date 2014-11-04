using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{
    /// <summary>
    /// The game state.
    /// </summary>
    /// NOTE: Currently not static (although such a property might be ideal). 
    /// We should have a disucssion about the mutability of state and such.
    public class GameState
    {
        public List<Character> Characters { get; set; }
        public Character Player { get; set; }
        public int Intensity { get; set; }

        // TODO: Add more things here as needed.
        // public List<Item> Items { get; set; }

        /// <summary>
        /// Creates a deep copy of the game state.
        /// </summary>
        /// <returns></returns>
        public GameState Clone()
        {
            GameState newState = new GameState();
            // Characters (as structs are copied by value)
            newState.Characters = new List<Character>(this.Characters);
            newState.Player = this.Player;
            newState.Intensity = this.Intensity;
            return newState;
        }
    }
}
