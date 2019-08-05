using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class CharacterManager
    {
        public static readonly CharacterManager instance = new CharacterManager();

        public Dictionary<int, Character> charDic = new Dictionary<int, Character>();
        public int chooseLocationCount = 0;

        //创建角色
        public void CreateCharacter(int clientId, int charType)
        {
            var character = CharacterController.instance.Create();
            CharacterController.instance.Init(character, 0);
            character.clientId = clientId;
            character.typeId = charType;
            Server.instance.clientId2CharId.TryAdd(character.clientId, character.charId);
            AddCharacter(character);
        }

        //增加角色
        public void AddCharacter(Character character)
        {
            charDic.TryAdd(character.charId, character);
            ConsoleLog.instance.Info(string.Format("增加角色，角色Id: {0}", character.charId));
        }

        //获取char实例
        public Character GetCharacter(int charId)
        {
            Character character;
            charDic.TryGetValue(charId, out character);
            return character;
        }
    }
}
