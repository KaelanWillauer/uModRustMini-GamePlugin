# uModRustMini-GamePlugin
Mini-Game system manager developed with the uMod API for Rust. Originally created to manage other plugins’ games, the manager acts as a simple method to add custom game-modes to Rust.
Although created around patch [234](https://rust.facepunch.com/changes/2), this plugin runs fully functional as of May 6, 2021 on patch [241](https://rust.facepunch.com/changes/1). The map is still supported by [241](https://rust.facepunch.com/changes/1) but must be re-exported from [RustEdit](https://www.rustedit.io/). 

# Systems
* Spawn system
> Allows an admin to create a server spawn and set any number of spawns for up to 3 custom maps.
* Game joining system
> Game creation starts from any player and others may join during the countdown. Only one game may run at a time, however simultaneous games are almost supported.
* Map voting system
> Prompts players to vote on predefined maps for the given game.
# Integrated Game-modes
* Free For All
* Gun Game
* One In The Chamber
* Infected
* Snipers Only
* Tanks n Nails
# Maps (Day/Night)
## Shipment
![](/Photos/ShipmentDay.png)
![](/Photos/ShipmentNight.png)
## Cargo
![](/Photos/CargoDay.png)
![](/Photos/CargoNight.png)
## Rust
![](/Photos/RustDay.png)
![](/Photos/RustNight.png)
