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
            if (DoPreconditionsHold(state, instigator, receiver, item)) {
				GameState newState = this.Action(state, instigator, receiver, item);
				DramaManager.Instance.CheckIntensity(Intensity);
				return this.Action(state, instigator, receiver, item);
            }
			else{
				return null;
			}
            
        }

		public GameState EvaluateFutureAction(GameState state, Character instigator, Character receiver, Item item)
		{
			if (FuturePreconditionsHold(state, instigator, receiver, item)) {
				return this.Action(state, instigator, receiver, item);
			}
			else{
				return null;
			}
			
		}

        /// <summary>
        /// Checks if the preconditions for this action hold on a given state.
        /// </summary>
        /// <param name="state">The current game state.</param>
        /// <returns>True if none of the preconditions are violated, false otherwise.</returns>
        public bool DoPreconditionsHold(GameState state, Character instigator, Character receiver, Item item){
			 return this.Preconditions.Aggregate(true, (value, pc) => value & pc(state, instigator, receiver, item));
        }

		public bool FuturePreconditionsHold(GameState state, Character instigator, Character receiver, Item item){
			bool precogHolds = true;

			foreach (Condition cond in Preconditions) {
				if(!ActionManager.FuturePlayerConditions.Contains(cond)){
					if(!cond(state,instigator,receiver,item)){
						precogHolds =false;
					}
				}
			}

			return precogHolds;
		}

    } // END ActionAggregate

    /// <summary>
    /// A container which stores all possible actions and provides access to them.
    /// </summary>
    public class ActionManager
    {
        private List<ActionAggregate> allActions;
		private List<ActionAggregate> dmActions;
		public static readonly List<Condition> FuturePlayerConditions = new List<Condition> (){ConditionLibrary.CloseEnoughToReveal, ConditionLibrary.CloseEnoughToInteract, ConditionLibrary.CloseEnoughToPlayer};

        public ActionManager(){
			ConstructAllActions ();
			ConstructDMActions ();
        }

		private void ConstructAllActions(){
			allActions = new List<ActionAggregate>();

			var preconditions = new List<Condition>{ ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.PlayerKnowsAboutDesireToKill, ConditionLibrary.PlayerKnowsToKillReceiver, ConditionLibrary.PlayerHasLethalItem, ConditionLibrary.PlayerCloseEnoughToRevealReceiver, ConditionLibrary.PlayerHasInstigatorItem, ConditionLibrary.IsItemNotDistance};
			var killCharacter = new ActionAggregate(ActionLibrary.CloseKillCharacter, 5, preconditions);
			this.AddAction(killCharacter);

			preconditions = new List<Condition>{ ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.CloseEnoughToPlayer, ConditionLibrary.PlayerKnowsAboutDesireToKill, ConditionLibrary.PlayerHasLethalItem, ConditionLibrary.IsItemDistance, ConditionLibrary.PlayerHasInstigatorItem};//ConditionLibrary.IsItemDistance TODO; This is a problem
			killCharacter = new ActionAggregate(ActionLibrary.DistanceKillCharacter, 5, preconditions);
			this.AddAction(killCharacter);

			preconditions = new List<Condition>{ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.PlayerKnowsAboutDesireToKill, ConditionLibrary.PlayerHasLethalItem, ConditionLibrary.PlayerHasInstigatorItem, ConditionLibrary.PlayerDoesntKnowToKillReceiver, ConditionLibrary.PlayerDoesntKnowAboutWantsItem};
			killCharacter = new ActionAggregate(ActionLibrary.TellPlayerToComeBack, 3, preconditions);
			this.AddAction(killCharacter);

			preconditions = new List<Condition>{ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.PlayerHasLethalItem, ConditionLibrary.PlayerHasInstigatorItem, ConditionLibrary.PlayerDoesntKnowToKillReceiver, ConditionLibrary.PlayerKnowsAboutWantsItem, ConditionLibrary.CloseEnoughToPlayer, ConditionLibrary.IsItemNotDistance};
			//preconditions = new List<Condition>{ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.PlayerKnowsAboutDesireToKill, ConditionLibrary.PlayerHasLethalItem, ConditionLibrary.PlayerHasInstigatorItem, ConditionLibrary.PlayerDoesntKnowToKillReceiver, ConditionLibrary.PlayerDoesntKnowAboutWantsItem};
			killCharacter = new ActionAggregate(ActionLibrary.TellPlayerToKill, 4, preconditions);
			this.AddAction(killCharacter);

			preconditions = new List<Condition>{ ConditionLibrary.IsInstigatorHidden, ConditionLibrary.CloseEnoughToPlayer, ConditionLibrary.PlayerDoesntKnowAboutDesireToKill, ConditionLibrary.PlayerDoesntKnowsInstigator,  ConditionLibrary.InstigatorNotReceiver};
			var intro = new ActionAggregate(ActionLibrary.IntroduceMurderQuest, 2, preconditions);
			this.AddAction(intro);

			preconditions = new List<Condition>{ ConditionLibrary.IsInstigatorHidden, ConditionLibrary.CloseEnoughToPlayer, ConditionLibrary.PlayerDoesntKnowsInstigator};
			intro = new ActionAggregate(ActionLibrary.IntroduceSelf, 1, preconditions);
			this.AddAction(intro);

			preconditions = new List<Condition>{ConditionLibrary.CloseEnoughToPlayer, ConditionLibrary.PlayerDoesntKnowAboutDesireToKill, ConditionLibrary.PlayerKnowsInstigator, ConditionLibrary.InstigatorNotReceiver};
			intro = new ActionAggregate(ActionLibrary.TellWantToKill, 2, preconditions);
			this.AddAction(intro);

			preconditions = new List<Condition>{ConditionLibrary.PlayerDoesntKnowsInstigator, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.PlayerHasReceiverItem};
			intro = new ActionAggregate(ActionLibrary.IntroduceSelfDistance, 1, preconditions);
			this.AddAction(intro);

            // The escape quest.
            preconditions = new List<Condition> { ConditionLibrary.IsInstigatorHidden, ConditionLibrary.CloseEnoughToPlayer, ConditionLibrary.PlayerDoesntKnowsInstigator, ConditionLibrary.InstigatorNotReceiver };
            intro = new ActionAggregate(ActionLibrary.IntroduceEscapeQuest, 2, preconditions);
            this.AddAction(intro);

            preconditions = new List<Condition> { ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.PlayerHasLiberatingItem, ConditionLibrary.PlayerHasInstigatorItem, ConditionLibrary.PlayerDoesntKnowAboutWantsItem, ConditionLibrary.DoesStateHaveLiberatingItem };
            var escape = new ActionAggregate(ActionLibrary.TellPlayerToComeBack, 3, preconditions);
            this.AddAction(escape);

            preconditions = new List<Condition> { ConditionLibrary.IsInstigatorAlive, ConditionLibrary.IsReceiverAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.PlayerHasLiberatingItem, ConditionLibrary.PlayerHasInstigatorItem, ConditionLibrary.PlayerCloseEnoughToRevealReceiver, ConditionLibrary.DoesStateHaveLiberatingItem, ConditionLibrary.PlayerKnowsAboutWantsItem };
            escape = new ActionAggregate(ActionLibrary.TakeItemFromCharacter, 2, preconditions);
            this.AddAction(escape);

            preconditions = new List<Condition> { ConditionLibrary.IsInstigatorAlive, ConditionLibrary.InstigatorNotReceiver, ConditionLibrary.DoesInstigatorHaveItem, ConditionLibrary.IsItemLiberating, ConditionLibrary.DoesStateHaveLiberatingItem };
            escape = new ActionAggregate(ActionLibrary.FreeCharacterWithItem, 5, preconditions);
            this.AddAction(escape);        
        }

		private void ConstructDMActions(){
			dmActions = new List<ActionAggregate> ();

			var preconditions = new List<Condition>{ };
			var dramaManagerAction = new ActionAggregate(ActionLibrary.BlowUpThePrison, 6, preconditions);
			dmActions.Add(dramaManagerAction);
		}

        /// <summary>
        /// Adds a new action to the library.
        /// </summary>
        /// <param name="newAction">The new action.</param>
        public void AddAction(ActionAggregate newAction){
            allActions.Add(newAction);
        }

        /// <summary>
        /// Returns a subset of actions with the specified intensity.
        /// </summary>
        /// <param name="intensity">The intensity of the action.</param>
        /// <returns>An enumerable object of actions.</returns>
        public IEnumerable<ActionAggregate> GetActions()
        {
            return allActions; //Getting rid of intensity for characters
        }

		public IEnumerable<ActionAggregate> GetDMActions(int intensity){
			return dmActions.Where(action => action.Intensity == intensity);
		}
    }
}
