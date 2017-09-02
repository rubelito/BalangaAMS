using System.Windows;
using System.Windows.Input;
using BalangaAMS.ApplicationLayer.Settings;
using BalangaAMS.WPF.View.EnumData;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for MembersLoggin.xaml
    /// </summary>
    public partial class UserLogin
    {
        private UserType _userType = UserType.None;
        private readonly ISettingsManager _settingsManager;
        private bool _isExited;

        public UserLogin()
        {
            InitializeComponent();
            _settingsManager = UnityBootstrapper.Container.Resolve<ISettingsManager>();
        }

        public UserType GetUserType(){
            return _userType;
        }

        public bool IsExited(){
            return _isExited;
        }

        private void Login_Click(object sender, RoutedEventArgs e){
            LogUser();
        }

        private void PasswordBox_OnKeyDown(object sender, KeyEventArgs e){
            if (e.Key == Key.Enter){
                LogUser();
            }
        }

        private void LogUser(){
            if (IsUserIsAdmin())
            {
                _userType = UserType.Admin;
                _isExited = false;
                Close();
            }
            if (IsUserIsMember())
            {
                _userType = UserType.Member;
                _isExited = false;
                Close();
            }

            if (_userType == UserType.None)
                MessageBox.Show("Password Is Wrong");
        }

        private bool IsUserIsAdmin(){
            return PasswordBox.Password == _settingsManager.GetAdminPassword();
        }  

        private bool IsUserIsMember(){
            return PasswordBox.Password == _settingsManager.GetMemberPassword();
        }    

        private void Exit_Click(object sender, RoutedEventArgs e){
            _isExited = true;
            Close();
        }
    }
}
