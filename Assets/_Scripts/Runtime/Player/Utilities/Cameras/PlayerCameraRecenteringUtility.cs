using Cinemachine;
using UnityEngine;

namespace RPG.Player.Utils
{
    [System.Serializable]
    public class PlayerCameraRecenteringUtility
    {
        [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
        [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;
        [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 4f;

        private CinemachinePOV _cinemachinePOV;

        public void Initialize() => _cinemachinePOV = VirtualCamera.GetCinemachineComponent<CinemachinePOV>();

        public void EnableRecentering(float waitTime = -1f, float recenteringTime = -1f, float baseMovementSpeed = 1f, float movementSpeed = 1f)
        {
            _cinemachinePOV.m_HorizontalRecentering.m_enabled = true;

            _cinemachinePOV.m_HorizontalRecentering.CancelRecentering();

            if (waitTime == -1f)
            {
                waitTime = DefaultHorizontalWaitTime;
            }

            if (recenteringTime == -1f)
            {
                recenteringTime = DefaultHorizontalRecenteringTime;
            }

            recenteringTime = recenteringTime * baseMovementSpeed / movementSpeed;

            _cinemachinePOV.m_HorizontalRecentering.m_WaitTime = waitTime;
            _cinemachinePOV.m_HorizontalRecentering.m_RecenteringTime = recenteringTime;
        }

        public void DisableRecentering() => _cinemachinePOV.m_HorizontalRecentering.m_enabled = false;
    }
}