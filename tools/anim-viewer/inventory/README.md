# Animation and Sprite Inventory

Generated 2026-11-21 by `node tools/anim-viewer/build-inventory.js` from `all-animations.html`.

- Clips: 1771 (36 without sprite keys)
- Sprite textures: 1112 (451 used by a clip, 661 not used by any clip: UI, tiles, items, VFX, unused sheets) + 416 normal maps
- Full per-clip detail: `animation-clips.csv`; full per-texture detail: `sprite-textures.csv`.

## Clips by model

| Model | Clips | Folders |
|---|---:|---:|
| Enemy role · Assasin | 72 | 9 |
| Enemy role · Boss | 72 | 9 |
| Enemy role · Brute | 72 | 9 |
| Enemy role · Elite | 72 | 9 |
| Enemy role · FastSwarm | 72 | 9 |
| Enemy role · RangedCaster | 72 | 9 |
| Enemy role · Scatter | 72 | 9 |
| Enemy role · Sniper | 72 | 9 |
| Enemy role · Tank | 72 | 9 |
| Enemy role · TrashMelee | 72 | 9 |
| Enemy · Bat | 5 | 5 |
| Enemy · Crab | 8 | 8 |
| Enemy · Golem Phase 1 | 5 | 5 |
| Enemy · Golem Phase 2 | 6 | 6 |
| Enemy · Golem Phase 3 | 10 | 10 |
| Enemy · Pebble | 4 | 4 |
| Enemy · Rat | 6 | 6 |
| Enemy · Skull | 4 | 4 |
| Enemy · Spiked Slime | 6 | 6 |
| Model · Erika | 97 | 20 |
| Model · Maw | 44 | 9 |
| Model · Nightshade | 106 | 22 |
| Model · Paladin | 93 | 16 |
| Model · Vampire | 44 | 9 |
| Model · Warrok | 44 | 9 |
| Player · Archer | 176 | 22 |
| Player · Mage | 176 | 22 |
| Player · Paladin | 195 | 27 |
| VFX · Particle Effect | 21 | 14 |
| VFX · Sprite/VFX | 1 | 1 |

### Enemy role · Assasin

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | vampire_attack_8dir/vampire_attack*.png | Assasin.controller |
| Attack | 8 | 01234567 | 30 | 1.021 | 31 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | vampire_attack_8dir/vampire_attack*.png | Assasin.controller, Assasin_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | vampire_attack_8dir/vampire_attack*.png | Assasin.controller |
| Death | 8 | 01234567 | 30 | 1.5 | 45 | AnimationOnAction | vampire_death_8dir/vampire_death*.png | Assasin.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | vampire_attack_8dir/vampire_attack*.png | Assasin.controller |
| Idle | 8 | 01234567 | 30 | 3.333 | 100 | - | vampire_idle_8dir/vampire_idle*.png | Assasin.controller |
| Run | 8 | 01234567 | 30 | 0.9 | 27 | - | vampire_move_8dir/vampire_move*.png | Assasin.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.683 | 21 | AnimationFinishTrigger | vampire_takedamage_8dir/vampire_takedamage*.png | Assasin.controller |
| Walk | 8 | 01234567 | 30 | 0.9 | 27 | - | vampire_move_8dir/vampire_move*.png | Assasin.controller |

### Enemy role · Boss

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Boss.controller |
| Attack | 8 | 01234567 | 30 | 1.033 | 23 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | Boss.controller, Boss_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Boss.controller |
| Death | 8 | 01234567 | 30 | 0.9 | 27 | AnimationOnAction | paladin_death_8dir/paladin_death*.png | Boss.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | Boss.controller |
| Idle | 8 | 01234567 | 30 | 1.267 | 39 | - | paladin_idle_8dir/paladin_idle*.png | Boss.controller |
| Run | 8 | 01234567 | 30 | 1.767, 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | Boss.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.7 | 20 | AnimationFinishTrigger | paladin_hit_8dir/paladin_hit*.png | Boss.controller |
| Walk | 8 | 01234567 | 30 | 0.833 | 26 | - | paladin_move_8dir/paladin_move*.png | Boss.controller |

### Enemy role · Brute

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Brute.controller |
| Attack | 8 | 01234567 | 30 | 1.033 | 23 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | Brute.controller, Brute_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Brute.controller |
| Death | 8 | 01234567 | 30 | 0.9 | 27 | AnimationOnAction | paladin_death_8dir/paladin_death*.png | Brute.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | Brute.controller |
| Idle | 8 | 01234567 | 30 | 1.267 | 39 | - | paladin_idle_8dir/paladin_idle*.png | Brute.controller |
| Run | 8 | 01234567 | 30 | 1.767, 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | Brute.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.7 | 20 | AnimationFinishTrigger | paladin_hit_8dir/paladin_hit*.png | Brute.controller |
| Walk | 8 | 01234567 | 30 | 0.833 | 26 | - | paladin_move_8dir/paladin_move*.png | Brute.controller |

### Enemy role · Elite

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Elite.controller |
| Attack | 8 | 01234567 | 30 | 1.033 | 23 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | Elite.controller, Elite_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Elite.controller |
| Death | 8 | 01234567 | 30 | 0.9 | 27 | AnimationOnAction | paladin_death_8dir/paladin_death*.png | Elite.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | Elite.controller |
| Idle | 8 | 01234567 | 30 | 1.267 | 39 | - | paladin_idle_8dir/paladin_idle*.png | Elite.controller |
| Run | 8 | 01234567 | 30 | 1.767, 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | Elite.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.7 | 20 | AnimationFinishTrigger | paladin_hit_8dir/paladin_hit*.png | Elite.controller |
| Walk | 8 | 01234567 | 30 | 0.833 | 26 | - | paladin_move_8dir/paladin_move*.png | Elite.controller |

### Enemy role · FastSwarm

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | FastSwarm.controller |
| Attack | 8 | 01234567 | 30 | 1.033 | 23 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | FastSwarm.controller, FastSwarm_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | FastSwarm.controller |
| Death | 8 | 01234567 | 30 | 0.9 | 27 | AnimationOnAction | paladin_death_8dir/paladin_death*.png | FastSwarm.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | FastSwarm.controller |
| Idle | 8 | 01234567 | 30 | 1.267 | 39 | - | paladin_idle_8dir/paladin_idle*.png | FastSwarm.controller |
| Run | 8 | 01234567 | 30 | 1.767, 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | FastSwarm.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.7 | 20 | AnimationFinishTrigger | paladin_hit_8dir/paladin_hit*.png | FastSwarm.controller |
| Walk | 8 | 01234567 | 30 | 0.833 | 26 | - | paladin_move_8dir/paladin_move*.png | FastSwarm.controller |

### Enemy role · RangedCaster

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | maw_attack_8dir/maw_attack*.png | RangedCaster.controller |
| Attack | 8 | 01234567 | 30 | 1.021 | 31 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | maw_attack_8dir/maw_attack*.png | RangedCaster.controller, RangedCaster_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | maw_attack_8dir/maw_attack*.png | RangedCaster.controller |
| Death | 8 | 01234567 | 30 | 1.5 | 45 | AnimationOnAction | maw_death_8dir/maw_death*.png | RangedCaster.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | maw_attack_8dir/maw_attack*.png | RangedCaster.controller |
| Idle | 8 | 01234567 | 30 | 4.033 | 121 | - | maw_idle_8dir/maw_idle*.png | RangedCaster.controller |
| Run | 8 | 01234567 | 30 | 1.433 | 43 | - | maw_move_8dir/maw_move*.png | RangedCaster.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.683 | 21 | AnimationFinishTrigger | maw_takedamage_8dir/maw_takedamage*.png | RangedCaster.controller |
| Walk | 8 | 01234567 | 30 | 1.433 | 43 | - | maw_move_8dir/maw_move*.png | RangedCaster.controller |

