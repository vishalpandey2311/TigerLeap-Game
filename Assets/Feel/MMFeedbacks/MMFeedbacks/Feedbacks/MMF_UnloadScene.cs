﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.Feedbacks
{
	/// <summary>
	/// This feedback lets you unload a scene by name or build index
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you unload a scene by name or build index")]
	[MovedFrom(false, null, "MoreMountains.Feedbacks")]
	[FeedbackPath("Scene/Unload Scene")]
	public class MMF_UnloadScene : MMF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		public enum ColorModes { Instant, Gradient, Interpolate }

		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SceneColor; } }

		public override bool EvaluateRequiresSetup()
		{
			if (Method == Methods.BuildIndex)
			{
				return false;
                
			}
			else if (Method == Methods.SceneName)
			{
				return ((SceneName == null) || (SceneName == ""));
			}
			return false;
		}
		public override string RequiredTargetText { get { return SceneName;  } }
		public override string RequiresSetupText { get { return "This feedback requires that you specify a SceneName below. Make sure you also add that destination scene to your Build Settings."; } }
		#endif
        
		public enum Methods { BuildIndex, SceneName }

		[MMFInspectorGroup("Unload Scene", true, 43, false)]
        
		/// whether to unload a scene by build index or by name
		[Tooltip("whether to unload a scene by build index or by name")]
		public Methods Method = Methods.SceneName;

		/// the build ID of the scene to unload, find it in your Build Settings
		[Tooltip("the build ID of the scene to unload, find it in your Build Settings")]
		[MMFEnumCondition("Method", (int)Methods.BuildIndex)]
		public int BuildIndex = 0;

		/// the name of the scene to unload
		[Tooltip("the name of the scene to unload")]
		[MMFEnumCondition("Method", (int)Methods.SceneName)]
		public string SceneName = "";

        
		/// whether or not to output warnings if the scene doesn't exist or can't be loaded
		[Tooltip("whether or not to output warnings if the scene doesn't exist or can't be loaded")]
		public bool OutputWarningsIfNeeded = true;
        
		protected Scene _sceneToUnload;

		/// <summary>
		/// On play we change the text of our target TMPText
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}

			if (Method == Methods.BuildIndex)
			{
				_sceneToUnload = SceneManager.GetSceneByBuildIndex(BuildIndex);
			}
			else
			{
				_sceneToUnload = SceneManager.GetSceneByName(SceneName);
			}

			if ((_sceneToUnload != null) && (_sceneToUnload.isLoaded))
			{
				SceneManager.UnloadSceneAsync(_sceneToUnload);    
			}
			else
			{
				if (OutputWarningsIfNeeded)
				{
					Debug.LogWarning("[Unload Scene Feedback] The unload scene feedback on "+Owner.name+" is trying to unload a scene that hasn't been loaded.");   
				}
			}
		}
	}
}