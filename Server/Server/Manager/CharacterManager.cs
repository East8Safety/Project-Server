using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class CharacterManager
    {
        public static readonly CharacterManager instance = new CharacterManager();

        public Dictionary<int, Character> charDic = new Dictionary<int, Character>();

        //创建角色
        public void CreateCharacter(int clientId)
        {
            var character = CharacterController.instance.Create();
            CharacterController.instance.Init(character, 0, 0);
            character.clientId = clientId;
            Server.instance.clientId2CharId.TryAdd(character.clientId, character.charId);
            AddCharacter(character);
        }

        //增加角色
        public void AddCharacter(Character character)
        {
            charDic.TryAdd(character.charId, character);
        }
    }
}