### Enemy role · Scatter

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Scatter.controller |
| Attack | 8 | 01234567 | 30 | 1.033 | 23 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | Scatter.controller, Scatter_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Scatter.controller |
| Death | 8 | 01234567 | 30 | 0.9 | 27 | AnimationOnAction | paladin_death_8dir/paladin_death*.png | Scatter.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | Scatter.controller |
| Idle | 8 | 01234567 | 30 | 1.267 | 39 | - | paladin_idle_8dir/paladin_idle*.png | Scatter.controller |
| Run | 8 | 01234567 | 30 | 1.767, 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | Scatter.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.7 | 20 | AnimationFinishTrigger | paladin_hit_8dir/paladin_hit*.png | Scatter.controller |
| Walk | 8 | 01234567 | 30 | 0.833 | 26 | - | paladin_move_8dir/paladin_move*.png | Scatter.controller |

### Enemy role · Sniper

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | erika_attack_8dir/erika_attack*.png | Sniper.controller |
| Attack | 8 | 01234567 | 30 | 1.021 | 31 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_8dir/erika_attack*.png | Sniper.controller, Sniper_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | erika_attack_8dir/erika_attack*.png | Sniper.controller |
| Death | 8 | 01234567 | 30 | 1.5 | 45 | AnimationOnAction | erika_death_8dir/erika_death*.png | Sniper.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_8dir/erika_attack*.png | Sniper.controller |
| Idle | 8 | 01234567 | 30 | 5.1 | 153 | - | erika_idle_8dir/erika_idle*.png | Sniper.controller |
| Run | 8 | 01234567 | 30 | 0.867 | 26 | - | erika_run_8dir/erika_run*.png | Sniper.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.683 | 21 | AnimationFinishTrigger | erika_takedamage_8dir/erika_takedamage*.png | Sniper.controller |
| Walk | 8 | 01234567 | 30 | 1.2 | 36 | - | erika_move_8dir/erika_move*.png | Sniper.controller |

### Enemy role · Tank

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | warrok_attack_8dir/warrok_attack*.png | Tank.controller |
| Attack | 8 | 01234567 | 30 | 1.021 | 31 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | warrok_attack_8dir/warrok_attack*.png | Tank.controller, Tank_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | warrok_attack_8dir/warrok_attack*.png | Tank.controller |
| Death | 8 | 01234567 | 30 | 1.5 | 45 | AnimationOnAction | warrok_death_8dir/warrok_death*.png | Tank.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | warrok_attack_8dir/warrok_attack*.png | Tank.controller |
| Idle | 8 | 01234567 | 30 | 4.033 | 121 | - | warrok_idle_8dir/warrok_idle*.png | Tank.controller |
| Run | 8 | 01234567 | 30 | 1.433 | 43 | - | warrok_move_8dir/warrok_move*.png | Tank.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.683 | 21 | AnimationFinishTrigger | warrok_takedamage_8dir/warrok_takedamage*.png | Tank.controller |
| Walk | 8 | 01234567 | 30 | 1.433 | 43 | - | warrok_move_8dir/warrok_move*.png | Tank.controller |

### Enemy role · TrashMelee

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Activate | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | TrashMelee.controller |
| Attack | 8 | 01234567 | 30 | 1.033 | 23 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | TrashMelee.controller, TrashMelee_AttackStage1.overrideController |
| Cast | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | TrashMelee.controller |
| Death | 8 | 01234567 | 30 | 0.9 | 27 | AnimationOnAction | paladin_death_8dir/paladin_death*.png | TrashMelee.controller |
| Do | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | TrashMelee.controller |
| Idle | 8 | 01234567 | 30 | 1.267 | 39 | - | paladin_idle_8dir/paladin_idle*.png | TrashMelee.controller |
| Run | 8 | 01234567 | 30 | 1.767, 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | TrashMelee.controller |
| TakeDamage | 8 | 01234567 | 30 | 0.7 | 20 | AnimationFinishTrigger | paladin_hit_8dir/paladin_hit*.png | TrashMelee.controller |
| Walk | 8 | 01234567 | 30 | 0.833 | 26 | - | paladin_move_8dir/paladin_move*.png | TrashMelee.controller |

### Enemy · Bat

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Bat_Attack | 1 | - | 30 | 0.567 | 7 | SetIsAttack | Bat/Bat_Full_FX.png | Bat_Controller.controller, Enemy.controller (+4) |
| Bat_Death | 1 | - | 30 | 0.767 | 9 | Die | Bat/Bat_Full_FX.png, Bat/Bat_Death.png | Bat_Controller.controller, Enemy.controller (+4) |
| Bat_Hit | 1 | - | 30 | 0.4 | 5 | - | Bat/Bat_Full_FX.png | Bat_Controller.controller, Enemy.controller (+4) |
| Bat_Idle | 1 | - | 30 | 0.333 | 4 | - | Bat/Bat_Full_FX.png | Bat_Controller.controller, Enemy.controller (+4) |
| Bat_Move | 1 | - | 30 | 0.333 | 4 | - | Bat/Bat_Full_FX.png | Bat_Controller.controller |

### Enemy · Crab

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Crab_Ability | 1 | - | 30 | 0.567 | 7 | - | Crab/Crab_Full_FX.png | Crab_Controller.controller |
| Crab_Attack_A | 1 | - | 30 | 0.9 | 11 | SetIsAttack | Crab/Crab_Full_FX.png | Crab_Controller.controller |
| Crab_Attack_B | 1 | - | 30 | 0.5 | 6 | - | Crab/Crab_Full_FX.png | Crab_Controller.controller |
| Crab_Attack_C | 1 | - | 30 | 0.667 | 8 | - | Crab/Crab_Full_FX.png | Crab_Controller.controller |
| Crab_Death | 1 | - | 30 | 0.767 | 9 | - | Crab/Crab_Full_FX.png | Crab_Controller.controller |
| Crab_Hit | 1 | - | 30 | 0.267 | 3 | - | Crab/Crab_Full_FX.png | Crab_Controller.controller |
| Crab_Idle | 1 | - | 30 | 0.333 | 4 | - | Crab/Crab_Full_FX.png | Crab_Controller.controller |
| Crab_Run | 1 | - | 30 | 0.5 | 6 | SetIsChase | Crab/Crab_Full_FX.png | Crab_Controller.controller |

### Enemy · Golem Phase 1

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| GolemP1_Attack_1 | 1 | - | 30 | 0.667 | 8 | SetIsAttack | Golem/Golem_Full_FX.png | Golem_A_Controller.controller |
| GolemP1_Attack_2 | 1 | - | 30 | 0.567 | 7 | - | Golem/Golem_Full_FX.png | Golem_A_Controller.controller |
| GolemP1_Death_A | 1 | - | 30 | 0.667 | 8 | - | Golem/Golem_Full_FX.png | Golem_A_Controller.controller |
| GolemP1_Hit_A | 1 | - | 30 | 0.4 | 5 | - | Golem/Golem_Full_FX.png | Golem_A_Controller.controller |
| GolemP1_Idle_A | 1 | - | 30 | 0.333 | 4 | SetIsChase | Golem/Golem_Full_FX.png | Golem_A_Controller.controller |

