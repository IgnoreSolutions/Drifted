using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CheckForStandardAssets : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        // This only works in editor mode. Checking if editor mode still wont let Drifted build. Do we even use this?
        /*
        if(Application.isEditor && Application.isPlaying)
        {
            var guids = AssetDatabase.FindAssets("FXWater4Advanced", null);
            Debug.Assert(guids.Length > 0, "Please add Unity's Standard Assets to make water works! https://www.assetstore.unity3d.com/en/#!/content/32351");
        }
        */
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
