using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Configuration;

namespace StatusMonitor
{
    public class MonitorUser
    {
        public string Id { get; set; }
        public string Display { get; set; }
        public bool Enabled { get; set; }
        public string Status { get; set; }
        public int LEDStart { get; set; }
        public int LEDEnd { get; set; }

        public static List<MonitorUser> FromList(SortedList<string,string> UserList)
        {
            List<MonitorUser> list = new List<MonitorUser>();
            foreach(KeyValuePair<string,string> user in UserList)
            {
                MonitorUser tmpUser = new MonitorUser() { Display = user.Key, Id = user.Value, Enabled = false, Status = "", LEDStart = -1, LEDEnd = -1 };
                list.Add(tmpUser);
            }

            return list;
        }
    }

    public class SystemSettings
    {
        public string EMailFilter { get; set; }
        public string UserSelection { get; set; }
        public string DeviceConfig { get; set; }

        public void SaveSettings()
        {
            AddUpdateAppSettings("EmailFilter", EMailFilter);
            AddUpdateAppSettings("UserSelection", UserSelection);
        }

        public void LoadSettings()
        {
            EMailFilter = ReadSetting("EmailFilter");
            UserSelection = ReadSetting("UserSelection");
        }

        public void SaveUserSelection(List<MonitorUser> Users)
        {
            UserSelection = JsonSerializer.Serialize(Users); 
            AddUpdateAppSettings("UserSelection", UserSelection);
        }

        public void LoadUserSelection(List<MonitorUser> Users)
        {
            UserSelection = ReadSetting("UserSelection");
            List<MonitorUser> LoadedUsers = JsonSerializer.Deserialize<List<MonitorUser>>(UserSelection);

            foreach(MonitorUser user in Users)
            {
                if (LoadedUsers.Where(x => x.Id == user.Id).FirstOrDefault() != null)
                {
                    MonitorUser loadedUser = LoadedUsers.Where(x => x.Id == user.Id).FirstOrDefault();
                    user.Enabled = loadedUser.Enabled;
                    user.LEDStart = loadedUser.LEDStart;
                    user.LEDEnd = loadedUser.LEDEnd;
                }
            }
        }

        public void SaveDeviceSettings(List<DeviceSettings> DeviceSettings)
        {
            DeviceSettings.RemoveAll(device => device.Deleted);

            DeviceConfig = JsonSerializer.Serialize(DeviceSettings);
            AddUpdateAppSettings("DeviceSettings", DeviceConfig);
        }

        public List<DeviceSettings> LoadDeviceSettings()
        {
            List<DeviceSettings> DeviceSettings = new List<DeviceSettings>();
            DeviceConfig = ReadSetting("DeviceSettings");
            if(!string.IsNullOrEmpty(DeviceConfig)) DeviceSettings = JsonSerializer.Deserialize<List<DeviceSettings>>(DeviceConfig);
            return DeviceSettings;
        }

        private string ReadSetting(string key)
        {
            string result = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "";
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

            return result;
        }

        private void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }

    public class DeviceSettings
    {
        public string DeviceName { get;set; }
        public string TargetIPAddress { get; set; }
        public int TargetPort { get; set; }
        public int LEDCount { get; set; }
        public bool FillString { get; set; }
        public bool RotateLights { get; set; }
        public string DeviceID { get; set; }
        public int RotationOffset { get; set; }
        public bool Deleted { get; set; }

        public DeviceSettings()
        {
            DeviceID = Guid.NewGuid().ToString();
            RotationOffset = 0;
            TargetPort = 2390;
            Deleted = false;
        }
    }
}