### Enemy · Golem Phase 2

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| GolemP2_Ability | 1 | - | 30 | 0.933 | 11 | - | Golem/Golem_Full_FX.png | Golem_B_Controller.controller, Golem_Reinforced_Controller.controller |
| GolemP2_Attack_1 | 1 | - | 30 | 1.1 | 13 | SetIsChase | Golem/Golem_Full_FX.png | Golem_B_Controller.controller |
| GolemP2_Death_B | 1 | - | 30 | 0.5 | 6 | - | Golem/Golem_Full_FX.png | Golem_B_Controller.controller |
| GolemP2_Hit_B | 1 | - | 30 | 0.4 | 5 | - | Golem/Golem_Full_FX.png | Golem_B_Controller.controller |
| GolemP2_Idle_B | 1 | - | 30 | 0.333 | 4 | - | Golem/Golem_Full_FX.png | Golem_B_Controller.controller |
| GolemP2_Run | 1 | - | 30 | 0.333 | 4 | - | Golem/Golem_Full_FX.png | Golem_B_Controller.controller |

### Enemy · Golem Phase 3

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| GolemP3_Ability | 1 | - | 30 | 0.933 | 11 | - | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |
| GolemP3_Attack_1 | 1 | - | 30 | 0.833 | 10 | SetIsAttack | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |
| GolemP3_Attack_1_Projectile | 1 | - | 30 | 0.267 | 3 | - | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Projectile_Controller.controller |
| GolemP3_Attack_2 | 1 | - | 30 | 0.767 | 9 | - | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |
| GolemP3_Attack_3 | 1 | - | 30 | 1.167 | 14 | - | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |
| GolemP3_Death | 1 | - | 30 | 0.667 | 8 | - | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |
| GolemP3_Hit | 1 | - | 30 | 0.267 | 3 | - | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |
| GolemP3_Idle | 1 | - | 30 | 0.333 | 4 | - | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |
| GolemP3_Reset | 1 | - | 30 | 0.567 | 7 | - | Golem/Golem_Full_FX.png | Golem_A_Controller.controller |
| GolemP3_Run | 1 | - | 30 | 0.333 | 4 | SetIsChase | Reinforced Golem/Golem_Armor_Full_FX.png | Golem_Reinforced_Controller.controller |

### Enemy · Pebble

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Pebble_Death | 1 | - | 30 | 0.667 | 8 | - | Pebble/Pebble_Death.png | Pebble_Controller.controller |
| Pebble_Hit | 1 | - | 30 | 0.4 | 5 | - | Pebble/Pebble_Hit.png | Pebble_Controller.controller |
| Pebble_Idle | 1 | - | 30 | 0.333 | 4 | - | Pebble/Pebble_Idle.png | Pebble_Controller.controller |
| Pebble_Run | 1 | - | 30 | 0.4 | 5 | - | Pebble/Pebble_Run.png | Pebble_Controller.controller |

### Enemy · Rat

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Rat_Ability | 1 | - | 30 | 0.567 | 7 | - | Rat/Rat_Full_FX.png | Rat_Controller.controller |
| Rat_Attack | 1 | - | 30 | 0.667 | 8 | - | Rat/Rat_Full_FX.png | Rat_Controller.controller |
| Rat_Death | 1 | - | 30 | 0.767 | 9 | - | Rat/Rat_Full_FX.png | Rat_Controller.controller |
| Rat_Hit | 1 | - | 30 | 0.333 | 4 | - | Rat/Rat_Full_FX.png | Rat_Controller.controller |
| Rat_Idle | 1 | - | 30 | 0.333 | 4 | - | Rat/Rat_Full_FX.png | Rat_Controller.controller |
| Rat_Run | 1 | - | 30 | 0.5 | 6 | - | Rat/Rat_Full_FX.png | Rat_Controller.controller |

### Enemy · Skull

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Skull_Death | 1 | - | 30 | 0.833 | 10 | - | Skull/Bones_SingleSkull_Full_FX.png | Skull_Controller.controller |
| Skull_Hit | 1 | - | 30 | 0.333 | 4 | - | Skull/Bones_SingleSkull_Full_FX.png | Skull_Controller.controller |
| Skull_Idle | 1 | - | 30 | 0.333 | 4 | - | Skull/Bones_SingleSkull_Full_FX.png | Skull_Controller.controller |
| Skull_Run | 1 | - | 30 | 0.667 | 8 | - | Skull/Bones_SingleSkull_Full_FX.png | Skull_Controller.controller |

### Enemy · Spiked Slime

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| SpikedSlime_Ability | 1 | - | 30 | 0.333 | 4 | - | Spiked Slime/Slime_Spiked_Full_FX.png | SpikedSlime_Controller.controller |
| SpikedSlime_Attack | 1 | - | 30 | 0.833 | 10 | - | Spiked Slime/Slime_Spiked_Full_FX.png | Crab_Controller.controller, Golem_A_Controller.controller (+4) |
| SpikedSlime_Death | 1 | - | 30 | 0.767 | 9 | - | Spiked Slime/Slime_Spiked_Full_FX.png | SpikedSlime_Controller.controller |
| SpikedSlime_Hit | 1 | - | 30 | 0.333 | 4 | - | Spiked Slime/Slime_Spiked_Full_FX.png | SpikedSlime_Controller.controller |
| SpikedSlime_Idle | 1 | - | 30 | 0.333 | 4 | - | Spiked Slime/Slime_Spiked_Full_FX.png | Crab_Controller.controller, Golem_A_Controller.controller (+4) |
| SpikedSlime_Run | 1 | - | 30 | 0.333 | 4 | - | Spiked Slime/Slime_Spiked_Full_FX.png | SpikedSlime_Controller.controller |

### Model · Erika

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Animations / erika_attack_inplace | 1 (1 empty) | - | 30 | 5 | 0 | - | - | - |
| Animations / erika_attack_up45_inplace | 1 (1 empty) | - | 30 | 5 | 0 | - | - | - |
| Animations / erika_death_inplace | 1 (1 empty) | - | 30 | 3.1 | 0 | - | - | - |
| Animations / erika_draw_aim_recoil_inplace | 1 (1 empty) | - | 30 | 1.733 | 0 | - | - | - |
| Animations / erika_draw_aim_recoil_up45_inplace | 1 (1 empty) | - | 30 | 1.733 | 0 | - | - | - |
| Animations / erika_move_inplace | 1 (1 empty) | - | 30 | 1.333 | 0 | - | - | - |
| Animations / erika_run_inplace | 1 (1 empty) | - | 30 | 0.867 | 0 | - | - | - |
| Animations / erika_spin_slash_inplace | 1 (1 empty) | - | 30 | 1.3 | 0 | - | - | - |
| Animations / erika_takedamage_inplace | 1 (1 empty) | - | 30 | 1.267 | 0 | - | - | - |
| Animations2D / erika_attack | 8 | 01234567 | 30 | 5 | 150 | - | erika_attack_8dir/erika_attack*.png | erika_8dir_controller.controller |
| Animations2D / erika_attack_up45 | 8 | 01234567 | 30 | 5 | 150 | - | erika_attack_up45_8dir/erika_attack_up45*.png | erika_8dir_controller.controller |
| Animations2D / erika_crouch | 8 | 01234567 | 30 | 0.933 | 28 | - | erika_crouch_8dir/erika_crouch*.png | erika_8dir_controller.controller |
| Animations2D / erika_death | 8 | 01234567 | 30 | 3.1 | 93 | - | erika_death_8dir/erika_death*.png | erika_8dir_controller.controller |
| Animations2D / erika_draw_aim_recoil | 8 | 01234567 | 30 | 1.733 | 52 | - | erika_draw_aim_recoil_8dir/erika_draw_aim_recoil*.png | erika_8dir_controller.controller |
| Animations2D / erika_draw_aim_recoil_up45 | 8 | 01234567 | 30 | 1.733 | 52 | - | erika_draw_aim_recoil_up45_8dir/erika_draw_aim_recoil_up45*.png | erika_8dir_controller.controller |
| Animations2D / erika_idle | 8 | 01234567 | 30 | 5.1 | 153 | - | erika_idle_8dir/erika_idle*.png | erika_8dir_controller.controller |
| Animations2D / erika_move | 8 | 01234567 | 30 | 1.333 | 40 | - | erika_move_8dir/erika_move*.png | erika_8dir_controller.controller |
| Animations2D / erika_run | 8 | 01234567 | 30 | 0.867 | 26 | - | erika_run_8dir/erika_run*.png | erika_8dir_controller.controller |
| Animations2D / erika_spin_slash | 8 | 01234567 | 30 | 1.3 | 39 | - | erika_spin_slash_8dir/erika_spin_slash*.png | erika_8dir_controller.controller |
| Animations2D / erika_takedamage | 8 | 01234567 | 30 | 1.267 | 38 | - | erika_takedamage_8dir/erika_takedamage*.png | erika_8dir_controller.controller |

