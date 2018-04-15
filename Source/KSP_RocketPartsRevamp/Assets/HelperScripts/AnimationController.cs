using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {


    public Animation controlledAnimation;

    public string animationName = "name";

    public float animationTime = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (controlledAnimation != null)
        {
            animationTime = Mathf.Clamp(animationTime, 0f, 1f);
            controlledAnimation[animationName].enabled = true;
            controlledAnimation[animationName].speed = 0f;
            controlledAnimation[animationName].normalizedTime = animationTime;

            controlledAnimation.Play(animationName);
        }
	}
}
