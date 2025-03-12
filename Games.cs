using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oxide.Plugins
{
    [Info("Testing stuff", "Willauer", "0.2")]
    public class Games : RustPlugin
    {
        string[] gameModes = { " ", "Free For All", "Gun Game", "One In The Chamber", "Infected", "Snipers Only", "Tanks n Nails", "Zookas" };

        string[] maps = { " ", "Cargo", "Shipment", "Rust" };
        public enum GameModes
        {
            nothing, FreeForAll, GunGame, OneInTheChamber, Infected, SnipersOnly, TanksnNails, Zookas
        }

        public enum Maps
        {
            nothing, Cargo, Shipment, Rust
        }

        List<BasePlayer> playerList = new List<BasePlayer>();
        List<BasePlayer> playerList2 = new List<BasePlayer>();
        List<BasePlayer> playerList3 = new List<BasePlayer>();
        List<int> playerNumbers = new List<int>();
        List<int> playerNumbers2 = new List<int>();
        List<Vector3> spawns = new List<Vector3>();
        List<Vector3> spawns2 = new List<Vector3>();
        List<Vector3> spawns3 = new List<Vector3>();
        List<int> GGWeapons = new List<int>();
        List<int> voteMap = new List<int>();

        [ChatCommand("s1")]
        private void setSpawnPoint1(BasePlayer player)
        {
            Vector3 spawnPoint = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            spawns.Add(spawnPoint);
        }

        [ChatCommand("s2")]
        private void setSpawnPoint2(BasePlayer player)
        {
            Vector3 spawnPoint = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            spawns2.Add(spawnPoint);
        }

        [ChatCommand("s3")]
        private void setSpawnPoint3(BasePlayer player)
        {
            Vector3 spawnPoint = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            spawns3.Add(spawnPoint);
        }

        Vector3 spawn = new Vector3(0, 0, 0);
        Vector3 infSpawn = new Vector3(0, 0, 0);
        bool isGame = false;
        bool isStarted = false;
        int ID = 0;
        GameModes EID = 0;
        Maps MID = 0;
        int counter = 15;
        int infCounter = 15;

        private void Loaded()
        {
            GGWeapons.Add(-1812555177); // lr
            GGWeapons.Add(1545779598); // ak
            GGWeapons.Add(1796682209); // custom
            GGWeapons.Add(1318558775); // mp5
            GGWeapons.Add(442886268); // Rocket
            GGWeapons.Add(884424049); // compound
            GGWeapons.Add(2040726127); // combat knife

            voteMap.Add(0);
            voteMap.Add(0);
            voteMap.Add(0);
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
        [ChatCommand("INF")]
        private void INF(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 4;
                EID = GameModes.Infected;
                playerList.Add(player);
                initilizeGame(player, ID);
            }
        }
        [ChatCommand("SO")]
        private void SO(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 5;
                EID = GameModes.SnipersOnly;
                playerList.Add(player);
                initilizeGame(player, ID);
            }
        }

        [ChatCommand("TNN")]
        private void TNN(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 6;
                EID = GameModes.TanksnNails;
                playerList.Add(player);
                initilizeGame(player, ID);
            }
        }

        [ChatCommand("ZOO")]
        private void ZOO(BasePlayer player)
        {
            if (!isGame)
            {
                ID = 6;
                EID = GameModes.Zookas;
                playerList.Add(player);
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
                    case GameModes.GunGame:
                        playerNumbers.Add(0);
                        break;
                    case GameModes.OneInTheChamber:
                        playerList2.Add(player);
                        playerNumbers.Add(0);
                        playerNumbers2.Add(3);
                        break;
                    default:
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

            countDown(15);
            timer.Once(counter + 5, () =>
            {
                pickMap(ID);
            });

            switch (EID)
            {
                case GameModes.GunGame:
                    playerNumbers.Add(0);
                    break;
                default:
                    break;
            }
        }


        private void pickMap(int ID)
        {
            PrintToChat("Vote for map");
            for (int i = 1; i < maps.Length; i++)
            {
                PrintToChat(i.ToString() + ": " + maps[i]);
            }

            countDown(ID, counter);
            timer.Once(counter + 5, () =>
            {
                whichMap(voteMap);
                start(ID);
            });

        }

        private void whichMap(List<int> voteMap)
        {
            int index = 0;
            int max = voteMap[0];
            for (int i = 1; i < voteMap.Count; i++)
            {
                if (max < voteMap[i])
                {
                    max = voteMap[i];
                    index = i;
                }
            }

            switch (index)
            {

                case 0:
                    MID = Maps.Cargo;
                    break;
                case 1:
                    MID = Maps.Shipment;
                    break;
                case 2:
                    MID = Maps.Rust;
                    break;
            }
        }

        [ChatCommand("1")]
        private void voteCargo(BasePlayer player)
        {
            if (!playerList3.Contains(player))
            {
                voteMap[0] += 1;
                playerList3.Add(player);
            }
        }

        [ChatCommand("2")]
        private void voteShipment(BasePlayer player)
        {
            if (!playerList3.Contains(player))
            {
                voteMap[1] += 1;
                playerList3.Add(player);
            }
        }

        [ChatCommand("3")]
        private void voteRust(BasePlayer player)
        {
            if (!playerList3.Contains(player))
            {
                voteMap[2] += 1;
                playerList3.Add(player);
            }
        }


        /* Start game */
        private void start(int ID)
        {
            PrintToChat(gameModes[ID] + " STARTING on " + MID.ToString() + "!");

            isStarted = true;
            wipe();

            switch (EID)
            {
                case GameModes.SnipersOnly:
                case GameModes.Zookas:
                case GameModes.FreeForAll:
                case GameModes.GunGame:
                case GameModes.TanksnNails:
                case GameModes.OneInTheChamber:
                    foreach (BasePlayer player in playerList)
                    {
                        strip(player);
                        giveItems(player);
                        teleport(player);
                    }
                    break;
                case GameModes.Infected:
                    foreach (BasePlayer player in playerList)
                    {
                        strip(player);
                        giveItems(player);
                        teleport(player);
                    }

                    timer.Repeat(infCounter / 3f, 3, () =>
                    {
                        PrintToChat("First infected in " + infCounter.ToString());
                        infCounter -= 5;
                    });
                    timer.Once(infCounter + 5, () =>
                    {
                        int first = Random.Range(0, playerList.Count);

                        playerList2.Add(playerList[first]);
                        playerList.RemoveAt(first);
                        giveItems(playerList2[0]);
                        PrintToChat(playerList2[0], "You're first infected!");

                    });
                    break;
            }
        }

        /* Give items */
        private void giveItems(BasePlayer player)
        {
            strip(player);
            player.SetHealth(100);
            player.metabolism.hydration.Add(250);
            player.metabolism.calories.Add(250);

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
                    belt(player, -567909622, 20); // pumpkins
                    break;

                case GameModes.GunGame:
                    player.GiveItem(ItemManager.CreateByItemID(-1211166256, 200)); // 5.56
                    player.GiveItem(ItemManager.CreateByItemID(785728077, 200)); // pistol ammo
                    player.GiveItem(ItemManager.CreateByItemID(-1234735557, 10)); // arrow
                    player.GiveItem(ItemManager.CreateByItemID(-742865266, 3)); // rockets

                    player.GiveItem(maxAmmo(GGWeapons[playerNumbers[playerList.LastIndexOf(player)]]));

                    belt(player, 2040726127, 1); // combat knife

                    break;

                case GameModes.OneInTheChamber:
                    player.GiveItem(setAmmo(-852563019, 1)); // M92
                    player.GiveItem(ItemManager.CreateByItemID(2040726127)); // combat knife
                    break;

                case GameModes.Infected:
                    if (playerList.Contains(player))
                    {
                        player.GiveItem(maxAmmo(-1758372725)); // custom
                        chest(player, 785728077, 200); // pistol ammo
                    }
                    else if (playerList2.Contains(player))
                    {
                        wear(player, -194953424); // mask
                        wear(player, 1751045826); // shirt
                        wear(player, 1110385766); // chest
                        wear(player, 237239288); // pants
                        wear(player, 1850456855); // kilt
                        wear(player, -1108136649); // tac gloves
                        wear(player, -1549739227); // boots

                        player.GiveItem(ItemManager.CreateByItemID(-1469578201)); // longsword
                        player.GiveItem(ItemManager.CreateByItemID(1602646136)); // stone spear
                        player.GiveItem(ItemManager.CreateByItemID(1602646136));
                        belt(player, 1079279582, 20); // medical

                    }
                    break;
                case GameModes.SnipersOnly:
                    chest(player, -1211166256, 200); // 5.56
                    chest(player, 567235583, 1); // 8x scope

                    player.GiveItem(maxAmmo(-778367295)); // L96
                    break;
                case GameModes.TanksnNails:
                    wear(player, 1181207482); // heavy helmet
                    wear(player, -1102429027); // heavy jacket
                    wear(player, -1778159885); // heavy pants

                    belt(player, 1953903201, 1); // nailgun
                    belt(player, 1079279582, 20); // medical
                    chest(player, -2097376851, 200); // nails
                    break;
                case GameModes.Zookas:
                    player.GiveItem(maxAmmo(442886268)); // Rocket Launcher
                    belt(player, -742865266, 9); // rocket
                    belt(player, 1638322904, 10); // incen rocket
                    belt(player, -1841918730, 10); // high rocket
                    break;
            }
        }

        void OnPlayerAttack(BasePlayer attacker, HitInfo info)
        {
            if (isStarted && (playerList.Contains(attacker) || playerList2.Contains(attacker)))
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
                            if (playerNumbers[playerList.LastIndexOf(attacker)] == GGWeapons.Count - 1)
                            {
                                PrintToChat(attacker.displayName + " has won " + gameModes[ID] + "!");
                                reset();
                                victim.DieInstantly();

                            }
                            else if (hit == Rust.DamageType.Slash || hit == Rust.DamageType.Fun_Water)
                            {
                                victim.DieInstantly();
                                if (playerNumbers[playerList.LastIndexOf(victim)] > 0)
                                {
                                    playerNumbers[playerList.LastIndexOf(victim)] -= 1;

                                }
                            }

                        }
                        catch { }
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
                        catch { }
                        break;

                    case GameModes.Infected:

                        try
                        {
                            if (playerList.Contains(info.HitEntity.ToPlayer()) && playerList.Contains(attacker) ||
                                playerList2.Contains(info.HitEntity.ToPlayer()) && playerList2.Contains(attacker)) // If attack hit a player of the same team
                            {
                                info.damageTypes.ScaleAll(0);
                            }
                            else if (info.damageTypes.GetMajorityDamageType() == Rust.DamageType.Slash ||
                                     info.damageTypes.GetMajorityDamageType() == Rust.DamageType.Fun_Water ||
                                     info.damageTypes.GetMajorityDamageType() == Rust.DamageType.Arrow)
                            {
                                info.HitEntity.ToPlayer().DieInstantly();
                                wipe();
                                if (playerList.Contains(info.HitEntity.ToPlayer()))
                                {
                                    playerList.Remove(info.HitEntity.ToPlayer());
                                    playerList2.Add(info.HitEntity.ToPlayer());
                                }
                                if (playerList.Count == 0)
                                {
                                    PrintToChat("Final kill: " + attacker.displayName + " killed " + info.HitEntity.ToPlayer().displayName);
                                    cleanPlayers();
                                    reset();
                                }

                            }
                        }
                        catch { }
                        break;

                }
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
                case GameModes.Infected:
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
                case GameModes.Infected:
                case GameModes.Zookas:
                case GameModes.TanksnNails:
                    wipe();
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
                case GameModes.Infected:
                case GameModes.Zookas:
                    strip(player);
                    break;
            }
        }

        void OnLootEntity(BasePlayer player, BaseEntity entity)
        {

        }

        object OnPlayerDeath(BasePlayer player, HitInfo info)
        {
            if (isStarted && (playerList.Contains(player) || playerList2.Contains(player)))
            {
                switch (EID)
                {
                    case GameModes.FreeForAll:
                        break;
                    case GameModes.GunGame:
                        try
                        {
                            BasePlayer killer = info.InitiatorPlayer.ToPlayer();
                            player.DieInstantly();
                            PrintToChat(killer.displayName + " has killed " + player.displayName + "!");

                            /* Next weapon */
                            if (player == killer)
                            {
                                if (playerNumbers[playerList.LastIndexOf(killer)] > 0)
                                {
                                    playerNumbers[playerList.LastIndexOf(killer)] -= 1;
                                }
                            }
                            else
                            {
                                playerNumbers[playerList.LastIndexOf(killer)] += 1;
                            }
                            strip(killer);
                            giveItems(killer);
                            wipe();
                        }
                        catch { }

                        break;
                    case GameModes.OneInTheChamber:
                        break;
                    case GameModes.Infected:
                        player.DieInstantly();
                        wipe();
                        break;
                }
            }
            return null;
        }

        void OnPlayerRespawned(BasePlayer player)
        {
            player.MovePosition(spawn);
            strip(player);
            if (isStarted && (playerList.Contains(player) || playerList2.Contains(player)))
            {
                switch (EID)
                {
                    case GameModes.SnipersOnly:
                    case GameModes.Zookas:
                    case GameModes.TanksnNails:
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
                    case GameModes.Infected:
                        teleport(player);
                        giveItems(player);
                        break;
                    default:

                        break;
                }
            }
        }

        [ConsoleCommand("rr")]
        private void reset()
        {
            ID = 0;
            EID = GameModes.nothing;
            isGame = false;
            isStarted = false;
            infCounter = 15;
            cleanPlayers();
            wipe();

            foreach (BasePlayer player in playerList)
            {
                player.MovePosition(spawn);
            }
            foreach (BasePlayer player in playerList2)
            {
                player.MovePosition(spawn);
            }

            playerList = new List<BasePlayer>();
            playerList2 = new List<BasePlayer>();
            playerList3 = new List<BasePlayer>();
            voteMap = new List<int>();
            voteMap.Add(0);
            voteMap.Add(0);
            voteMap.Add(0);
            playerNumbers = new List<int>();
            playerNumbers2 = new List<int>();

            voteMap[0] = 0;
            voteMap[1] = 0;
            MID = Maps.nothing;

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
            for (int i = 0; i < ammount - 1; i++)
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
            foreach (BasePlayer player in playerList2)
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

        /* Teleports player to a valid spawn */
        private void teleport(BasePlayer player)
        {
            int telCounter = 10;
            if (playerList.Contains(player) || playerList2.Contains(player))
            {
                player.MovePosition(infSpawn);

                timer.Repeat(telCounter / 3f, 3, () =>
                {
                    PrintToChat(player, "On the ground in " + telCounter.ToString());
                    telCounter -= 5;
                });
                timer.Once(telCounter, () =>
                {
                    switch (MID)
                    {
                        case Maps.nothing:
                        case Maps.Cargo:
                            player.MovePosition(spawns[Random.Range(0, spawns.Count)]);
                            break;
                        case Maps.Shipment:
                            player.MovePosition(spawns2[Random.Range(0, spawns2.Count)]);
                            break;
                        case Maps.Rust:
                            player.MovePosition(spawns3[Random.Range(0, spawns3.Count)]);
                            break;

                    }

                });

            }
            else
            {
                player.MovePosition(spawn);
            }
        }

        private void countDown(int ID, int counter)
        {
            timer.Repeat(counter / 3f, 3, () =>
            {
                PrintToChat(gameModes[ID] + " starting in " + counter);
                counter -= 5;
            });
        }

        private void countDown(int counter)
        {
            timer.Repeat(counter / 3f, 3, () =>
            {
                PrintToChat(counter + " seconds to join!");
                counter -= 5;
            });
        }


        private void OnEntitySpawned(BaseNpc entity)
        {
            timer.Once(1f, () => {
                foreach (var npc in UnityEngine.Object.FindObjectsOfType<BaseNpc>())
                {
                    if (npc == null)
                    {
                        return;
                    }

                    npc.Kill();

                }
                ;
            });
        }


        [ChatCommand("ss")]
        private void settingSpawn(BasePlayer player)
        {
            spawn = new Vector3(0, 0, 0);
            spawn += new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        }

        [ChatCommand("drop")]
        private void setInfSpawn(BasePlayer player)
        {
            infSpawn = new Vector3(player.transform.position.x, 400, player.transform.position.z);
        }

        /* Clears map of bodies, body bags, and items */
        [ConsoleCommand("wipe")]
        private void wipe()
        {
            var droppedItems = UnityEngine.GameObject.FindObjectsOfType<DroppedItem>();
            var playerCorpses = UnityEngine.GameObject.FindObjectsOfType<PlayerCorpse>();
            foreach (var npc in UnityEngine.Object.FindObjectsOfType<BaseCorpse>())
            {

                npc.Kill();

            }

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
            PrintToChat(player, "inf - Infected");
            PrintToChat(player, "so - Snipers Only");
            PrintToChat(player, "tnn - Tanks n' Nails");
            PrintToChat(player, "zoo - Zookas");
            PrintToChat(player, "a - Accept Match");
        }

    }
}
