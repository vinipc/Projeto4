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

        ButtonControllerType button;
        Image currentImage;
        Sprite buttonRegularSprite;
        string buttonKey;
        Rect screenPixelsRect;
        int touchCount;
        bool pressed;

        // Use this for initialization
        void Awake()
        {
            buttonKey = EasyWiFiController.registerControl(EasyWiFiConstants.CONTROLLERTYPE_BUTTON, controlName);
            button = (ButtonControllerType)EasyWiFiController.controllerDataDictionary[buttonKey];
            currentImage = gameObject.GetComponent<Image>();
            buttonRegularSprite = currentImage.sprite;
            
        }

        void Start()
        {
            screenPixelsRect = EasyWiFiUtilities.GetScreenRect(currentImage.rectTransform);
        }

        //here we grab the input and map it to the data list
        void Update()
        {
            mapInputToDataStream();
        }

        public void mapInputToDataStream()
        {

            //reset to default values;
            //touch count is 0
            button.BUTTON_STATE_IS_PRESSED = false;
            pressed = false;

            //touch
            touchCount = Input.touchCount;

			if (touchCount > 0)
            {				
                for (int i = 0; i < touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began)
					{
						pressed = true;
						break;
					}
                }
            }
			else if (!Application.isMobilePlatform && Input.GetMouseButtonDown(0))
			{
				pressed = true;
			}

            //show the correct image
            if (pressed)
            {
                button.BUTTON_STATE_IS_PRESSED = true;
                currentImage.sprite = buttonPressedSprite;
            }
            else
            {
                button.BUTTON_STATE_IS_PRESSED = false;
                currentImage.sprite = buttonRegularSprite;

            }

        }

    }

}