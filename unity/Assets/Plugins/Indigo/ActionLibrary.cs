using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{
    /// <summary>
    /// A static class which enumerates all possible actions (delegates).
    /// These are then paired with preconditions to form the actions that the drama manager can act on.
    /// </summary>
    public static class ActionLibrary
    {
        // TEMP: Here's an example of an action that kills a character.
        public static GameState KillCharacter(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var killedIndex = newState.Characters.FindIndex(c => c.Name == receiver.Name);
            // If the character exists...
            if (killedIndex >= 0 && killedIndex < newState.Characters.Count()) {
                var killed = newState.Characters[killedIndex];
                // KILL IT (and overwrite the old character)
                killed.RemoveStatus("Alive");
                newState.Characters[killedIndex] = killed;
            }
			item.SetAlive (false);
			newState.AddLine(instigator.Name, new DialogueLine(instigator.Name,"Haha, now I can kill the "+receiver.Name));

            return newState;
        }

        // One character waits.
        public static GameState Wait(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            newState.AddLine(instigator.Name, new DialogueLine(instigator.Name, "I guess I can do nothing but wait."));
            return newState;
        }

        // One character locks up another character.
        public static GameState LockUpCharacter(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();

            // May not be an actual guard, but this needed a variable name that wasn't "lockerUpper"
            var guard = newState.GetCharacter(instigator.Name);
            var prisoner = newState.GetCharacter(receiver.Name);

            if (guard != null && prisoner != null) {
                prisoner.RemoveStatus("Mobile");
                // TODO (kasiu): Add some lines?
                newState.AddLine(instigator.Name, new DialogueLine(instigator.Name, "Stay locked up in there!"));
            }

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

        // We assume the instigator is the one hiding? Maybe I have hiding all wrong.
        public static GameState HideCharacter(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var hiddenIndex = newState.Characters.FindIndex(c => c.Name == instigator.Name);
            // If the character exists...
            if (hiddenIndex >= 0 && hiddenIndex < newState.Characters.Count()) {
                var hidden = newState.Characters[hiddenIndex];
                hidden.Hidden = false;
                newState.Characters[hiddenIndex] = hidden;
            }
            item.SetAlive(false);
            newState.AddLine(instigator.Name, new DialogueLine(instigator.Name, "No one will find me now!"));
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
