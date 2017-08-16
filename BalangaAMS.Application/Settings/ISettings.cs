
namespace BalangaAMS.ApplicationLayer.Settings
{
    public interface ISettingsManager
    {
        string GetDivisionName();
        void SetDivisionName(string divisionName);
        string GetDistrictName();
        void SetDistricName(string districtName);
        string GetLocalName();
        void SetLocalName(string localName);
        int GetNumberOfDaysToBeConsideredNewlyBaptised();
        void SetNumberOfDaysToBeConsideredNewlyBaptised(int daysCount); 
        string GetAdminPassword();
        void SetAdminPassword(string oldPassword, string newPassword);
        string GetMemberPassword();
        void SetMemberPassword(string oldPassword, string newPassword);
        void Save();
    }
}