### Model · Maw

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Animations / maw_attack_inplace | 1 (1 empty) | - | 30 | 2.667 | 0 | - | - | - |
| Animations / maw_death_inplace | 1 (1 empty) | - | 30 | 4.6 | 0 | - | - | - |
| Animations / maw_move_inplace | 1 (1 empty) | - | 30 | 1.433 | 0 | - | - | - |
| Animations / maw_takedamage_inplace | 1 (1 empty) | - | 30 | 1.2 | 0 | - | - | - |
| Animations2D / maw_attack | 8 | 01234567 | 30 | 2.667 | 80 | - | maw_attack_8dir/maw_attack*.png | maw_8dir_controller.controller |
| Animations2D / maw_death | 8 | 01234567 | 30 | 4.6 | 138 | - | maw_death_8dir/maw_death*.png | maw_8dir_controller.controller |
| Animations2D / maw_idle | 8 | 01234567 | 30 | 4.033 | 121 | - | maw_idle_8dir/maw_idle*.png | maw_8dir_controller.controller |
| Animations2D / maw_move | 8 | 01234567 | 30 | 1.433 | 43 | - | maw_move_8dir/maw_move*.png | maw_8dir_controller.controller |
| Animations2D / maw_takedamage | 8 | 01234567 | 30 | 1.2 | 36 | - | maw_takedamage_8dir/maw_takedamage*.png | maw_8dir_controller.controller |

### Model · Nightshade

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Animations / nightshade_attack_2h_inplace | 1 (1 empty) | - | 30 | 2.7 | 0 | - | - | - |
| Animations / nightshade_attack_2h_sustain_inplace | 1 (1 empty) | - | 30 | 4.3 | 0 | - | - | - |
| Animations / nightshade_attack_inplace | 1 (1 empty) | - | 30 | 2.3 | 0 | - | - | - |
| Animations / nightshade_cast_inplace | 1 (1 empty) | - | 30 | 2.267 | 0 | - | - | - |
| Animations / nightshade_death_inplace | 1 (1 empty) | - | 30 | 3.633 | 0 | - | - | - |
| Animations / nightshade_hit_inplace | 1 (1 empty) | - | 30 | 1.2 | 0 | - | - | - |
| Animations / nightshade_move_inplace | 1 (1 empty) | - | 30 | 1.133 | 0 | - | - | - |
| Animations / nightshade_run_inplace | 1 (1 empty) | - | 30 | 0.733 | 0 | - | - | - |
| Animations / nightshade_spell_throw_inplace | 1 (1 empty) | - | 30 | 4 | 0 | - | - | - |
| Animations / nightshade_spin_slash_inplace | 1 (1 empty) | - | 30 | 1.3 | 0 | - | - | - |
| Animations2D / nightshade_attack | 8 | 01234567 | 30 | 2.3 | 69 | - | nightshade_attack_8dir/nightshade_attack*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_attack_2h | 8 | 01234567 | 30 | 2.7 | 81 | - | nightshade_attack_2h_8dir/nightshade_attack_2h*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_attack_2h_sustain | 8 | 01234567 | 30 | 4.3 | 129 | - | nightshade_attack_2h_sustain_8dir/nightshade_attack_2h_sustain*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_cast | 8 | 01234567 | 30 | 2.267 | 68 | - | nightshade_cast_8dir/nightshade_cast*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_crouch | 8 | 01234567 | 30 | 1 | 30 | - | nightshade_crouch_8dir/nightshade_crouch*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_death | 8 | 01234567 | 30 | 3.633 | 109 | - | nightshade_death_8dir/nightshade_death*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_hit | 8 | 01234567 | 30 | 1.2 | 36 | - | nightshade_hit_8dir/nightshade_hit*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_idle | 8 | 01234567 | 30 | 1.867 | 56 | - | nightshade_idle_8dir/nightshade_idle*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_move | 8 | 01234567 | 30 | 1.133 | 34 | - | nightshade_move_8dir/nightshade_move*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_run | 8 | 01234567 | 30 | 0.733 | 22 | - | nightshade_run_8dir/nightshade_run*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_spell_throw | 8 | 01234567 | 30 | 4 | 120 | - | nightshade_spell_throw_8dir/nightshade_spell_throw*.png | nightshade_8dir_controller.controller |
| Animations2D / nightshade_spin_slash | 8 | 01234567 | 30 | 1.3 | 39 | - | nightshade_spin_slash_8dir/nightshade_spin_slash*.png | nightshade_8dir_controller.controller |

### Model · Paladin

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Animations / paladin_attack_inplace | 1 (1 empty) | - | 30 | 3.533 | 0 | - | - | - |
| Animations / paladin_death_inplace | 1 (1 empty) | - | 30 | 3.9 | 0 | - | - | - |
| Animations / paladin_hit_inplace | 1 (1 empty) | - | 30 | 0.967 | 0 | - | - | - |
| Animations / paladin_slash_up_inplace | 1 (1 empty) | - | 30 | 1.667 | 0 | - | - | - |
| Animations / paladin_spin_slash_inplace | 1 (1 empty) | - | 30 | 1.3 | 0 | - | - | - |
| Animations2D / paladin_attack | 8 | 01234567 | 30 | 3.533 | 106 | - | paladin_attack_8dir/paladin_attack*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_cast | 8 | 01234567 | 30 | 2.967 | 89 | - | paladin_cast_8dir/paladin_cast*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_crouch | 8 | 01234567 | 30 | 0.567 | 17 | - | paladin_crouch_8dir/paladin_crouch*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_death | 8 | 01234567 | 30 | 3.9 | 117 | - | paladin_death_8dir/paladin_death*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_hit | 8 | 01234567 | 30 | 0.967 | 29 | - | paladin_hit_8dir/paladin_hit*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_idle | 8 | 01234567 | 30 | 2.533 | 76 | - | paladin_idle_8dir/paladin_idle*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_move | 8 | 01234567 | 30 | 1.1 | 33 | - | paladin_move_8dir/paladin_move*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_power_up | 8 | 01234567 | 30 | 2.367 | 71 | - | paladin_power_up_8dir/paladin_power_up*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_run | 8 | 01234567 | 30 | 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_slash_up | 8 | 01234567 | 30 | 1.667 | 50 | - | paladin_slash_up_8dir/paladin_slash_up*.png | paladin_8dir_controller.controller |
| Animations2D / paladin_spin_slash | 8 | 01234567 | 30 | 1.3 | 39 | - | paladin_spin_slash_8dir/paladin_spin_slash*.png | paladin_8dir_controller.controller |

