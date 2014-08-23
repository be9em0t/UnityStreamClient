///<summary>
/// XsLiveAnimator is a component to bind Xsens MVN Studio stream to Unity3D Game Engine.
/// MVN Studio capable to stream 4 actors at the same time and this component makes the 
/// connections between those actors and the characters in Unity3D.
/// 
/// Using the same settings on different characters will result of multiple characters are playing the same animation.
/// 
/// Relocation of the animation based on the start position of the character.
/// 
/// This component uses Macanim own animator to bind the bones with MVN avatar and the real model in Unity3D.
/// 
/// The animation applied on the pelvis as global position and rotation, while only the local rotation applied on each segments.
///</summary>
/// <version>
/// Version 1.0, 2013.04.11 by Attila Odry
/// </version>
///<remarks>
///
///		Copyright (c) Xsens Technologies B.V., 2006-2012. All rights reserved.
/// 
///     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
///     KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
///     IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
///     PARTICULAR PURPOSE.
/// 
///</remarks>

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace xsens
{
	
	/// <summary>
	/// Xsens Live Animator.
	/// 
	/// This class provide the logic to read an actor pose from MVN Stream and 
 	/// retarget the animation to a character.
	/// </summary>
	/// <remarks>
	/// Attach this component to the character and bind the MvnActors with this object.
	/// </remarks>
	public class XsLiveAnimator : MonoBehaviour {
		
		public XsStreamReader mvnActors;			//network streamer, which contains all 4 actor's poses
		public int actorID = 1;						//current actor ID, where 1 is the first streamed character from MVN
		
		private Transform mvnActor; 				//reference for MVN Actor. This has the same layout as the streamed pose.
		private Transform target;                	// Reference to the character in Unity3D.
		private Transform origPos;					//original position of the animation, this is the zero
		private const int NUM_SEGMENTS = 23;		//number of segments in MVN animation
		private Transform[] targetModel;			//Model holds each segments
		private Transform[] currentPose;			//animation applyed on skeleton. global position and rotation, used to represent a pose	
		private Quaternion[] modelRotTP;			//T-Pose reference rotation on each segment
		private Vector3[] modelPosTP;				//T-Pose reference position on each segment
		private Vector3 pelvisPosTP; 				//T-Pose's pos for the pelvis
		private float scale = 0;
		private GameObject missingSegments;			//Empty object for not used segments
		private Animator animator;					//Animator object to get the Humanoid character mapping correct.
		private Dictionary<XsAnimationSegment, HumanBodyBones> macanimBones;
		private bool isInited;						//flag to check if the plugin was correctly intialized.
	  
		/// <summary>
		/// Contains the segment numbers for the animation
		/// </summary>
	    public enum XsAnimationSegment
	    {
	        Pelvis = 0, //hips
			
			RightUpperLeg = 1,
			RightLowerLeg = 2,	
			RightFoot = 3,	
			RightToe = 4,
	        //--
			LeftUpperLeg = 5,
			LeftLowerLeg = 6,
			LeftFoot = 7,
	        LeftToe = 8,
			
			L5 = 9,		//not used
	        L3 = 10,	//spine
	        T12 = 11,	//not used
	        T8 = 12,	//chest
			
	        LeftShoulder = 13,
	        LeftUpperArm = 14,
	        LeftLowerArm = 15,
	        LeftHand = 16,
			
	        RightShoulder = 17,
	        RightUpperArm = 18,
	        RightLowerArm = 19,
	        RightHand = 20,
	
	        Neck = 21,
	        Head = 22
			
	    }   
		
		/// <summary>
		/// The segment order.
		/// </summary>
		int[] segmentOrder =
		{
					(int)XsAnimationSegment.Pelvis,
					(int)XsAnimationSegment.RightUpperLeg,
					(int)XsAnimationSegment.RightLowerLeg,
					(int)XsAnimationSegment.RightFoot,
					(int)XsAnimationSegment.RightToe,
					(int)XsAnimationSegment.LeftUpperLeg,
					(int)XsAnimationSegment.LeftLowerLeg,
					(int)XsAnimationSegment.LeftFoot,
					(int)XsAnimationSegment.LeftToe,
			
					(int)XsAnimationSegment.L5,
					(int)XsAnimationSegment.L3,
					(int)XsAnimationSegment.T12,
					(int)XsAnimationSegment.T8,
			
					(int)XsAnimationSegment.LeftShoulder,
					(int)XsAnimationSegment.LeftUpperArm,
					(int)XsAnimationSegment.LeftLowerArm,
					(int)XsAnimationSegment.LeftHand,
					
					(int)XsAnimationSegment.RightShoulder,
					(int)XsAnimationSegment.RightUpperArm,
					(int)XsAnimationSegment.RightLowerArm,
					(int)XsAnimationSegment.RightHand,
					(int)XsAnimationSegment.Neck,
					(int)XsAnimationSegment.Head
	
		};
				
		/// <summary>
		/// Maps the macanim bones.
		/// </summary>
		protected void mapMacanimBones()
		{
			macanimBones = new Dictionary<XsAnimationSegment, HumanBodyBones>();
			
			macanimBones.Add(XsAnimationSegment.Pelvis,			HumanBodyBones.Hips);
			macanimBones.Add(XsAnimationSegment.LeftUpperLeg,	HumanBodyBones.LeftUpperLeg);
			macanimBones.Add(XsAnimationSegment.LeftLowerLeg,	HumanBodyBones.LeftLowerLeg);
			macanimBones.Add(XsAnimationSegment.LeftFoot,		HumanBodyBones.LeftFoot);
			macanimBones.Add(XsAnimationSegment.LeftToe,		HumanBodyBones.LeftToes);
			macanimBones.Add(XsAnimationSegment.RightUpperLeg,	HumanBodyBones.RightUpperLeg);
			macanimBones.Add(XsAnimationSegment.RightLowerLeg,	HumanBodyBones.RightLowerLeg);
			macanimBones.Add(XsAnimationSegment.RightFoot,		HumanBodyBones.RightFoot);
			macanimBones.Add(XsAnimationSegment.RightToe,		HumanBodyBones.RightToes);
			macanimBones.Add(XsAnimationSegment.L5,				HumanBodyBones.LastBone);	//not used
			macanimBones.Add(XsAnimationSegment.L3,				HumanBodyBones.Spine);
			macanimBones.Add(XsAnimationSegment.T12,			HumanBodyBones.LastBone);	//not used
			macanimBones.Add(XsAnimationSegment.T8,				HumanBodyBones.Chest);
			macanimBones.Add(XsAnimationSegment.LeftShoulder,	HumanBodyBones.LeftShoulder);
			macanimBones.Add(XsAnimationSegment.LeftUpperArm,	HumanBodyBones.LeftUpperArm);
			macanimBones.Add(XsAnimationSegment.LeftLowerArm,	HumanBodyBones.LeftLowerArm);
			macanimBones.Add(XsAnimationSegment.LeftHand,		HumanBodyBones.LeftHand);
			macanimBones.Add(XsAnimationSegment.RightShoulder,	HumanBodyBones.RightShoulder);
			macanimBones.Add(XsAnimationSegment.RightUpperArm,	HumanBodyBones.RightUpperArm);
			macanimBones.Add(XsAnimationSegment.RightLowerArm,	HumanBodyBones.RightLowerArm);
			macanimBones.Add(XsAnimationSegment.RightHand,		HumanBodyBones.RightHand);
			macanimBones.Add(XsAnimationSegment.Neck,			HumanBodyBones.Neck);
			macanimBones.Add(XsAnimationSegment.Head,			HumanBodyBones.Head);
		}
		
		/// <summary>
		/// Awake this instance and initialize the live objects.
		/// </summary>
		void Awake()		
	    {
			
			isInited = false;
			
			//save start positions
			target = gameObject.transform;
			origPos = target;
			
			//create an MvnActor 
			GameObject obj = (GameObject)Instantiate(Resources.Load("MvnActor"));
			obj.transform.parent = gameObject.transform;
			mvnActor = obj.transform;
			if(!mvnActor)
			{
				Debug.LogError("No AnimationSkeleton found!");
				return;
			}
	
			// Search for the network stream, so we can communicate with it.
			if(!mvnActors)
			{
				Debug.LogError("No MvnActor found! You must assing an MvnActors to this component.");
				return;
			}
			
	        try
	        {
	
				//setup arrays for pose's
		        targetModel    	= new Transform[NUM_SEGMENTS];
				modelRotTP		= new Quaternion[NUM_SEGMENTS];		
				modelPosTP		= new Vector3[NUM_SEGMENTS]; 
				currentPose	    = new Transform[NUM_SEGMENTS];
				
				//add an empty object, which we can use for missing segments
				missingSegments = new GameObject("MissingSegments");
				missingSegments.transform.parent = gameObject.transform;
				
				//map each bone with xsens bipad model and macanim bones
				mapMacanimBones();
				//setup the animation and the model as well
				if( !setupMvnActor() )
				{
					Debug.Log ("failed to init MvnActor");
					return;
				}
				
            	if(!setupModel(target, targetModel))
				{
					Debug.Log ("failed to init the model");
					return;
				}
				
				//calculate simple scale factor
				scale = modelPosTP[(int)XsAnimationSegment.Pelvis].y / pelvisPosTP.y;
				
				//face model to the right direction	
				target.transform.rotation = transform.rotation;

				isInited =  true;
	        }
	        catch (Exception e)
	        {
	            print("Something went wrong setting up.");
				Debug.LogException(e);
	        }
	
		}
		
	
		/// <summary>
		/// Setups the mvn actor, with binding the currentPose to the actor bones.
		/// </summary>
		/// <returns>
		/// true on success
		/// </returns>
		public bool setupMvnActor()
		{
			
			mvnActor.rotation = transform.rotation;
			mvnActor.position = transform.position;
			
			currentPose[(int)XsAnimationSegment.Pelvis] = mvnActor.Find("Pelvis");
			currentPose[(int)XsAnimationSegment.L5] = mvnActor.Find("Pelvis/L5");
					
			currentPose[(int)XsAnimationSegment.L3] = mvnActor.Find("Pelvis/L5/L3");
			currentPose[(int)XsAnimationSegment.T12] = mvnActor.Find("Pelvis/L5/L3/T12");
			currentPose[(int)XsAnimationSegment.T8] = mvnActor.Find("Pelvis/L5/L3/T12/T8");
			currentPose[(int)XsAnimationSegment.LeftShoulder] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder");
			currentPose[(int)XsAnimationSegment.LeftUpperArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm");
			currentPose[(int)XsAnimationSegment.LeftLowerArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm/LeftLowerArm");
			currentPose[(int)XsAnimationSegment.LeftHand] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm/LeftLowerArm/LeftHand");
			
			currentPose[(int)XsAnimationSegment.Neck] = mvnActor.Find("Pelvis/L5/L3/T12/T8/Neck");
			currentPose[(int)XsAnimationSegment.Head] = mvnActor.Find("Pelvis/L5/L3/T12/T8/Neck/Head");
			
			currentPose[(int)XsAnimationSegment.RightShoulder] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder");
			currentPose[(int)XsAnimationSegment.RightUpperArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm");
			currentPose[(int)XsAnimationSegment.RightLowerArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm/RightLowerArm");
			currentPose[(int)XsAnimationSegment.RightHand] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm/RightLowerArm/RightHand");		
			
			currentPose[(int)XsAnimationSegment.LeftUpperLeg] = mvnActor.Find("Pelvis/LeftUpperLeg");
			currentPose[(int)XsAnimationSegment.LeftLowerLeg] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg");
			currentPose[(int)XsAnimationSegment.LeftFoot] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg/LeftFoot");
			currentPose[(int)XsAnimationSegment.LeftToe] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg/LeftFoot/LeftToe");
			currentPose[(int)XsAnimationSegment.RightUpperLeg] = mvnActor.Find("Pelvis/RightUpperLeg");
			currentPose[(int)XsAnimationSegment.RightLowerLeg] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg");
			currentPose[(int)XsAnimationSegment.RightFoot] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg/RightFoot");
			currentPose[(int)XsAnimationSegment.RightToe] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg/RightFoot/RightToe");
			
			//reset pelvis TP
			pelvisPosTP = currentPose[(int)XsAnimationSegment.Pelvis].position;
			
			return true;
			
		}

		/// <summary>
		/// Setups the model.
		/// Bind the model with the animation.
		/// This funciton will use Macanim animator to find the right bones, 
		/// then it will store it in an array by animation segment id
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		/// <param name='modelRef'>
		/// Model reference.
		/// </param>
		/// <returns>
		/// true on success
		/// </returns>
		public bool setupModel(Transform model, Transform[] modelRef)
		{
			
			animator = model.GetComponent("Animator") as Animator;
			if(!animator)
			{
				Debug.LogError("No Animator found for the model!");
				return false;
			}
			
			//face the imput model same as our animation
			model.rotation = transform.rotation;
			model.position = transform.position;
	
			//go through the model's segments and store values
			for(int i = 0; i < NUM_SEGMENTS; i++)
			{
				
				XsAnimationSegment segID = (XsAnimationSegment)segmentOrder[i];
				HumanBodyBones boneID = macanimBones[(XsAnimationSegment)segmentOrder[i]];
				
				try
				{	
					
					if(boneID != HumanBodyBones.LastBone)
					{
						//used bones
						modelRef[(int)segID] = animator.GetBoneTransform(boneID);
						modelPosTP[(int)segID] = modelRef[(int)segID].position;
						modelRotTP[(int)segID] = modelRef[(int)segID].rotation;
					}
					else
					{
						//not used bones
						modelRef[(int)segID] = null;
						modelPosTP[(int)segID] = Vector3.zero;
						modelRotTP[(int)segID] = Quaternion.Euler(Vector3.zero);
	
					}
				}
				catch(Exception e)
				{
					Debug.Log("Can't find "+boneID+" in the model!");
					modelRef[(int)segID] = null;
					modelPosTP[(int)segID] = Vector3.zero;
					modelRotTP[(int)segID] = Quaternion.Euler(Vector3.zero);

					//TODO IVO CHECK BONES
					//return false;
					return true;
				}
				
			}
			
			return true;
			
		}
		
	  
	    /* Update the body segments in every frame.
	     * 
	     * The mvn actor's segment orientations and positions is read from the network,
	     * using the MvnLiveActor component. 
	     * This component provides all data for the current pose for all actors.
	     */	
	    /// <summary>
		///	Update the body segments in every frame.
	    ///
	    /// The mvn actor's segment orientations and positions is read from the network,
	    /// using the MvnLiveActor component. 
	    /// This component provides all data for the current pose for all actors.
	    /// </summary>
		void Update()
	    {
			//only do magic if we have everything ready
			if(!isInited)
				return;

			//update rotation		
			updateMvnActor(currentPose, mvnActors.getLatestPositions(actorID), mvnActors.getLatestOrientations(actorID));		
			updateModel(currentPose, targetModel);
		
	    }
		
		/// <summary>
		/// Updates the mvn actor segment orientations and positions.
		/// </summary>
		/// <param name='model'>
		/// Model to update.
		/// </param>
		/// <param name='positions'>
		/// Positions in array
		/// </param>
		/// <param name='orientations'>
		/// Orientations in array
		/// </param>
		private void updateMvnActor(Transform[] model, Vector3[] positions, Quaternion[] orientations)
	    {
				
	        try
	        {
				
	            for (int i = 0; i < segmentOrder.Length; i++)	//front
	            {				
					if(XsAnimationSegment.Pelvis == (XsAnimationSegment)segmentOrder[i])
					{
						
						//we apply global position and orientaion to the pelvis
						model[segmentOrder[i]].transform.position = positions[segmentOrder[i]] + origPos.position;
						model[segmentOrder[i]].transform.rotation = orientations[segmentOrder[i]];
					}					
					else
					{
						//segment's data in local orientation (no position)
						if(model[segmentOrder[i]] == null)
						{
							Debug.LogError("Missing bone from mvn actor! Did you change MvnLive plugin? Please check if you are using the right actor!");
							break;
								
						}
						model[segmentOrder[i]].transform.localRotation = orientations[segmentOrder[i]];				
					}
	
	            }
	        }
	        catch(Exception e) 
			{
				Debug.LogException(e);
			}
	    }
		
		/// <summary>
		/// Updates the model.
		/// Evert pose contains the transform objects for each segments within the model.
		/// </summary>
		/// <param name='pose'>
		/// Pose holds the positions and orientations of the actor.
		/// </param>
		/// <param name='model'>
		/// Model to update with the pose.
		/// </param>
		private void updateModel(Transform[] pose, Transform[] model)
		{
			
			for(int i=0; i < NUM_SEGMENTS; i++)
			{
				
				switch(i)
				{
					//no update required
					case (int)XsAnimationSegment.L5:
					case (int)XsAnimationSegment.T12:					
						break;					
					
					case (int)XsAnimationSegment.Pelvis:
						//model[i].position = pose[i].position * animationScale;
						model[i].position = new Vector3(pose[i].position.x, (pose[i].position.y), pose[i].position.z);
						model[i].position *= scale;
						model[i].rotation = pose[i].rotation * modelRotTP[i];
						break;
						
					default:
						//rest of the segments
						if(model[i] != null)
						{
							model[i].rotation = pose[i].rotation * modelRotTP[i];
						}
					break;
				}
				
			}
		}
		
	    
	}//class XsLiveAnimator

}//namespace Xsens