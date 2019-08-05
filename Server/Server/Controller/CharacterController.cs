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
        public void Init(Character character, int typeId)
        {
            character.x = 0;
            character.z = 0;
            character.locationX = 0;
            character.locationZ = 0;
            character.HP = 100;
            character.speed = 1;
            ConsoleLog.instance.Info(string.Format("初始化新角色"));
        }

        //角色受到伤害
        public void Damage(Character character, int damage)
        {
            character.HP -= damage;

            //角色死亡
            if (character.HP <= 0)
            {
                S2CDie s2CDie = new S2CDie();
                s2CDie.charId = character.charId;
                GameProcess.instance.SendCharDie(s2CDie);
                return;
            }

            S2CHPChange s2CHPChange = new S2CHPChange();
            s2CHPChange.charId = character.charId;
            s2CHPChange.nowHP = character.HP;
            GameProcess.instance.SendHPChange(s2CHPChange);
        }

        //设置角色位置
        public void SetLocation(Character character, float x, float z)
        {
            character.locationX = x;
            character.locationZ = z;
        }
    }
}