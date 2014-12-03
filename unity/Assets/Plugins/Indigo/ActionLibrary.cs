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

		#region INTRODUCTIONS

		//Introduces a character by dehiding them, and adding their initial item quest to the world state
		public static GameState IntroduceMurderQuest(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();

			if(instigator!=null && receiver!=null){
				Location loc = DramaManager.Instance
					.GetNewItemLocation (state,instigator);

				var instigatorIndex = newState.Characters.FindIndex(c => c.Name == instigator.Name);

				newState.Characters[instigatorIndex].Hidden = false;

				Item guardItem = new Item (instigator.Name + "Item"+"Lethal", loc.X, loc.Y);
				newState.AddItem (guardItem);
				guardItem.AddStatus ("Lethal");
				guardItem.SetHidden (false);

				Random r = new Random();
				
				if(r.Next(0,2)==0){
					guardItem.AddStatus ("Distance");
				}

				newState.Player.AddStatus(instigator.Name+"WantsToKill"+receiver.Name);
				newState.Player.AddStatus("Knows"+instigator.Name);

				if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Hey buddy, not sure who you are, but I need this thing."));
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Gonna use it to take down a jerk. Sounds good, right?"));

				}
				else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Entity, I require your assistance."));
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "One of these monsters wishes me ill. Fetch this device to stop them."));
				}
			}
			return newState;
		}

		//Introduces a character by dehiding them, and adding their initial item quest to the world state
		public static GameState IntroduceSelf(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			var instigatorIndex = newState.Characters.FindIndex(c => c.Name == instigator.Name);
			
			newState.Characters[instigatorIndex].Hidden = false;

			newState.Player.AddStatus("Knows"+instigator.Name);

			if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
				newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Who the hell are you? You aren't a guard, or a prisoner. What's up with that?"));

			}
			else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
				newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Entity, what's your designation? You are not a guard or prisoner. But perhaps you can be useful."));
			}
			
			return newState;
		}

		//This one works from a distance, if the player is already helping the other character
		public static GameState IntroduceSelfDistance(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			var instigatorIndex = newState.Characters.FindIndex(c => c.Name == instigator.Name);
			
			newState.Characters[instigatorIndex].Hidden = false;
			
			newState.Player.AddStatus("Knows"+instigator.Name);
			
			if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
				newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Now what have we got here? Someone who can move through the prison?"));
				newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Why don't you come see me."));
				
			}
			else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
				newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Stop what you're doing immediately."));
				newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Report to me, Entity."));
			}
			
			return newState;
		}

		#endregion

		#region TELLSTUFF

		public static GameState TellWantToKill(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();

			if(instigator!=null && receiver!=null){
				newState.Player.AddStatus(instigator.Name+"WantsToKill"+receiver.Name);

				Location loc = DramaManager.Instance
					.GetNewItemLocation (state,instigator);

				Item guardItem = new Item (instigator.Name + "Item"+"Lethal", loc.X, loc.Y);
				newState.AddItem (guardItem);
				guardItem.AddStatus ("Lethal");

				Random r = new Random();

				if(r.Next(0,2)==0){
					guardItem.AddStatus ("Distance");
				}

				guardItem.SetHidden (false);

				if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Hey so I need you to get this thing for me. In order to take out a jerk."));
					
				}
				else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "One of these monsters wishes me ill. Fetch me this device to stop them."));
				}
			}
			return newState;
		}

		public static GameState TellPlayerToComeBack(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			if(instigator!=null && item!=null){
				newState.Player.AddStatus(instigator.Name+"WantsItem");

				if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "You got it! Head on back."));
				}
				else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Readings indicate you have the item, come on back."));
				}
			}
			return newState;
		}

		public static GameState TellPlayerToKill(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			if(instigator!=null && receiver!=null){
				newState.Player.AddStatus("KnowsToKill"+receiver.Name);
				
				if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Hey so you'll actually need to take this to the jerk to kill him."));
				}
				else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
					newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "You'll need to take this item to the other to neutralize the threat."));
				}
			}
			return newState;
		}

		#endregion

		#region KILL REGION
		public static GameState DistanceKillCharacter(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			if(instigator!=null  && receiver!=null){//&& item!=null
				var killedIndex = newState.Characters.FindIndex(c => c.Name == receiver.Name);
				if (killedIndex >= 0 && killedIndex < newState.Characters.Count()) {
					var killed = newState.Characters[killedIndex];

					// KILL IT (and overwrite the old character)
					killed.RemoveStatus("Alive");
					newState.Characters[killedIndex] = killed;
					killed.Hidden = true;
					//item.SetAlive (false);//TODO; this is a problem
				
					if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
						newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Haha, that'll show him."));
					}
					else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
						newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Threat neutralized, well done."));
					}
				}

			}
			return newState;
		}

		public static GameState CloseKillCharacter(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			if(item!=null && receiver!=null && instigator!=null){
				var killedIndex = newState.Characters.FindIndex(c => c.Name == receiver.Name);
				// If the character exists...
				if (killedIndex >= 0 && killedIndex < newState.Characters.Count()) {
					var killed = newState.Characters[killedIndex];
					// KILL IT (and overwrite the old character)
					killed.RemoveStatus("Alive");
					newState.Characters[killedIndex] = killed;

					item.SetAlive (false);

					if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
						newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Haha, that'll show him."));
					}
					else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
						newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Threat neutralized, well done."));
					}
				}

				//newState.AddLine(instigator.Name, new DialogueLine(instigator.Name,"));
			}
			return newState;
		}

		public static GameState TestKillCharacter(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			if(receiver!=null && instigator!=null){
				var killedIndex = newState.Characters.FindIndex(c => c.Name == receiver.Name);
				// If the character exists...
				if (killedIndex >= 0 && killedIndex < newState.Characters.Count()) {
					var killed = newState.Characters[killedIndex];
					// KILL IT (and overwrite the old character)
					killed.RemoveStatus("Alive");
					newState.Characters[killedIndex] = killed;

					
					if(instigator.Name.Contains(DramaManager.PRISONER_TITLE)){
						newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Haha, that'll show him."));
					}
					else if(instigator.Name.Contains(DramaManager.GUARD_TITLE)){
						newState.AddLine (instigator.Name,new DialogueLine(instigator.Name, "Threat neutralized, well done."));
					}
				}
				
				//newState.AddLine(instigator.Name, new DialogueLine(instigator.Name,"));
			}
			return newState;
		}

		#endregion

		#region DM ACTIONS

		public static GameState BlowUpThePrison(GameState state, Character instigator, Character receiver, Item item){
			GameState newState = state.Clone();
			
			newState.AddLine ("Indigo Prison",new DialogueLine("Indigo Prison", "The prison blew up. Whoops."));


			return newState;
		}

		#endregion

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

		#endregion
		
		#region TWO-CHARACTER ACTIONS

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

        #endregion
    }
}