### Model · Vampire

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Animations / vampire_attack_inplace | 1 (1 empty) | - | 30 | 3.067 | 0 | - | - | - |
| Animations / vampire_death_inplace | 1 (1 empty) | - | 30 | 4.6 | 0 | - | - | - |
| Animations / vampire_move_inplace | 1 (1 empty) | - | 30 | 0.9 | 0 | - | - | - |
| Animations / vampire_takedamage_inplace | 1 (1 empty) | - | 30 | 1.2 | 0 | - | - | - |
| Animations2D / vampire_attack | 8 | 01234567 | 30 | 3.067 | 92 | - | vampire_attack_8dir/vampire_attack*.png | vampire_8dir_controller.controller |
| Animations2D / vampire_death | 8 | 01234567 | 30 | 4.6 | 138 | - | vampire_death_8dir/vampire_death*.png | vampire_8dir_controller.controller |
| Animations2D / vampire_idle | 8 | 01234567 | 30 | 3.333 | 100 | - | vampire_idle_8dir/vampire_idle*.png | vampire_8dir_controller.controller |
| Animations2D / vampire_move | 8 | 01234567 | 30 | 0.9 | 27 | - | vampire_move_8dir/vampire_move*.png | vampire_8dir_controller.controller |
| Animations2D / vampire_takedamage | 8 | 01234567 | 30 | 1.2 | 36 | - | vampire_takedamage_8dir/vampire_takedamage*.png | vampire_8dir_controller.controller |

### Model · Warrok

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Animations / warrok_attack_inplace | 1 (1 empty) | - | 30 | 2.667 | 0 | - | - | - |
| Animations / warrok_death_inplace | 1 (1 empty) | - | 30 | 4.6 | 0 | - | - | - |
| Animations / warrok_move_inplace | 1 (1 empty) | - | 30 | 1.433 | 0 | - | - | - |
| Animations / warrok_takedamage_inplace | 1 (1 empty) | - | 30 | 1.267 | 0 | - | - | - |
| Animations2D / warrok_attack | 8 | 01234567 | 30 | 2.667 | 80 | - | warrok_attack_8dir/warrok_attack*.png | warrok_8dir_controller.controller |
| Animations2D / warrok_death | 8 | 01234567 | 30 | 4.6 | 138 | - | warrok_death_8dir/warrok_death*.png | warrok_8dir_controller.controller |
| Animations2D / warrok_idle | 8 | 01234567 | 30 | 4.033 | 121 | - | warrok_idle_8dir/warrok_idle*.png | warrok_8dir_controller.controller |
| Animations2D / warrok_move | 8 | 01234567 | 30 | 1.433 | 43 | - | warrok_move_8dir/warrok_move*.png | warrok_8dir_controller.controller |
| Animations2D / warrok_takedamage | 8 | 01234567 | 30 | 1.267 | 38 | - | warrok_takedamage_8dir/warrok_takedamage*.png | warrok_8dir_controller.controller |

### Player · Archer

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Ability-Skill / Hawk Eye / Archer Hawk Eye State 1 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | erika_attack_8dir/erika_attack*.png | Archer Hawk Eye.overrideController |
| Ability-Skill / Hawk Eye / Archer Hawk Eye State 2 | 8 | 01234567 | 30 | 5 | 150 | AnimationOnAction, AnimationFinishTrigger | erika_attack_8dir/erika_attack*.png | Archer Hawk Eye.overrideController |
| Ability-Skill / Hawk Eye / Archer Hawk Eye State 3 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_8dir/erika_attack*.png | Archer Hawk Eye.overrideController |
| Ability-Skill / Kick-back Trap / Archer Kick-back Trap State 1 | 8 | 01234567 | 30 | 0.933 | 28 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | erika_crouch_8dir/erika_crouch*.png | Archer Kick-back Trap.overrideController |
| Ability-Skill / Kick-back Trap / Archer Kick-back Trap State 2 | 8 | 01234567 | 30 | 0.933 | 28 | AnimationOnAction, AnimationFinishTrigger | erika_crouch_8dir/erika_crouch*.png | Archer Kick-back Trap.overrideController |
| Ability-Skill / Kick-back Trap / Archer Kick-back Trap State 3 | 8 | 01234567 | 30 | 0.933 | 28 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_crouch_8dir/erika_crouch*.png | Archer Kick-back Trap.overrideController |
| Ability-Skill / Piercing Shot / Archer_Piercing_Shot_State_1 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | erika_attack_8dir/erika_attack*.png | Archer Piercing Shot.overrideController |
| Ability-Skill / Piercing Shot / Archer_Piercing_Shot_State_2 | 8 | 01234567 | 30 | 5 | 150 | AnimationOnAction, AnimationFinishTrigger | erika_attack_8dir/erika_attack*.png | Archer Piercing Shot.overrideController |
| Ability-Skill / Piercing Shot / Archer_Piercing_Shot_State_3 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_8dir/erika_attack*.png | Archer Piercing Shot.overrideController |
| Ability-Skill / Volley / Archer Volley State 1 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | erika_attack_up45_8dir/erika_attack_up45*.png | Archer Volley.overrideController |
| Ability-Skill / Volley / Archer Volley State 2 | 8 | 01234567 | 30 | 5 | 150 | AnimationOnAction, AnimationFinishTrigger | erika_attack_up45_8dir/erika_attack_up45*.png | Archer Volley.overrideController |
| Ability-Skill / Volley / Archer Volley State 3 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_up45_8dir/erika_attack_up45*.png | Archer Volley.overrideController |
| Archer_ComboAttack / Archer_ComboAttack_State 1 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_8dir/erika_attack*.png | Archer_ComboAttack_State1.overrideController |
| Archer_ComboAttack / Archer_ComboAttack_State 2 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_8dir/erika_attack*.png | Archer_ComboAttack_State2.overrideController |
| Archer_ComboAttack / Archer_ComboAttack_State 3 | 8 | 01234567 | 30 | 5 | 150 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | erika_attack_8dir/erika_attack*.png | Archer_ComboAttack_State3.overrideController |
| Base / Dash | 8 | 01234567 | 30 | 0.433 | 9 | AnimationOnAction, AnimationOffAction, AnimationEnd | erika_spin_slash_8dir/erika_spin_slash*.png | Archer.overrideController |
| Base / Death | 8 | 01234567 | 30 | 3.1 | 93 | - | erika_death_8dir/erika_death*.png | Archer.controller, Archer.overrideController |
| Base / Equid_unEquid | 8 | 01234567 | 30 | 0.933 | 28 | - | erika_crouch_8dir/erika_crouch*.png | Archer.controller, Archer.overrideController |
| Base / idle | 8 | 01234567 | 30 | 5.1 | 153 | - | erika_idle_8dir/erika_idle*.png | Archer.controller, Archer.overrideController |
| Base / Run | 8 | 01234567 | 30 | 0.867 | 26 | - | erika_run_8dir/erika_run*.png | Archer.controller, Archer.overrideController |
| Base / TakeDamage | 8 | 01234567 | 30 | 1.267 | 38 | - | erika_takedamage_8dir/erika_takedamage*.png | Archer.controller, Archer.overrideController |
| Base / Walk | 8 | 01234567 | 30 | 1.2 | 36 | - | erika_move_8dir/erika_move*.png | Archer.controller, Archer.overrideController |

