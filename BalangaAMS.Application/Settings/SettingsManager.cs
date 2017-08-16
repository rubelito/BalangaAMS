using System;
using System.Configuration;

namespace BalangaAMS.ApplicationLayer.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly Configuration _config;

        public SettingsManager()
        {
            var appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var configFile = System.IO.Path.Combine(appPath, "AMS.exe.config");
            var configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            _config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        public string GetDivisionName()
        {
            return _config.AppSettings.Settings["DivisionName"].Value;
        }

        public void SetDivisionName(string divisionName)
        {
            if (string.IsNullOrWhiteSpace(divisionName))
                throw new ArgumentNullException("Division Name cannot be empty");
            _config.AppSettings.Settings.Remove("DivisionName");
            _config.AppSettings.Settings.Add("DivisionName", divisionName);
        }

        public string GetDistrictName()
        {
            return _config.AppSettings.Settings["DistrictName"].Value;
        }

        public void SetDistricName(string districtName)
        {
            if (string.IsNullOrWhiteSpace(districtName))
                throw new ArgumentNullException("District Name cannot be empty");
            _config.AppSettings.Settings.Remove("DistrictName");
            _config.AppSettings.Settings.Add("DistrictName", districtName);
        }

        public string GetLocalName()
        {
            return _config.AppSettings.Settings["LocalName"].Value;
        }

        public void SetLocalName(string localName)
        {
            if (string.IsNullOrWhiteSpace(localName))
                throw new ArgumentNullException("Local Name cannot be empty");
            _config.AppSettings.Settings.Remove("LocalName");
            _config.AppSettings.Settings.Add("LocalName", localName);
        }

        public void Save()
        {
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("AppSettings");
        }

        public int GetNumberOfDaysToBeConsideredNewlyBaptised()
        {
            return Convert.ToInt32(_config.AppSettings.Settings["daysToConsiderNewlyBaptised"].Value);
        }

        public void SetNumberOfDaysToBeConsideredNewlyBaptised(int daysCount)
        {
            _config.AppSettings.Settings.Remove("daysToConsiderNewlyBaptised");
            _config.AppSettings.Settings.Add("daysToConsiderNewlyBaptised", daysCount.ToString());
        }


        public string GetAdminPassword()
        {
            return _config.AppSettings.Settings["AdminPassword"].Value;
        }

        public void SetAdminPassword(string oldPassword, string newPassword)
        {
            if (_config.AppSettings.Settings["AdminPassword"].Value == oldPassword) {
                _config.AppSettings.Settings.Remove("AdminPassword");
                _config.AppSettings.Settings.Add("AdminPassword", newPassword);
            }
            else {
                throw new ArgumentException("You should know the old password to set the new password");
            }
        }

        public string GetMemberPassword()
        {
            return _config.AppSettings.Settings["MemberPassword"].Value;
        }

        public void SetMemberPassword(string oldPassword, string newPassword)
        {
            if (_config.AppSettings.Settings["MemberPassword"].Value == oldPassword){
                _config.AppSettings.Settings.Remove("MemberPassword");
                _config.AppSettings.Settings.Add("MemberPassword", newPassword);
            }
            else{
                throw new ArgumentException("You should know the old password to set the new password");                
            }
        }
    }
}
