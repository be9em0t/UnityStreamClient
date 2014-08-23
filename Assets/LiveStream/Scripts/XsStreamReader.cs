///<summary>
/// Xsens Stream Reader is a component which will read directly from the network.
/// It will spawn 4 threads, 1 for each actor. (MVN Studio can stream up to 4 actors)
/// 
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

using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;

namespace xsens
{

	/// <summary>
	/// This class is responsible for setting up the connection with MVN studio.
	/// MVN Studio can stream up to 4 actors, therefore we have 1 thread for each (4 total).
	/// It also reads which datapacket it should create, and is responsible for always having the last correct pose ready to read for Unity3D
	/// </summary>
	public class XsStreamReader : MonoBehaviour
	{  
		public int listenPort = 9763;
	    private UdpClient udpClient;
	    private Thread connectionThread;
		private const int NUM_SEGMENTS = 23;
		
		private	Vector3[] emptyPositions;
		private Quaternion[] emptyOrientations;
		
		//threads for each actors
	    private XsStreamReaderThread poseActor1 = new XsStreamReaderThread();
	    private XsStreamReaderThread poseActor2 = new XsStreamReaderThread();
	    private XsStreamReaderThread poseActor3 = new XsStreamReaderThread();
	    private XsStreamReaderThread poseActor4 = new XsStreamReaderThread();
	
	    private bool isEnabledActor1, isEnabledActor2, isEnabledActor3, isEnabledActor4;
	
	    private int counter;
		
		/// <summary>
		/// Awake this instance.
		/// </summary>
	    void Awake()
	    {
			//create empty list for reasons when no data arrives
			emptyPositions = new Vector3[NUM_SEGMENTS];
			emptyOrientations = new Quaternion[NUM_SEGMENTS];
			for(int i=0; i < NUM_SEGMENTS; ++i)
			{
				emptyPositions[i] = Vector3.zero;
				emptyOrientations[i] = Quaternion.Euler(Vector3.zero);
			}
				
	        connectionThread = new Thread(new ThreadStart(read));             // Make a thread to read from the connection with MVN studio
	        connectionThread.Start();                                         // Start the thread..
	    }
	
		/// <summary>
		/// Read this instance.
		/// </summary>
	    private void read()
	    {
	        //int listenPort = 9763;
	        bool done = false;
	
	        udpClient = new UdpClient(listenPort);
	        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
	
	        try
	        {
	            while (!done)
	            {
	
	                byte[] receiveBytes = udpClient.Receive(ref groupEP);
	                if (receiveBytes[16] == 0)
	                {
	                    isEnabledActor1 = true;
	                    poseActor1.setPacket(receiveBytes);
	                }
	                else if (receiveBytes[16] == 1)
	                {
	                    isEnabledActor2 = true;
	                    poseActor2.setPacket(receiveBytes);
	                }
	                else if (receiveBytes[16] == 2)
	                {
	                    isEnabledActor3 = true;
	                    poseActor3.setPacket(receiveBytes);
	                }
	                else
	                {
	                    isEnabledActor4 = true;
	                    poseActor4.setPacket(receiveBytes);
	                }
	            }
	        }
	        catch (Exception e)
	        {
				Debug.LogException(e);
				Console.WriteLine("NetworkReader terminated.");
	        }
	        finally
	        {
	            udpClient.Close();
	        }
	    }
		
		/// <summary>
		/// Raises the application quit event.
		/// </summary>
	    private void OnApplicationQuit()
	    {
	        try
	        {
	            udpClient.Close();
	            udpClient = null;
	
	            connectionThread.Abort();                             // Abort the connection thread, else we cant connect again.
	
	            poseActor1.killThread();
	            poseActor2.killThread();
	            poseActor3.killThread();
	            poseActor4.killThread();
	        }
	        catch 
	        {
	            Debug.Log("Something went wrong when trying to close down everything. This is not a critical error.");
	        }
	    }
		
		/// <summary>
		/// Ises the enabled.
		/// </summary>
		/// <returns>
		/// The enabled.
		/// </returns>
		/// <param name='actorID'>
		/// actor ID
		/// </param>
	    public bool isEnabled(int actorID)
	    {
	        if (actorID == 1)
	        {
	            if (isEnabledActor1)
	                return true;
	        }
	        else if (actorID == 2)
	        {
	            if (isEnabledActor2)
	                return true;
	        }
	        else if (actorID == 3)
	        {
	            if (isEnabledActor3)
	                return true;
	        }
	        else if (actorID == 4)
	        {
	            if (isEnabledActor4)
	                return true;
	        }
	        
	        return false;
	    }
	
		/// <summary>
		/// Gets the latest orientations for the actor.
		/// </summary>
		/// <returns>
		/// The latest orientations.
		/// </returns>
		/// <param name='actorID'>
		/// Actor Id
		/// </param>
	    public Quaternion[] getLatestOrientations(int actorID)
	    {
	        if(actorID == 1 && poseActor1.dataAvailable())
			{	
	            return poseActor1.getLastCorrectOrientations();
			}
	        else if(actorID == 2)
	            return poseActor2.getLastCorrectOrientations();
	        else if (actorID == 3)
	            return poseActor3.getLastCorrectOrientations();
	        else if (actorID == 4)
	            return poseActor4.getLastCorrectOrientations();
			else
			{	
				return emptyOrientations;
			}	
	    }
	
	    /// <summary>
	    /// Gets the latest positions for the actor.
	    /// </summary>
	    /// <returns>
	    /// The latest positions.
	    /// </returns>
	    /// <param name='actorID'>
	    /// Actor Id
	    /// </param>
	    public Vector3[] getLatestPositions(int actorID)
	    {
	        if (actorID == 1 && poseActor1.dataAvailable())
	            return poseActor1.getLastCorrectPositions();
	        else if (actorID == 2)
	            return poseActor2.getLastCorrectPositions();
	        else if (actorID == 3)
	            return poseActor3.getLastCorrectPositions();
	        else if (actorID == 4)
	            return poseActor4.getLastCorrectPositions();
			else 
			{
				return emptyPositions;
			}
			
	    }
		
	}//class XsStreamReader
}//namespace xsens
