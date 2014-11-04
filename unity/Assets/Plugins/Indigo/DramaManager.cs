using System.Collections;
using System.Collections.Generic;

namespace Indigo{
	public class DramaManager  {
		//Pretend these are constants
		private float MAX_WIDTH, MAX_HEIGHT;
		public static readonly string PLAYER_NAME = "Player";
		public static readonly string GUARD_TITLE = "Guard";
		public static readonly string PRISONER_TITLE = "Prisoner";

		public static readonly float MIN_DIST = 3000f;

		public DramaManager(float maxWidth, float maxHeight){
			//Set these for where we can put the Character
			MAX_WIDTH = maxWidth; 
			MAX_HEIGHT = maxHeight;
		}

		public GameState InitializeGameState(){
			GameState initState = new GameState ();

			//Make Character 
			Character playerCharacter = new Character (PLAYER_NAME, MAX_WIDTH / 2f, MAX_HEIGHT / 2f);
			initState.Player = playerCharacter;

			Character prisonerCharacter =new Character(PRISONER_TITLE,playerCharacter.X+20,playerCharacter.Y);
			initState.AddCharacter (prisonerCharacter);

			Character guardCharacter =new Character(GUARD_TITLE,60f,playerCharacter.Y);
			guardCharacter.Hidden = true;
			initState.AddCharacter (guardCharacter);

			Item prisonerItem = new Item (PRISONER_TITLE + "Item", 10f, playerCharacter.Y);
			prisonerItem.SetHidden (false);
			initState.AddItem (prisonerItem);

			Item guardItem = new Item (GUARD_TITLE + "Item", MAX_WIDTH-30f, playerCharacter.Y);
			initState.AddItem (guardItem);

			DialogueLine line = new DialogueLine (PRISONER_TITLE, "Hey get this thing");
			initState.Conversations.Add (PRISONER_TITLE, new List<DialogueLine>(){line});

			return initState;
		}

		public GameState UpdateGameState(GameState currentGameState){

			//Check if Player is close enough to anything to do anything

			//Characters
			foreach (Character character in currentGameState.Characters) {
				if(character.Alive && GetDist(character,currentGameState.Player)<MIN_DIST){
					string otherChar = character.Name.Equals(GUARD_TITLE) ? PRISONER_TITLE: GUARD_TITLE;
					Character other = currentGameState.GetCharacter(otherChar);
					Item charItem = currentGameState.GetItem(character.Name+"Item");

					if(character.Hidden && !currentGameState.Player.HasItem(charItem) && other.Alive && charItem.Alive && charItem.Hidden){
						currentGameState.AddLine(character.Name, new DialogueLine(character.Name,"No, get this"));
						character.Hidden = false;
						charItem.SetHidden(false);
					}
					else if(currentGameState.Player.HasItem(charItem) && other.Alive){
						currentGameState.AddLine(character.Name, new DialogueLine(character.Name,"Haha, now I can kill the "+otherChar));
						charItem.SetAlive(false);
						currentGameState = ActionLibrary.KillCharacter(currentGameState,character,other);
					}
				}
			}

			//Items
			foreach (Item item in currentGameState.Items) {
				if(GetDist(currentGameState.Player,item)<MIN_DIST){
					currentGameState.Player.Items.Add(item);
					item.SetHidden(true);
				}
			}


			return currentGameState;
		}

		public float Abs(float val){
			if (val < 0) {
				val*=-1f;			
			}

			return val;
		}

		public float GetDist(Character character, Character character2){
			return	Abs(SqrDist (character.X, character.Y, character2.X, character2.Y));
		}

		public float GetDist(Character character, Item item){
			return	Abs(SqrDist (character.X, character.Y, item.X, item.Y));
		}

		//SqrDist for now because screw the rules
		public float SqrDist(float x1, float y1, float x2, float y2){
			return ((x2 * x2) - (x1 * x1)) + ((y2 * y2) - (y1 * y1));
		}




	}


}
