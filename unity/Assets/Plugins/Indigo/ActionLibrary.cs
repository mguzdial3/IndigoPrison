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
        #region SINGLE CHARACTER ACTIONS
        /// <summary>
        /// The instigating character finds the item lying around the environment.
        /// </summary>
        public static GameState FindItem(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var finder = newState.GetCharacter(instigator.Name);

            if (finder != null && newState.Items.Contains(item)) {
                newState.Items.Remove(item);
                finder.Items.Add(item);
                newState.AddLine(finder.Name, new DialogueLine(finder.Name, "Well look here! I found a " + item.Name));
            }

            return newState;
        }

        // The instigating character frees him/her/itself with a liberating item.
        public static GameState FreeCharacterWithItem(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var liberated = newState.GetCharacter(instigator.Name);

            if (liberated != null && liberated.HasItem(item)) {
                liberated.Items.Remove(item);
                newState.AddLine(liberated.Name, new DialogueLine(liberated.Name, "I'm free, no thanks to this " + item.Name + "!"));
            }

            return newState;
        }

        /// <summary>
        /// The instigating character hides him/her/itself.
        /// </summary>
        public static GameState HideSelf(GameState state, Character instigator, Character receiver, Item item) {
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

        /// <summary>
        /// The instigating character chooses to wait. The state is unchanged.
        /// </summary>
        public static GameState Wait(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            if (newState.GetCharacter(instigator.Name) != null) {
                newState.AddLine(instigator.Name, new DialogueLine(instigator.Name, "I guess I can do nothing but wait."));
            }
            return newState;
        }
        #endregion

        #region TWO-CHARACTER ACTIONS
        /// <summary>
        /// The instgating character gives the receiving character an item.
        /// </summary>
        public static GameState GiveCharacterItem(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var giver = newState.GetCharacter(instigator.Name);
            var given = newState.GetCharacter(receiver.Name);

            // Obligatory null checks.
            if (giver != null && given != null && giver.HasItem(item)) {
                giver.Items.Remove(item);
                given.Items.Add(item);
                newState.AddLine(giver.Name, new DialogueLine(giver.Name, "Here you go."));
                newState.AddLine(given.Name, new DialogueLine(receiver.Name, "Hey thanks for the " + item.Name + "."));
            }

            return newState;
        }

        /// <summary>
        /// The instigating character insults the receiving character, lowering trust/like of the receiver for the instigator.
        /// </summary>
        public static GameState InsultCharacter(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var jerk = newState.GetCharacter(instigator.Name);
            var insultee = newState.GetCharacter(receiver.Name);

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

        /// <summary>
        /// The instigating character is given a quest by the the receiving character.
        /// The receiving character is dehidden and their initial item quest is added to the world state.
        /// </summary>
        public static GameState IntroduceCharacterQuest(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            Location loc = DramaManager.Instance
                .GetNewItemLocation(state, instigator);

            var instigatorIndex = newState.Characters.FindIndex(c => c.Name == instigator.Name);

            newState.Characters[instigatorIndex].Hidden = false;

            Item guardItem = new Item(instigator.Name + "Item", loc.X, loc.Y);
            newState.AddItem(guardItem);
            guardItem.SetHidden(false);

            newState.AddLine(instigator.Name, new DialogueLine(instigator.Name, "Hey get this!"));

            return newState;
        }

        /// <summary>
        /// The instigating character kills the receiving character. No items are involved.
        /// </summary>
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
			
			newState.AddLine(instigator.Name, new DialogueLine(instigator.Name,"Haha, now I can kill the "+receiver.Name));
            return newState;
        }

        /// <summary>
        /// The instigating character locks up the receiving character.
        /// </summary>
        public static GameState LockUpCharacter(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();

            // May not be an actual guard, but this needed a variable name that wasn't "lockerUpper"
            var guard = newState.GetCharacter(instigator.Name);
            var prisoner = newState.GetCharacter(receiver.Name);

            if (guard != null && prisoner != null) {
                prisoner.RemoveStatus("Mobile");
                // TODO (kasiu): Add some more/better lines?
                newState.AddLine(instigator.Name, new DialogueLine(instigator.Name, "Stay locked up in there!"));
            }

            return newState;
        }

        // Steals an item from another character. One of the preconditions should be that the receiver has the item.
        /// <summary>
        /// The instigating character steals an item from the receiving character.
        /// One of the preconditions should be that the receiving character has the item.
        /// </summary>
        public static GameState StealItemFromCharacter(GameState state, Character instigator, Character receiver, Item item) {
            GameState newState = state.Clone();
            var thief = newState.GetCharacter(instigator.Name);
            var victim = newState.GetCharacter(receiver.Name);

            // Nothing should happen if for some reason the game state is mucked up.
            if (thief != null && victim != null && victim.HasItem(item)) {
                victim.Items.Remove(item);
                thief.Items.Add(item);
                newState.AddLine(thief.Name, new DialogueLine(thief.Name, "Success! This " + item.Name + " is mine!"));
                newState.AddLine(victim.Name, new DialogueLine(victim.Name, "Wait...where did the " + item.Name + " go?"));
            }

            return newState;
        }
        #endregion
    }
}
