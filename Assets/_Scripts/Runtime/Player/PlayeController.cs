using RPG.Player.Data.Animations;
using RPG.Player.Data.Layers;
using RPG.Player.Data.ScriptableObjects;
using RPG.Player.StateMachines.Movement;
using RPG.Player.Utilities.Colliders;
using RPG.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace RPG.Player
{
    [SelectionBase]
    [RequireComponent(typeof(PlayerResizableCapsuleCollider))]
    public class PlayeController : MonoBehaviour, IMovementStateAnimationEventsHandler
    {
        private PlayerStateFactory _stateFactory;

        [field: Header("References")]
        [field: SerializeField] public PlayerStateMachineConfigSO Data { get; private set; }

        [field: Header("Collisions")]
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        [field: Header("Input")]
        [field: SerializeField] public InputReader Input { get; private set; }

        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }

        public PlayerResizableCapsuleCollider ResizableCapsuleCollider { get; private set; }

        private void Awake()
        {
            AnimationData.Initialize();
            Input.Initialize();
            Input.EnablePlayerActions();

            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();

            ResizableCapsuleCollider = GetComponent<PlayerResizableCapsuleCollider>();

            _stateFactory = new PlayerStateFactory(this);
        }

        private void Start() => _stateFactory.SwitchState(_stateFactory.IdleState);

        private void Update()
        {
            _stateFactory.HandleInput();
            _stateFactory.UpdateState();
        }

        private void FixedUpdate() => _stateFactory.PhysicsUpdate();

        private void OnTriggerEnter(Collider collider) => _stateFactory.OnTriggerEnter(collider);

        private void OnTriggerExit(Collider collider) => _stateFactory.OnTriggerExit(collider);

        public void OnMovementStateAnimationEnterEvent() => _stateFactory.OnAnimationEnterEvent();

        public void OnMovementStateAnimationExitEvent() => _stateFactory.OnAnimationExitEvent();

        public void OnMovementStateAnimationTransitionEvent() => _stateFactory.OnAnimationTransitionEvent();
    }
}