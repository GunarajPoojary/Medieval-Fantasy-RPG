using RPG.Core.SaveLoad;
using RPG.Player.Data;
using RPG.Player.StateMachine;
using RPG.Player.Utils;
using UnityEngine;

namespace RPG.Player
{
    [SelectionBase]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerResizableCapsuleCollider))]
    public class Player : MonoBehaviour, ISaveable
    {
        [field: Header("References")]
        [field: SerializeField] public PlayerSO Data { get; private set; }

        [field: Header("Collisions")]
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

        [field: Header("Camera")]
        [field: SerializeField] public PlayerCameraRecenteringUtility CameraRecenteringUtility { get; private set; }

        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }

        public PlayerInput Input { get; private set; }
        public PlayerResizableCapsuleCollider ResizableCapsuleCollider { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        private PlayerMovementStateMachine _movementStateMachine;

        private void Awake()
        {
            CameraRecenteringUtility.Initialize();
            AnimationData.Initialize();

            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();

            Input = GetComponent<PlayerInput>();
            ResizableCapsuleCollider = GetComponent<PlayerResizableCapsuleCollider>();

            MainCameraTransform = Camera.main.transform;

            _movementStateMachine = new PlayerMovementStateMachine(this);
        }

        private void Start() => _movementStateMachine.ChangeState(_movementStateMachine.IdlingState);

        private void Update()
        {
            _movementStateMachine.HandleInput();

            _movementStateMachine.Update();
        }

        private void FixedUpdate() => _movementStateMachine.PhysicsUpdate();

        private void OnTriggerEnter(Collider collider) => _movementStateMachine.OnTriggerEnter(collider);

        private void OnTriggerExit(Collider collider) => _movementStateMachine.OnTriggerExit(collider);

        public void OnMovementStateAnimationEnterEvent() => _movementStateMachine.OnAnimationEnterEvent();

        public void OnMovementStateAnimationExitEvent() => _movementStateMachine.OnAnimationExitEvent();

        public void OnMovementStateAnimationTransitionEvent() => _movementStateMachine.OnAnimationTransitionEvent();

        #region ISaveable Methods
        public void LoadData(GameData data)
        {
            Rigidbody.position = data.Position;
            Rigidbody.rotation = data.Rotation;
        }

        public void SaveData(GameData data)
        {
            data.Position = Rigidbody.position;
            data.Rotation = Rigidbody.rotation;
        }
        #endregion
    }
}