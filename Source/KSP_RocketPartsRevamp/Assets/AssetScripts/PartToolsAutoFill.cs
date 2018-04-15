using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using KSPPartTools;

public class PartToolsAutoFill : EditorWindow 
{
 	private string path = "Squad/Parts/";
 	private string modelname = "model";
 	private bool autoname = true;
 	private bool usefolder = true;
 	private bool production = true;
 	private string folderpath = "Assets/PartPrefabs/Parts/";
 	private PartTools.TextureFormat textureFormat = PartTools.TextureFormat.MBM;
 	
 	
    [MenuItem("Tools/KSP Part Tools Auto-Fill")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PartToolsAutoFill));
    }
 
    public void OnGUI()
    {
        
            GUILayout.Label("Part Tools Options", EditorStyles.boldLabel);
            modelname = EditorGUILayout.TextField ("Model Name", modelname);
            production = EditorGUILayout.Toggle ("Production", production);
            textureFormat = (PartTools.TextureFormat)EditorGUILayout.EnumPopup ("Texture Format", textureFormat);
            
            EditorGUILayout.Separator();
            GUILayout.Label("Export Path", EditorStyles.boldLabel);
            path = EditorGUILayout.TextField ("Root Directory", path);
            autoname = EditorGUILayout.Toggle ("Use Prefab Name", autoname);
            usefolder = EditorGUILayout.Toggle ("Append Parent Folder", usefolder);
            
            
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            folderpath = EditorGUILayout.TextField ("Target Folder", folderpath);
        if (GUILayout.Button("Set"))
        {
        	folderpath = EditorUtility.OpenFolderPanel("Set Path", folderpath, folderpath);
        }
            EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Fill"))
        {
            AutoFill();
        }
            
            EditorGUILayout.Separator();
        if (GUILayout.Button("List Unexported"))
        {
     
            foreach(PartTools pt in GetPartToolsAtPath(folderpath))
            {
            	if(!File.Exists("GameData/" + pt.filePath + "/" + pt.modelName))
            		Debug.Log("Part at " + pt.filePath + " is not yet exported.");
            }
        }
    }
    
    private List<string> GetPrefabsAtPath(string path)
    {
    	List<string> list = new List<string>();
    	string[] dirs = System.IO.Directory.GetDirectories(path);
    	string[] files = System.IO.Directory.GetFiles(path);
    	foreach(string s in dirs)
				list.AddRange(GetPrefabsAtPath(s));
		foreach(string s in files)
		{
			if(s.EndsWith(".prefab"))
    			list.Add(s);
    	}
    	return list;
    }
    
    private List<PartTools> GetPartToolsAtPath(string path)  
    {
    	List<string> paths = GetPrefabsAtPath(path);
    	List<PartTools> parttools = new List<PartTools>();
    	foreach(string s in paths)
    	{
    		Object o = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject));
    		GameObject go = o as GameObject;
    		PartTools pt = go.GetComponent<PartTools>();
    		if(pt != null)
    			parttools.Add(pt);
    	}
    	return parttools;
    }  
    
    private void AutoFill()
    {
    	Debug.Log("Autofilling");
    	List<string> paths = GetPrefabsAtPath(folderpath);
    	foreach(string s in paths)
    	{
    		Debug.Log("Loading asset " + s);
    		string[] pathsplit = Regex.Split(s, @"[/\\]");
    		string ptpath = path;
    		if(usefolder && pathsplit.Length > 1)
    		{
    			ptpath += pathsplit[pathsplit.Length - 2] + "/";
    		}
    		if(autoname && pathsplit.Length > 0)
    		{
    			string partname = pathsplit[pathsplit.Length - 1];
    			ptpath += partname.Split('.')[0];
    		}
    		Object o = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject));
    		GameObject go = o as GameObject;
    		if(go == null)
    		{
    			Debug.LogWarning("Cannot load asset at " + s);
    			continue;
    		}
            PartTools pt = null;
            if((pt = go.GetComponent<PartTools>()) == null)
            	pt = go.AddComponent<PartTools>();
            pt.modelName = modelname;
            pt.filePath = ptpath;
            pt.production = production;
            pt.textureFormat = textureFormat;
            EditorUtility.SetDirty(go);	   
    	}
     }
}