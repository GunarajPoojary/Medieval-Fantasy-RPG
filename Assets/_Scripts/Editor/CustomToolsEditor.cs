using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Unity Editor tools for UI operations.
/// </summary>
public static class CustomToolsEditor
{
	#region Constants
	private const string TOOLS_MENU_PATH = "Tools/Custom/";
	private const string UGUI_MENU_PATH = TOOLS_MENU_PATH + "UGUI/";
	#endregion

	#region PlayerPrefs Management
	[MenuItem(TOOLS_MENU_PATH + "Clear All PlayerPrefs", false)]
	public static void ClearAllPlayerPreferences()
	{
		if (EditorUtility.DisplayDialog("Clear PlayerPrefs",
			"This will delete all PlayerPrefs data. This action cannot be undone. Are you sure?",
			"Yes", "Cancel"))
		{
			PlayerPrefs.DeleteAll();
			Debug.Log("All PlayerPrefs have been cleared.");
		}
	}
	#endregion

	#region UGUI Operations
	[MenuItem(UGUI_MENU_PATH + "Anchors to Corners %[")]
	public static void SetAnchorsToCorners()
	{
		RectTransform rectTransform = Selection.activeTransform as RectTransform;
		RectTransform parentRectTransform = Selection.activeTransform?.parent as RectTransform;

		if (rectTransform == null)
		{
			Debug.LogWarning("Selected object is not a RectTransform.");
			return;
		}

		if (parentRectTransform == null)
		{
			Debug.LogWarning("Selected object's parent is not a RectTransform.");
			return;
		}

		Vector2 newMinAnchors = new Vector2(
			rectTransform.anchorMin.x + rectTransform.offsetMin.x / parentRectTransform.rect.width,
			rectTransform.anchorMin.y + rectTransform.offsetMin.y / parentRectTransform.rect.height
		);

		Vector2 newMaxAnchors = new Vector2(
			rectTransform.anchorMax.x + rectTransform.offsetMax.x / parentRectTransform.rect.width,
			rectTransform.anchorMax.y + rectTransform.offsetMax.y / parentRectTransform.rect.height
		);

		rectTransform.anchorMin = newMinAnchors;
		rectTransform.anchorMax = newMaxAnchors;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;

		Debug.Log($"Anchors set to corners for {rectTransform.name}.");
	}

	[MenuItem(UGUI_MENU_PATH + "Anchors to Corners (Recursive) %#[" )]
	public static void SetAnchorsToCornersRecursive()
	{
		Transform selected = Selection.activeTransform;

		if (selected == null)
		{
			Debug.LogWarning("No GameObject selected.");
			return;
		}

		RectTransform[] rects = selected.GetComponentsInChildren<RectTransform>(true);

		int updatedCount = 0;

		foreach (RectTransform rectTransform in rects)
		{
			if (rectTransform == selected) continue; // skip the root itself if you want

			RectTransform parentRectTransform = rectTransform.parent as RectTransform;
			if (parentRectTransform == null) continue;

			Undo.RecordObject(rectTransform, "Set Anchors to Corners");

			Vector2 newMinAnchors = new Vector2(
				rectTransform.anchorMin.x + rectTransform.offsetMin.x / parentRectTransform.rect.width,
				rectTransform.anchorMin.y + rectTransform.offsetMin.y / parentRectTransform.rect.height
			);

			Vector2 newMaxAnchors = new Vector2(
				rectTransform.anchorMax.x + rectTransform.offsetMax.x / parentRectTransform.rect.width,
				rectTransform.anchorMax.y + rectTransform.offsetMax.y / parentRectTransform.rect.height
			);

			rectTransform.anchorMin = newMinAnchors;
			rectTransform.anchorMax = newMaxAnchors;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;

			EditorUtility.SetDirty(rectTransform);
			updatedCount++;
		}

		Debug.Log($"Anchors set to corners on {updatedCount} child RectTransform(s).");
	}

	[MenuItem(UGUI_MENU_PATH + "Corners to Anchors %]")]
	public static void SetCornersToAnchors()
	{
		RectTransform rectTransform = Selection.activeTransform as RectTransform;

		if (rectTransform == null)
		{
			Debug.LogWarning("Selected object is not a RectTransform.");
			return;
		}

		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;

		Debug.Log($"Corners set to anchors for {rectTransform.name}.");
	}
	#endregion
}