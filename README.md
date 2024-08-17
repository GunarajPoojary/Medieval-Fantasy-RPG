## Project Status

### Implemented

- **New Input System**: Managed player inputs with callbacks for actions like `started`, `performed`, and `canceled`. Implemented input interactions and camera control using Unity's New Input System.
- **Movement System**: Includes features such as:
  - **Cinemachine**: Orbital rotation and zoom functionality for the camera.
  - **State Machines**: Efficient state transitions and state caching.
  - **Floating Capsule**: Prevents the player from getting stuck or falling improperly with dynamic capsule adjustments.
  - **Physics-Based Movement**: Character movement handled via Rigidbody and Unity's physics system.
  - **Player Movement**: States including Walking, Running, Dashing, Sprinting, Jumping, and Falling.
  - **Player Rotation**: Rotation relative to the camera and automatic rotation handling.
  - **Player Dash**: Allows dashing with cooldown management.
  - **Player Jump**: Directional jumping and force adjustments based on state and slope.
  - **Player Fall**: Handling falling transitions and velocity limits.
  - **Player Landing**: Different landing states for varied fall heights.
  - **Player Stopping**: Different stopping states for varied velocities.
- **Equipments**: Used Scriptable Objects.
- **Inventory System**: Integrated to manage items and interactions.
- **Equipment System**: Includes a Character Menu for equipping items.
- **Save/Load System**: Added functionality for saving and loading game states.
  - **Animations**: Utilized Mixamo animations with reusable sub-state machines in the Animator Controller.
### Not Implemented

-**Animations**: Some Animations for Equipments.
- **Combat System**: The project does not include a combat system or predefined control scheme for combat.
- **Enemy AI**: The project does not yet include enemy AI systems.
- **NPC**: There are no NPCs (Non-Player Characters) or associated systems implemented in the project.

### Next To Work

- **UI Improvements**

## Resources

### Unity State Machine Implementation

I used the following video to guide my implementation of the state machine in this project:
[Genshin Impact Movement in Unity](https://www.youtube.com/watch?v=kluTqsSUyN0&t=16274s)
This video provided valuable insights into setting up different states. The explanation of how to manage state transitions based on the player's velocity and input was particularly helpful in refining the Player Movement system in this project.
## New Input System

This project utilizes Unity's **New Input System** to manage player inputs more effectively. The New Input System allows for the addition of callbacks whenever a key is pressed, changed, or released, corresponding to the actions of `started`, `performed`, and `canceled`.

### Key Features

- **Callbacks**: The system provides easy-to-use callbacks that trigger specific actions based on input states:
  - **Started**: Triggered when a key is initially pressed.
  - **Performed**: Triggered while the key remains pressed, allowing for continuous actions.
  - **Canceled**: Triggered when the key is released.

- **Input Interactions**: Features like delaying the recognition of a key press until after a certain amount of time has passed can be easily implemented using `Interactions`. This is useful for more nuanced control schemes, such as charging an attack or holding down a button for a special action.

- **Camera Control**: The Camera in this project is also controlled using the New Input System, providing a seamless experience between player actions and camera movements.

### Limitations

- **Control Scheme**: The project does not provide a pre-defined Control Scheme. Currently, it only supports keyboard inputs, which you can customize according to your needs.
![Player Input System](https://github.com/user-attachments/assets/eb68b524-f495-4d6e-87e2-4776755536f5)
![UI Input System](https://github.com/user-attachments/assets/becea9fa-44c7-4a33-aca3-e83ec069c47f)
## Movement System

### Cinemachine

The Camera offers an **Orbital Rotation** around the Player. You can Zoom In and Zoom Out using the Mouse Scroll Wheel. It also provides different rotations at various angles or movement directions.

- **Sideways Movement**: Moving the Player to the sides slowly rotates the Camera, which in turn rotates the Player, as the Player's rotation is relative to the Camera.
- **Top/Bottom View**: When looking from the top or bottom, the Camera rotates faster, causing the Player to move in a small circular motion.

### States & State Machines

The main logic of the Player Movement follows the **State Machines Pattern**.

- **State Caching**: Existing states are cached instead of instantiating new ones. This allows for efficient state transitions.
- **State Transition**: Changing to another state can be done through a simple line of code: `stateMachine.ChangeState(stateMachine.IdlingState)`.

### Floating Capsule

To prevent the Player from getting stuck on steps and to avoid jumping on slopes, a force is constantly added to the Player to keep it afloat.

- **Resizable Capsule Collider**: A feature is provided that adjusts the capsule collider's size, corresponding to the maximum climbable step height. The size change only affects the bottom of the collider.
- **Raycast and Force Adjustment**: A Ray is cast downward, and a force is added or subtracted depending on the distance from the collider center to the ground. This keeps the Player at a consistent height when moving on slopes or climbing steps.

### Physics-Based Movement

The Character Movement is handled using a **Rigidbody**, meaning it moves based on Unity's built-in Physics system.

### Player Movement

The Ground Movement system includes multiple states such as Walking, Running, Dashing, Sprinting, Landing, and Stopping. It also allows toggling between Walking and Running with a button press.

- **Jumping and Falling**: The Player can jump and fall, with slower movement on steeper slopes (controlled by an Animation Curve that sets angles and speeds).
- **Sprinting Transition**: The system handles transitions from Sprinting to Running or Walking when the Sprint key is not held long enough.

### Player Rotation

The Player's rotation is relative to the Camera and Movement Input. It offers automatic rotation in certain situations.

- **Automatic Rotation**: For example, quickly pressing a movement key will keep rotating the Player until it is facing the desired movement direction.

### Player Dash

The Player can dash up to 2 times (updatable) within 1 second (updatable).

- **Cooldown**: If the Player dashes too quickly, it enters a cooldown (updatable), and the Player will need to wait for the cooldown to end before dashing again.

### Player Jump

The Player Jump offers different possibilities based on input.

- **Jump Direction**: If a movement key is pressed, the Player will jump in that direction. If no movement key is pressed, the Player will jump in the direction it is facing.
- **Force Adjustment**: The jump force depends on the current Player State and the Slope/Ground Angle (managed by an Animation Curve). This prevents sliding on slopes due to excessive horizontal velocity.

### Player Fall

When the Player leaves the ground, it enters a Falling state.

- **Transition to Falling**: The Jump state transitions to Falling when the Player's vertical velocity peaks and starts to decrease.
- **Velocity Limit**: A velocity limit ensures proper collision with shallow ground colliders.

### Player Landing

When the Player collides with the ground after being airborne, it enters a Landing state.

- **Landing States**: 
  - **Light Landing**: Common across all airborne states, occurring when falling from a small height.
  - **Hard Landing**: Happens when falling from a great height with no movement key pressed at the moment of contact.
  - **Rolling**: Occurs when falling from a great height with a movement key pressed at the moment of contact.

### Animations

Animations are handled through an Animator Controller using **Reusable Sub-State Machines** (No Blend Trees are used).

### Mixamo Animations

For animations, this project utilizes assets from **Mixamo**. Mixamo provides a variety of pre-made animations that were integrated into the Animator Controller. These animations include walking, running, jumping, and landing, which were essential for creating a responsive and dynamic player movement system.

- **Mixamo Animations**: You can explore Mixamo's animation library [here](https://www.mixamo.com/). The animations were imported into Unity and used to set up the Player's movement and action states effectively.
