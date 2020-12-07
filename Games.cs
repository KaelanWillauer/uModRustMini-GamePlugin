using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Testing stuff", "Willauer", "0.1")]
    public class Games : RustPlugin
    {
        string[] gameModes = { " ", "Free For All", "Gun Game", "One In The Chamber" };
        public enum GameModes
        {
            nothing, FreeForAll, GunGame, OneInTheChamber
        }


        Item gun;

        List<BasePlayer> playerList = new List<BasePlayer>();
        List<BasePlayer> playerList2 = new List<BasePlayer>();
        List<int> playerNumbers = new List<int>();
        List<int> playerNumbers2 = new List<int>();
        List<int> GGWeapons = new List<int>();

        Vector3 spawn = new Vector3(0, 0, 0);
        bool isGame = false;
        bool isStarted = false;
        int ID = 0;
        GameModes EID = 0;
        int counter = 15;

        private void Loaded()
        {
            PrintToChat("Injected");
            GGWeapons.Add(-1812555177); // lr
            GGWeapons.Add(1545779598); // ak
            GGWeapons.Add(1796682209); // custom
            GGWeapons.Add(1318558775); // mp5
            GGWeapons.Add(884424049); // compound
            GGWeapons.Add(2040726127); // combat knife
        }

        /* Request game */
        [ChatCommand("FFA")]
        private void FFA(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 1;
                EID = GameModes.FreeForAll;
                playerList.Add(player);
                initilizeGame(player, ID);
            }
        }
        [ChatCommand("GG")]
        private void GG(BasePlayer player)
        {
            if (!isGame)
            {

                ID = 2;
                EID = GameModes.GunGame;
                playerList.Add(player);
                playerNumbers.Add(0);
                initilizeGame(player, ID);
            }
        }
        [ChatCommand("OIC")]
        private void OIC(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 3;
                EID = GameModes.OneInTheChamber;
                playerList.Add(player);
                playerList2.Add(player);
                playerNumbers.Add(0);
                playerNumbers2.Add(3);
                initilizeGame(player, ID);
            }
        }

        /* Join match */
        [ChatCommand("a")]
        private void accept(BasePlayer player)
        {
            if (isStarted)
            {
                PrintToChat(player, "Error: Game already started!");
            }
            else if (isGame && !playerList.Contains(player))
            {
                playerList.Add(player);

                PrintToChat(player.displayName + " joined " + gameModes[ID]);
                switch (EID)
                {
                    case GameModes.FreeForAll:
                        break;
                    case GameModes.GunGame:
                        playerNumbers.Add(0);
                        break;
                    case GameModes.OneInTheChamber:
                        playerList2.Add(player);
                        playerNumbers.Add(0);
                        playerNumbers2.Add(3);
                        break;
                }
            }
        }

        /* Create a game */
        private void initilizeGame(BasePlayer player, int ID)
        {
            PrintToChat(player.displayName + " has started " + gameModes[ID]);
            PrintToChat("Enter 'a' to join!");

            isGame = true;
     
            countDown(ID, counter);
            timer.Once(counter + 5, () =>
            {
                start(ID);
            });

            switch (EID)
            {
                case GameModes.FreeForAll:
                    break;
                case GameModes.GunGame:
                    playerNumbers.Add(0);
                    break;
                case GameModes.OneInTheChamber:
                    break;
            }
        }

        /* Start game */
        private void start(int ID)
        {
            PrintToChat(gameModes[ID] + " STARTING!");

            isStarted = true;
            wipe();

            switch (EID)
            {
                case GameModes.FreeForAll:
                case GameModes.GunGame:
                case GameModes.OneInTheChamber:
 
                    foreach (BasePlayer player in playerList)
                    {
                        strip(player);
                        giveItems(player);
                        teleport(player);
                    }
                    break;
            }
        }

        /* Give items */
        private void giveItems(BasePlayer player)
        {
            strip(player);
            player.SetHealth(100);

            switch (EID)
            {
                case GameModes.FreeForAll:
                    chest(player, -1211166256, 200); // 5.56
                    chest(player, 442289265, 1); // holo

                    wear(player, -194953424); // mask
                    wear(player, 1751045826); // shirt
                    wear(player, 1110385766); // chest
                    wear(player, 237239288); // pants
                    wear(player, 1850456855); // kilt
                    wear(player, -1108136649); // tac gloves
                    wear(player, -1549739227); // boots

                    player.GiveItem(maxAmmo(-1812555177)); // lr
                    player.GiveItem(maxAmmo(1545779598)); // ak

                    belt(player, 1079279582, 20); // medical
                    belt(player, 99588025, 2); // wood wall
                    belt(player, -567909622, 20); // pumpkins
                    break;

                case GameModes.GunGame:
                    player.GiveItem(ItemManager.CreateByItemID(-1211166256, 200)); // 5.56
                    player.GiveItem(ItemManager.CreateByItemID(785728077, 200)); // pistol ammo
                    player.GiveItem(ItemManager.CreateByItemID(-1234735557, 10)); // arrow
                    player.GiveItem(maxAmmo((GGWeapons[playerNumbers[playerList.LastIndexOf(player)]])));
                    player.GiveItem(ItemManager.CreateByItemID(2040726127)); // combat knife
                    break;

                case GameModes.OneInTheChamber:
                    player.GiveItem(setAmmo(-852563019, 1)); // M92
                    player.GiveItem(ItemManager.CreateByItemID(2040726127)); // combat knife
                    break;
            }
        }

        void OnPlayerAttack(BasePlayer attacker, HitInfo info)
        {
            switch (EID)
            {
                case GameModes.FreeForAll:
                    break;
                case GameModes.GunGame:
                    try
                    {
                        Rust.DamageType hit = info.damageTypes.GetMajorityDamageType();
                        BasePlayer victim = info.HitEntity.ToPlayer();
                        if (hit == Rust.DamageType.Slash || hit == Rust.DamageType.Fun_Water)
                        {
                            victim.DieInstantly();
                            if (playerNumbers[playerList.LastIndexOf(victim)] > 0)
                            {
                                playerNumbers[playerList.LastIndexOf(victim)] -= 1;
                            }
                        }
                    }
                    catch {}
                    break;
                case GameModes.OneInTheChamber:
                    try
                    {
                        if (playerList.Contains(info.HitEntity.ToPlayer())) // If attack hit player
                        {
                            BasePlayer target = info.HitEntity.ToPlayer();
                            playerNumbers2[playerList.LastIndexOf(target)] -= 1;
                            playerNumbers[playerList.LastIndexOf(attacker)] += 1;
                            PrintToChat(target, playerNumbers2[playerList.LastIndexOf(target)] + " lives left.");

                            if (playerNumbers2[playerList.LastIndexOf(target)] <= 0) // If player is out of lives
                            {
                                PrintToChat(target, "Out of lives!");
                                playerList.Remove(target);
                            }

                            target.DieInstantly();

                            if (playerList.Count == 1) // If one player remains
                            {
                                int winner = 0;
                                int index = 0;
                                for (int i = 0; i < playerNumbers.Count; i++)
                                {
                                    if (winner < playerNumbers[i])
                                    {
                                        winner = playerNumbers[i];
                                        index = i;
                                    }
                                }
                                PrintToChat(playerList2[index].displayName + " has won with " + playerNumbers[index].ToString() + " kills!");
                                reset();
                            }
                            else
                            {

                                /* Weird memory work around */
                                List<Item> belt = attacker.inventory.containerBelt.FindItemsByItemID(-852563019);
                                Item gun = ItemManager.CreateByItemID(-852563019);
                                BaseProjectile newGun = gun.GetHeldEntity() as BaseProjectile;
                                BaseProjectile oldGun = belt[0].GetHeldEntity() as BaseProjectile;
                                newGun.primaryMagazine.contents = oldGun.primaryMagazine.contents;
                                strip(attacker);
                                attacker.inventory.GiveItem(incAmmo(gun));
                                attacker.GiveItem(ItemManager.CreateByItemID(2040726127)); // combat 12
                                wipe();
                            }
                        }
                    }
                    catch {}
                    break;
            }
        }

        private object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            /* Remove all fall damage */
 
                Rust.DamageType damageType = info.damageTypes.GetMajorityDamageType();
                if (damageType == Rust.DamageType.Fall)
                {
                    info.damageTypes.Set(damageType, 0);
                }

                switch (EID)
                {
                    case GameModes.FreeForAll:
                        break;
                    case GameModes.GunGame:
                        break;
                    case GameModes.OneInTheChamber:
                        break;
                }
            return null;
        }


        object OnPlayerWound(BasePlayer player, BasePlayer source)
        {
            switch (EID)
            {
                case GameModes.FreeForAll:
                    break;
                case GameModes.GunGame:
                    wipe();
                    break;
                case GameModes.OneInTheChamber:
                    break;
            }
            return null;
        }

        void OnLootPlayer(BasePlayer player, BasePlayer target)
        {
            switch (EID)
            {
                case GameModes.FreeForAll:
                    break;
                case GameModes.GunGame:
                    strip(player);
                    break;
                case GameModes.OneInTheChamber:
                    break;
            }
        }

        void OnLootEntity(BasePlayer player, BaseEntity entity)
        {
            
            BasePlayer enti = entity.ToPlayer();
            player.metabolism.hydration.Add(10);
            player.metabolism.calories.Add(10);
            enti.DisablePlayerCollider();
        }

        object OnPlayerDeath(BasePlayer player, HitInfo info)
        {

            switch (EID)
            {
                case GameModes.FreeForAll:
                    break;
                case GameModes.GunGame:
                    BasePlayer killer = info.InitiatorPlayer.ToPlayer();
                    player.DieInstantly();
                    PrintToChat(killer.displayName + " has killed " + player.displayName + "!");

                    /* Win condition */
                    if (playerNumbers[playerList.LastIndexOf(killer)] == GGWeapons.Count - 1)
                    {
                        PrintToChat(killer.displayName + " has won " + gameModes[ID] + "!");
                        reset();
                    }
                    else
                    {
                        /* Next weapon */
                        playerNumbers[playerList.LastIndexOf(killer)] += 1;
                        strip(killer);
                        giveItems(killer);
                        wipe();
                    }
                    break;
                case GameModes.OneInTheChamber:
                    break;
            }
            return null;
        }

        void OnPlayerRespawned(BasePlayer player)
        {
            player.MovePosition(spawn);
            switch (EID)
            {
                case GameModes.FreeForAll:
                    teleport(player);
                    giveItems(player);
                    break;
                case GameModes.GunGame:
                    teleport(player);
                    giveItems(player);
                    break;
                case GameModes.OneInTheChamber:
                    if (playerList.Contains(player))
                    {
                        teleport(player);
                        giveItems(player);
                    }
                    break;
                default:
                  
                    break;
            }
        }

        [ConsoleCommand("r")]
        private void reset()
        {
            ID = 0;
            EID = GameModes.nothing;
            isGame = false;
            isStarted = false;
            cleanPlayers();
            wipe();

            playerList = new List<BasePlayer>();
            playerList2 = new List<BasePlayer>();
            playerNumbers = new List<int>();
            playerNumbers2 = new List<int>();
        }

        void strip(BasePlayer player)
        {
            foreach (var item in player.inventory.containerBelt.itemList)
            {
                item.Remove();
            }
            foreach (var item in player.inventory.containerMain.itemList)
            {
                item.Remove();
            }
            foreach (var item in player.inventory.containerWear.itemList)
            {
                item.Remove();
            }
        }

        /* Takes an item ID and returns the item with max ammo */
        private Item maxAmmo(int item)
        {
            Item gun = ItemManager.CreateByItemID(item, 1);
            try
            {
                BaseProjectile gunAmmo = gun.GetHeldEntity() as BaseProjectile;

                gunAmmo.primaryMagazine.contents = gunAmmo.primaryMagazine.capacity;
                return gun;
            }
            catch { return gun; }
        }

        /* Takes an item ID and returns the item with a specified amount of ammo but not 0 */
        private Item setAmmo(int item, int ammount)
        {
            var rocket = ItemManager.CreateByItemID(442886268); // rocket launch
            var rocketammo = rocket.GetHeldEntity() as BaseProjectile;

            Item gun = ItemManager.CreateByItemID(item, 1);
            BaseProjectile gunAmmo = gun.GetHeldEntity() as BaseProjectile;

            gunAmmo.primaryMagazine.contents = rocketammo.primaryMagazine.capacity;
            for (int i = 0; i < ammount-1; i++)
            {
                gunAmmo.primaryMagazine.contents += rocketammo.primaryMagazine.capacity;
            }

            return gun;
        }

        /* Takes an Item and returns the Item with one more ammo */
        private Item incAmmo(Item item)
        {
            var rocket = ItemManager.CreateByItemID(442886268);
            var rocketammo = rocket.GetHeldEntity() as BaseProjectile;
        
            BaseProjectile gunAmmo = item.GetHeldEntity() as BaseProjectile;

            gunAmmo.primaryMagazine.contents += rocketammo.primaryMagazine.capacity;

            return item;
        }

        /* Takes an item ID and places it on a player */
        private void wear(BasePlayer player, int item)
        {
            ItemDefinition itemToCreate = ItemManager.FindItemDefinition(item);
            player.inventory.containerWear.AddItem(itemToCreate, 1);
        }
        private void chest(BasePlayer player, int item, int ammount)
        {
            ItemDefinition itemToCreate = ItemManager.FindItemDefinition(item);
            player.inventory.containerMain.AddItem(itemToCreate, ammount);
        }
        private void belt(BasePlayer player, int item, int ammount)
        {
            ItemDefinition itemToCreate = ItemManager.FindItemDefinition(item);
            player.inventory.containerBelt.AddItem(itemToCreate, ammount);
        }

        /* Removes all items from a player */
        private void cleanPlayers()
        {
            foreach (BasePlayer player in playerList)
            {
                foreach (var item in player.inventory.containerBelt.itemList)
                {
                    item.Remove();
                }
                foreach (var item in player.inventory.containerMain.itemList)
                {
                    item.Remove();
                }
                foreach (var item in player.inventory.containerWear.itemList)
                {
                    item.Remove();
                }
            }
        }

        /* Teleports player randomly around spawn */
        private void teleport(BasePlayer player)
        {
            Vector3 shift = spawn + new Vector3(Random.Range(-10, 10), 500, Random.Range(-10, 10));
            player.MovePosition(shift);
        }

        private void countDown(int ID, int counter)
        {
            timer.Repeat(counter / 3f, 3, () =>
            {
                PrintToChat(gameModes[ID] + " starting in " + counter);
                counter -= 5;
            });
        }

        [ChatCommand("setspawn")]
        private void settingSpawn(BasePlayer player)
        {
            spawn = new Vector3(0, 0, 0);
            spawn += new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        }

        /* Clears map of bodies, body bags, and items */
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
            PrintToChat(player, "ffa - Free For All");
            PrintToChat(player, "gg - Gun Game");
            PrintToChat(player, "oic - One In The Chamber");
            PrintToChat(player, "a - Accept Match");
        }

    }
}
