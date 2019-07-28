using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class CharacterController
    {
        public static CharacterController instance = new CharacterController();

        public static int charGuid = 1000;

        public Character Create()
        {
            Character character = new Character();
            charGuid++;
            character.charId = charGuid;
            return character;
        }

        public void Init(Character character, float locationX, float locationZ)
        {
            character.x = 0;
            character.z = 0;
            character.locationX = locationX;
            character.locationZ = locationZ;
        }
    }
}
