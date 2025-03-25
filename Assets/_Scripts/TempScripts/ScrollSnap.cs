using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollSnap : MonoBehaviour
{
    [Header("ScrollSnap Settings")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _snapSpeed = 10f;
    [SerializeField] private AnimationCurve _scaleCurve; // AnimationCurve for scaling

    private RectTransform _content;
    private float[] _positions;
    private int _targetIndex;
    private bool _isDragging = false;
    private bool _shouldLerp = false;

    private void Start()
    {
        Initialize();
        CalculatePositions();
        SetMiddleElementAsStart();
    }

    private void Update()
    {
        HandleMouseInput();
        if (_shouldLerp)
        {
            LerpToTarget();
        }
        UpdateChildScales();
    }

    #region Initialization
    private void Initialize()
    {
        _content = _scrollRect.content;
    }

    private void CalculatePositions()
    {
        int childCount = _content.childCount;
        _positions = new float[childCount];
        float distance = 1f / (childCount - 1);

        for (int i = 0; i < childCount; i++)
        {
            _positions[i] = distance * i;
        }
    }

    private void SetMiddleElementAsStart()
    {
        // Set the target index to the middle element
        _targetIndex = _positions.Length / 2;
        _scrollRect.horizontalNormalizedPosition = _positions[_targetIndex];
        UpdateChildScales();
    }
    #endregion

    #region Mouse Input Handling
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            _isDragging = true;
            _shouldLerp = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            SnapToClosest();
        }
    }
    #endregion

    #region Lerp and Snap Logic
    private void SnapToClosest()
    {
        float closestDistance = float.MaxValue;

        for (int i = 0; i < _positions.Length; i++)
        {
            float distance = Mathf.Abs(_scrollRect.horizontalNormalizedPosition - _positions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                _targetIndex = i;
            }
        }

        _shouldLerp = true;
    }

    private void LerpToTarget()
    {
        float targetPosition = _positions[_targetIndex];
        _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_scrollRect.horizontalNormalizedPosition, targetPosition, Time.deltaTime * _snapSpeed);

        if (Mathf.Abs(_scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
        {
            _scrollRect.horizontalNormalizedPosition = targetPosition;
            _shouldLerp = false;
        }
    }
    #endregion

    #region Child Scaling
    private void UpdateChildScales()
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            RectTransform child = _content.GetChild(i) as RectTransform;
            float distance = Mathf.Abs(_scrollRect.horizontalNormalizedPosition - _positions[i]);

            // Use the AnimationCurve to determine scale
            float scale = _scaleCurve.Evaluate(1f - distance);
            child.localScale = Vector3.one * scale;
        }
    }
    #endregion

    #region Navigation Methods
    public void Next()
    {
        if (_targetIndex < _positions.Length - 1)
        {
            _targetIndex++;
            _shouldLerp = true;
        }
    }

    public void Back()
    {
        if (_targetIndex > 0)
        {
            _targetIndex--;
            _shouldLerp = true;
        }
    }
    #endregion

    #region UI Detection
    private bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
    {
        foreach (var raycastResult in eventSystemRaycastResults)
        {
            if (raycastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }
        return false;
    }

    private static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
    #endregion
}
