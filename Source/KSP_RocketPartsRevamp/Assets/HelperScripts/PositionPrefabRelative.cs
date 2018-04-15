using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/* This script will spawn prefabs automatically in the editor. This version uses the center of the renderer's bounds as the orign point for spawning. With an optional offset.
 * 
 * Chad Jenkins 2013
*/

[ExecuteInEditMode]
public class PositionPrefabRelative : MonoBehaviour
{

    /// <summary>
    /// Clones an object (non-linked to prefab) if true, you must specify the prefab path!
    /// </summary>
    public bool clonePrefab = false;

    public GameObject prefabToSpawn = null;

    /// <summary>
    ///  Search all transforms in the game for this name
    /// </summary>
    public string transformsName = "mesh";

    /// <summary>
    /// Use this if you do not wish to clone prefabs, and want to instantiate directly from source
    /// </summary>
    public string pathToPrefab = "Prefab Path Here";

    /// <summary>
    /// Where to spawn the prefab in relation to the center of the mesh (local space)
    /// </summary>
    public Vector3 localOffset = Vector3.zero;

    /// <summary>
    /// Enforce exact matches on names
    /// </summary>
    public bool strictNames = false;

    /// <summary>
    /// This option parents the newly created object to the transform that was found
    /// </summary>
    public bool parentToTarget = false;

    /// <summary>
    /// This option will determine if mesh bounds are used for finding the center point. If enabled, spawning requires mesh renderers to be present.
    /// </summary>
    public bool useMeshBounds = true;

    public bool requireMeshRenderer = true;

    /// <summary>
    /// Click to try and find your prefab
    /// </summary>
    public bool checkPrefabPath = false;

    /// <summary>
    /// Click this to find objects. It can take a while. Please do not mash it. Patience!
    /// </summary>
    public bool findObjects = false;

    /// <summary>
    /// After you have checked the list, or not. This will spawn the objects for you.
    /// </summary>
    public bool spawnObjects = false;




    /// <summary>
    /// The list of found objects if you want to verify them
    /// </summary>
    public List<Transform> cleanedList;

    // Warranty void if you mess with any of this.

    private Vector3 transformedOffset;

    private List<Transform> foundObjects;

    private Renderer meshRenderer = null;

    private GameObject newObject = null;

    private GameObject targetPrefab;

    private Object newPrefab;

    private void Start()
    {

    }

    private void RefreshList()
    {
        if (foundObjects == null)
            foundObjects = new List<Transform>();

        else
            foundObjects.Clear();

        if (cleanedList == null)
            cleanedList = new List<Transform>();
        else
            cleanedList.Clear();

        foundObjects = new List<Transform>(Transform.FindObjectsOfType(typeof(Transform)) as Transform[]);

        foreach (Transform t in foundObjects)
        {
            if (t == null)
                continue;

            if (strictNames)
            {
                if (t.name == transformsName)
                {
                    cleanedList.Add(t);
                }
            }
            else
            {
                if (t.name.Contains(transformsName))
                {
                    cleanedList.Add(t);
                }
            }
        }

        foundObjects.Clear();

    }

    private void SpawnObjects()
    {
        if (prefabToSpawn == null)
        {
            if (clonePrefab == true)
            {
                Debug.LogError("No prefab set! Failed to Spawn Objects");
                return;
            }
            else
            {
                if (targetPrefab == null)
                {
                    Debug.LogError("Check prefab path, and search for asset again. Failed to Spawn");
                    return;
                }
            }
        }



        foreach (Transform t in cleanedList)
        {
            if (t == null)
                continue;

            meshRenderer = t.GetComponent<Renderer>();

            if (t.GetComponent<Renderer>() != null || useMeshBounds == false)
            {
                if (useMeshBounds == false)
                {
                    transformedOffset = t.TransformPoint(localOffset);
                }
                else
                {
                    transformedOffset = t.TransformPoint(t.InverseTransformPoint(meshRenderer.bounds.center) + localOffset);
                }

                if (clonePrefab)
                {
                    newObject = GameObject.Instantiate(prefabToSpawn, transformedOffset, t.rotation) as GameObject;
                }
                else
                {
                    newObject = UnityEditor.PrefabUtility.InstantiatePrefab(targetPrefab) as GameObject;
                    if (newObject != null)
                    {
                        newObject.transform.position = transformedOffset;
                        newObject.transform.rotation = t.rotation;
                    }
                    else
                    {
                        Debug.LogError("Instantiation of prefab failed! Please use Check Path switch and try again");
                    }
                }

                if (parentToTarget)
                    newObject.transform.parent = t;
            }
        }
    }

    void FindPrefab()
    {
        targetPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath(pathToPrefab, typeof(GameObject)) as GameObject;

        if (targetPrefab == null)
            Debug.LogError("Failed to find prefab at location : " + pathToPrefab);
        else
            Debug.Log("Great Success, prefab :" + pathToPrefab + " Found!");
    }

    // Update is called once per frame
    void Update()
    {
        requireMeshRenderer = useMeshBounds;

        if (checkPrefabPath)
        {
            FindPrefab();

            checkPrefabPath = false;
        }


        if (findObjects)
        {
            RefreshList();

            findObjects = false;
        }

        if (spawnObjects)
        {
            if (cleanedList == null)
            {
                RefreshList();
            }

            SpawnObjects();

            spawnObjects = false;
        }
    }
}
