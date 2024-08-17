## Hierarchical State Machine (HSM)

In this project, we implement a **Hierarchical State Machine (HSM)** to manage the complex behavior of game objects, such as player characters or AI entities. A Hierarchical State Machine is an extension of the traditional finite state machine (FSM) that allows for the organization of states into a hierarchy, enabling more structured and maintainable code.

### Why Use a Hierarchical State Machine?

- **State Hierarchy**: HSMs allow states to be nested within each other, forming a parent-child relationship. This hierarchy lets us define shared behavior for multiple states at a higher level, reducing code duplication and simplifying transitions.

- **Reusability**: By organizing states hierarchically, common functionality (like handling player inputs or animations) can be defined in a parent state and inherited by child states. This promotes reusability and easier maintenance.

- **Simplified Transitions**: Transitions between states in a traditional FSM can become complex and error-prone as the number of states grows. An HSM allows for more manageable transitions by leveraging the hierarchy, where a child state can automatically transition back to its parent without explicit conditions.

### Structure of the Hierarchical State Machine

In our Unity project, the HSM is composed of the following components:

- **Base State Class**: This is the foundational class from which all other states inherit. It defines common properties and methods, such as entering and exiting a state, updating logic, and handling transitions.

- **Parent States**: These are high-level states that encapsulate shared behavior for a group of related child states. For example, a `PlayerMovementState` might serve as a parent state, providing basic movement logic that all movement-related states (e.g., walking, running) share.

- **Child States**: These are specific states that inherit from a parent state, adding specialized behavior or overriding the base functionality. For instance, `PlayerRunningState` and `PlayerWalkingState` would inherit from `PlayerMovementState`, each implementing unique logic for running and walking, respectively.

### Example: Player Movement State Machine

Here's a simple example of how the HSM is structured for player movement:

1. **PlayerState** (Base State)
    - Handles common functionality for all player states, such as input processing.
  
2. **PlayerMovementState** (Parent State)
    - Inherits from `PlayerState`.
    - Provides shared movement logic, such as velocity calculation and collision detection.
  
3. **PlayerWalkingState** (Child State)
    - Inherits from `PlayerMovementState`.
    - Implements walking-specific logic, such as adjusting speed and animation.
  
4. **PlayerRunningState** (Child State)
    - Inherits from `PlayerMovementState`.
    - Implements running-specific logic, like sprint speed and stamina reduction.

### Benefits in Practice

Using an HSM in this project leads to cleaner, more maintainable code by promoting separation of concerns. Each state is responsible for a specific aspect of behavior, and shared logic is naturally organized at the appropriate level of the hierarchy. This makes it easier to add new states or modify existing ones without introducing bugs or unnecessary complexity.
