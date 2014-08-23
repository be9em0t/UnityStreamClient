///<summary>
/// This class will read the data from the stream as Euler angles and convert them to Quaternions.
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
	/// Xsens euler packet parse euler angles values from the stream.
	/// </summary>
	class XsEulerPacket : XsDataPacket
	{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="xsens.XsEulerPacket"/> class.
		/// </summary>
		/// <param name='readData'>
		/// Create the packet from this data.
		/// </param>
	    public XsEulerPacket(byte[] readData)
	        : base(readData)
	    {
	
	    }
	
		/// <summary>
		/// Parses the payload.
		/// </summary>
		/// <param name='startPoint'>
		/// Start point.
		/// </param>
	    protected override void parsePayload(int startPoint)
	    {
	        payloadData[startPoint + 0] = convert32BitInt(binReader.ReadBytes(4));     // Segment ID
	
	        payloadData[startPoint + 1] = -convert32BitFloat(binReader.ReadBytes(4)) / 100;   // X position  <- switched with -Z
	        payloadData[startPoint + 2] = convert32BitFloat(binReader.ReadBytes(4)) / 100;   // Y Position
	        payloadData[startPoint + 3] = convert32BitFloat(binReader.ReadBytes(4)) / 100;   // Z Position  <- switched with x
	        
	        float x = (float)convert32BitFloat(binReader.ReadBytes(4)); //X ori
	        float y = (float)convert32BitFloat(binReader.ReadBytes(4)); //Y ori
	        float z = (float)convert32BitFloat(binReader.ReadBytes(4)); //Z ori
			Vector3 v = new Vector3(x,-y,-z); // orig, data already in good order +z
			Quaternion q = Quaternion.Euler(v);
			
			payloadData[startPoint + 4] = q.w;   // Quaternion W
	        payloadData[startPoint + 5] = q.x;   // Quaternion X
	        payloadData[startPoint + 6] = q.y;   // Quaternion Y
	        payloadData[startPoint + 7] = q.z;   // Quaternion Z
			
	        segmentCounter++;
	
	        // If we did not parse all the segments payload yet, start again with the current start position plus 8, which is the next segment in the data
	        if (segmentCounter != 23)
	            parsePayload(startPoint + 8);
	    }
		
	}//class XsEulerPacket
	
}//namespace xsens	
