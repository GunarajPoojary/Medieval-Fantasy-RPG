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
