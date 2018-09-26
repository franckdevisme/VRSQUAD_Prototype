using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MORPH3D;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.MCS
{
    /// <summary>
    /// This script acts as a proxy between SALSA with RandomEyes and MCS characters,
    /// and allows users to link SALSA with RandomEyes to MCS characters without any model
    /// modifications.
    /// 
    /// Good default inspector values
    /// Salsa3D
    /// 	Trigger values will depend on your recordings
    /// 	Blend Speed: 12
    /// 	Range of Motion: 75
    /// RandomEyes3D
    /// 	Range of Motion: 60
    /// 
    /// Crazy Minnow Studio, LLC
    /// CrazyMinnowStudio.com
    /// 
    /// NOTE: While every attempt has been made to ensure the safe content and operation of 
    /// these files, they are provided as-is, without warranty or guarantee of any kind. 
    /// By downloading and using these files you are accepting any and all risks associated 
    /// and release Crazy Minnow Studio, LLC of any and all liability.
    /// </summary>
    // [AddComponentMenu("Crazy Minnow Studio/SALSA/Addons/MCS/CM_MCSSync")]
	public class CM_MCSSync : MonoBehaviour 
	{
		public Salsa3D salsa3D; // Salsa3D mouth component
		public RandomEyes3D reEyes; // RandomEyes3D eye component
        public RandomEyes3D reExpression; // RandomEyes3D expression component
		public SkinnedMeshRenderer skinnedMeshRenderer; // MCS character SkinnedMeshRenderer
		public string leftEyeName = "lEye"; // Used in search for left eye bone
		public GameObject leftEyeBone; // Left eye bone
		public string rightEyeName = "rEye"; // Used in search for right eye bone
		public GameObject rightEyeBone; // Right eye bone
        public string blinkShape = "eCTRLEyesClosed";
        public int blinkIndex = -1;
		public string jawBoneName = "lowerJaw"; // Use in search for jaw bone
		public GameObject jawBone; // Jaw bone
		[Range(0f, 1f)]
		public float jawRangeOfMotion = 0.2f; // Total jaw range of motion
		public List<CM_ShapeGroup> saySmall = new List<CM_ShapeGroup>(); // saySmall shape group
		public List<CM_ShapeGroup> sayMedium = new List<CM_ShapeGroup>(); // sayMedium shape group
		public List<CM_ShapeGroup> sayLarge = new List<CM_ShapeGroup>(); // sayLarge shape group
		public string[] shapeNames; // Shape name string array for name picker popups
		public List<string> removeBlendSmrs = new List<string>();
        public enum AccuracyMode { Efficient, Accurate }
        public AccuracyMode accuracyMode = AccuracyMode.Efficient;
        public SkinnedMeshRenderer[] lods;
        public SkinnedMeshRenderer[] allSMRs;
        public int lodIndex;
		public float prevLod = -1f;
        public M3DCharacterManager charMan; // MCS character manager for LOD event hook
		
		private Transform[] children; // For searching through child objects during initialization
		private float eyeSensativity = 500f; // Eye movement reduction from shape value to bone transform value
        private float blinkWeight; // Blink weight is applied to the body Blink_Left and Blink_Right BlendShapes
		private float vertical; // Vertical eye bone movement amount
		private float horizontal; // Horizontal eye bone movement amount
		private Vector3 jawRest; // Rest local eular rotation
		private float jawAmount; // Current movement amount
        private float csAmount; // Custom Shape amount
        private string csName; // Custom Shape name
        private int csIndex; // Custom Shape index

        /// <summary>
        /// Unsubscribe from the MCS LOD change event
        /// </summary>
        private void OnDestroy()
        {
            if (charMan) charMan.OnPostLODChange -= CharMan_OnPostLODChange;
	        if (charMan) charMan.coreMorphs.OnPostMorph -= CharMan_OnPostMorph;
        }

        /// <summary>
        /// Reset the component to default values
        /// </summary>
        void Reset()
		{
			GetM3DCharacterManager();
			GetSalsa3D();
			GetRandomEyes3D();
			GetSmrs();
			GetEyeBones();
            GetBlinkIndexes();
			GetJawBone();
			saySmall = new List<CM_ShapeGroup>();
			sayMedium = new List<CM_ShapeGroup>();
			sayLarge = new List<CM_ShapeGroup>();
			removeBlendSmrs = new List<string>()
			{
				"hair",
				"ponytail"
			};
			GetShapeNames();
			SetSmall();
			SetMedium();
			SetLarge();
		}

        /// <summary>
        /// Initial setup
        /// </summary>
		void Start()
		{
            if (!charMan) GetM3DCharacterManager();
            if (charMan) charMan.OnPostLODChange += CharMan_OnPostLODChange;
			if (charMan) charMan.coreMorphs.OnPostMorph += CharMan_OnPostMorph;

			CharMan_OnPostMorph();
			
			GetSalsa3D();
			GetRandomEyes3D();
			GetSmrs();
			GetEyeBones();
            GetBlinkIndexes();
			GetJawBone();
			jawRest = jawBone.transform.localEulerAngles;
			jawAmount = jawRest.x;
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			VerifyIndexes();
		}

		/// <summary>
        /// MCS LOD change event remaps Salsa3D and RandomEyes3D
        /// </summary>
        /// <param name="level"></param>
        /// <param name="activeFigure"></param>
        /// <param name="figureChanged"></param>
        public void CharMan_OnPostLODChange(float level, SkinnedMeshRenderer activeFigure, bool figureChanged)
		{
			CharMan_OnPostMorph();
	        
            skinnedMeshRenderer = activeFigure;
            if (reExpression) reExpression.skinnedMeshRenderer = skinnedMeshRenderer;
        }
		
		/// <summary>
		/// Remove all BlendShapes from any smr that contains the names listed in removeBlendSmrs
		/// </summary>
		private void CharMan_OnPostMorph()
		{
			SkinnedMeshRenderer[] smrs;

			smrs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

			if (removeBlendSmrs.Count > 0)
			{
				for (int i = 0; i < removeBlendSmrs.Count; i++)
				{
					for (int j = 0; j < smrs.Length; j++)
					{				
						if (smrs[j].name.ToLower().Contains(removeBlendSmrs[i]))
						{
							if (smrs[j].sharedMesh.blendShapeCount > 0)
								smrs[j].sharedMesh.ClearBlendShapes();
						}
					}
				}
			}
		}

        /// <summary>
        /// Perform the blendshape changes in LateUpdate for mechanim compatibility
        /// </summary>
        void LateUpdate() 
		{
			if (salsa3D && skinnedMeshRenderer && salsa3D.lockShapes)
			{
				// Sync SALSA shapes
				for (int i=0; i<saySmall.Count; i++)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(
						saySmall[i].shapeIndex, ((saySmall[i].percentage/100)*salsa3D.sayAmount.saySmall));
				}
				for (int i=0; i<sayMedium.Count; i++)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(
						sayMedium[i].shapeIndex, ((sayMedium[i].percentage/100)*salsa3D.sayAmount.sayMedium));
				}			
				for (int i=0; i<sayLarge.Count; i++)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(
						sayLarge[i].shapeIndex, ((sayLarge[i].percentage/100)*salsa3D.sayAmount.sayLarge));
				}
			}

			// Sync Blink
			if (reEyes)
			{
				blinkWeight = reEyes.lookAmount.blink;

				// Apply blink action
				if (skinnedMeshRenderer)
				{
                    if (blinkIndex != -1)
                        skinnedMeshRenderer.SetBlendShapeWeight(blinkIndex, blinkWeight);
                    else
                        GetBlinkIndexes();
                }

				// Apply look amount to bone rotation
				if (leftEyeBone || rightEyeBone)
				{
					// Apply eye movement weight direction variables
					if (reEyes.lookAmount.lookUp > 0) 
						vertical = -(reEyes.lookAmount.lookUp / eyeSensativity) * reEyes.rangeOfMotion;
					if (reEyes.lookAmount.lookDown > 0) 
						vertical = (reEyes.lookAmount.lookDown / eyeSensativity) * reEyes.rangeOfMotion;
					if (reEyes.lookAmount.lookLeft > 0) 
						horizontal = -(reEyes.lookAmount.lookLeft / eyeSensativity) * reEyes.rangeOfMotion;
					if (reEyes.lookAmount.lookRight > 0) 
						horizontal = (reEyes.lookAmount.lookRight / eyeSensativity) * reEyes.rangeOfMotion;

					// Set eye bone rotations
					if (leftEyeBone) leftEyeBone.transform.localRotation = Quaternion.Euler(vertical, horizontal, 0);
					if (rightEyeBone) rightEyeBone.transform.localRotation = Quaternion.Euler(vertical, horizontal, 0);
				}
				
				// Sync jaw bone movement
				switch (salsa3D.sayIndex)
				{
					case 0: // sayRest
						jawAmount = Mathf.Lerp(jawAmount, jawRest.x, Time.deltaTime * salsa3D.blendSpeed);
						break;
					case 1: // saySmall
						jawAmount = Mathf.Lerp(jawAmount, jawRest.x + (jawRangeOfMotion*5), Time.deltaTime * salsa3D.blendSpeed);
						break;
					case 2: // sayMedium
						jawAmount = Mathf.Lerp(jawAmount, jawRest.x + (jawRangeOfMotion*10), Time.deltaTime * salsa3D.blendSpeed);
						break;
					case 3: // sayLarge
						jawAmount = Mathf.Lerp(jawAmount, jawRest.x + (jawRangeOfMotion*15), Time.deltaTime * salsa3D.blendSpeed);
						break;
				}
				if (jawBone)
				{
					jawBone.transform.localEulerAngles = new Vector3(jawAmount, jawRest.y, jawRest.z);
				}

                // Apply blend shape values to all SMR's
                if (skinnedMeshRenderer != null)
                {
                    // Loop custom shape smr
                    for (int cs=0; cs<skinnedMeshRenderer.sharedMesh.blendShapeCount; cs++)
                    {
                        csAmount = skinnedMeshRenderer.GetBlendShapeWeight(cs);
                        csName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(cs);

                        if (accuracyMode == AccuracyMode.Efficient)
                        {
                            if (csAmount > 0)
                            {
                                SyncShapes();
                            }
                        }
                        else // Accurate
                        {
                            SyncShapes();
                        }
                    }
                }
			}
		}

        /// <summary>
        /// Sync primary SMR shapes to all SMR's
        /// </summary>
        private void SyncShapes()
        {
            // Loop all SMR's
            for (int all = 0; all < allSMRs.Length; all++)
            {
                if (skinnedMeshRenderer != allSMRs[all])
                {
                    csIndex = -1;
                    csIndex = allSMRs[all].sharedMesh.GetBlendShapeIndex(csName);
                    if (csIndex != -1)
                    {
                        allSMRs[all].SetBlendShapeWeight(csIndex, csAmount);
                    }
                }
            }
        }

		/// <summary>
		/// Call this when initializing characters at runtime
		/// </summary>
		public void Initialize()
		{
			GetSalsa3D();
			GetRandomEyes3D();
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			if (removeBlendSmrs == null) removeBlendSmrs = new List<string>()
			{
				"hair",
				"ponytail"
			};
			VerifyIndexes();
            UpdateBlendShapes();
		}

		/// <summary>
		/// Verify all mapped BlendShape indexes by name and names by verified index
		/// </summary>
		public void VerifyIndexes()
		{
			int index = -1;
			GetShapeNames();
						
			if (saySmall.Count > 0)
			{
				for (int i = saySmall.Count-1; i >= 0; i--)
				{
					index = -1;
					index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(saySmall[i].shapeName);
					if (index != -1)
					{
						if (index != saySmall[i].shapeIndex)
						{
							saySmall[i].shapeIndex = index;
						}
					}
					else
					{
						saySmall.RemoveAt(i);
					}
				}
			}
			
			if (sayMedium.Count > 0)
			{
				for (int i = sayMedium.Count-1; i >= 0; i--)
				{
					index = -1;
					index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(sayMedium[i].shapeName);
					if (index != -1)
					{
						if (index != sayMedium[i].shapeIndex)
						{
							sayMedium[i].shapeIndex = index;
						}
					}
					else
					{
						sayMedium.RemoveAt(i);
					}
				}
			}
			
			if (sayLarge.Count > 0)
			{
				for (int i = sayLarge.Count-1; i >= 0; i--)
				{
					index = -1;
					index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(sayLarge[i].shapeName);
					if (index != -1)
					{
						if (index != sayLarge[i].shapeIndex)
						{
							sayLarge[i].shapeIndex = index;
						}
					}
					else
					{
						sayLarge.RemoveAt(i);
					}
				}
			}

			if (reExpression & salsa3D)
			{
				reExpression.GetBlendShapes();
				reExpression.AutoLinkCustomShapes(true, salsa3D);
			}
		}

		/// <summary>
		/// Find the M3DCharacterManager component
		/// </summary>
		public void GetM3DCharacterManager()
		{
			charMan = GetComponent<M3DCharacterManager>();
		}

        /// <summary>
        /// Update RandomEyes3D expression instance BlendShapes
        /// </summary>
        public void UpdateBlendShapes()
        {
            if (reExpression) reExpression.GetBlendShapes();
        }

        /// <summary>
        /// Get the Salsa3D component
        /// </summary>
        public void GetSalsa3D()
		{
			if (!salsa3D) salsa3D = GetComponent<Salsa3D>();
		}

		/// <summary>
		/// Get the RandomEyes3D component
		/// </summary>
		public void GetRandomEyes3D()
		{
			//if (!randomEyes3D) randomEyes3D = GetComponent<RandomEyes3D>();

            RandomEyes3D[] randomEyes = GetComponents<RandomEyes3D>();
            if (randomEyes.Length > 1)
            {
                for (int i = 0; i < randomEyes.Length; i++)
                {
                    // Get RandomEyes3D expression instance
                    if (randomEyes[i].useCustomShapesOnly)
                        reExpression = randomEyes[i];
                    else // Get RandomEyes3D eyes instance
                        reEyes = randomEyes[i];
                }
            }
		}

		/// <summary>
		/// Find the Body child object SkinnedMeshRenderer
		/// </summary>
		public void GetSmrs()
		{
            lods = new SkinnedMeshRenderer[4];
            lodIndex = -1;
			allSMRs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (allSMRs.Length > 0)
			{
				for (int i=0; i<allSMRs.Length; i++)
				{
                    if (allSMRs[i].gameObject.name.Contains("M3D") && allSMRs[i].gameObject.name.Contains("LOD0"))
                        lods[0] = allSMRs[i];

                    if (allSMRs[i].gameObject.name.Contains("M3D") && allSMRs[i].gameObject.name.Contains("LOD1"))
                        lods[1] = allSMRs[i];

                    if (allSMRs[i].gameObject.name.Contains("M3D") && allSMRs[i].gameObject.name.Contains("LOD2"))
                        lods[2] = allSMRs[i];

                    if (allSMRs[i].gameObject.name.Contains("M3D") && allSMRs[i].gameObject.name.Contains("LOD3"))
                        lods[3] = allSMRs[i];

                    if (allSMRs[i].gameObject.name.Contains("M3D") && allSMRs[i].enabled)
						lodIndex = i;												
				}
			}

			if (lodIndex != -1)
				skinnedMeshRenderer = allSMRs[lodIndex];
		}

		/// <summary>
		/// Find left and right eye bones
		/// </summary>
		public void GetEyeBones()
		{
			Transform leftEyeTrans = ChildSearch(leftEyeName);
			if(leftEyeTrans) 
			{
				if (!leftEyeBone) leftEyeBone = leftEyeTrans.gameObject;
			}
			Transform rightEyeTrans = ChildSearch(rightEyeName);
			if (rightEyeTrans) 
			{
				if (!rightEyeBone) rightEyeBone = rightEyeTrans.gameObject;
			}
		}
		
        /// <summary>
        /// Get blink indexes for multiple MCS BlendShape name variations
        /// </summary>
        public void GetBlinkIndexes()
        {
            if (skinnedMeshRenderer)
            {
                blinkIndex = ShapeSearch(skinnedMeshRenderer, blinkShape);
            }
        }
		
		/// <summary>
		/// Find the jaw bone
		/// </summary>
		public void GetJawBone()
		{
			Transform jawTrans = ChildSearch(jawBoneName);
			if (jawTrans)
			{
				if (!jawBone) jawBone = jawTrans.gameObject;
			}
		}

		/// <summary>
        /// Find a child by name that ends with the search string. 
        /// This should compensates for BlendShape name prefixes variations.
		/// </summary>
		/// <param name="endsWith"></param>
		/// <returns></returns>
		public Transform ChildSearch(string endsWith)
		{
			Transform trans = null;
			children = transform.GetChild(0).Find("hip").gameObject.GetComponentsInChildren<Transform>();

			for (int i=0; i<children.Length; i++)
			{
                if (children[i].name.EndsWith(endsWith)) trans = children[i];
			}

			return trans;
		}	

		/// <summary>
        /// Find a shape by name, that ends with the search string.
		/// </summary>
		/// <param name="skndMshRndr"></param>
		/// <param name="endsWith"></param>
		/// <returns></returns>
        public int ShapeSearch(SkinnedMeshRenderer skndMshRndr, string endsWith)
		{
			int index = -1;
			if (skndMshRndr)
			{
				for (int i=0; i<skndMshRndr.sharedMesh.blendShapeCount; i++)
				{
                    if (skndMshRndr.sharedMesh.GetBlendShapeName(i).EndsWith(endsWith))
					{
						index = i;
						break;
					}
				}
			}
			return index;
		}

		/// <summary>
		/// Populate the shapeName popup list
		/// </summary>
		public int GetShapeNames()
		{
			int nameCount = 0;

			if (skinnedMeshRenderer)
			{
				shapeNames = new string[skinnedMeshRenderer.sharedMesh.blendShapeCount];
				for (int i=0; i<skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
				{
					shapeNames[i] = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
					if (shapeNames[i] != "") nameCount++;
				}
			}

			return nameCount;
		}

		/// <summary>
		/// Set the MCS saySmall shape group
		/// </summary>
		public void SetSmall()
		{
			int index = -1;
			string name = "";

			saySmall = new List<CM_ShapeGroup>();

			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvER");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				saySmall.Add(new CM_ShapeGroup(index, name, 20f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvW");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				saySmall.Add(new CM_ShapeGroup(index, name, 40f));
			}
			
			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvOW");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				saySmall.Add(new CM_ShapeGroup(index, name, 20f));
			}
		}
		/// <summary>
        /// Set the MCS sayMedium shape group
		/// </summary>
        public void SetMedium()
		{
			int index = -1;;
			string name = "";

			sayMedium = new List<CM_ShapeGroup>();
			
			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvM");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 100f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvAA");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 10f));
			}
			
			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvEE");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 50f));
			}
			
			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvS");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 60f));
			}
			
			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvT");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 50f));
			}
		}
		/// <summary>
        /// Set the MCS sayLarge shape group
		/// </summary>
        public void SetLarge()
		{
			int index = -1;;
			string name = "";

			sayLarge = new List<CM_ShapeGroup>();

			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvTH");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayLarge.Add(new CM_ShapeGroup(index, name, 60f));
			}
			
			index = ShapeSearch(skinnedMeshRenderer, "eCTRLvUW");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayLarge.Add(new CM_ShapeGroup(index, name, 100f));
			}
		}
	}

	/// <summary>
	/// Shape index and percentage class for SALSA/MCS shape groups
	/// </summary>
	[System.Serializable]
	public class CM_ShapeGroup
	{
		public int shapeIndex;
		public string shapeName;
		public float percentage;

		public CM_ShapeGroup()
		{
			this.shapeIndex = 0;
			this.shapeName = "";
			this.percentage = 100f;
		}

		public CM_ShapeGroup(int shapeIndex, string shapeName, float percentage)
		{
			this.shapeIndex = shapeIndex;
			this.shapeName = shapeName;
			this.percentage = percentage;
		}
	}
}
