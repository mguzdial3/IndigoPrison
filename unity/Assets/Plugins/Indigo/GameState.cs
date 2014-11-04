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
        public List<Character> Characters { get; private set; }
		public List<Item> Items { get; private set; }
        public Character Player { get; set; }
        public int Intensity { get; set; }
		public Dictionary<string, List<DialogueLine> > Conversations;


       	public GameState(){
			Characters = new List<Character> ();
			Items = new List<Item> ();
			Conversations = new Dictionary<string, List<DialogueLine>> ();
		}


		//CONVERSATION STUFF
		public void AddLine(string characterName, DialogueLine line){
			if (Conversations.ContainsKey (characterName)) {
				Conversations[characterName].Add(line);			
			}
			else{
				Conversations.Add(characterName,new List<DialogueLine>{line});
			}
		}

		public void SetConversation(string characterName, List<DialogueLine> lines){
			if (Conversations.ContainsKey (characterName)) {
				Conversations[characterName]=lines;			
			}
			else{
				Conversations.Add(characterName,lines);
			}
		}

		public bool ConversationChanged(string characterName, int length){
			return 	Conversations.ContainsKey (characterName) && Conversations [characterName].Count != length;
		}

		public List<DialogueLine> GetConversation(string characterName){
			List<DialogueLine> list = null;

			if (Conversations.ContainsKey (characterName)) {
				list = Conversations[characterName];			
			}

			return list;
		}

		//ITEM STUFF
		public void AddItem(Item item){
			Items.Add (item);
		}

		public Item GetItem(string itemName){
			Item toReturn = null;

			foreach (Item item in Items) {
				if(item.Name.Equals(itemName)){
					toReturn = item;
				}
			}
			return toReturn;
		}

		//CHARACTER STUFF
		public void AddCharacter(Character character){
			Characters.Add (character);
		}

		public Character GetCharacter(string characterName){
			Character toReturn = null;
			
			foreach (Character character in Characters) {
				if(character.Name.Equals(characterName)){
					toReturn = character;
				}
			}
			return toReturn;
		}

        /// <summary>
        /// Creates a deep copy of the game state.
        /// </summary>
        /// <returns></returns>
        public GameState Clone()
        {
            GameState newState = new GameState();
            // Characters (as structs are copied by value)
            newState.Characters = new List<Character>();
			foreach (Character character in Characters) {
				newState.AddCharacter(character.Clone());			
			}

			foreach (Item item in Items) {
				newState.AddItem(item.Clone());			
			}

			foreach(KeyValuePair<string,List<DialogueLine>> kvp in Conversations){
				List<DialogueLine> lines = new List<DialogueLine>();
				foreach(DialogueLine line in kvp.Value){
					lines.Add(line);
				}
				newState.SetConversation(kvp.Key,lines);
			}

            newState.Player = this.Player.Clone();
            newState.Intensity = this.Intensity;
            return newState;
        }
    }
}
