using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace ChuongGa
{
    public class Status_Data
    {
        public float temperature { get; set; }
        public float moisture { get; set; }
        public float min_temperature { get; set; }
        public float max_temperature { get; set; }
        public int device_access { get; set; }
        public Status_Data(float temp, float moisture, float min_temp, float max_temp, int access){
            this.temperature = temp;
            this.moisture = moisture;
            this.min_temperature = min_temp;
            this.max_temperature = max_temp;
            this.device_access = access;
        }
    }

    public class data_ss
    {
        public string ss_name { get; set; }
        public string ss_unit { get; set; }
        public string ss_value { get; set; }
    }

    public class Config_Data
    {
        public float min_temperature { get; set; }
        public float max_temperature { get; set; }
        public int device_access { get; set; }
        public Config_Data(float min, float max, int access){
            this.min_temperature = min;
            this.max_temperature = max;
            this.device_access = access;
        }
    }

    public class ControlFan_Data
    {
        public int fan_status { get; set; }
        public int device_status { get; set; }

    }

    public class Pump_Data
    {
        public string device {get; set;}
        public string status {get; set;} 
        public Pump_Data(string device, string status){
            this.device = device;
            this.status = status;
        }
    }

    public class Led_Data
    {
        public string device {get; set;}
        public string status {get; set;} 
        public Led_Data(string device, string status){
            this.device = device;
            this.status = status;
        }
    }

    public class ChuongGaMqtt : M2MqttUnityClient
    {
        public InputField addressInputField;
        public InputField userInputField;
        public InputField pwdInputField;
        public Text text_display;
        public ChuongGaManager manager;


        public List<string> topics = new List<string>();


        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_led = "";
        public string msg_received_from_topic_pump = "";


        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        // [SerializeField]
        // public Config_Data _config_data;
        [SerializeField]
        public ControlFan_Data _controlFan_data;
        [SerializeField]
        public Pump_Data _pump_data;
        [SerializeField]
        public Led_Data _led_data;
        [SerializeField]
        public Config_Data _config_data;

	    public override void Connect(){
            if(addressInputField.text != ""){
                this.brokerAddress = addressInputField.text;
            }
            else{
                this.brokerAddress = "mqttserver.tk";
            }

            if(userInputField.text != ""){
                this.mqttUserName = userInputField.text;
            }
            else{
                this.mqttUserName = "bkiot";
            }

            if(pwdInputField.text != ""){
                this.mqttPassword = pwdInputField.text;
            }
            else{
                this.mqttPassword = "12345678";
            }
	        this.brokerPort = 1883;
            base.Connect();
	    }

        public async void publishPump(bool isOn){
            _pump_data = new Pump_Data("PUMP",isOn? "ON" : "OFF");
            string msg = JsonConvert.SerializeObject(_pump_data);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("ChuongGaMqtt, line 112, publish to " + topics[2] + " : " + isOn);
        }

        public async void publishLed(bool isOn){
            _led_data = new Led_Data("LED",isOn? "ON" : "OFF");
            string msg = JsonConvert.SerializeObject(_led_data);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("ChuongGaMqtt, line 134, publish to " + topics[1] + " : " + isOn);
        }

        public async void publishDeviceAccess(){
            _status_data = new Status_Data(0,0,0,0,1);
            string msg = JsonConvert.SerializeObject(_status_data);
            client.Publish(topics[0], System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("ChuongGaMqtt, line 141, publish to " + topics[0]);
        }

        public void PublishConfig()
        {
            manager.Update_Config_Value(_status_data);
            string msg_config = JsonConvert.SerializeObject(_status_data);
            client.Publish(topics[0], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish config");
        }

        // public void PublishFan()
        // {
        //     _controlFan_data = manager.Update_ControlFan_Value(_controlFan_data);
        //     string msg_config = JsonConvert.SerializeObject(_controlFan_data);
        //     client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        //     Debug.Log("publish fan");
        // }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        protected override void OnConnecting()
        {
            base.OnConnecting();
            text_display.text = "Connecting";
            //SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }


        IEnumerator wait(float seconds){
            yield return new WaitForSeconds(seconds);
        }
        protected override async void OnConnected()
        {
	        manager.SwitchLayer();
            // StartCoroutine(wait(2f));
            base.OnConnected();
            SubscribeTopics();
            // publishDeviceAccess(); 
        }

        protected override void SubscribeTopics()
        {

            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    Debug.Log("ChuongGaMqtt, line 150, subscribe " + topic);
                }
            }

        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
	        text_display.text = "Connection failed!";
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            text_display.text = "";
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }



        protected override void Start()
        {

            base.Start();
        }

        protected override async void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            //StoreMessage(msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);

            if (topic == topics[1])
                ProcessMessageLed(msg);

            if (topic == topics[2])
                ProcessMessagePump(msg);
        }


        private void ProcessMessageStatus(string msg)
        {
             _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            manager.Update_Status(_status_data);
        }

        private void ProcessMessageLed(string msg)
        {
            // _led_data = new Led_Data("LED","ON");
            _led_data = JsonConvert.DeserializeObject<Led_Data>(msg);
            msg_received_from_topic_led = msg;
            manager.Update_Led(_led_data);
        }
        private void ProcessMessagePump(string msg)
        {
            // _pump_data = new Pump_Data("PUMP","ON");
            _pump_data = JsonConvert.DeserializeObject<Pump_Data>(msg);
            msg_received_from_topic_pump = msg;
            manager.Update_Pump(_pump_data);
        }

        // private void ProcessMessageConfig(string msg)
        // {
        //     _config_data = JsonConvert.DeserializeObject<Config_Data>(msg);
        //     manager.Update_Config(_config_data);
        // }

        // private void ProcessMessageControl(string msg)
        // {
        //     _controlFan_data = JsonConvert.DeserializeObject<ControlFan_Data>(msg);
        //     msg_received_from_topic_control = msg;
        //     manager.Update_Control(_controlFan_data);
        // }

        private void OnDestroy()
        {
            Disconnect();
            
        }

        private void OnValidate()
        {
            //if (autoTest)
            //{
            //    autoConnect = true;
            //}
        }

        public void UpdateConfig()
        {
           
        }

        public void UpdateControl()
        {

        }
    }
}
