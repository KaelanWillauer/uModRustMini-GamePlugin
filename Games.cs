using System;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Games", "Kaelan Willauer", 0.1)]
    [Description("A bunch of mini-games")]
    
    class Games : RustPlugin
    {
        int counter = 15;
        string[] gameModes = { " FFA" };
        int IDgamemode = 0;

        private void initilizeGame(BasePlayer player, int IDgamemode)
        {
            rust.RunServerCommand("oxide.group add" + gameModes[IDgamemode]);
            PrintToChat(player.displayName + " has started " + gameModes[IDgamemode]);
            PrintToChat("Enter 'accept' to join!");
            countDown(IDgamemode, counter);
        }

        void countDown(int IDgamemode, int counter)
        {
            timer.Repeat(counter / 3f, 3, () =>
            {
                PrintToChat(gameModes[IDgamemode] + " starting in " + counter);
                counter -= 5;
            });
        }

        [ChatCommand("accept")] //[chatcommand("accept"), Permission(
        private void acceptGame(BasePlayer player)
        {
            if (player.IPlayer.BelongsToGroup("default"))
            {
                rust.RunServerCommand("oxide.usergroup remove " + player.displayName + " default");
                rust.RunServerCommand("oxide.usergroup add " + player.displayName + gameModes[IDgamemode]);
                PrintToChat(player.displayName + " joined " + gameModes[IDgamemode]);
                startGame(player, IDgamemode);
            }
        }

        [ChatCommand("FFA")]
        private void attemptFFA(BasePlayer player) 
        { 
            if (player.IPlayer.BelongsToGroup("default"))
            {
                initilizeGame(player, 0);
            }
        }

        private void startGame(BasePlayer player, int IDgamemode)
        {
            timer.Once(counter+5, () => // counter+5
            {
                Random rnd = new System.Random();
                int x = rnd.Next(-500, 500);
                int z = rnd.Next(-500, 500);

                switch (IDgamemode)
                {
                    case 0:
                        player.Heal(100f);
                        rust.RunServerCommand("teleport.topos " + player.displayName + " " + x.ToString() + " 80 " + z.ToString());
                        rust.RunServerCommand("inventory.giveto " + player.displayName + " lr 1");
                        rust.RunServerCommand("inventory.giveto " + player.displayName + " pumpkin 100");
                        break;
                    default:
                        PrintToChat("IDK HOW I GOT HERE??");
                        break;
                }
            });
        }
       
        [ChatCommand("reset")]
        private void reset(BasePlayer player)
        {
            rust.RunServerCommand("oxide.usergroup add " + player.displayName + " default");
        }

        void Loaded()
        {
            PrintToChat("Games Injected!");
        }

    }

}
