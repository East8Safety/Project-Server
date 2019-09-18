using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class ItemController
    {
        public static readonly ItemController instance = new ItemController();

        public Item Create(int id)
        {
            Item item = new Item();
            item.id = id;
            return item;
        }

        public void AddItem(Player player, int itemId, int count)
        {
            if (player.index2ItemId.Count >= 6)
            {
                return;
            }

            for (int i = 0; i < 6; i++)
            {
                if (!player.index2ItemId.ContainsKey(i))
                {
                    player.index2ItemId[i] = itemId;
                    return;
                }
            }
        }

        public void DeleteItem(Player player, int index)
        {
            if (!player.index2ItemId.ContainsKey(index))
            {
                return;
            }

            player.index2ItemId.Remove(index);
        }

        public bool IsHaveItem(Player player, int index)
        {
            if (player.index2ItemId.ContainsKey(index))
            {
                return true;
            }
            return false;
        }

        public void UseItem(Player player, int index, int itemId)
        {
            DeleteItem(player, index);
            GameProcess.instance.SyncItem(player);

            switch (itemId)
            {
                case 2001:
                    var hp1 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.HP + hp1) > player.HPMax)
                    {
                        player.HP = player.HPMax;
                    }
                    else
                    {
                        player.HP += hp1;
                    }
                    //GameProcess.instance.SendHPChange(player);
                    GameProcess.instance.SyncState(player);
                    break;
                case 2002:
                    var hp2 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.HP + hp2) > player.HPMax)
                    {
                        player.HP = player.HPMax;
                    }
                    else
                    {
                        player.HP += hp2;
                    }
                    //GameProcess.instance.SendHPChange(player);
                    GameProcess.instance.SyncState(player);
                    break;
                case 2003:
                    var hp3 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.HP + hp3) > player.HPMax)
                    {
                        player.HP = player.HPMax;
                    }
                    else
                    {
                        player.HP += hp3;
                    }
                    //GameProcess.instance.SendHPChange(player);
                    GameProcess.instance.SyncState(player);
                    break;
                case 2004:
                    var shieldValue1 = ReadConfig.instance.ItemId2Value[itemId];
                    player.shield = shieldValue1;
                    GameProcess.instance.SyncState(player);
                    break;
                case 2005:
                    var shieldValue2 = ReadConfig.instance.ItemId2Value[itemId];
                    player.shield = shieldValue2;
                    GameProcess.instance.SyncState(player);
                    break;
                case 2006:
                    var shieldValue3 = ReadConfig.instance.ItemId2Value[itemId];
                    player.shield = shieldValue3;
                    GameProcess.instance.SyncState(player);
                    break;
                case 2007:
                    var bombCount1 = ReadConfig.instance.ItemId2Value[itemId];
                    player.bombCount += bombCount1;
                    GameProcess.instance.SyncState(player);
                    break;
                case 2008:
                    var bombCount2 = ReadConfig.instance.ItemId2Value[itemId];
                    player.bombCount += bombCount2;
                    GameProcess.instance.SyncState(player);
                    break;
                case 2009:
                    var bombCount3 = ReadConfig.instance.ItemId2Value[itemId];
                    player.bombCount += bombCount3;
                    GameProcess.instance.SyncState(player);
                    break;
                case 2010:
                    var speedValue1 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.speed + speedValue1) > player.speedMax)
                    {
                        player.speed = player.speedMax;
                    }
                    else
                    {
                        player.speed += speedValue1;
                    }
                    GameProcess.instance.SyncState(player);
                    break;
                case 2011:
                    var speedValue2 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.speed + speedValue2) > player.speedMax)
                    {
                        player.speed = player.speedMax;
                    }
                    else
                    {
                        player.speed += speedValue2;
                    }
                    GameProcess.instance.SyncState(player);
                    break;
                case 2012:
                    var speedValue3 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.speed + speedValue3) > player.speedMax)
                    {
                        player.speed = player.speedMax;
                    }
                    else
                    {
                        player.speed += speedValue3;
                    }
                    GameProcess.instance.SyncState(player);
                    break;
                case 2013:
                    var damage1 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.damage + damage1) > player.damageMax)
                    {
                        player.damage = player.damageMax;
                    }
                    else
                    {
                        player.damage += damage1;
                    }
                    GameProcess.instance.SyncState(player);
                    break;
                case 2014:
                    var damage2 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.damage + damage2) > player.damageMax)
                    {
                        player.damage = player.damageMax;
                    }
                    else
                    {
                        player.damage += damage2;
                    }
                    GameProcess.instance.SyncState(player);
                    break;
                case 2015:
                    var damage3 = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.damage + damage3) > player.damageMax)
                    {
                        player.damage = player.damageMax;
                    }
                    else
                    {
                        player.damage += damage3;
                    }
                    GameProcess.instance.SyncState(player);
                    break;
                default:
                    break;
            }
        }
    }
}
