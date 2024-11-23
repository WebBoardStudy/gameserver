using System;
using System.IO;
using System.Net;

namespace Server.data;


    [Serializable]
    public class ServerConfigs
    {
        public string dataPath;
    }
    
    public class ConfigManager
    {
        public static ServerConfigs Config { get; private set; }

        public static void LoadConfig()
        {
            string text = File.ReadAllText("config.json");
            Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfigs>(text);
        }
    }
