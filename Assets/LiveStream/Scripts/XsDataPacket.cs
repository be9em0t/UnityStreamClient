///<summary>
/// Xsens Data Packet represents the data comming from the network stream.
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
using System.IO;
using System.Globalization;

namespace xsens{
	
	/// <summary>
	/// This class represnets an Xsens data packet.
	/// </summary>
	abstract class XsDataPacket
	{
	    protected double[] headerData = new double[5];         	// Contains the header data of the packet, for both packets the header is almost the same
	    protected double[] payloadData = new double[184];      	// Contains all the segment data to create a pose with
	    protected BinaryReader binReader;                      	// A binary reader to convert the hex values in the packet to a value humans can read
	    protected MemoryStream memStream;                      	// Stores the packet data, so the binaryreader can read it
	    protected int segmentCounter = 0;						//segment counter
	    protected bool isQuaternionPacket;						//set to true if the packet is quaternion based
	    public XsMvnPose pose;                          		// Every packet has a pose
	
		/// <summary>
		/// Initializes a new instance of the <see cref="xsens.XsDataPacket"/> class.
		/// </summary>
		/// <param name='readData'>
		/// Create the packed from this byte array.
		/// </param>
	    public XsDataPacket(byte[] readData)
	    {
	        memStream = new MemoryStream(readData);            // Create a memory stream of the packet data
	        binReader = new BinaryReader(memStream);          // Create a binary reader on that memory stream, so we can easely convert the data it contains
	
	        parseHeader();
	        parsePayload(0);                                    // Calls the correct classes parsePayload by itself (inheritance)
	
	        pose = new XsMvnPose(isQuaternionPacket);
	        pose.setHeaderData(headerData);                    // TODO: figure out if this is even neccesary
	        pose.setPayloadData(payloadData);                  // Sets the payload data in the pose
	
	        pose.createPose(0, 0);                              // Try to create a new pose with the data that was send
	    }
	
		/// <summary>
		/// Parses the payload depends on the current network mode.
		/// </summary>
		/// <param name='startPoint'>
		/// Start point in the data array.
		/// </param>
	    protected abstract void parsePayload(int startPoint);   // 
	
		/// <summary>
		/// Parses the header.
		/// </summary>
	    private void parseHeader()
	    {
	        binReader.BaseStream.Position = 6;                         // Need to head up 6 steps because ReadString does not do this for us
	
	        headerData[1] = convert32BitInt(binReader.ReadBytes(4));  // Sample Counter
	        binReader.BaseStream.Position = 10;
	
	        headerData[2] = binReader.ReadByte();          // Datagram Counter
	        headerData[3] = binReader.ReadByte();          // Number of items
	        binReader.BaseStream.Position += 4;             // Time stamp which we dont need, so we step over it
	        headerData[4] = binReader.ReadByte();          // Avatar ID
	        binReader.BaseStream.Position += 7;             // We also skip the next 7 empty bytes, this way the position is correct for the payload data
	    }
	
	    /// <summary>
	    /// Since the binary reader is small endian, and the data from the packet is big endian we need to convert the data
	    /// This is done here, and simply puts the reverse data into a temp buffer and the memorystream and binaryreader make an integer of the data
	    /// </summary>
	    /// <param name="incomingByteArray"></param>
	    /// <returns></returns>
	    protected double convert32BitInt(byte[] incomingByteArray)
	    {
	        byte[] tempByteArray = new byte[4];
	
	        tempByteArray[0] = incomingByteArray[3];
	        tempByteArray[1] = incomingByteArray[2];
	        tempByteArray[2] = incomingByteArray[1];
	        tempByteArray[3] = incomingByteArray[0];
	
	        return BitConverter.ToInt32(tempByteArray, 0);
	    }
	
	    /// <summary>
	    /// Since the binary reader is small endian, and the data from the packet is big endian we need to convert the data
	    /// This is done here, and simply puts the reverse data into a temp buffer and the memorystream and binaryreader make an float of the data
	    /// </summary>
	    /// <param name="incomingByteArray"></param>
	    /// <returns></returns>
	    protected double convert32BitFloat(byte[] incomingByteArray)
	    {
	        byte[] tempByteArray = new byte[4];
	
	        tempByteArray[0] = incomingByteArray[3];
	        tempByteArray[1] = incomingByteArray[2];
	        tempByteArray[2] = incomingByteArray[1];
	        tempByteArray[3] = incomingByteArray[0];
	
	        return BitConverter.ToSingle(tempByteArray, 0);
	    }
		
	}//class XsDataPacket

}//namespace xsens