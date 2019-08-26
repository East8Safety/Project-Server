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

        public void ChangeItemCount(Player player, int itemId, int count)
        {
            if (!player.itemId2Count.ContainsKey(itemId))
            {
                player.itemId2Count[itemId] = count;
                if (player.itemId2Count[itemId] <= 0)
                {
                    player.itemId2Count.Remove(itemId);
                }
                return;
            }

            player.itemId2Count[itemId] += count;

            if(player.itemId2Count[itemId] <= 0)
            {
                player.itemId2Count.Remove(itemId);
            }
        }

        public bool IsHaveItem(Player player, int itemId)
        {
            if (player.itemId2Count.ContainsKey(itemId))
            {
                return true;
            }
            return false;
        }

        public void UseItem(Player player, int itemId)
        {
            ChangeItemCount(player, itemId, -1);
        }
    }
}
