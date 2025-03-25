using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;


public class TMCustomTools
{
	public static bool MobileInputEnabled;

	[MenuItem( "TMCustomTools/Input/Mobile/Enable")]
	public static void EnableMobileInput()
	{
		PlayerPrefs.SetInt ("MobileInputEnabled",0);
		MobileInputEnabled = true;
	}

	[MenuItem( "TMCustomTools/Input/Mobile/Enable",true)]
	public static bool EnableMobileInputValitade()
	{

		return !MobileInputEnabled;

	}

	[MenuItem( "TMCustomTools/Input/Mobile/Disable")]
	public static void DisableMobileInput()
	{
		PlayerPrefs.SetInt ("MobileInputEnabled", 1);
		MobileInputEnabled = false;
	}

	[MenuItem( "TM/Input/Mobile/Disable",true)]
	public static bool DisableMobileInputValitade()
	{
		return MobileInputEnabled;
	}
	

	[MenuItem( "TMCustomTools/ClearPrefs",false)]
	public static void ClearPrefrence()
	{
		PlayerPrefs.DeleteAll ();
	}
	[MenuItem( "TMCustomTools/ReimportUI",false)]
	public static void ReImportUI()
	{
		#if UNITY_4_6
		var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{0}/{1}";
		var version = Regex.Match( Application.unityVersion,@"^[0-9]+\.[0-9]+\.[0-9]+").Value;
		#else
		var path ="/Contents/UnityExtensions/Unity/GUISystem/{1}";
		var version = string.Empty;
		#endif
		string engineDll = string.Format( path, version, "UnityEngine.UI.dll");
		string editorDll = string.Format( path, version, "Editor/UnityEditor.UI.dll");
		ReimportDll( engineDll );
		ReimportDll( editorDll );
	}

