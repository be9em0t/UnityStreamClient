///<summary>
/// Xsens Stream Reader Thread read from the stream and store the latest pose from 1 actor.
/// 
///</summary>
///<remarks>
///
///		Copyright (c) Xsens Technologies B.V., 2006-2013. All rights reserved.
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
using System.Threading;

namespace xsens
{
	
	/// <summary>
	/// Xsens Stream Reader Thread.
	/// Every actor from MVN Stream has it's own reader trhead.
	/// </summary>
	class XsStreamReaderThread
	{
		
	    public XsDataPacket datapacket;
	    public Mutex packetMutex = new Mutex(false, "DataMutex");
	    public Mutex lastPoseMutex = new Mutex(false, "DataMutex");
		
		private const int NUM_SEGMENTS = 23;
	    private Thread thread;
	    private byte[] lastPackets;
	    private bool newData = false;
		private bool dataUpdated = true;
	
	    private Vector3[] lastPosePositions;
	    private Quaternion[] lastPoseOrientations;
	
		/// <summary>
		/// Initializes a new instance of the <see cref="xsens.XsStreamReaderThread"/> class.
		/// </summary>
	    public XsStreamReaderThread()
	    {
	        lastPackets = new byte[1000];
			//make sure we always have some date, even when no streaming
			lastPosePositions = new Vector3[NUM_SEGMENTS];
			lastPoseOrientations = new Quaternion[NUM_SEGMENTS];
			//start a new thread		
	        thread = new Thread(new ThreadStart(start));
	        thread.Start();
	    }
	
		/// <summary>
		/// Start this instance.
		/// </summary>
	    public void start()
	    {
			dataUpdated =  false;
	        while (true)
	        {
				
	            if (newData)
	            {
	                if (packetMutex.WaitOne())
	                {
	                    try
	                    {
	                        newData = false;
	
	                        if (lastPackets[5] == 0x31)
	                            datapacket = new XsEulerPacket(lastPackets);
	                        else
	                            datapacket = new XsQuaternionPacket(lastPackets);
	                    }
	                    catch
	                    {
	
	                    }
	                    finally
	                    {
	                        packetMutex.ReleaseMutex();
	
	                        if (lastPoseMutex.WaitOne())
	                        {
	                            try
	                            {	
	                                lastPosePositions = datapacket.pose.positions;
	                                lastPoseOrientations = datapacket.pose.orientations;
									
									dataUpdated = true;
	                            }
	                            catch
	                            {
	
	                            }
	                            finally
	                            {
	                                lastPoseMutex.ReleaseMutex();
	                            }
	                        }
	                    }
	                }
	            }
				
	            Thread.Sleep(1);
	        }
	    }
	    
		/// <summary>
		/// Gets the last correct positions.
		/// </summary>
		/// <returns>
		/// The last correct positions.
		/// </returns>
	    public Vector3[] getLastCorrectPositions()
	    {
	        if (lastPoseMutex.WaitOne())
	        {
	            try
	            {
	                return lastPosePositions;
	            }
	            finally
	            {
	                lastPoseMutex.ReleaseMutex();
	            }
	        }
	        return null;
	    }
	
		/// <summary>
		/// Gets the last correct orientations.
		/// </summary>
		/// <returns>
		/// The last correct orientations.
		/// </returns>
	    public Quaternion[] getLastCorrectOrientations()
	    {
	        lastPoseMutex.WaitOne();
	
	        try
	        {
	            return lastPoseOrientations;
	        }
	        finally
	        {
	            lastPoseMutex.ReleaseMutex();
	        }
	    }
		
		/// <summary>
		/// Datas the available.
		/// </summary>
		/// <returns>
		/// true if data is available
		/// </returns>
	    public bool dataAvailable()
	    {
	        lastPoseMutex.WaitOne();
	
	        try
	        {
	            return dataUpdated;
	        }
	        finally
	        {
	            lastPoseMutex.ReleaseMutex();
	        }
	    }	
	
		/// <summary>
		/// Kills the thread.
		/// </summary>
	    public void killThread()
	    {
	        thread.Abort();
	    }
	
		/// <summary>
		/// Sets the packet.
		/// </summary>
		/// <param name='incomingData'>
		/// _incoming data in array
		/// </param>
	    public void setPacket(byte[] incomingData)
	    {
	        if (packetMutex.WaitOne())
	        {
	            try
	            {
	                lastPackets = incomingData;
	                newData = true;
	            }
	            finally
	            {
	                packetMutex.ReleaseMutex();
	            }
	        }
	    }
		
	}//class XsStreamReaderThread
}//namespace xsens