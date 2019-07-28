using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class CharacterManager
    {
        public static readonly CharacterManager instance = new CharacterManager();

        public Dictionary<int, Character> charDic = new Dictionary<int, Character>();

        public void AddCharacter(Character character)
        {
            charDic.TryAdd(character.charId, character);
        }
    }
}
