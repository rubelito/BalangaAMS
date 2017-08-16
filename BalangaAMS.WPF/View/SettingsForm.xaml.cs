using System;
using System.Windows;
using BalangaAMS.ApplicationLayer.Settings;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for SettingsForm.xaml
    /// </summary>
    public partial class SettingsForm
    {
        private readonly ISettingsManager _settingsManager;

        public SettingsForm() {
            InitializeComponent();
            _settingsManager = UnityBootstrapper.Container.Resolve<ISettingsManager>();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e) {
            LoadSettings();
        }

        private void LoadSettings() {
            DivisionName.Text = _settingsManager.GetDivisionName();
            DistrictName.Text = _settingsManager.GetDistrictName();
            LocalName.Text = _settingsManager.GetLocalName();
            NumberOfDays.Text = _settingsManager.GetNumberOfDaysToBeConsideredNewlyBaptised().ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) {
            SaveSettings();
        }

        private void SaveSettings() {
            try {
                if (!IsNumeric(NumberOfDays.Text)) throw new ArgumentException("Number of Days should be numeric");
                _settingsManager.SetDivisionName(DivisionName.Text);
                _settingsManager.SetDistricName(DistrictName.Text);
                _settingsManager.SetLocalName(LocalName.Text);
                _settingsManager.SetNumberOfDaysToBeConsideredNewlyBaptised(Convert.ToInt32(NumberOfDays.Text));
                _settingsManager.Save();
                MessageBox.Show("You must restart this System to take effect", "Restart", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private bool IsNumeric(string number) {
            int i;
            return int.TryParse(number, out i);
        }

        private void ChangeAdminPassword_Click(object sender, RoutedEventArgs e) {
            if (IsOldPasswordIsNotMatch()) {
                MessageBox.Show("Incorrect old Password, please enter the correct old password",
                    "Cannot set new password");
                return;
            }

            if (IsNewAdminPasswordAndRetypePasswordIsNotMatch()) {
                MessageBox.Show("New Password and retype Password should be match", "Cannot set new password");
                return;
            }
            
            SetAdminPassword();
        }

            private bool IsOldPasswordIsNotMatch() {
                return OldAdminPassword.Text != _settingsManager.GetAdminPassword();
            }

            private bool IsNewAdminPasswordAndRetypePasswordIsNotMatch() {
                return NewAdminPassword.Text != RetypeAdminPassWord.Text;
            }

            private void SetAdminPassword() {
                try {
                    _settingsManager.SetAdminPassword(OldAdminPassword.Text, NewAdminPassword.Text);
                    _settingsManager.Save();
                    MessageBox.Show("Succesful changing Admin Password, please restart to take effect", "Password Changed");

                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Cannot change Admin Password", MessageBoxButton.OK, MessageBoxImage.Error);
                }  
            }

        private void ChangeUserPassword_Click(object sender, RoutedEventArgs e)
        {
            if (IsUserOldPasswordIsNotMatch()){
                MessageBox.Show("Incorrect old Password, please enter the correct old password",
                    "Cannot set new password");
                return;
            }
            if (IsNewUserPasswordAndRetypePasswordIsNotMatch()){
                MessageBox.Show("New Password and retype Password should be match", "Cannot set new password");
                return;
            }
            SetUserPassword();
        }

        private bool IsUserOldPasswordIsNotMatch(){
            return OldUserPassWord.Text != _settingsManager.GetMemberPassword();
        }

        private bool IsNewUserPasswordAndRetypePasswordIsNotMatch(){
            return NewUserPassword.Text != RetypeUserPassword.Text;
        }

        private void SetUserPassword(){
            try{
                _settingsManager.SetMemberPassword(OldUserPassWord.Text, NewUserPassword.Text);
                _settingsManager.Save();
                MessageBox.Show("Succesful changing User Password, please restart to take effect", "Password Changed");

            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, "Cannot change User Password", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
