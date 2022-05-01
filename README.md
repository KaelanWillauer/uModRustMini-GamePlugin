# uModRustMini-GamePlugin
Mini-Game system manager developed with the uMod API for Rust. Originally created to manage other plugins’ games, the manager acts as a simple method to add custom game-modes to Rust.
Although created around patch [234](https://rust.facepunch.com/news/community-update-234), this plugin runs fully functional as of May 1, 2022 on patch [APRIL2022UPDATE](https://rust.facepunch.com/changelist/3921). The map is still supported but may need to be re-exported from [RustEdit](https://www.rustedit.io/). 
# GCLOUD HOSTING
#### [Here](/gcloudrust.pdf) is a quick guide to spinning up a rust server on [GCLOUD](https://cloud.google.com/). Although not quite comprehensive, it provides the steps necessary for hosting game servers.
# Systems
### Spawn system
* Allows an admin to create a server spawn and set custom spawns for maps.
### Game joining system
* Game creation starts from any player and others may join during the countdown. Only one game may run at a time, however simultaneous games are almost supported.
### Map voting system
* Prompts players to vote on predefined maps for the given game.
# Integrated Game-modes
* Free For All
* Gun Game
* One In The Chamber
* Infected
* Snipers Only
* Tanks n Nails
* Zookas
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
