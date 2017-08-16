using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.WPF.View.Dialogs;
using Microsoft.Practices.Unity;
using Path = System.IO.Path;

namespace BalangaAMS.WPF.View
{
    /// <summary>
    /// Interaction logic for ExportData.xaml
    /// </summary>
    public partial class ExportData
    {
        private readonly SaveFileDialog _saveFileDialog;
        private readonly List<BrethrenBasic> _brethrenList; 

        public ExportData()
        {
            IBrethrenManager brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            _saveFileDialog = new SaveFileDialog();
            InitializeComponent();
            PrepareSaveDialog();
            _brethrenList = brethrenManager.GetAllBrethren();

        }

        private void PrepareSaveDialog()
        {
            _saveFileDialog.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            _saveFileDialog.FileName = "BrethrenMasterList";
        }

        private void ExportMasterListButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = _saveFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Exportbrethren(_brethrenList, _saveFileDialog.FileName);
            }
        }

        private void Exportbrethren(List<BrethrenBasic> brethrenList, string fileName)
        {
            if (IsValidToExport(brethrenList, fileName))
            {
                var exportLoading = new ExportBrethrenMasterListLoading(brethrenList, fileName);
                try
                {
                    exportLoading.ShowDialog();
                }
                catch (Exception e)
                {
                    exportLoading.Close();
                    System.Windows.MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }               
            }
        }

        private bool IsValidToExport(List<BrethrenBasic> brethrenList, string fileName)
        {
            bool isValid = true;
            if (IsNullOrEmpty(brethrenList))
            {
                System.Windows.MessageBox.Show("Cannot Export, MasterList is empty", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                isValid = false;
            }
            if (IsNotValidPathOrFileName(fileName))
            {
                System.Windows.MessageBox.Show("Invalid FileName or Destination Path", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                isValid = false;
            }
            return isValid;
        }

            private bool IsNullOrEmpty(List<BrethrenBasic> brethrenList)
            {
                return brethrenList == null || brethrenList.Count == 0;
            }

            private bool IsNotValidPathOrFileName(string fileName)
            {
                return !Path.IsPathRooted(fileName) || Path.GetFileNameWithoutExtension(fileName).Length == 0;
            }
    }
}
