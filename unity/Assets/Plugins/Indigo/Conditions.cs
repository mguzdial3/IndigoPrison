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
        public CharacterGoal(Condition condition, Character instigator, Character receiver, Item item, int priority=0) {
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
            return (character != null) && character.IsAlive() ;
        }

        /// <summary>
        /// Returns whether or not the receiving character is alive.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsReceiverAlive(GameState state, Character instigator, Character receiver, Item item = null) {
			Character character = null;
			if(receiver!=null){
            	character = state.GetCharacter(receiver.Name);
			}
            return(character != null) && character.IsAlive() ;
        }

        /// <summary>
        /// Returns whether or not the instigating character is dead.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsInstigatorDead(GameState state, Character instigator, Character receiver = null, Item item = null) {
            var character = state.GetCharacter(instigator.Name);
            return (character != null) && !character.IsAlive();
        }

        /// <summary>
        /// Returns whether or not the receiving character is dead.
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsReceiverDead(GameState state, Character instigator, Character receiver, Item item = null) {
            var character = state.GetCharacter(receiver.Name);
            return (character != null) && !character.IsAlive();
        }
        #endregion

        /// <summary>
        /// Returns whether or not the instigating character has a "mobile" status (i.e. can move).
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsInstigatorMobile(GameState state, Character instigator, Character receiver = null, Item item = null) {
            var character = state.GetCharacter(instigator.Name);
            return (character != null) && character.HasStatus("Mobile");
        }

        /// <summary>
        /// Returns whether or not the receiving character has a "mobile" status (i.e. can move).
        /// Makes use of optional parameters on the condition.
        /// </summary>
        public static bool IsReceiverMobile(GameState state, Character instigator, Character receiver, Item item = null) {
            var character = state.GetCharacter(receiver.Name);
            return (character != null) && character.HasStatus("Mobile");
        }

		#region PLAYER CONDITIONS

		public static bool PlayerKnowsAboutDesireToKill(GameState state, Character instigator, Character receiver, Item item = null) {
			return instigator!=null && receiver!=null && state.Player.HasStatus (instigator.Name + "WantsToKill" + receiver.Name);
		}

		public static bool PlayerDoesntKnowAboutDesireToKill(GameState state, Character instigator, Character receiver, Item item = null) {
			return instigator!=null && receiver!=null && !state.Player.HasStatus (instigator.Name + "WantsToKill" + receiver.Name);
		}

		public static bool PlayerKnowsAboutWantsItem(GameState state, Character instigator, Character receiver, Item item = null) {
			return instigator!=null && state.Player.HasStatus (instigator.Name + "WantsItem");
		}
		
		public static bool PlayerDoesntKnowAboutWantsItem(GameState state, Character instigator, Character receiver, Item item = null) {
			return instigator!=null && !state.Player.HasStatus (instigator.Name + "WantsItem");
		}

		public static bool PlayerKnowsInstigator(GameState state, Character instigator, Character receiver, Item item = null) {
			return instigator!=null && state.Player.HasStatus ("Knows" + instigator.Name);
		}

		public static bool PlayerDoesntKnowsInstigator(GameState state, Character instigator, Character receiver, Item item = null) {
			return instigator!=null && !state.Player.HasStatus ("Knows" + instigator.Name);
		}

		public static bool PlayerKnowsToKillReceiver(GameState state, Character instigator, Character receiver, Item item = null) {
			return receiver!=null && state.Player.HasStatus ("KnowsToKill" + receiver.Name);
		}

		public static bool PlayerDoesntKnowToKillReceiver(GameState state, Character instigator, Character receiver, Item item = null) {
			return receiver!=null && !state.Player.HasStatus ("KnowsToKill" + receiver.Name);
		}

		public static bool PlayerHasLethalItem(GameState state, Character instigator, Character receiver, Item item = null) {
			return item!=null && state.Player.Items.Find (itom=>itom.HasStatus("Lethal"))!=null;
		}

        /// <summary>
        /// Returns whether or not the player has a liberating item.
        /// </summary>
        public static bool PlayerHasLiberatingItem(GameState state, Character instigator, Character receiver, Item item = null) {
            return (state.Player.Items.Find(i => i.HasStatus("Liberating")) != null);
        }

		public static bool PlayerKnowsSomeone(GameState state, Character instigator, Character receiver, Item item = null){
			bool knowsSomeone = false;

			foreach (string s in state.Player.Statuses) {
				if(s.Contains("Knows")){
					knowsSomeone = true;
				}
			}

			return knowsSomeone;
		}

		public static bool PlayerHasReceiverItem(GameState state, Character instigator, Character receiver, Item item) {
			bool canDo = false;
			if(receiver!=null){
				Item itm = state.Player.Items.Find (itom=>itom.Name.Contains(receiver.Name));
				canDo = itm!=null;
			}
			return canDo;
		}
		
		public static bool PlayerHasInstigatorItem(GameState state, Character instigator, Character receiver, Item item) {
			bool canDo = false;
			if(instigator!=null){
				Item itm = state.Player.Items.Find (itom=>itom.Name.Contains(instigator.Name));
				canDo = itm!=null;
			}
			return canDo;
		}

		public static bool PlayerCloseEnoughToRevealReceiver(GameState state, Character instigator, Character receiver, Item item) {
			return receiver != null && DramaManager.GetDist (state.Player, receiver) < DramaManager.MIN_DIST;
		}

		#endregion
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

		public static bool DoesInstigatorNotHaveItem(GameState state, Character instigator, Character receiver, Item item) {
			return !state.Characters.Find(c => c.Name == instigator.Name).Items.Contains(item);
		}

		public static bool ItemIsInstigators(GameState state, Character instigator, Character receiver, Item item) {
			return item!=null && instigator!=null &&state.Characters.Find(c => c.Name == instigator.Name).Items.Contains(item);
		}

        /// <summary>
        /// Returns whether or not the receiving character has a particular item.
        /// </summary>
        /// 
        public static bool DoesReceiverHaveItem(GameState state, Character instigator, Character receiver, Item item) {
            return state.Characters.Find(c => c.Name == receiver.Name).Items.Contains(item);
        }

		public static bool StateHasLethalItem(GameState state, Character instigator, Character receiver, Item item) {
			Item itm = state.Items.Find (itom=>itom.Statuses.Contains("Lethal"));
			return itm!=null;
		}

		public static bool DoesStateNotHaveLethalItem(GameState state, Character instigator, Character receiver, Item item) {
			Item itm = state.Items.Find(i => i.Statuses.Contains("Lethal"));
			return (itm == null);
		}

        /// <summary>
        /// Returns whether or not the game environment contains a liberating item.
        /// </summary>
        public static bool DoesStateHaveLiberatingItem(GameState state, Character instigator, Character receiver, Item item) {
            Item itm = state.Items.Find(i => i.Statuses.Contains("Liberating"));
            return (itm != null);
        }

		public static bool DoesStateNotHaveLiberatingItem(GameState state, Character instigator, Character receiver, Item item) {
			Item itm = state.Items.Find(i => i.Statuses.Contains("Liberating"));
			return (itm == null);
		}

		public static bool DoesStateNotHaveInstigatorItem(GameState state, Character instigator, Character receiver, Item item) {
			return instigator!=null && (state.Items.Find(i => i.Name.Contains(instigator.Name)) == null) ;
		}

		public static bool IsItemDistance(GameState state, Character instigator, Character receiver, Item item) {
			return item!=null && item.HasStatus("Distance");
		}

		public static bool IsItemNotDistance(GameState state, Character instigator, Character receiver, Item item) {
			return item!=null && !item.HasStatus("Distance");
		}

        public static bool IsItemLethal(GameState state, Character instigator, Character receiver, Item item) {
            return (item != null) && item.HasStatus("Lethal");
        }

        public static bool IsItemLiberating(GameState state, Character instigator, Character receiver, Item item) {
            return (item != null) && item.HasStatus("Liberating");
        }

		#endregion

		#region HIDDEN CONDITIONS

		/// <summary>
		/// Returns whether or not the instigator is visible
		/// </summary>
		public static bool IsInstigatorVisible(GameState state, Character instigator, Character receiver, Item item) {
			return instigator!=null &&!state.Characters.Find(c => c.Name == instigator.Name).Hidden;
		}

		/// <summary>
		/// Returns whether or not the instigator is hidden
		/// </summary>
		public static bool IsInstigatorHidden(GameState state, Character instigator, Character receiver, Item item) {
			return instigator!=null && state.Characters.Find(c => c.Name == instigator.Name).Hidden;
		}

        #endregion

		#region DISTANCE CONDITIONS

		public static bool CloseEnoughToReveal(GameState state, Character instigator, Character receiver, Item item) {
			return instigator != null && receiver != null && DramaManager.GetDist (instigator, receiver) < DramaManager.MIN_REVEAL;
		}

		public static bool CloseEnoughToInteract(GameState state, Character instigator, Character receiver, Item item) {
			return instigator != null && receiver != null && DramaManager.GetDist (instigator, receiver) < DramaManager.MIN_DIST;
		}

		public static bool CloseEnoughToPlayer(GameState state, Character instigator, Character receiver, Item item) {
			return instigator != null && DramaManager.GetDist (instigator, state.Player) < DramaManager.MIN_DIST;
		}

		#endregion

		#region WEIRD CONDITIONS

		public static bool NoCondition(GameState state, Character instigator, Character receiver, Item item) {
			return true;		
		}

		public static bool InstigatorNotReceiver(GameState state, Character instigator, Character receiver, Item item) {
			return instigator!=null && receiver!=null && instigator.Name!=receiver.Name;		
		}

		public static bool NotEnded(GameState state, Character instigator, Character receiver, Item item) {
			return state.Intensity!=6;		
		}

		#endregion
    }
}
