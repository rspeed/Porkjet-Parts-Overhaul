using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OrientToSurface : MonoBehaviour {


    public Transform objectToAlign;
    public Transform rayCaster;

    public Vector3 raycastAxis;

    public Vector3 alignmentAxis;

    private Vector3 raycastDirection;

    RaycastHit hit;

    private Vector3 hitNormal;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {

        raycastDirection = rayCaster.TransformDirection(raycastAxis);

        if (Physics.Raycast(rayCaster.position, raycastDirection, out hit))
        {
            hitNormal = hit.normal;
            
            Vector3 relativeNormal = objectToAlign.InverseTransformDirection(-hitNormal);

            float angle = Mathf.Atan2(-relativeNormal.y, relativeNormal.z) * Mathf.Rad2Deg;
            //

            Quaternion fromRot = objectToAlign.rotation * Quaternion.Euler(angle, 0, 0);

            objectToAlign.rotation = fromRot;
        }

	}

}
