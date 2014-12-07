using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


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

		public ActionManager actionManager;

		//Timer Section
		public int index = 0;
		private float[] timeSections = new float[]{60,300,300, 300, 300, 300, 30};//Intensity from 0 to 5 (6 is just as a means of allowing people to read final stuff before shift to the end)

		//Last Character Close To part
		private string lastCharacter = "";

		private delegate GameState InitializeState ();
		private InitializeState[] beginningStates;

		//Name stuff, just initials because fudge it
		private string[] initials = new string[]{"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};


		public DramaManager(float maxWidth, float maxHeight){
			if (Instance == null) {
				Instance=this;		
			}
			MAX_WIDTH = maxWidth; 
			MAX_HEIGHT = maxHeight;

			actionManager = new ActionManager ();
			beginningStates = new InitializeState[]{InitializeGameState, InitializeGameState2, InitializeGameState3};
		}

		public GameState GetRandomStartState(){
			//Pick random starting state
			Random rand = new Random ();
			InitializeState init = beginningStates [rand.Next (0, beginningStates.Length)];
			return init();
		}

		public GameState UpdateGameState(GameState currentGameState, float timeDelta){
			bool rePlan = false;
			//Characters
			foreach (Character character in currentGameState.Characters) {
				if(character.IsAlive() && GetDist(character,currentGameState.Player)<MIN_REVEAL && character.Name!=lastCharacter){

					if(character.Hidden){
						//Introduce Character
						rePlan = true;
						lastCharacter = character.Name;
					}
					else if(character.IsAlive() && GetDist(character,currentGameState.Player)<MIN_DIST){//Update you're nearby a new character
						rePlan = true;
						lastCharacter = character.Name;
					}
				}
			}

			//Items
			foreach (Item item in currentGameState.Items) {
				if(GetDist(currentGameState.Player,item)<MIN_DIST && !item.Hidden){ //Get New Item
					currentGameState.Player.Items.Add(item);
					item.SetHidden(true);
					rePlan = true;
				}
			}

			int indexToUse = index;

			if (index > timeSections.Length-1) {//Give them some time before the end
				indexToUse = timeSections.Length-1;			
			}

			timeSections [indexToUse] -= timeDelta; 

			if (timeSections [indexToUse] < 0) {
				if(indexToUse<timeSections.Length-1){
					lastCharacter="";
					rePlan = true;
					//index++;
				}
				else{
					return null;
				}
			}

			if(rePlan){
				List<ActionAggregate> actions= (actionManager.GetActions()).ToList();
				for(int i = index-1; i>0; i--){
					actions.AddRange(actionManager.GetActions());
				}

				bool someActionTaken = false;

				foreach(Character c in currentGameState.Characters){
					GameState newState = c.evaluateBestAction(actions,currentGameState);

					if(newState!=null){
						someActionTaken=true;

						//Add action
						currentGameState = newState;
					}
				}

				if(!someActionTaken){
					List<ActionAggregate> dmActions = (actionManager.GetDMActions((index+1))).ToList();

					foreach(ActionAggregate a in dmActions){
						if(a.DoPreconditionsHold(currentGameState,null,null,null)){
							currentGameState = a.EvaluateAction(currentGameState,null,null,null);
							someActionTaken = true;
							break;
						}
					}
				}

				if(someActionTaken){
					lastCharacter = "";

				}
			}


			return currentGameState;
		}

		public static float Abs(float val){
			if (val < 0) {
				val*=-1f;			
			}

			return val;
		}

		public static float GetDist(Character character, Character character2){
			return	Abs(SqrDist (character.X, character.Y, character2.X, character2.Y));
		}

		public float GetDist(Character character, Location loc){
			return	Abs(SqrDist (character.X, character.Y, loc.X, loc.Y));
		}

		public float GetDist(Character character, Item item){
			return	Abs(SqrDist (character.X, character.Y, item.X, item.Y));
		}

		//SqrDist for now because screw the rules, Sqrt is expensive
		public static float SqrDist(float x1, float y1, float x2, float y2){
			return (float)(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
		}


		public Location GetNewCharacterLocation(GameState currGameState){
			Location loc = new Location();
			Random rand = new Random ();

			bool foundGoodLoc = false;
			int tries = 0;

			while(!foundGoodLoc && tries<1000){
			
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


			while(characterIndexes.Count>0 && !placed ){
				int charIndex = characterIndexes[rand.Next(0,characterIndexes.Count)];

				if(currGameState.Characters[charIndex].X!=placer.X && currGameState.Characters[charIndex].Y!=placer.Y){
					loc.X=currGameState.Characters[charIndex].X;
					loc.Y=currGameState.Characters[charIndex].Y;

					loc.X+=rand.Next((int)(-1*100),(int)(100));
					loc.Y+=rand.Next((int)(-1*100),(int)(100));

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


		//Name Stuff
		private string GetRandomPrisonerName(){
			Random r = new Random ();
			return PRISONER_TITLE+" "+initials[r.Next(initials.Length)]+".";
		}

		private string GetRandomGuardName(){
			Random r = new Random ();
			return GUARD_TITLE+" "+r.Next(initials.Length)+".";
		}

		//Index Stuff (if index passed in is higher, go to it)
		public void CheckIntensity(int _index){
			if (index < _index) {
				index=_index;			
			}
		}

		//Don't let the player go back to the Map if this is the case)
		public bool AreWeDone(){
			return index == timeSections.Length - 1;		
		}

		//INIT SECTION

		public GameState InitializeGameState(){
			GameState initState = new GameState ();
			
			//Make Characters
			Character playerCharacter = new Character (PLAYER_NAME, MAX_WIDTH / 2f, MAX_HEIGHT / 2f);
			initState.Player = playerCharacter;
			
			Character prisonerCharacter =new Character(GetRandomPrisonerName(),playerCharacter.X+20,playerCharacter.Y);
			prisonerCharacter.Hidden = true;
			prisonerCharacter.AddStatus ("Alive");
			initState.AddCharacter (prisonerCharacter);
			
			Location loc = GetNewCharacterLocation (initState);
			
			Character guardCharacter =new Character(GetRandomGuardName(),loc.X,loc.Y);
			guardCharacter.Hidden = true;
			guardCharacter.AddStatus ("Alive");
			initState.AddCharacter (guardCharacter);
			
			guardCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsInstigatorAlive, guardCharacter, null, null));
			guardCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsReceiverDead, guardCharacter, prisonerCharacter, null));
			guardCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.StateHasLethalItem, null, null, null));

			prisonerCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsInstigatorAlive, prisonerCharacter, null, null));
			prisonerCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsReceiverDead, prisonerCharacter, guardCharacter, null));
			prisonerCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.StateHasLethalItem, null, null, null));


			return initState;
		}

		public GameState InitializeGameState2(){
			GameState initState = new GameState ();
			
			//Make Characters
			Character playerCharacter = new Character (PLAYER_NAME, MAX_WIDTH / 2f, MAX_HEIGHT / 2f);
			initState.Player = playerCharacter;
			
			Character prisonerCharacter =new Character(GetRandomPrisonerName(),playerCharacter.X+20,playerCharacter.Y);
			prisonerCharacter.Hidden = true;
			prisonerCharacter.AddStatus ("Alive");
			initState.AddCharacter (prisonerCharacter);
			
			Location loc = GetNewCharacterLocation (initState);
			
			Character guardCharacter =new Character(GetRandomPrisonerName(),loc.X,loc.Y);
			guardCharacter.Hidden = true;
			guardCharacter.AddStatus ("Alive");
			initState.AddCharacter (guardCharacter);
			
			guardCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsInstigatorAlive, guardCharacter, null, null));
			guardCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsReceiverDead, guardCharacter, prisonerCharacter, null));
			guardCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.StateHasLethalItem, null, null, null));
			
			prisonerCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsInstigatorAlive, prisonerCharacter, null, null));
			prisonerCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.IsReceiverDead, prisonerCharacter, guardCharacter, null));
			prisonerCharacter.Goals.Add (new CharacterGoal (ConditionLibrary.StateHasLethalItem, null, null, null));
			
			
			return initState;
		}

        /// <summary>
        /// Creates a game state to facilitate an escape quest.
        /// </summary>
        public GameState InitializeGameState3() {
            GameState initState = new GameState();

            //Make Characters
            Character playerCharacter = new Character(PLAYER_NAME, MAX_WIDTH / 2f, MAX_HEIGHT / 2f);
            initState.Player = playerCharacter;

            Character prisonerCharacter = new Character(GetRandomPrisonerName(), playerCharacter.X + 20, playerCharacter.Y);
            prisonerCharacter.Hidden = true;
            prisonerCharacter.AddStatus("Alive");
            initState.AddCharacter(prisonerCharacter);

            Location loc = GetNewCharacterLocation(initState);

            Character guardCharacter = new Character(GetRandomGuardName(), loc.X, loc.Y);
            guardCharacter.Hidden = true;
            guardCharacter.AddStatus("Alive");
            initState.AddCharacter(guardCharacter);

            guardCharacter.Goals.Add(new CharacterGoal(ConditionLibrary.IsInstigatorAlive, guardCharacter, null, null));
            guardCharacter.Goals.Add(new CharacterGoal(ConditionLibrary.DoesStateHaveLiberatingItem, null, null, null));

            prisonerCharacter.Goals.Add(new CharacterGoal(ConditionLibrary.IsInstigatorAlive, prisonerCharacter, null, null));
            prisonerCharacter.Goals.Add(new CharacterGoal(ConditionLibrary.IsInstigatorMobile, prisonerCharacter, null, null));
            prisonerCharacter.Goals.Add(new CharacterGoal(ConditionLibrary.DoesStateHaveLiberatingItem, null, null, null));

            return initState;
        }
	}


}