	public static void ReimportDll(string path )
	{
		if ( File.Exists( path ) )
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate| ImportAssetOptions.DontDownloadFromCacheServer );
		else
			Debug.LogError( string.Format( "DLL not found {0}", path ) );
	}



	public delegate void ApplyOrRevert(GameObject _goCurrentGo, Object _ObjPrefabParent, ReplacePrefabOptions _eReplaceOptions);
	[MenuItem ("TMCustomTools/Apply all selected prefabs %#a")]
	static void ApplyPrefabs()
	{
		SearchPrefabConnections (ApplyToSelectedPrefabs);
	}
	
	[MenuItem ("TMCustomTools/Revert all selected prefabs %#r")]
	static void ResetPrefabs()
	{
		SearchPrefabConnections (RevertToSelectedPrefabs);
	}

	/// <summary>
	/// Offs the cast shadow.
	/// </summary>
	[MenuItem ("TMCustomTools/Set shadows/Off cast shadows")]
	static void offCastShadow()
	{
		OffCastShadows ();
	}
	static void OffCastShadows()
	{
		GameObject[] tSelection = Selection.gameObjects;
		foreach(GameObject go in tSelection)
		{
			MeshRenderer[] renders=go.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer r in renders)
			{
				r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			}
		}
	}


	/// <summary>
	/// Ons the cast shadow.
	/// </summary>
	[MenuItem ("TMCustomTools/Set shadows/on cast shadows")]
	static void onCastShadow()
	{
		OnCastShadows ();
	}
	static void OnCastShadows()
	{
		GameObject[] tSelection = Selection.gameObjects;
		foreach(GameObject go in tSelection)
		{
			MeshRenderer[] renders=go.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer r in renders)
			{
				r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			}
		}
	}

	/// <summary>
	/// Offs the receive shadow.
	/// </summary>
	[MenuItem ("TMCustomTools/Set shadows/Off Receive shadows")]
	static void OffReceiveShadow()
	{
		OffReceiveShadows ();
	}
	static void OffReceiveShadows()
	{
		GameObject[] tSelection = Selection.gameObjects;
		foreach(GameObject go in tSelection)
		{
			MeshRenderer[] renders=go.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer r in renders)
			{
				r.receiveShadows = false;

			}
		}
	}


	/// <summary>
	/// Ons the receive shadow.
	/// </summary>
	[MenuItem ("TMCustomTools/Set shadows/on Receive shadows")]
	static void onReceiveShadow()
	{
		OnReceiveShadows ();
	}
	static void OnReceiveShadows()
	{
		GameObject[] tSelection = Selection.gameObjects;
		foreach(GameObject go in tSelection)
		{
			MeshRenderer[] renders=go.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer r in renders)
			{
				r.receiveShadows = true;
				
			}
		}
	}
	

	//Look for connections
	static void SearchPrefabConnections(ApplyOrRevert _applyOrRevert)
	{
		GameObject[] tSelection = Selection.gameObjects;
		
		if (tSelection.Length > 0)
		{
			GameObject goPrefabRoot;
			GameObject goParent;
			GameObject goCur;
			bool bTopHierarchyFound;
			int iCount=0;
			PrefabType prefabType;
			bool bCanApply;
			//Iterate through all the selected gameobjects
			foreach(GameObject go in tSelection)
			{
				prefabType = PrefabUtility.GetPrefabType(go);
				//Is the selected gameobject a prefab?
				if(prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
				{
					//Prefab Root;
					goPrefabRoot = ((GameObject)PrefabUtility.GetCorrespondingObjectFromSource(go)).transform.root.gameObject;
					goCur = go;
					bTopHierarchyFound = false;
					bCanApply = true;
					//We go up in the hierarchy to apply the root of the go to the prefab
					while(goCur.transform.parent != null && !bTopHierarchyFound)
					{  
						//Are we still in the same prefab?
						if(goPrefabRoot == ((GameObject)PrefabUtility.GetCorrespondingObjectFromSource(goCur.transform.parent.gameObject)).transform.root.gameObject)
						{
							goCur = goCur.transform.parent.gameObject;
						}
						else
						{
							//The gameobject parent is another prefab, we stop here
							bTopHierarchyFound = true;
							if(goPrefabRoot !=  ((GameObject)PrefabUtility.GetCorrespondingObjectFromSource(goCur)))
							{
								//Gameobject is part of another prefab
								bCanApply = false;
							}
						}
					}
					
					if(_applyOrRevert != null && bCanApply)
					{
						iCount++;
						_applyOrRevert(goCur, PrefabUtility.GetCorrespondingObjectFromSource(goCur),ReplacePrefabOptions.ConnectToPrefab);
					}
				}
			}
			Debug.Log(iCount + " prefab" + (iCount>1 ? "s" : "") + " updated");
		}
	}
	
	//Apply      
	public static void ApplyToSelectedPrefabs(GameObject _goCurrentGo, Object _ObjPrefabParent, ReplacePrefabOptions _eReplaceOptions)
	{
		PrefabUtility.ReplacePrefab(_goCurrentGo, _ObjPrefabParent,_eReplaceOptions);
	}
	
	//Revert
	public static void RevertToSelectedPrefabs(GameObject _goCurrentGo, Object _ObjPrefabParent, ReplacePrefabOptions _eReplaceOptions)
	{
		PrefabUtility.ReconnectToLastPrefab(_goCurrentGo);
		PrefabUtility.RevertPrefabInstance(_goCurrentGo);
	}

	//GUI
	[MenuItem("TMCustomTools/uGUI/Anchors to Corners %[")]
	static void AnchorsToCorners(){
		RectTransform t = Selection.activeTransform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;
		
		if(t == null || pt == null) return;
		
		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
		                                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
		                                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);
		
		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}
	
	[MenuItem("TMCustomTools/uGUI/Corners to Anchors %]")]
	static void CornersToAnchors(){
		RectTransform t = Selection.activeTransform as RectTransform;
		
		if(t == null) return;

		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}


	[MenuItem("TMCustomTools/Set Terrin Hiegt]")]
	static void SetTerrienHeight()
	{
		ChangeTerrainResolution(1025);
	}

	static void ChangeTerrainResolution (int newResolution)
	{
		Terrain t = Terrain.activeTerrain;
		TerrainData d = t.terrainData;
		float[,] heights = d.GetHeights(0, 0, d.heightmapResolution, d.heightmapResolution);
		
		int previousResolution = d.heightmapResolution;
		d.heightmapResolution = newResolution;
		float[,] newHeights = new float[newResolution,newResolution];
		for(int x = 0; x < previousResolution; x++)
		{
			for(int y = 0; y < previousResolution; y++)
			{
				newHeights[x,y] = heights[x,y];
			}
		}
		d.SetHeights(0, 0, newHeights);
	}

	[MenuItem ("TMCustomTools/Disconnect Prefab")]
	static void DisconnectPrefabConnection()
	{
		GameObject[] selectedPrefabs = Selection.gameObjects;

		if (selectedPrefabs.Length > 0)
		{
			if (EditorUtility.DisplayDialog("Breaking prefab connection", "This is dangerous and can lead to broken objects and broken links to this object because it will be destroyed and recreated. Are you sure?", "I'm Sure", "Cancel"))
			{
				for (int i=0; i< selectedPrefabs.Length; i++)
				{
					PrefabUtility.DisconnectPrefabInstance (selectedPrefabs [i]);
				}
			}
		}
	}
}
