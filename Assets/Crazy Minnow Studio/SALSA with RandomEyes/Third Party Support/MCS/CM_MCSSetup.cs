using UnityEngine;
using System.Collections.Generic;
using MORPH3D;
using MORPH3D.FOUNDATIONS;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.MCS
{
    [AddComponentMenu("Crazy Minnow Studio/SALSA/Addons/MCS/SALSA 1-Click MCS Setup")]
    public class CM_MCSSetup : MonoBehaviour
    {
		/// <summary>
		/// This initializes Setup when setting up characters at runtime
		/// </summary>
		void Awake()
		{
			Setup();
			Destroy(this);
		}

		/// <summary>
		/// Configures a complete SALSA with RandomEyes enabled MCS character
		/// </summary>
		public void Setup()
        {            
            GameObject activeObj; // Selected hierarchy object
            M3DCharacterManager charman;
            Salsa3D salsa3D; // Salsa3D
            RandomEyes3D reEyes; // RandomEyes3D for eye
            RandomEyes3D reShapes; // RandomEyes3D for custom shapes
            CM_MCSSync mcsSync; // CM_MCSSync
			Transform lEye = null;
			Transform rEye = null;
			Transform[] children;

			activeObj = this.gameObject;

            #region Get the M3DCharacterManager and add SALSA BlendShapes
            charman = activeObj.GetComponent<M3DCharacterManager>();
            if (charman)
            {
                Morph[] morphs = new Morph[18];
                morphs[0] = new Morph("eCTRLEyesClosed", 0, true, false);
                morphs[1] = new Morph("eCTRLvAA", 0, true, false);
                morphs[2] = new Morph("eCTRLvEE", 0, true, false);
                morphs[3] = new Morph("eCTRLvEH", 0, true, false);
                morphs[4] = new Morph("eCTRLvER", 0, true, false);
                morphs[5] = new Morph("eCTRLvF", 0, true, false);
                morphs[6] = new Morph("eCTRLvIH", 0, true, false);
                morphs[7] = new Morph("eCTRLvIY", 0, true, false);
                morphs[8] = new Morph("eCTRLvK", 0, true, false);
                morphs[9] = new Morph("eCTRLvL", 0, true, false);
                morphs[10] = new Morph("eCTRLvM", 0, true, false);
                morphs[11] = new Morph("eCTRLvOW", 0, true, false);
                morphs[12] = new Morph("eCTRLvS", 0, true, false);
	            morphs[13] = new Morph("eCTRLvSH", 0, true, false);
                morphs[14] = new Morph("eCTRLvT", 0, true, false);
                morphs[15] = new Morph("eCTRLvTH", 0, true, false);
                morphs[16] = new Morph("eCTRLvUW", 0, true, false);
                morphs[17] = new Morph("eCTRLvW", 0, true, false);                
                charman.coreMorphs.AttachMorphs(morphs);
                charman.SyncAllBlendShapes();

                #region Add and get components
                salsa3D = activeObj.AddComponent<Salsa3D>(); // Add/get Salsa3D
                reEyes = activeObj.AddComponent<RandomEyes3D>(); // Add/get reEyes
                reEyes.FindOrCreateEyePositionGizmo();
                children = activeObj.GetComponentsInChildren<Transform>();
                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i].name == "lEye") lEye = children[i];
                    if (children[i].name == "rEye") rEye = children[i];
                }
                if (lEye && rEye) // Position the RandomEyes_Eye_Position gizmo between the eyes
                {
                    reEyes.eyePosition.transform.position = ((lEye.position - rEye.position) * 0.5f) + rEye.position;
                }
                children = null;
                reShapes = activeObj.AddComponent<RandomEyes3D>(); // Add/get reShapes
                mcsSync = activeObj.AddComponent<CM_MCSSync>(); // Add/get CM_MCSSync                
                #endregion

                #region Set Salsa3D and RandomEyes3D component parameters
                salsa3D.saySmallTrigger = 0.0005f;
                salsa3D.sayMediumTrigger = 0.002f;
                salsa3D.sayLargeTrigger = 0.004f;
                salsa3D.SetRangeOfMotion(100f); // Set mouth range of motion
                salsa3D.blendSpeed = 12f; // Set blend speed

                salsa3D.audioSrc = activeObj.GetComponent<AudioSource>(); // Set the salsa3D.audioSrc
                if (salsa3D.audioSrc) salsa3D.audioSrc.playOnAwake = false; // Disable play on wake

                reEyes.SetRangeOfMotion(60f); // Set eye range of motion
                reEyes.SetBlinkDuration(0.03f); // Set blink duration
                reEyes.SetBlinkSpeed(30f); // Set blink speed
                reShapes.useCustomShapesOnly = true; // Set reShapes to custom shapes only
                reShapes.skinnedMeshRenderer = mcsSync.skinnedMeshRenderer; // Set the SkinnedMeshRenderer
                reShapes.noneShapeIndex = reShapes.RebuildCurrentCustomShapeList();
                reShapes.selectedCustomShape = reShapes.noneShapeIndex;
	            reShapes.randomCustomShapes = false;
                #endregion

                #region CM_MCSSync settings
	            mcsSync.Initialize();
                #endregion
            }
            else
            {
                Debug.LogError("The MCS M3DCharacterManager component could not be found. Please use the SALSA MCS 1-click setup on the character root.");
            }
            #endregion
        }
    }
}