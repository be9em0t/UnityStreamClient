using UnityEngine;
using System.Collections;

namespace MocapiLiveStream
{

    public class CameraGUI : MonoBehaviour
    {

        //Gui elements
        public Texture2D controlTex_Up;
        public Texture2D controlTex_Down;
        public Texture2D controlTex_Left;
        public Texture2D controlTex_Right;
        public Texture2D controlTex_High;
        public Texture2D controlTex_Low;
        private int buttWH = 48;

        //Values
        public GUISkin MocapiSkin = null;
        int bottomMargin = 21;
        int leftMargin = 5;

        //make values available in other scripts
        public static bool Up = false;
        public static bool Left = false;
        public static bool Right = false;
        public static bool Down = false;
        public static bool High = false;
        public static bool Low = false;


        void OnGUI()
        {

            //Load the skin
            GUI.skin = MocapiSkin;

            // Viewport Label
            //GUI.Label(new Rect(0, 10, Screen.width, 100), "Mocapi Live Stream Client");

            // button up - camera forward
            if (GUI.RepeatButton(new Rect(leftMargin + buttWH, Screen.height - bottomMargin - (buttWH * 3), buttWH, buttWH), controlTex_Up)) { Up = true; }
            else Up = false;

            // button down - camera back
            if (GUI.RepeatButton(new Rect(leftMargin + buttWH, Screen.height - bottomMargin - buttWH, buttWH, buttWH), controlTex_Down)) { Down = true; }
            else Down = false;

            // button left - camera left
            if (GUI.RepeatButton(new Rect(leftMargin, Screen.height - bottomMargin - (buttWH*2), buttWH, buttWH), controlTex_Left)) { Left = true; }
            else Left = false;

            // button right - camera right
            if (GUI.RepeatButton(new Rect(leftMargin + (buttWH * 2), Screen.height - bottomMargin - (buttWH * 2), buttWH, buttWH), controlTex_Right)) { Right = true; }
            else Right = false;


            // button higher - camera up
            if (GUI.RepeatButton(new Rect((Screen.width - leftMargin) - (buttWH * 2) + 16, Screen.height - bottomMargin - (buttWH * 3), buttWH, buttWH), controlTex_High)) { High = true; }
            else High = false;

            // button lower - camera down
            if (GUI.RepeatButton(new Rect((Screen.width - leftMargin) - (buttWH * 2) + 16, Screen.height - bottomMargin - buttWH, buttWH, buttWH), controlTex_Low)) { Low = true; }
            else Low = false;

        }

    }
}