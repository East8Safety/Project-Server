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

            switch (itemId)
            {
                case 2001:
                    var hp = ReadConfig.instance.ItemId2Value[itemId];
                    if ((player.HP + hp) > player.HPMax)
                    {
                        player.HP = player.HPMax;
                    }
                    else
                    {
                        player.HP += hp;
                    }
                    GameProcess.instance.SendHPChange(player);
                    break;
                case 2004:
                    var shieldValue = ReadConfig.instance.ItemId2Value[itemId];
                    player.shield = shieldValue;
                    break;
                default:
                    break;
            }
        }
    }
}
