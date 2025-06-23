using System;
using System.Collections.Generic;
using RPG.Player.Data.Animations;
using RPG.Player.Data.Layers;
using RPG.Player.Data.ScriptableObjects;
using RPG.Player.StateMachines.Movement;
using RPG.Player.Utilities.Colliders;
using RPG.Utilities.Inputs.ScriptableObjects;
using Unity.Cinemachine;
using UnityEngine;

namespace RPG.Player
{
    [SelectionBase]
    [RequireComponent(typeof(PlayerResizableCapsuleCollider))]
    public class PlayerController : MonoBehaviour, IMovementStateAnimationEventsHandler, Unity.Cinemachine.IInputAxisOwner
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

        [Header("Input Axes")]
        [Tooltip("X Axis movement.  Value is -1..1.  Controls the sideways movement")]
        public InputAxis MoveX = InputAxis.DefaultMomentary;

        [Tooltip("Z Axis movement.  Value is -1..1. Controls the forward movement")]
        public InputAxis MoveZ = InputAxis.DefaultMomentary;

        [Tooltip("Jump movement.  Value is 0 or 1. Controls the vertical movement")]
        public InputAxis Jump = InputAxis.DefaultMomentary;

        [Tooltip("Sprint movement.  Value is 0 or 1. If 1, then is sprinting")]
        public InputAxis Sprint = InputAxis.DefaultMomentary;

        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }

        public Action PreUpdate;
        public Action<Vector3, float> PostUpdate;

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

        private void CreateStateFactory() => _stateFactory = new PlayerStateFactory(this);

        private void SetDefaultState() => _stateFactory.SwitchState(_stateFactory.IdleState);
        private void HandleInput() => _stateFactory.HandleInput();
        private void UpdateState() => _stateFactory.UpdateState();
        private void PhysicsUpdate() => _stateFactory.PhysicsUpdate();
        private void StateOnTriggerEnter(Collider collider) => _stateFactory.OnTriggerEnter(collider);
        private void StateOnTriggerExit(Collider collider) => _stateFactory.OnTriggerExit(collider);

        public void OnMovementStateAnimationEnterEvent() => _stateFactory.OnAnimationEnterEvent();

        public void OnMovementStateAnimationExitEvent() => _stateFactory.OnAnimationExitEvent();

        public void OnMovementStateAnimationTransitionEvent() => _stateFactory.OnAnimationTransitionEvent();

        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref MoveX, Name = "Move X", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new() { DrivenAxis = () => ref MoveZ, Name = "Move Z", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new() { DrivenAxis = () => ref Jump, Name = "Jump" });
            axes.Add(new() { DrivenAxis = () => ref Sprint, Name = "Sprint" });
        }
    }
}