using System;
using System.Collections;
using System.Collections.Generic;

namespace Indigo{
	public class DramaManager  {
		//Pretend these are constants
		private float MAX_WIDTH, MAX_HEIGHT;
		public static readonly string PLAYER_NAME = "Player";
		public static readonly string GUARD_TITLE = "Guard";
		public static readonly string PRISONER_TITLE = "Prisoner";

		public static readonly float MIN_DIST = 1000f;
		public static readonly float MIN_REVEAL = 5000f;
		public static readonly float MAX_DIST = 10000f;

		//I know, sorry.
		public static DramaManager Instance;

		public DramaManager(float maxWidth, float maxHeight){
			//Set these for where we can put the Character

			if (Instance == null) {
				Instance=this;		
			}
			MAX_WIDTH = maxWidth; 
			MAX_HEIGHT = maxHeight;
		}

		public GameState InitializeGameState(){
			GameState initState = new GameState ();

			//Make Characters
			Character playerCharacter = new Character (PLAYER_NAME, MAX_WIDTH / 2f, MAX_HEIGHT / 2f);
			initState.Player = playerCharacter;

			Character prisonerCharacter =new Character(PRISONER_TITLE,playerCharacter.X+20,playerCharacter.Y);
			prisonerCharacter.Hidden = true;
			initState.AddCharacter (prisonerCharacter);

			Location loc = GetNewCharacterLocation (initState);

			Character guardCharacter =new Character(GUARD_TITLE,loc.X,loc.Y);
			guardCharacter.Hidden = true;
			initState.AddCharacter (guardCharacter);

			return initState;
		}

		public GameState UpdateGameState(GameState currentGameState){

			//Check if Player is close enough to anything to do anything

			//Characters
			foreach (Character character in currentGameState.Characters) {
				if(character.Alive && GetDist(character,currentGameState.Player)<MIN_REVEAL){
					string otherChar = character.Name.Equals(GUARD_TITLE) ? PRISONER_TITLE: GUARD_TITLE;
					Character other = currentGameState.GetCharacter(otherChar);
					Item charItem = currentGameState.GetItem(character.Name+"Item"); //TODO; better way to do this, probs based on what's in that characters needed inventory or conditions or something

					if(character.Hidden && character.Alive){
						currentGameState = ActionLibrary.IntroduceCharacterQuest (currentGameState, character, null, null);
					}
					else if(charItem!=null && currentGameState.Player.HasItem(charItem) && other.Alive && character.Alive && GetDist(character,currentGameState.Player)<MIN_DIST){
						currentGameState = ActionLibrary.KillCharacter(currentGameState,character,other,charItem);
					}
				}
			}

			//Items
			foreach (Item item in currentGameState.Items) {
				if(GetDist(currentGameState.Player,item)<MIN_DIST && !item.Hidden){
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

		public float GetDist(Character character, Location loc){
			return	Abs(SqrDist (character.X, character.Y, loc.X, loc.Y));
		}

		public float GetDist(Character character, Item item){
			return	Abs(SqrDist (character.X, character.Y, item.X, item.Y));
		}

		//SqrDist for now because screw the rules, Sqrt is expensive
		public float SqrDist(float x1, float y1, float x2, float y2){
			return (float)(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
		}


		public Location GetNewCharacterLocation(GameState currGameState){
			Location loc = new Location();
			Random rand = new Random ();

			bool foundGoodLoc = false;
			int tries = 0;

			while(!foundGoodLoc && tries<100){
			
				loc.X = (float)rand.Next(0,(int)MAX_WIDTH);
				loc.Y = (float)rand.Next (0, (int)MAX_HEIGHT);

				foreach(Character c in currGameState.Characters){
					if(GetDist(c,loc)<MIN_DIST){
						float diffX = loc.X-c.X;
						float diffY = loc.Y-c.Y;

						float sqrmag = (diffX*diffX + diffY*diffY);

						diffX/=sqrmag;
						diffX*=MIN_DIST;
						diffY/=sqrmag;
						diffY*=MIN_DIST;

						loc.X=c.X+diffX;
						loc.Y=c.Y+diffY;
					}
				}

				if(OnScreen(loc)){
					foundGoodLoc = true;
				}

				tries++;
			}

			return loc;
		}
			                  
		//Placer is the character that needs the item
		public Location GetNewItemLocation(GameState currGameState, Character placer){
			Location loc = new Location();
			loc.X = 0;
			loc.Y = 0;

			Random rand = new Random ();
			bool placed=false;
			List<int> characterIndexes = new List<int> ();

			for (int i = 0; i<currGameState.Characters.Count; i++) {
				characterIndexes.Add(i);		
			}

			while(characterIndexes.Count>0 && !placed){
				int charIndex = characterIndexes[rand.Next(0,characterIndexes.Count)];

				if(currGameState.Characters[charIndex].X!=placer.X && currGameState.Characters[charIndex].Y!=placer.Y){
					loc.X=currGameState.Characters[charIndex].X;
					loc.Y=currGameState.Characters[charIndex].Y;

					loc.X+=rand.Next((int)(-30),(int)(30));
					loc.Y+=rand.Next((int)(-30),(int)(30));

					if(OnScreen(loc)){
						placed = true;
					}
				}
				characterIndexes.Remove(charIndex);

			}

			if (!placed) {
				loc =GetNewCharacterLocation(currGameState);		
			}

			return loc;
		}

		public bool OnScreen(Location loc){
			return loc.X > 100 && loc.Y > 100 && loc.X < MAX_WIDTH-100 && loc.Y < MAX_HEIGHT-100;
		}

	}


}
