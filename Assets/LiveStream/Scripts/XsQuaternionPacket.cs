///<summary>
/// This class will read the data from the stream and convert it to valid Quaternions.
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
	/// Parse the date from the stream as quaternions.
	/// </summary>
	class XsQuaternionPacket : XsDataPacket
	{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="xsens.XsQuaternionPacket"/> class.
		/// </summary>
		/// <param name='readData'>
		/// Create the packet from this data.
		/// </param>
	    public XsQuaternionPacket(byte[] readData)
	        : base(readData)
	    {
	
	    }
	
	    protected override void parsePayload(int startPoint)
	    {
		
	        payloadData[startPoint + 0] = convert32BitInt(binReader.ReadBytes(4));     // Segment ID

			payloadData[startPoint + 1] = convert32BitFloat(binReader.ReadBytes(4));   // X position
	        payloadData[startPoint + 2] = convert32BitFloat(binReader.ReadBytes(4));   // Y Position
	        payloadData[startPoint + 3] = convert32BitFloat(binReader.ReadBytes(4));   // Z Position
	
	        payloadData[startPoint + 4] = convert32BitFloat(binReader.ReadBytes(4));   // Quaternion W
	        payloadData[startPoint + 5] = convert32BitFloat(binReader.ReadBytes(4));   // Quaternion X
	        payloadData[startPoint + 6] = convert32BitFloat(binReader.ReadBytes(4));   // Quaternion Y 
	        payloadData[startPoint + 7] = convert32BitFloat(binReader.ReadBytes(4));   // Quaternion Z				
	
	        segmentCounter++;
			
	        // If we did not parse all the segments payload yet, start again with the current start position plus 8, which is the next segment in the data
	        if (segmentCounter != 23)
	            parsePayload(startPoint + 8);
	
	        isQuaternionPacket = true;
	    }
		
	}//class XsQuaternionPacket
}//namespace xsens