### Player · Mage

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Ability-Skill / Chain Lightning / Mage Chain Lightning State 1 | 8 | 01234567 | 30 | 4 | 120 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | nightshade_spell_throw_8dir/nightshade_spell_throw*.png | Mage Chain Lightning.overrideController |
| Ability-Skill / Chain Lightning / Mage Chain Lightning State 2 | 8 | 01234567 | 30 | 4 | 120 | AnimationOnAction, AnimationFinishTrigger | nightshade_spell_throw_8dir/nightshade_spell_throw*.png | Mage Chain Lightning.overrideController |
| Ability-Skill / Chain Lightning / Mage Chain Lightning State 3 | 8 | 01234567 | 30 | 4 | 120 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | nightshade_spell_throw_8dir/nightshade_spell_throw*.png | Mage Chain Lightning.overrideController |
| Ability-Skill / Fireball / Mage Fireball State 1 | 8 | 01234567 | 30 | 2.7 | 81 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | nightshade_attack_2h_8dir/nightshade_attack_2h*.png | Mage Fireball.overrideController |
| Ability-Skill / Fireball / Mage Fireball State 2 | 8 | 01234567 | 30 | 2.7 | 81 | AnimationOnAction, AnimationFinishTrigger | nightshade_attack_2h_8dir/nightshade_attack_2h*.png | Mage Fireball.overrideController |
| Ability-Skill / Fireball / Mage Fireball State 3 | 8 | 01234567 | 30 | 2.7 | 81 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | nightshade_attack_2h_8dir/nightshade_attack_2h*.png | Mage Fireball.overrideController |
| Ability-Skill / Frost Nova / Mage Frost Nova State 1 | 8 | 01234567 | 30 | 4.3 | 129 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | nightshade_attack_2h_sustain_8dir/nightshade_attack_2h_sustain*.png | Mage Frost Nova.overrideController |
| Ability-Skill / Frost Nova / Mage Frost Nova State 2 | 8 | 01234567 | 30 | 4.3 | 129 | AnimationOnAction, AnimationFinishTrigger | nightshade_attack_2h_sustain_8dir/nightshade_attack_2h_sustain*.png | Mage Frost Nova.overrideController |
| Ability-Skill / Frost Nova / Mage Frost Nova State 3 | 8 | 01234567 | 30 | 4.3 | 129 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | nightshade_attack_2h_sustain_8dir/nightshade_attack_2h_sustain*.png | Mage Frost Nova.overrideController |
| Ability-Skill / Meteor / Mage Meteor State 1 | 8 | 01234567 | 30 | 2.267 | 68 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | nightshade_cast_8dir/nightshade_cast*.png | Mage Meteor.overrideController |
| Ability-Skill / Meteor / Mage Meteor State 2 | 8 | 01234567 | 30 | 2.267 | 68 | AnimationOnAction, AnimationFinishTrigger | nightshade_cast_8dir/nightshade_cast*.png | Mage Meteor.overrideController |
| Ability-Skill / Meteor / Mage Meteor State 3 | 8 | 01234567 | 30 | 2.267 | 68 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | nightshade_cast_8dir/nightshade_cast*.png | Mage Meteor.overrideController |
| Base / Dash | 8 | 01234567 | 30 | 0.433 | 9 | AnimationOnAction, AnimationOffAction, AnimationEnd | nightshade_spin_slash_8dir/nightshade_spin_slash*.png | Mage.overrideController |
| Base / Die | 8 | 01234567 | 30 | 3.633 | 109 | - | nightshade_death_8dir/nightshade_death*.png | Mage.controller, Mage.overrideController |
| Base / Equid_UnEquid | 8 | 01234567 | 30 | 1 | 30 | - | nightshade_crouch_8dir/nightshade_crouch*.png | Mage.controller, Mage.overrideController |
| Base / Idle | 8 | 01234567 | 30 | 1.867 | 56 | - | nightshade_idle_8dir/nightshade_idle*.png | Mage.controller, Mage.overrideController |
| Base / Run | 8 | 01234567 | 30 | 0.733 | 22 | - | nightshade_run_8dir/nightshade_run*.png | Mage.controller, Mage.overrideController |
| Base / Take Damage | 8 | 01234567 | 30 | 1.2 | 36 | - | nightshade_hit_8dir/nightshade_hit*.png | Mage.controller, Mage.overrideController |
| Base / Walk | 8 | 01234567 | 30 | 1.133 | 34 | - | nightshade_move_8dir/nightshade_move*.png | Mage.controller, Mage.overrideController |
| Mage_ComboAttack / Mage_ComboAttack_State1 | 8 | 01234567 | 30 | 2.3 | 69 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | nightshade_attack_8dir/nightshade_attack*.png | Mage_ComboAttack_State1.overrideController |
| Mage_ComboAttack / Mage_ComboAttack_State2 | 8 | 01234567 | 30 | 2.3 | 69 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | nightshade_attack_8dir/nightshade_attack*.png | Mage_ComboAttack_State2.overrideController |
| Mage_ComboAttack / Mage_ComboAttack_State3 | 8 | 01234567 | 30 | 2.3 | 69 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | nightshade_attack_8dir/nightshade_attack*.png | Mage_ComboAttack_State3.overrideController |

