using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyWiFi.Core;

namespace EasyWiFi.ClientControls
{
    public class ShakeClientController : MonoBehaviour, IClientController
    {
        public string controlName = "Button1";
		public Text debugText;

        ButtonControllerType button;
        Image currentImage;
        Sprite buttonRegularSprite;
        string buttonKey;
        Rect screenPixelsRect;
        int touchCount;
        bool pressed;

		float accelerometerUpdateInterval = 1.0f / 60.0f;
		// The greater the value of LowPassKernelWidthInSeconds, the slower the
		// filtered value will converge towards current input sample (and vice versa).
		float lowPassKernelWidthInSeconds = 1.0f;
		// This next parameter is initialized to 2.0 per Apple's recommendation,
		// or at least according to Brady! ;)
		float shakeDetectionThreshold = 2.0f;

		float lowPassFilterFactor;
		Vector3 lowPassValue;

        void Awake()
        {
            buttonKey = EasyWiFiController.registerControl(EasyWiFiConstants.CONTROLLERTYPE_BUTTON, controlName);
            button = (ButtonControllerType)EasyWiFiController.controllerDataDictionary[buttonKey];
            currentImage = gameObject.GetComponent<Image>();
            buttonRegularSprite = currentImage.sprite;
            
			lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
			shakeDetectionThreshold *= shakeDetectionThreshold;
			lowPassValue = Input.acceleration;
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

			if (Application.isMobilePlatform)
            {	
				Vector3 acceleration = Input.acceleration;
				lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
				Vector3 deltaAcceleration = acceleration - lowPassValue;
				
				if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
				{
					// Perform your "shaking actions" here. If necessary, add suitable
					// guards in the if check above to avoid redundant handling during
					// the same shake (e.g. a minimum refractory period).
					//Debug.Log("Shake event detected at time "+Time.time);
					debugText.text = "Pressed button at time: " + Time.time;
					button.BUTTON_STATE_IS_PRESSED = true;
					pressed = true;
				}
            }
			else if (!Application.isMobilePlatform && Input.GetMouseButtonDown(0))
			{
				pressed = true;

				//pressed = true;
			}

            //show the correct image
//            if (pressed)
//            {
//                button.BUTTON_STATE_IS_PRESSED = true;
//            }
//            else
//            {
//                button.BUTTON_STATE_IS_PRESSED = false;
//                currentImage.sprite = buttonRegularSprite;
//            }

        }

    }

}