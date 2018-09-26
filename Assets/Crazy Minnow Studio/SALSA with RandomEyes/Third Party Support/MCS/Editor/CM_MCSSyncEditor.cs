using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.MCS
{
    /// <summary>
    /// This is the custom inspector for CM_MCSSync, a script that acts as a proxy between 
    /// SALSA with RandomEyes and MCS characters, and allows users to link SALSA with 
    /// RandomEyes to MCS characters without any model modifications.
    /// 
    /// Crazy Minnow Studio, LLC
    /// CrazyMinnowStudio.com
    /// 
    /// NOTE:While every attempt has been made to ensure the safe content and operation of 
    /// these files, they are provided as-is, without warranty or guarantee of any kind. 
    /// By downloading and using these files you are accepting any and all risks associated 
    /// and release Crazy Minnow Studio, LLC of any and all liability.
    [CustomEditor(typeof(CM_MCSSync)), CanEditMultipleObjects]
	public class CM_MCSSyncEditor : Editor 
	{
		private CM_MCSSync mcsSync; // CM_MCSSync reference
		private bool dirtySmall; // SaySmall dirty inspector status
		private bool dirtyMedium; // SayMedum dirty inspector status
		private bool dirtyLarge; // SayLarge dirty inspector status

		private float width = 0f; // Inspector width
		private float addWidth = 10f; // Percentage
		private float deleteWidth = 10f; // Percentage
		private float shapeNameWidth = 50f; // Percentage
		private float percentageWidth = 40f; // Percentage

		public void OnEnable()
		{
			// Get reference
			mcsSync = target as CM_MCSSync;
		    mcsSync.VerifyIndexes();
		}

		public override void OnInspectorGUI()
        {
            // Minus 45 width for the scroll bar
            width = Screen.width - 75f;

            // Set dirty flags
            dirtySmall = false;
            dirtyMedium = false;
            dirtyLarge = false;

            if (mcsSync.charMan == null)
                mcsSync.GetM3DCharacterManager();

            if (mcsSync.lods.Length == 0)
                mcsSync.GetSmrs();

            if (mcsSync.charMan != null)
            {
                if (mcsSync.charMan.currentLODLevel <= 0.125f && mcsSync.lodIndex != 3)
                    mcsSync.lodIndex = 3;

                if (mcsSync.charMan.currentLODLevel >= 0.126f && 
                    mcsSync.charMan.currentLODLevel <=0.25f && mcsSync.lodIndex != 2)
                    mcsSync.lodIndex = 2;
                
                if (mcsSync.charMan.currentLODLevel >= 0.251f && 
                    mcsSync.charMan.currentLODLevel <=0.5f && mcsSync.lodIndex != 1)
                    mcsSync.lodIndex = 1;

                if (mcsSync.charMan.currentLODLevel >= 0.501f && mcsSync.lodIndex != 0)
                    mcsSync.lodIndex = 0;

                if (mcsSync.skinnedMeshRenderer != mcsSync.lods[mcsSync.lodIndex])
                    mcsSync.CharMan_OnPostLODChange(mcsSync.charMan.currentLODLevel, mcsSync.lods[mcsSync.lodIndex], true);
            }

            // Keep trying to get the shapeNames until I've got them
            if (mcsSync.GetShapeNames() == 0) mcsSync.GetShapeNames();

            // Make sure the CM_ShapeGroups are initialized
            if (mcsSync.saySmall == null) mcsSync.saySmall = new System.Collections.Generic.List<CM_ShapeGroup>();
            if (mcsSync.sayMedium == null) mcsSync.sayMedium = new System.Collections.Generic.List<CM_ShapeGroup>();
            if (mcsSync.sayLarge == null) mcsSync.sayLarge = new System.Collections.Generic.List<CM_ShapeGroup>();

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
            {
                mcsSync.salsa3D = EditorGUILayout.ObjectField(
                    "Salsa3D", mcsSync.salsa3D, typeof(Salsa3D), true) as Salsa3D;
                mcsSync.reEyes = EditorGUILayout.ObjectField(
                    new GUIContent("RandomEyes3D (Eyes)", "The RandomEyes3D instance for controlling eye movement."),
                    mcsSync.reEyes, typeof(RandomEyes3D), true) as RandomEyes3D;
                mcsSync.reExpression = EditorGUILayout.ObjectField(
                    new GUIContent("RandomEyes3D (Expressions)", "The RandomEyes3D instance for controlling expressions."),
                    mcsSync.reExpression, typeof(RandomEyes3D), true) as RandomEyes3D;
                mcsSync.skinnedMeshRenderer = EditorGUILayout.ObjectField(
                    new GUIContent("SkinnedMeshRenderer", "The SkinnedMeshRenderer child object that contains all the BlendShapes"),
                    mcsSync.skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
                mcsSync.blinkShape = EditorGUILayout.TextField(
                    new GUIContent("Blink Shape Name", "Blink shape used for eye blinking"), mcsSync.blinkShape);
                mcsSync.leftEyeBone = EditorGUILayout.ObjectField(
                    "Left Eye Bone", mcsSync.leftEyeBone, typeof(GameObject), true) as GameObject;
                mcsSync.rightEyeBone = EditorGUILayout.ObjectField(
                    "Right Eye Bone", mcsSync.rightEyeBone, typeof(GameObject), true) as GameObject;
                mcsSync.jawBone = EditorGUILayout.ObjectField(
                    "Jaw Bone", mcsSync.jawBone, typeof(GameObject), true) as GameObject;
                mcsSync.jawRangeOfMotion = EditorGUILayout.Slider(
                    "Jaw Range of Motion", mcsSync.jawRangeOfMotion, 0f, 1f);
                mcsSync.accuracyMode = (CM_MCSSync.AccuracyMode)EditorGUILayout.EnumPopup(
                    new GUIContent("Sync Accuracy", 
                    "Efficient - Fast, less accurate\n" +
                    "Accurate - Slow, more accurate"),
                    mcsSync.accuracyMode);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.HelpBox(
                    "Any SkinnedMeshRenderer that contains these names will have their BlendShapes removed on Start. " +
                    "This is non-destructive since the removal is not serialized or saved.", MessageType.Info);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(new GUIContent("Remove BlendShapes", "Remove Blends from SMR's that contain these names."));
                    if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
                    {
                        mcsSync.removeBlendSmrs.Add("");
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (mcsSync.removeBlendSmrs.Count > 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(
                            new GUIContent("Del", "Remove SMR contains string"),
                            GUILayout.Width((deleteWidth / 100) * width));
                        EditorGUILayout.LabelField(
                            new GUIContent("SMR Contains Name", "SMR's contain these names"),
                            GUILayout.Width((shapeNameWidth / 100) * width));
                    }
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < mcsSync.removeBlendSmrs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(
                                new GUIContent("X", "Remove this contains name"),
                                GUILayout.Width((deleteWidth / 100) * width)))
                            {
                                mcsSync.removeBlendSmrs.RemoveAt(i);
                                break;
                            }

                            mcsSync.removeBlendSmrs[i] = EditorGUILayout.TextField(mcsSync.removeBlendSmrs[i]);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(20);

            if (mcsSync.skinnedMeshRenderer)
            {
                if (mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount > 0)
                {
                    EditorGUILayout.LabelField("SALSA shape groups");

                    GUILayout.Space(10);

                    if (GUILayout.Button(new GUIContent("Remap BlendShape Indexes", "Remap indexes after MCS BlendShape changes.")))
                    {
                        mcsSync.Initialize();
                        if (EditorUtility.DisplayDialog("Remap Complete", "Indexes have been remapped.", "OK")) { }
                    }

                    GUILayout.Space(10);

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SaySmall Shapes");
                            if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
                            {
                                mcsSync.saySmall.Add(new CM_ShapeGroup());
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (mcsSync.saySmall.Count > 0)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("Del", "Remove shape"),
                                    GUILayout.Width((deleteWidth / 100) * width));
                                EditorGUILayout.LabelField(
                                    new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                    GUILayout.Width((shapeNameWidth / 100) * width));
                                EditorGUILayout.LabelField(
                                    new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
                                    GUILayout.Width((percentageWidth / 100) * width));
                            }
                            GUILayout.EndHorizontal();

                            for (int i = 0; i < mcsSync.saySmall.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button(
                                        new GUIContent("X", "Remove this shape from the list (index:" + mcsSync.saySmall[i].shapeIndex + ")"),
                                        GUILayout.Width((deleteWidth / 100) * width)))
                                    {
                                        mcsSync.saySmall.RemoveAt(i);
                                        dirtySmall = true;
                                        break;
                                    }
                                    if (!dirtySmall)
                                    {
                                        mcsSync.saySmall[i].shapeIndex = EditorGUILayout.Popup(
                                            mcsSync.saySmall[i].shapeIndex, mcsSync.shapeNames,
                                            GUILayout.Width((shapeNameWidth / 100) * width));

                                        if (mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount > 0)
                                        {
                                            if (mcsSync.saySmall[i].shapeIndex < mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount)
                                            {
                                                mcsSync.saySmall[i].shapeName =
                                                    mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.saySmall[i].shapeIndex);
                                            }
                                            else
                                            {
                                                mcsSync.Initialize();
                                                Debug.LogWarning("(CM_MCSSync) BlendShapes indexes for the " +
                                                    "SkinnedMeshRenderer (" + mcsSync.skinnedMeshRenderer.name + ") " +
                                                    "have changed. BlendShapes have been remapped in the inspector.");
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogError("(CM_MCSSync) The SkinnedMeshRenderer " +
                                                "(" + mcsSync.skinnedMeshRenderer.name + ") has no BlendShapes.");
                                            break;
                                        }

                                        mcsSync.saySmall[i].percentage = EditorGUILayout.Slider(
                                            mcsSync.saySmall[i].percentage, 0f, 100f,
                                            GUILayout.Width((percentageWidth / 100) * width));
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(10);

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SayMedium Shapes");
                            if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
                            {
                                mcsSync.sayMedium.Add(new CM_ShapeGroup());
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (mcsSync.sayMedium.Count > 0)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("Del", "Remove shape"),
                                    GUILayout.Width((deleteWidth / 100) * width));
                                EditorGUILayout.LabelField(
                                    new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                    GUILayout.Width((shapeNameWidth / 100) * width));
                                EditorGUILayout.LabelField(
                                    new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
                                    GUILayout.Width((percentageWidth / 100) * width));
                            }
                            GUILayout.EndHorizontal();

                            for (int i = 0; i < mcsSync.sayMedium.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button(
                                        new GUIContent("X", "Remove this shape from the list (index:" + mcsSync.sayMedium[i].shapeIndex + ")"),
                                        GUILayout.Width((deleteWidth / 100) * width)))
                                    {
                                        mcsSync.sayMedium.RemoveAt(i);
                                        dirtyMedium = true;
                                        break;
                                    }
                                    if (!dirtyMedium)
                                    {
                                        mcsSync.sayMedium[i].shapeIndex = EditorGUILayout.Popup(
                                            mcsSync.sayMedium[i].shapeIndex, mcsSync.shapeNames,
                                            GUILayout.Width((shapeNameWidth / 100) * width));

                                        if (mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount > 0)
                                        {
                                            if (mcsSync.sayMedium[i].shapeIndex < mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount)
                                            {
                                                mcsSync.sayMedium[i].shapeName =
                                                    mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.sayMedium[i].shapeIndex);
                                            }
                                            else
                                            {
                                                mcsSync.Initialize();
                                                Debug.LogWarning("(CM_MCSSync) BlendShapes indexes for the " +
                                                    "SkinnedMeshRenderer (" + mcsSync.skinnedMeshRenderer.name + ") " +
                                                    "have changed. BlendShapes have been remapped in the inspector.");
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogError("(CM_MCSSync) The SkinnedMeshRenderer " +
                                                "(" + mcsSync.skinnedMeshRenderer.name + ") has no BlendShapes.");
                                            break;
                                        }

                                        mcsSync.sayMedium[i].percentage = EditorGUILayout.Slider(
                                            mcsSync.sayMedium[i].percentage, 0f, 100f,
                                            GUILayout.Width((percentageWidth / 100) * width));
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(10);

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SayLarge Shapes");
                            if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
                            {
                                mcsSync.sayLarge.Add(new CM_ShapeGroup());
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (mcsSync.sayLarge.Count > 0)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("Del", "Remove shape"),
                                    GUILayout.Width((deleteWidth / 100) * width));
                                EditorGUILayout.LabelField(
                                    new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                    GUILayout.Width((shapeNameWidth / 100) * width));
                                EditorGUILayout.LabelField(
                                    new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
                                    GUILayout.Width((percentageWidth / 100) * width));
                            }
                            GUILayout.EndHorizontal();

                            for (int i = 0; i < mcsSync.sayLarge.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button(
                                        new GUIContent("X", "Remove this shape from the list (index:" + mcsSync.sayLarge[i].shapeIndex + ")"),
                                        GUILayout.Width((deleteWidth / 100) * width)))
                                    {
                                        mcsSync.sayLarge.RemoveAt(i);
                                        dirtyLarge = true;
                                        break;
                                    }
                                    if (!dirtyLarge)
                                    {
                                        mcsSync.sayLarge[i].shapeIndex = EditorGUILayout.Popup(
                                            mcsSync.sayLarge[i].shapeIndex, mcsSync.shapeNames,
                                            GUILayout.Width((shapeNameWidth / 100) * width));
                                        if (mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount > 0)
                                        {
                                            if (mcsSync.sayLarge[i].shapeIndex < mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount)
                                            {
                                                mcsSync.sayLarge[i].shapeName =
                                                    mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.sayLarge[i].shapeIndex);
                                            }
                                            else
                                            {
                                                mcsSync.Initialize();
                                                Debug.LogWarning("(CM_MCSSync) BlendShapes indexes for the " +
                                                    "SkinnedMeshRenderer (" + mcsSync.skinnedMeshRenderer.name + ") " +
                                                    "have changed. BlendShapes have been remapped in the inspector.");
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogError("(CM_MCSSync) The SkinnedMeshRenderer " +
                                                "(" + mcsSync.skinnedMeshRenderer.name + ") has no BlendShapes.");
                                            break;
                                        }

                                        mcsSync.sayLarge[i].percentage = EditorGUILayout.Slider(
                                            mcsSync.sayLarge[i].percentage, 0f, 100f,
                                            GUILayout.Width((percentageWidth / 100) * width));
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "The selected SkinnedMeshRenderer component has no BlendShapes.", 
                        MessageType.Error);
                }
            }
        }
	}
}