### Player · Paladin

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Ability-Skill / Block / Knight_Block_State_1 | 8 | 01234567 | 30 | 0.3 | 9 | AnimationTrigger, AnimationExitingState | Block/*.png | Knight_Block.overrideController |
| Ability-Skill / Block / Knight_Block_State_2 | 8 | 01234567 | 30 | 0.083, 0.1 | 2 | AnimationTrigger, AnimationExitingState | Block/*.png | Knight_Block.overrideController |
| Ability-Skill / Block / Knight_Block_State_3 | 8 | 01234567 | 30 | 0.567 | 17 | AnimationTrigger, AnimationFinishTrigger | Block/*.png | Knight_Block.overrideController |
| Ability-Skill / Consecrate / Consecrate_State1 | 8 | 01234567 | 30 | 0.633 | 20 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_cast_8dir/paladin_cast*.png | Paladin_Consecrate.overrideController |
| Ability-Skill / Consecrate / Consecrate_State2 | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_cast_8dir/paladin_cast*.png | Paladin_Consecrate.overrideController |
| Ability-Skill / Consecrate / Consecrate_State3 | 8 | 01234567 | 30 | 1.2 | 37 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_cast_8dir/paladin_cast*.png | Paladin_Consecrate.overrideController |
| Ability-Skill / Dash Ability / Knight_SpinAttack_State1 | 1 | 0 | 30 | 0.067 | 2 | AnimationTrigger | SpinAttack/Knight_SpinAttack*.png | Paladin_Dash_Skill.overrideController |
| Ability-Skill / Dash Ability / Knight_SpinAttack_State2 | 1 | 0 | 30 | 0.067 | 2 | AnimationTrigger | SpinAttack/Knight_SpinAttack*.png | Paladin_Dash_Skill.overrideController |
| Ability-Skill / Dash Ability / Knight_SpinAttack_State3 | 1 | 0 | 30 | 0.2 | 5 | AnimationTrigger, AnimationFinishTrigger | SpinAttack/Knight_SpinAttack*.png | Paladin_Dash_Skill.overrideController |
| Ability-Skill / Dual Ability | 8 | 01234567 | 30 | 0.267 | 8 | IsOnAbility, IsOffAbility | Attack/Knight_Attack*.png | - |
| Ability-Skill / Power Up / Knight_PowerUp_State1 | 8 | 01234567 | 30 | 0.833 | 25 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_power_up_8dir/paladin_power_up*.png | Paladin_Blessing.overrideController |
| Ability-Skill / Power Up / Knight_PowerUp_State2 | 8 | 01234567 | 30 | 0.367 | 10 | AnimationOnAction, AnimationFinishTrigger | paladin_power_up_8dir/paladin_power_up*.png | Paladin_Blessing.overrideController |
| Ability-Skill / Power Up / Knight_PowerUp_State3 | 8 | 01234567 | 30 | 0.633 | 19 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_power_up_8dir/paladin_power_up*.png | Paladin_Blessing.overrideController |
| Ability-Skill / SlashAbility(Hold Cast Skill) / Paladin_SpinAttack_State1 | 8 | 01234567 | 30 | 0.333 | 10 | AnimationStart, AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Enemy.controller, Archer.controller (+16) |
| Ability-Skill / SlashAbility(Hold Cast Skill) / Paladin_SpinAttack_State2 | 8 | 01234567 | 30 | 0.067 | 2 | AnimationOnAction, AnimationFinishTrigger | paladin_spin_slash_8dir/paladin_spin_slash*.png | Enemy.controller, Archer.controller (+16) |
| Ability-Skill / SlashAbility(Hold Cast Skill) / Paladin_SpinAttack_State3 | 8 | 01234567 | 30 | 0.5 | 15 | AnimationStart, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | Enemy.controller, Archer.controller (+16) |
| Basic / Dash | 8 | 01234567 | 30 | 0.433 | 9 | AnimationOnAction, AnimationOffAction, AnimationEnd | paladin_spin_slash_8dir/paladin_spin_slash*.png | Player.controller, Archer.overrideController (+1) |
| Basic / Die | 8 | 01234567 | 30 | 0.9 | 27 | AnimationOnAction | paladin_death_8dir/paladin_death*.png | Enemy.controller, Paladin.controller (+4) |
| Basic / Equid_UnEquid | 8 | 01234567 | 30 | 0.567 | 17 | AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_crouch_8dir/paladin_crouch*.png | Enemy.controller, Paladin.controller (+4) |
| Basic / Idle | 8 | 01234567 | 30 | 1.267 | 39 | - | paladin_idle_8dir/paladin_idle*.png | Enemy.controller, Paladin.controller (+4) |
| Basic / Interactor | 8 | 01234567 | 30 | 0.767 | 23 | AnimationStart, AnimationOnAction, AnimationEnd | Interract/*.png | Enemy.controller, Archer.controller (+4) |
| Basic / Run | 8 | 01234567 | 30 | 1.767, 0.7 | 21 | - | paladin_run_8dir/paladin_run*.png | Enemy.controller, Paladin.controller (+4) |
| Basic / Take Damage | 8 | 01234567 | 30 | 0.7 | 20 | AnimationFinishTrigger | paladin_hit_8dir/paladin_hit*.png | Enemy.controller, Paladin.controller (+4) |
| Basic / Walk | 8 | 01234567 | 30 | 0.833 | 26 | - | paladin_move_8dir/paladin_move*.png | Enemy.controller, Paladin.controller (+4) |
| Paladin_ComboAttack / Paladin_ComboAttack_State1 | 8 | 01234567 | 30 | 1.033 | 23 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationEnd, AnimationFinishTrigger / AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | Archer.controller, Mage.controller (+11) |
| Paladin_ComboAttack / Paladin_ComboAttack_State2 | 8 | 01234567 | 30 | 0.433 | 13 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | Paladin_ComboAttack_State2.overrideController |
| Paladin_ComboAttack / Paladin_ComboAttack_State3 | 8 | 01234567 | 30 | 1.3 | 39 | AnimationStart, AnimationTrigger, AnimationOnAction, AnimationFinishTrigger, AnimationEnd | paladin_attack_8dir/paladin_attack*.png | Paladin_ComboAttack_State3.overrideController |

### VFX · Particle Effect

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| CircleSummon | 1 | - | 30 | 2.033 | 53 | Execute, DespawnOneSelf | SummonCircle/SummonCircle_oneshot_344x184_8c8r_64f.png | CircleSummonEffect.controller |
| HealthEffect | 1 | - | 30 | 0.6 | 10 | - | Particle  Effect/18.png | HealthEffect.controller |
| Lightning / lightning_iso_blue_arc | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_blue_arc.png | LightningEffect.controller |
| Lightning / lightning_iso_classic | 1 | - | 30 | 0.367 | 9 | Execute, DespawnOneSelf | Lightning/lightning_iso_classic.png | LightningEffect.controller |
| Lightning / lightning_iso_forked | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_forked.png | LightningEffect.controller |
| Lightning / lightning_iso_frenzy | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_frenzy.png | LightningEffect.controller |
| Lightning / lightning_iso_heavy_bolt | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_heavy_bolt.png | LightningEffect.controller |
| Lightning / lightning_iso_pink_hot | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_pink_hot.png | LightningEffect.controller |
| Lightning / lightning_iso_razor | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_razor.png | LightningEffect.controller |
| Lightning / lightning_iso_titan | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_titan.png | LightningEffect.controller |
| Lightning / lightning_iso_twin_strike | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_twin_strike.png | LightningEffect.controller |
| Lightning / lightning_iso_wide_shock | 1 | - | 30 | 0.367 | 10 | Execute, DespawnOneSelf | Lightning/lightning_iso_wide_shock.png | LightningEffect.controller |
| RainArrows | 8 | 01234567 | 30 | 2.033 | 44, 47, 50 | OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, DespawnOneSelf / OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, DespawnOneSelf / OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, DespawnOneSelf / OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, OnArrowImpact, DespawnOneSelf | ArrowRain/ArrowRain_dir0_trai_duoi_oneshot_496x448_8c8r_64f.png, ArrowRain/ArrowRain_dir1_trai_oneshot_496x448_8c8r_64f.png (+6) | RainArrows.controller |
| RuneCircle | 1 | - | 30 | 1.033 | 20 | Execute, DespawnOneSelf | Rune cirle/rune_transparent_7x3.png | RuneCircleEffect.controller |

### VFX · Sprite/VFX

| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |
|---|---:|---|---|---|---|---|---|---|
| Slash FX / Slash_1 | 1 | - | 30 | 0.367 | 4 | - | Slash FX/small_slash_1.png | Test.controller |

## Sprite textures by folder

| Folder | Textures | Sprites | Used by a clip | Not used | Normal maps |
|---|---:|---:|---:|---:|---:|
| Assets/Sprite/Bar | 2 | 2 | 0 | 2 | 0 |
| Assets/Sprite/Bar/Materials | 3 | 3 | 0 | 3 | 0 |
| Assets/Sprite/Enemy/Bat | 8 | 81 | 2 | 6 | 0 |
| Assets/Sprite/Enemy/Crab | 13 | 161 | 1 | 12 | 0 |
| Assets/Sprite/Enemy/Golem | 18 | 257 | 1 | 17 | 0 |
| Assets/Sprite/Enemy/Pebble | 5 | 42 | 4 | 1 | 0 |
| Assets/Sprite/Enemy/Rat | 10 | 113 | 1 | 9 | 0 |
| Assets/Sprite/Enemy/Reinforced Golem | 13 | 196 | 1 | 12 | 0 |
| Assets/Sprite/Enemy/Skull | 7 | 81 | 1 | 6 | 0 |
| Assets/Sprite/Enemy/Spiked Slime | 10 | 97 | 1 | 9 | 0 |
| Assets/Sprite/Erika/Sprites/erika_attack_8dir | 8 | 1200 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_attack_up45_8dir | 8 | 1200 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_crouch_8dir | 8 | 224 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_death_8dir | 8 | 744 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_draw_aim_recoil_8dir | 8 | 416 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_draw_aim_recoil_up45_8dir | 8 | 416 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_idle_8dir | 8 | 1224 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_move_8dir | 8 | 320 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_run_8dir | 8 | 208 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_spin_slash_8dir | 8 | 312 | 8 | 0 | 8 |
| Assets/Sprite/Erika/Sprites/erika_takedamage_8dir | 8 | 304 | 8 | 0 | 8 |
| Assets/Sprite/Items | 2 | 109 | 0 | 2 | 0 |
| Assets/Sprite/Kinght/Knight-non-Weapon/Idle | 8 | 472 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/Knight-non-Weapon/Interract | 8 | 184 | 8 | 0 | 0 |
| Assets/Sprite/Kinght/Knight-non-Weapon/Pick Up | 8 | 136 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/Knight-non-Weapon/Run | 8 | 128 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/Knight-non-Weapon/Walk | 8 | 248 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Attack/Combo Attack | 8 | 848 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Attack/Jump Attack | 8 | 560 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Attack/Spin Attack | 8 | 312 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Block | 8 | 136 | 8 | 0 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Death | 8 | 936 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Idle | 8 | 608 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Run | 8 | 168 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Take Damage | 8 | 168 | 0 | 8 | 0 |
| Assets/Sprite/Kinght/KnightSnS/Walk | 8 | 264 | 0 | 8 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack | 2 | 2 | 0 | 2 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/skeleton1/v1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/skeleton1/v2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/skeleton2/v1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/skeleton2/v2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/skull/v1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/skull/v2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/vampire/v1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/monsters_idle/vampire/v2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/priests_idle/priest1/v1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/priests_idle/priest1/v2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/priests_idle/priest2/v1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/priests_idle/priest2/v2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/priests_idle/priest3/v1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/Character_animation/priests_idle/priest3/v2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/character and tileset | 4 | 102 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/interface | 24 | 24 | 0 | 24 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/arrow | 5 | 5 | 0 | 5 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/box_1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/box_2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/chest | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/coin | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/flag | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/flamethrower | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/flasks | 16 | 16 | 0 | 16 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/keys | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/mini_box_1 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/mini_box_2 | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/mini_chest | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/peaks | 4 | 4 | 0 | 4 | 0 |
| Assets/Sprite/Map/2D Pixel Dungeon Asset Pack/items and trap_animation/torch | 16 | 16 | 0 | 16 | 0 |
| Assets/Sprite/Maw/Sprites/maw_attack_8dir | 8 | 640 | 8 | 0 | 8 |
| Assets/Sprite/Maw/Sprites/maw_death_8dir | 8 | 1104 | 8 | 0 | 8 |
| Assets/Sprite/Maw/Sprites/maw_idle_8dir | 8 | 968 | 8 | 0 | 8 |
| Assets/Sprite/Maw/Sprites/maw_move_8dir | 8 | 344 | 8 | 0 | 8 |
| Assets/Sprite/Maw/Sprites/maw_takedamage_8dir | 8 | 288 | 8 | 0 | 8 |
| Assets/Sprite/NPC | 3 | 3 | 0 | 3 | 0 |
| Assets/Sprite/Nightshade/Sprites/nightshade_attack_2h_8dir | 8 | 648 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_attack_2h_sustain_8dir | 8 | 1032 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_attack_8dir | 8 | 552 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_cast_8dir | 8 | 544 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_crouch_8dir | 8 | 240 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_death_8dir | 8 | 872 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_hit_8dir | 8 | 288 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_idle_8dir | 8 | 448 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_move_8dir | 8 | 272 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_run_8dir | 8 | 176 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_spell_throw_8dir | 8 | 960 | 8 | 0 | 8 |
| Assets/Sprite/Nightshade/Sprites/nightshade_spin_slash_8dir | 8 | 312 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_attack_8dir | 8 | 848 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_attack_8dir/Fler | 0 | 0 | 0 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_cast_8dir | 8 | 712 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_cast_8dir/Fler | 0 | 0 | 0 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_crouch_8dir | 8 | 136 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_death_8dir | 8 | 936 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_hit_8dir | 8 | 232 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_idle_8dir | 8 | 608 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_idle_8dir/Fler | 0 | 0 | 0 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_move_8dir | 8 | 264 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_power_up_8dir | 8 | 568 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_run_8dir | 8 | 168 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_slash_up_8dir | 8 | 400 | 8 | 0 | 8 |
| Assets/Sprite/Paladin/Sprites/paladin_spin_slash_8dir | 8 | 312 | 8 | 0 | 8 |
| Assets/Sprite/Player/Attack/Block | 8 | 48 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/Block2 | 8 | 64 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/Cast | 8 | 10 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/CastSky | 8 | 20 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/ComboAttack | 8 | 280 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/CrouchAttack | 8 | 104 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/Die2 | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/Impact | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/JumpAttack | 8 | 184 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/JumpSpinAttack | 8 | 192 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/Kick | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/Knock | 8 | 0 | 0 | 8 | 0 |
| Assets/Sprite/Player/Attack/SpinAttack | 8 | 136 | 1 | 7 | 0 |
| Assets/Sprite/Player/Basic/Attack | 8 | 120 | 8 | 0 | 0 |
| Assets/Sprite/Player/Basic/Die | 8 | 216 | 0 | 8 | 0 |
| Assets/Sprite/Player/Basic/Idle | 8 | 136 | 0 | 8 | 0 |
| Assets/Sprite/Player/Basic/Jump | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/Basic/Run | 8 | 64 | 0 | 8 | 0 |
| Assets/Sprite/Player/Basic/Walk | 8 | 88 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/Climb | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/Crouch | 8 | 48 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/Draw | 8 | 56 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/Idle2 | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/Idle3 | 8 | 44 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/PowerUp | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/RunJump | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/Slide | 7 | 15 | 0 | 7 | 0 |
| Assets/Sprite/Player/ExMovement/StrafeBack | 8 | 40 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/StrafeLeft | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/StrafeRight | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/WalkBack | 8 | 19 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/WalkLeft | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/Player/ExMovement/WalkRight | 8 | 8 | 0 | 8 | 0 |
| Assets/Sprite/UIFlow | 1 | 1 | 0 | 1 | 0 |
| Assets/Sprite/VFX/Bonfire | 1 | 6 | 0 | 1 | 0 |
| Assets/Sprite/VFX/Lightning | 10 | 100 | 10 | 0 | 0 |
| Assets/Sprite/VFX/Particle  Effect | 35 | 44 | 1 | 34 | 0 |
| Assets/Sprite/VFX/RainArrow_Ultimate_Unity_PPU32/Sprites/ArrowRain | 8 | 415 | 8 | 0 | 0 |
| Assets/Sprite/VFX/RainArrow_Ultimate_Unity_PPU32/Sprites/SummonCircle | 1 | 62 | 1 | 0 | 0 |
| Assets/Sprite/VFX/Rune cirle | 1 | 20 | 1 | 0 | 0 |
| Assets/Sprite/VFX/Slash FX | 8 | 14 | 1 | 7 | 0 |
| Assets/Sprite/Vampire/Sprites/vampire_attack_8dir | 8 | 736 | 8 | 0 | 8 |
| Assets/Sprite/Vampire/Sprites/vampire_death_8dir | 8 | 1104 | 8 | 0 | 8 |
| Assets/Sprite/Vampire/Sprites/vampire_idle_8dir | 8 | 800 | 8 | 0 | 8 |
| Assets/Sprite/Vampire/Sprites/vampire_move_8dir | 8 | 216 | 8 | 0 | 8 |
| Assets/Sprite/Vampire/Sprites/vampire_takedamage_8dir | 8 | 288 | 8 | 0 | 8 |
| Assets/Sprite/Warrok/Sprites/warrok_attack_8dir | 8 | 640 | 8 | 0 | 8 |
| Assets/Sprite/Warrok/Sprites/warrok_death_8dir | 8 | 1104 | 8 | 0 | 8 |
| Assets/Sprite/Warrok/Sprites/warrok_idle_8dir | 8 | 968 | 8 | 0 | 8 |
| Assets/Sprite/Warrok/Sprites/warrok_move_8dir | 8 | 344 | 8 | 0 | 8 |
| Assets/Sprite/Warrok/Sprites/warrok_takedamage_8dir | 8 | 304 | 8 | 0 | 8 |
| Assets/Sprite/Weapons/Melee | 1 | 1 | 0 | 1 | 0 |
| Assets/Sprite/Weapons/Range | 2 | 2 | 0 | 2 | 0 |
