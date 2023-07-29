# 2DPlatformerPlayerMechanics
This contains code from my personal 2d platformer project. I invested a lot of time in implementing player mechanics and integrating various mechanics toghether.

## Player Mechanics Included:
Movement : Walk, Run, movement on slope.  
Jump, Air Jump.  
8-directional Dash.  
Wall Slide, Wall Grab, and Wall Jump of 3 types.  
Grapple mechanics.  

Uses Unity's Input System.  
Uses Script Seperation technique which allows for writing different mechanics in different scripts which depend on one another.  
Added various sublte features that make the game feel better such as Jump Buffer, Coyote Time, JumpApex Modulation and much more.  

### Other Player scripts of importance:  
PlayerStatsScript : Manages health of the player  
PlayerRespawnScript : Respawns player to the lastest checkpoint on death  
PlayerAnimationScript : Mangages all animations to be played using a State Machine  
PlayerInputScript : Handles all input and supports both Keyboard-Mouse & Controller  
PlayerCollisionScript : Detects Collision with different objects  
