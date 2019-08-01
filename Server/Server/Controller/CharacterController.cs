using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class CharacterController
    {
        public static CharacterController instance = new CharacterController();

        //角色递增Id
        public static int charGuid = 1000;

        //创建角色
        public Character Create()
        {
            Character character = new Character();
            charGuid++;
            character.charId = charGuid;
            ConsoleLog.instance.Info(string.Format("创建一个新角色,角色Id: {0}", character.charId));
            return character;
        }

        //初始化角色
        public void Init(Character character, float locationX, float locationZ)
        {
            character.x = 0;
            character.z = 0;
            character.locationX = locationX;
            character.locationZ = locationZ;
            ConsoleLog.instance.Info(string.Format("初始化新角色,角色位置: {0} {1}", locationX, locationZ));
        }
    }
}
