///<summary>
/// Xsens Mvn Pose represents a all segments data to create a pose.
///</summary>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace xsens
{
	/// <summary>
	/// This class converts all the data from the packet into something Unity3D can easely read.
	/// This also contains the orientations and position fixes needed because of the different coordinate system.
	/// </summary>
	class XsMvnPose
	{
	    private double[] g_headerData;
	    private double[] g_payloadData;
	    
	    public Vector3[] positions;
	    public Quaternion[] orientations;
	
	    public bool done = false;
	    private bool _isQuaternion = false;
	
	    public XsMvnPose(bool isQuaternion)
	    {
	        _isQuaternion = isQuaternion;
	
	        positions = new Vector3[23];
	        orientations = new Quaternion[23];
	    }
	
		/// <summary>
		/// Creates the vector3 positions and the Quaternion rotations for unity, based on the current data packet.
	    /// Recursive so it does every segment
		/// </summary>
		/// <param name='startPosition'>
		/// Start position.
		/// </param>
		/// <param name='segmentCounter'>
		/// Segment counter.
		/// </param>
	    public void createPose(int startPosition, int segmentCounter)
	    {
			
	        Quaternion rotation = new Quaternion();
			Vector3 position = new Vector3();
	
	        if (_isQuaternion)
	        {
				
				//Debug.Log ("Quaternion");
				position.x = Convert.ToSingle(g_payloadData[startPosition + 1]);   //X=1
	            position.y = Convert.ToSingle(g_payloadData[startPosition + 2]);	//Y=2
	            position.z = Convert.ToSingle(g_payloadData[startPosition + 3]);	//Z=3
				
	            rotation.w = Convert.ToSingle(g_payloadData[startPosition + 4]);	//W=4
	            rotation.x = Convert.ToSingle(g_payloadData[startPosition + 5]);	//x=5 
				rotation.y = Convert.ToSingle(g_payloadData[startPosition + 6]);	//y=6
	            rotation.z = Convert.ToSingle(g_payloadData[startPosition + 7]);	//Z=7
				
	
	        }
	        else
	        {
	            Debug.Log ("Euler");
	            position.x = Convert.ToSingle(g_payloadData[startPosition + 1]);
	            position.y = Convert.ToSingle(g_payloadData[startPosition + 2]);
	            position.z = Convert.ToSingle(g_payloadData[startPosition + 3]);
	
				rotation.w = Convert.ToSingle(g_payloadData[startPosition + 4]);
	            rotation.x = Convert.ToSingle(g_payloadData[startPosition + 5]);
	            rotation.y = Convert.ToSingle(g_payloadData[startPosition + 6]);
	            rotation.z = Convert.ToSingle(g_payloadData[startPosition + 7]);
				
	        }
			
			positions[segmentCounter] = position;
	        orientations[segmentCounter] = rotation;
	
	        segmentCounter++;
	
	        if (segmentCounter != 23)
	            createPose(startPosition + 8, segmentCounter);
	        
	        done = true;
	    }
	
		/// <summary>
		/// Sets the header data.
		/// </summary>
		/// <param name='headerData'>
		/// Header data.
		/// </param>
	    public void setHeaderData(double[] headerData)
	    {
	        g_headerData = headerData;
	    }
		
		/// <summary>
		/// Sets the payload data.
		/// </summary>
		/// <param name='payloadData'>
		/// Payload data.
		/// </param>
	    public void setPayloadData(double[] payloadData)
	    {
	        g_payloadData = payloadData;
	    }
	
	}//class XsMvnPose	
}//namespace xsens