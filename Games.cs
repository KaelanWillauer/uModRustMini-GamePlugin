using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Testing stuff", "Willauer", "0.1")]
    public class Games : RustPlugin
    {
        string[] gameModes = { " ", "Free For All", "Gun Game" };
        List<BasePlayer> playerList = new List<BasePlayer>();
        Vector3 spawn = new Vector3(0, 100, 0);
        bool isGame = false;
        bool isStarted = false;
        int ID = 0;
        int counter = 15;

        List<int> playerNumbers = new List<int>();
        List<int> GGWeapons = new List<int>();


        private void Loaded()
        {
            PrintToChat("Injected");
            GGWeapons.Add(-1812555177); // lr
            GGWeapons.Add(1545779598); // ak
            GGWeapons.Add(1796682209); // custom
            GGWeapons.Add(1318558775); // mp5
            GGWeapons.Add(884424049); // compound
            GGWeapons.Add(963906841); // rock
        }

        private void start(int ID)
        {
            isStarted = true;
            PrintToChat(gameModes[ID] + " STARTING!");
            foreach (BasePlayer player in playerList)
            {
                player.DieInstantly();
            }
            wipe();

            switch (ID)
            {
                case 1:

                    break;

            }
        }

        private void initilizeGame(BasePlayer player, int ID)
        {

            isGame = true;
            playerList.Add(player);
            playerNumbers.Add(0);
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
            Vector3 shift = spawn + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            player.MovePosition(shift);
        }

        private void giveItems(BasePlayer player)
        {
            
            player.SetHealth(100);
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

                case 2:
                    player.GiveItem(ItemManager.CreateByItemID(GGWeapons[playerNumbers[playerList.LastIndexOf(player)]]));
                    player.GiveItem(ItemManager.CreateByItemID(-1211166256, 200)); // 5.56
                    player.GiveItem(ItemManager.CreateByItemID(785728077, 200)); // pistol ammo
                    break;
            }

        }

        void OnPlayerRespawned(BasePlayer player)
        {
            switch (ID)
            {
                case 1:
                case 2:
                    giveItems(player);
                    teleport(player);
                    break;
            }

        }

        object OnPlayerWound(BasePlayer player, BasePlayer source)
        {
          
            switch (ID)
            {
                case 1:
                    break;
                case 2:
                    player.DieInstantly();
                    break;
            }
            return null;
        }

        object OnPlayerDeath(BasePlayer player, HitInfo info)
        {

            switch (ID)
            {
                case 1:
                    break;
                case 2:
                    player.DieInstantly();
                    BasePlayer killer = info.InitiatorPlayer.ToPlayer();
                    PrintToChat(killer.displayName + " has killed " + player.displayName + "!");
                    if (playerNumbers[playerList.LastIndexOf(killer)] == GGWeapons.Count - 1)
                    {
                        reset();
                        PrintToChat(killer.displayName + " has won " + gameModes[ID] + "!");
                        break;
                    }
                    playerNumbers[playerList.LastIndexOf(killer)] += 1;
                    killer.GiveItem(ItemManager.CreateByItemID(GGWeapons[playerNumbers[playerList.LastIndexOf(killer)]]));
                    wipe();
                    break;
            }
 
            return null;
        }

        private object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo hitInfo)
        {
            switch (ID)
            {
                
                case 1:
                case 2:
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

        [ChatCommand("GG")]
        private void GG(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 2;
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
                playerNumbers.Add(0);
                PrintToChat(player.displayName + " joined " + gameModes[ID]);
            }
        }

        [ChatCommand("setspawn")]
        private void settingSpawn(BasePlayer player)
        {

            spawn += new Vector3(player.transform.position.x, 0, player.transform.position.z);
            PrintToChat(spawn.ToString());
    
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
            playerNumbers = new List<int>();
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
            PrintToChat(player, "'GG' - Gun Game");
            PrintToChat(player, "'a' - Accept Match");
        }


    }
}
