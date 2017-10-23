using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyWiFi.Core;

namespace EasyWiFi.ClientControls
{
    [AddComponentMenu("EasyWiFiController/Client/UserControls/Button")]
    public class CustomButtonClientController : MonoBehaviour, IClientController
    {
		public string controlName = "Button1";
		public Sprite buttonPressedSprite;
		public Text debugText;

        private ButtonControllerType button;
        private string buttonKey;

        // Use this for initialization
        void Awake()
        {
            buttonKey = EasyWiFiController.registerControl(EasyWiFiConstants.CONTROLLERTYPE_BUTTON, controlName);
            button = (ButtonControllerType)EasyWiFiController.controllerDataDictionary[buttonKey];

			GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        }

        //here we grab the input and map it to the data list
        void LateUpdate()
        {
			button.BUTTON_STATE_IS_PRESSED = false;
            //mapInputToDataStream();
        }

        public void mapInputToDataStream()
        {
            //reset to default values;
            //touch count is 0
//            button.BUTTON_STATE_IS_PRESSED = false;
//            bool pressed = false;
//
//            //touch
//            int touchCount = Input.touchCount;
//
//			if (touchCount > 0)
//            {				
//                for (int i = 0; i < touchCount; i++)
//                {
//                    Touch touch = Input.GetTouch(i);
//					if (touch.phase == TouchPhase.Began)
//					{
//						debugText.text = "Button pressed at time: " + Time.time;
//						pressed = true;
//						break;
//					}
//                }
//            }
//			else if (!Application.isMobilePlatform && Input.GetMouseButtonDown(0))
//			{
//				pressed = true;
//			}
//
//			button.BUTTON_STATE_IS_PRESSED = pressed;
        }

		public void OnButtonPressed()
		{
			button.BUTTON_STATE_IS_PRESSED = true;
			debugText.text = controlName + " pressed at time: " + Time.time;

			/*
			int touchCount = Input.touchCount;
			button.BUTTON_STATE_IS_PRESSED = false;
			for (int i = 0; i < touchCount; i++)
			{
				if (Input.GetTouch(i).phase == TouchPhase.Began)
				{
				}
			}
			*/
		}
    }

}