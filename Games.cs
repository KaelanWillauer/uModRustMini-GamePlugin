using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Testing stuff", "Willauer", "0.1")]
    public class Games : RustPlugin
    {
        string[] gameModes = { " ", "Free For All" };
        List<BasePlayer> playerList = new List<BasePlayer>();
        Vector3 spawn = new Vector3(0, 100, 0);
        bool isGame = false;
        bool isStarted = false;
        int ID = 0;
        int counter = 15;

        private void start(int ID)
        {
            isStarted = true;
            PrintToChat(gameModes[ID] + " STARTING!");

            switch (ID)
            {
                case 1:
                    foreach (BasePlayer player in playerList)
                    {
                        player.DieInstantly();
                    }
                    wipe();

                    break;

            }
        }

        private void initilizeGame(BasePlayer player, int ID)
        {

            isGame = true;
            playerList.Add(player);
            PrintToChat(player.displayName + " has started " + gameModes[ID]);
            PrintToChat("Enter 'a' to join!");
            countDown(ID, counter);
            timer.Once(counter + 5, () =>
            {
                start(ID);
            });


        }

        private void countDown(int ID, int counter)
        {
            timer.Repeat(counter / 3f, 3, () =>
            {
                PrintToChat(gameModes[ID] + " starting in " + counter);
                counter -= 5;
            });
        }

        private void teleport(BasePlayer player)
        {
            Vector3 shift = spawn + new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            player.MovePosition(shift);
        }

        private void giveItems(BasePlayer player)
        {
            
            player.SetHealth(100);
            PrintToChat("Worked");
            switch (ID)
            {
                case 1:
                    player.GiveItem(ItemManager.CreateByItemID(-1812555177)); // lr
                    player.GiveItem(ItemManager.CreateByItemID(1545779598)); // ak
                    player.GiveItem(ItemManager.CreateByItemID(1079279582, 20)); // medical
                    player.GiveItem(ItemManager.CreateByItemID(-1211166256, 200)); // 5.56
                    player.GiveItem(ItemManager.CreateByItemID(99588025, 2)); // wood wall
                    player.GiveItem(ItemManager.CreateByItemID(-567909622, 20)); // pumpkins

                    player.GiveItem(ItemManager.CreateByItemID(442289265)); // holo
                    player.GiveItem(ItemManager.CreateByItemID(-194953424)); // mask
                    player.GiveItem(ItemManager.CreateByItemID(1751045826)); // hoodie
                    player.GiveItem(ItemManager.CreateByItemID(1110385766)); // chest
                    player.GiveItem(ItemManager.CreateByItemID(237239288)); // pants
                    player.GiveItem(ItemManager.CreateByItemID(1850456855)); // kilt
                    player.GiveItem(ItemManager.CreateByItemID(-1108136649)); // tac gloves
                    player.GiveItem(ItemManager.CreateByItemID(-1549739227)); // boots

                    break;
            }

        }

        void OnPlayerRespawned(BasePlayer player)
        {
            switch (ID)
            {
                case 1:
                    giveItems(player);
                    teleport(player);
                    break;
            }

        }
        private object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo hitInfo)
        {
            switch (ID)
            {
                case 1:
                    Rust.DamageType damageType = hitInfo.damageTypes.GetMajorityDamageType();
                    if (damageType == Rust.DamageType.Fall)
                    {
                        hitInfo.damageTypes.Set(damageType, 0);
                    }
                    break;
            }
            return null;

        }

        [ChatCommand("FFA")]
        private void FFA(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 1;
                initilizeGame(player, ID);
            }
        }

        [ChatCommand("a")]
        private void accept(BasePlayer player)
        {
            if (isStarted)
            {
                PrintToChat(player, "Error: Game already started!");
            } else if (isGame && !playerList.Contains(player))
            {
                playerList.Add(player);
                PrintToChat(player.displayName + " joined " + gameModes[ID]);
            }
        }

        [ConsoleCommand("setspawn")]
        private void setSpawn(BasePlayer player)
        {

            spawn += new Vector3(player.transform.position.x, 0, player.transform.position.z);
        }



        [ConsoleCommand("r")]
        private void reset()
        {
            ID = 0;
            isGame = false;

            isStarted = false;

            foreach (BasePlayer player in playerList)
            {
                player.DieInstantly();

            }

            wipe();

            playerList = new List<BasePlayer>();
        }

        [ConsoleCommand("w")]
        private void wipe()
        {
            var droppedItems = UnityEngine.GameObject.FindObjectsOfType<DroppedItem>();
            var playerCorpses = UnityEngine.GameObject.FindObjectsOfType<PlayerCorpse>();


            foreach (var playerCorpse in playerCorpses)
            {
                playerCorpse.Kill();
            }
            var deadBags = UnityEngine.GameObject.FindObjectsOfType<DroppedItemContainer>();
            foreach (var droppedItem in droppedItems)
            {
                droppedItem.Kill();
            }
            foreach (var deadBag in deadBags)
            {
                deadBag.Kill();
            }
        }

        [ChatCommand("help")]
        private void help(BasePlayer player)
        {
            PrintToChat(player, "'FFA' - Free For All");
            PrintToChat(player, "'a' - Accept Match");
        }


    }
}
