using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ChuongGa
{
    public class ChuongGaManager : MonoBehaviour
    {
        [SerializeField]
        private ChuongGaMqtt client;

        [SerializeField]
        private CanvasGroup _canvasLogin;

        [SerializeField]
        private Text station_name;
        
        [SerializeField]
        private CanvasGroup _canvasLayer1;
        
        [SerializeField]
        private Text temperature;
        [SerializeField]
        private Text humidity;
        [SerializeField]
        private Text min_temperature;
        [SerializeField]
        private Text max_temperature;
        [SerializeField]
        private Toggle PumpControl;
        [SerializeField]
        private Text PumpStatus;
        [SerializeField]
        private CanvasGroup status_pump_on;
        [SerializeField]
        private CanvasGroup status_pump_off;

        [SerializeField]
        private Toggle LedControl;
        [SerializeField]
        private Text LedStatus;
        [SerializeField]
        private CanvasGroup status_led_on;
        [SerializeField]
        private CanvasGroup status_led_off;
 
        [SerializeField]
        private Button _btn_config;
        /// <summary>
        /// Layer 2 elements
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasLayer2;
        [SerializeField]
        private InputField _input_min_tempe;
        [SerializeField]
        private InputField _input_max_tempe;
        [SerializeField]
        private Toggle ModeAuto;

        /// <summary>
        /// General elements
        /// </summary>
        [SerializeField]
        private GameObject Btn_Quit;

        private Tween twenFade;

        private bool device_status = false;
        private Status_Data _status_data = new Status_Data(0,0,0,0,0);

        public void Update_Status(Status_Data _status_data)
        {
            if(this._status_data.temperature != _status_data.temperature){
                temperature.text = _status_data.temperature + "°C";
                this._status_data.temperature = _status_data.temperature;
            }
            if(this._status_data.moisture != _status_data.moisture){
                humidity.text = _status_data.moisture + "%";
                this._status_data.moisture = _status_data.moisture;
            }
            if(this._status_data.min_temperature != _status_data.min_temperature){
                min_temperature.text = _status_data.min_temperature + "°C";
                _input_min_tempe.text = string.Format("{0:N1}", _status_data.min_temperature);
                this._status_data.min_temperature = _status_data.min_temperature;
            }
            if(this._status_data.max_temperature != _status_data.max_temperature){
                max_temperature.text = _status_data.max_temperature + "°C";
                _input_max_tempe.text = string.Format("{0:N1}", _status_data.max_temperature);
                this._status_data.max_temperature = _status_data.max_temperature;
            }
            
            
            
            
            // min_temperature.text =_status_data.min_temperature + "°C";
            // _input_min_tempe.text = string.Format("{0:N1}", _status_data.min_temperature);
            // max_temperature.text = _status_data.max_temperature + "°C";
            // _input_max_tempe.text = string.Format("{0:N1}",_status_data.max_temperature);
            // foreach(data_ss _data in _status_data.data_ss)
            // {
            //     switch (_data.ss_name)
            //     {

            //         case "temperature_min":
            //             min_temperature.text = _data.ss_value + "°C";
            //             _input_min_tempe.text = _data.ss_value;
            //             break;

            //         case "temperature_max":
            //             max_temperature.text = _data.ss_value + "°C";
            //             _input_max_tempe.text = _data.ss_value;
            //             break;

            //         case "fan_temperature":
            //             temperature.text = _data.ss_value + "°C";
            //             break;

            //         case "fan_humidity":
            //             humidity.text = _data.ss_value + "%";
            //             break;

                    // case "mode_fan_auto":
                    //     if (_data.ss_value == "1") { 
                    //         ModeAuto.isOn = true;
                    //         LampControl.interactable = false;
                    //         Debug.Log("I have go hereeeeeeeeeeeeee");
                    //     }
                    //     else { 
                    //         ModeAuto.isOn = false;
                    //         LampControl.interactable = true;
                    //     }
                    //     break;
                    // case "fan_status":
                    //     if(_data.ss_value=="1"){
                    //         LampControl.isOn = true;
                    //         LampStatus.text = "ON";
                    //         LampStatus.color = new Color(0f, 255f, 0f);
                    //         // FadeOut(status_fan_lamp_off, 0f);
                    //         // FadeIn(status_fan_lamp_on, 0f);
                    //     }
                    //     else{
                    //         LampControl.isOn = false;
                    //         LampStatus.text = "OFF";
                    //         LampStatus.color = new Color(255f, 0f, 0f);
                    //         // FadeIn(status_fan_lamp_off, 0f);
                    //         // FadeOut(status_fan_lamp_on, 0f);
                    //     }
                    //     break;
                    // case "device_status":
                    //     Debug.Log("_data.ss_value " + _data.ss_value);
                    //     if (_data.ss_value == "1")
                    //         _btn_config.interactable = true;
                       
                    //     break;
            //     }
                
            // }
            // if(_status_data.device_status=="1")
            //     _btn_config.interactable = true;

        }

        public void Update_Led(Led_Data _led_data)
        {
            if(_led_data.status == "ON"){
                LedStatus.text = "ON";
                LedStatus.color = new Color(0f, 255f, 0f);
                LedControl.isOn = true;
                SwitchLed(true);
            }
            else{
                LedStatus.text = "OFF";
                LedStatus.color = new Color(255f, 0f, 0f);
                LedControl.isOn = false;
                SwitchLed(false);
            }
        }

        public void Update_Pump(Pump_Data _pump_data)
        {
            if(_pump_data.status == "ON"){
                PumpStatus.text = "ON";
                PumpStatus.color = new Color(0f, 255f, 0f);
                PumpControl.isOn = true;
                SwitchPump(true);
            }
            else{
                PumpStatus.text = "OFF";
                PumpStatus.color = new Color(255f, 0f, 0f);
                PumpControl.isOn = false;
                SwitchPump(false);
            }
        }

        // public void Update_Config(Config_Data _config_data)
        // {
        //     min_temperature.text =_config_data.min_temperature + "°C";
        //     _input_min_tempe.text = string.Format("{0:N1}", _config_data.min_temperature);
        //     max_temperature.text = _config_data.max_temperature + "°C";
        //     _input_max_tempe.text = string.Format("{0:N1}",_config_data.max_temperature);
        // }

        // public void Update_Control(ControlFan_Data _control_data)
        // {
        //     if (_control_data.device_status == 1)
        //     {
        //         LampControl.interactable = true;
        //         if (_control_data.fan_status == 1)
        //             LampControl.isOn = true;
        //         else
        //             LampControl.isOn = false;
        //     }

        // }

        // public ControlFan_Data Update_ControlFan_Value(ControlFan_Data _controlFan)
        // {
        //     _controlFan.device_status = 0;
        //     _controlFan.fan_status = (LampControl.isOn ? 1 : 0);
        //     LampControl.interactable = false;
        //     return _controlFan;
        // }

        public Status_Data Update_Config_Value(Status_Data _status_data)
        {
            _status_data.max_temperature = float.Parse(_input_max_tempe.text);
            _status_data.min_temperature = float.Parse(_input_min_tempe.text);
           
            return _status_data;
        }


        /*Toggle pump button*/
        public void OnClickPump()
        {
            if (PumpControl.isOn == true)
            {
                PumpStatus.text = "ON";
                PumpStatus.color = new Color(0f, 255f, 0f);
            }
            else
            {
                PumpStatus.text = "OFF";
                PumpStatus.color = new Color(255f, 0f, 0f);
            }
            SwitchPump(PumpControl.isOn);
            client.publishPump(PumpControl.isOn);
        }

        public void OnClickLed()
        {
            if (LedControl.isOn == true)
            {
                LedStatus.text = "ON";
                LedStatus.color = new Color(0f, 255f, 0f);
            }
            else
            {
                LedStatus.text = "OFF";
                LedStatus.color = new Color(255f, 0f, 0f);
            }
            SwitchLed(LedControl.isOn);
            client.publishLed(LedControl.isOn);
        }

        public void Disable_Config_Btn()
        {
            _btn_config.interactable = false;
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }

        IEnumerator _onClickX(){
            FadeOut(_canvasLayer1,0f);
            yield return new WaitForSeconds(0.2f);
            FadeOut(_canvasLayer2,0f);
            yield return new WaitForSeconds(0.2f);
            FadeIn(_canvasLogin,0.25f);
        }

        public void onClickX(){
            StartCoroutine(_onClickX());
        }


        IEnumerator _IESwitchLayer()
        {
            if(_canvasLogin.interactable == true){
                FadeOut(_canvasLogin, 0f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
            }
            else if (_canvasLayer1.interactable == true)

            {
                FadeOut(_canvasLayer1, 0f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer2, 0.25f);
            }
            else
            {
                FadeOut(_canvasLayer2, 0f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
            }
        }

        IEnumerator _IESwitchPump(bool isOn)
        {
            if (isOn)
            {
                FadeOut(status_pump_off, 0f);
                yield return new WaitForSeconds(0f);
                FadeIn(status_pump_on, 0f);
            }
            else
            {
                FadeOut(status_pump_on, 0f);
                yield return new WaitForSeconds(0f);
                FadeIn(status_pump_off, 0f);
            }
        }
        IEnumerator _IESwitchLed(bool isOn)
        {
            if (isOn)
            {
                FadeOut(status_led_off, 0f);
                yield return new WaitForSeconds(0f);
                FadeIn(status_led_on, 0f);
            }
            else
            {
                FadeOut(status_led_on, 0f);
                yield return new WaitForSeconds(0f);
                FadeIn(status_led_off, 0f);
            }
        }
        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }

        public void SwitchPump(bool isOn)
        {
            StartCoroutine(_IESwitchPump(isOn));

        }
        public void SwitchLed(bool isOn)
        {
            StartCoroutine(_IESwitchLed(isOn));

        }
    }
}
