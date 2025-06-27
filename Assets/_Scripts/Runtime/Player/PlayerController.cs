using System;
using ProjectEmbersteel.Player.Data.Animations;
using ProjectEmbersteel.Player.Data.Layers;
using ProjectEmbersteel.Player.Data.ScriptableObjects;
using ProjectEmbersteel.Player.StateMachines.Movement;
using ProjectEmbersteel.Player.Utilities.Colliders;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel.Player
{
    [SelectionBase]
    [RequireComponent(typeof(PlayerResizableCapsuleCollider))]
    public class PlayerController : MonoBehaviour, IMovementStateAnimationEventsHandler
    {
        private PlayerStateFactory _stateMachine;

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
            InitializeAnimationData();
            InitializeInput();

            InitializeComponents();

            CreateStateFactory();
        }

        private void Start() => SetDefaultState();

        private void Update()
        {
            HandleInput();
            UpdateState();
        }

        private void FixedUpdate() => PhysicsUpdate();

        private void OnTriggerEnter(Collider collider) => StateOnTriggerEnter(collider);

        private void OnTriggerExit(Collider collider) => StateOnTriggerExit(collider);

        private void InitializeAnimationData() => AnimationData.Initialize();

        private void InitializeInput()
        {
            Input.Initialize();
            Input.EnablePlayerActions();
        }

        private void InitializeComponents()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();

            ResizableCapsuleCollider = GetComponent<PlayerResizableCapsuleCollider>();
        }

        private void CreateStateFactory() => _stateMachine = new PlayerStateFactory(this);

        private void SetDefaultState() => _stateMachine.SwitchState(_stateMachine.IdleState);
        private void HandleInput() => _stateMachine.HandleInput();
        private void UpdateState() => _stateMachine.UpdateState();
        private void PhysicsUpdate() => _stateMachine.PhysicsUpdate();
        private void StateOnTriggerEnter(Collider collider) => _stateMachine.OnTriggerEnter(collider);
        private void StateOnTriggerExit(Collider collider) => _stateMachine.OnTriggerExit(collider);

        public void OnMovementStateAnimationEnterEvent() => _stateMachine.OnAnimationEnterEvent();

        public void OnMovementStateAnimationExitEvent() => _stateMachine.OnAnimationExitEvent();

        public void OnMovementStateAnimationTransitionEvent() => _stateMachine.OnAnimationTransitionEvent();
    }
}