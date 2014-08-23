using UnityEngine;
using System.Collections;

namespace MocapiLiveStream
{

public class CameraGUI : MonoBehaviour {
	
		int bottoMargin = 5;
		int leftMargin = 5;

        public static bool Up = false;
        public static bool Left = false;
        public static bool Right = false;
        public static bool Down = false;
        public static bool Plus = false;
        public static bool Minus = false;

		public static float Zoom = 60f;
        public GUISkin MocapiSkin = null;

		void OnGUI () {

            //Load the skin
            GUI.skin = MocapiSkin;

//			// Make a background box
//			GUI.Box(new Rect(10,10,100,90), "Loader Menu");
//			
            // Contents
            GUI.Label(new Rect(0, 10, Screen.width, 100), "Mocapi Live Stream Client");
			
			// button up
            if (GUI.RepeatButton(new Rect(leftMargin + 50, (Screen.height - bottoMargin) - 150, 50, 50), "Up")) { Up = true; }
            else Up = false;

            // button down
            if (GUI.RepeatButton(new Rect(leftMargin + 50, (Screen.height - bottoMargin) - 50, 50, 50), "Down")) { Down = true; }
            else Down = false;

			// button left
            if (GUI.RepeatButton(new Rect(leftMargin + 0, (Screen.height - bottoMargin) - 100, 50, 50), "Left")) { Left = true; }
            else Left = false;

			// button right
            if (GUI.RepeatButton(new Rect(leftMargin + 100, (Screen.height - bottoMargin) - 100, 50, 50), "Right")) { Right = true; }
            else Right = false;
			

			// button zoom in
			if(GUI.Button(new Rect((Screen.width - leftMargin) - 50,(Screen.height - bottoMargin) - 150,50,50), "In")) {
				//MocapiLiveStream.Player.scalePlayer.y = MocapiLiveStream.Player.scalePlayer.y -.5f;
				Zoom = Zoom - 2f;
				//MocapiLiveStream.LookAtCamera.zoomly = MocapiLiveStream.LookAtCamera.zoomly +0.5f;				
			}
				
			// button zoom out
			if(GUI.Button(new Rect((Screen.width - leftMargin) - 50,(Screen.height - bottoMargin) - 50,50,50), "Out")) {
				//MocapiLiveStream.Player.scalePlayer.y = MocapiLiveStream.Player.scalePlayer.y -.5f;
				Zoom = Zoom + 2f;
				//MocapiLiveStream.LookAtCamera.zoomly = MocapiLiveStream.LookAtCamera.zoomly -0.5f;				
				
			}
			
			
	}
}
	
}