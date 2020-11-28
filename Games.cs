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
                        teleport(player);
                        giveItems(player);
                    }

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
            switch (ID)
            {
                case 1:
                    rust.RunServerCommand("inventory.giveto " + player.displayName + " lr 1");
                    rust.RunServerCommand("inventory.giveto " + player.displayName + " ammo.rifle 200");
                    rust.RunServerCommand("inventory.giveto " + player.displayName + " pumpkin 20");
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

        [ChatCommand("setspawn")]
        private void setSpawn(BasePlayer player)
        {

            spawn += new Vector3(player.transform.position.x, 0, player.transform.position.z);
        }



        [ChatCommand("r")]
        private void reset()
        {
            ID = 0;
            isGame = false;

            isStarted = false;

            foreach (BasePlayer player in playerList)
            {
                player.DieInstantly();

            }

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

            playerList = new List<BasePlayer>();
        }


    }
}
