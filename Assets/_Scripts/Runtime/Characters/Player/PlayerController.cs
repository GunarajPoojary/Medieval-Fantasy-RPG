using System;
using ProjectEmbersteel.Player.Data.Animations;
using ProjectEmbersteel.Player.Data.Layers;
using ProjectEmbersteel.Player.Data.ScriptableObjects;
using ProjectEmbersteel.Player.StateMachines.Movement;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel.Player
{
    [SelectionBase]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IMovementStateAnimationEventsHandler
    {
        private PlayerStateMachine _stateMachine;

        [field: Header("References")]
        [field: SerializeField] public PlayerStateMachineConfigSO Data { get; private set; }

        [field: Header("Collisions")]
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        [field: Header("Input")]
        [field: SerializeField] public InputReader Input { get; private set; }
        public CharacterController Controller { get; private set; }
        public Animator Animator { get; private set; }

        // Delegates used by Third Person Follow Camera Component 
        public Action PreUpdate;
        public Action PostUpdate;

        private void Awake()
        {
            InitializeAnimationData();
            InitializeInput();

            InitializeComponents();

            CreateStateMachine();
        }

        private void Start() => SetDefaultState();

        private void Update()
        {
            PreUpdate?.Invoke();

            HandleInput();
            UpdateState();

            PostUpdate?.Invoke();
        }

        private void InitializeAnimationData() => AnimationData.Initialize();

        private void InitializeInput()
        {
            Input.Initialize();
            Input.EnablePlayerActions();
        }

        private void InitializeComponents()
        {
            Controller = GetComponent<CharacterController>();
            Animator = GetComponentInChildren<Animator>();
        }

        private void CreateStateMachine() => _stateMachine = new PlayerStateMachine(this);

        private void SetDefaultState() => _stateMachine.SwitchState(_stateMachine.IdleState);

        private void HandleInput() => _stateMachine.HandleInput();
        private void UpdateState() => _stateMachine.UpdateState();

        public void OnMovementStateAnimationEnterEvent() => _stateMachine.OnAnimationEnterEvent();
        public void OnMovementStateAnimationExitEvent() => _stateMachine.OnAnimationExitEvent();
        public void OnMovementStateAnimationTransitionEvent() => _stateMachine.OnAnimationTransitionEvent();
    }
}