using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BalangaAMS.ApplicationLayer.ImportExcelData;
using BalangaAMS.ApplicationLayer.Interfaces.ImportExcelData;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.DataLayer.EntityFramework;
using Microsoft.Practices.Unity;

namespace BalangaAMS.ImportData
{
    public partial class Main : Form
    {
        public Main(){
            InitializeComponent();
            UnityBootstrapper.Configure();
        }

        private void Create_Click(object sender, EventArgs e){
            try{
                CreateDatabase();
                MessageBox.Show("Successful Creating Database", "Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Browse_Click(object sender, EventArgs e){
            var openFile = new OpenFileDialog{Filter = "Excel (*.XLSX)|*.XLSX"};
            var isOpen = openFile.ShowDialog();

            if (isOpen != DialogResult.OK)
                return;
            txtFilePath.Text = openFile.FileName;
        }

        private void Save_Click(object sender, EventArgs e){
            try{
                if (string.IsNullOrWhiteSpace(txtFilePath.Text)){
                    MessageBox.Show("Must supply file path");
                    return;
                }

                var brethrenData = GetBrethren(txtFilePath.Text);
                ImportData(brethrenData);
                MessageBox.Show("Successful Importing Data", "Success!", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private List<DatatoImport> GetBrethren(string filePath){
            var importer = new BrethrenImporter(filePath);
            return importer.Loadbretrhen();
        }

        private void CreateDatabase(){
            var connection = new EfSQLite("SQLiteDb");
            using (var ams = new AMSDbContext(connection)){
                if (ams.Database.Exists()){
                    ams.Database.Delete();
                }
                ams.Database.Create();
            }
        }

        private void ImportData(List<DatatoImport> dataToImports){
            var dbImporter =
                UnityBootstrapper.Container.Resolve<IImporttoDb>(new ParameterOverride("lastStatusUpdate",
                    datePicker.Value));
            dbImporter.Import(dataToImports);
        }
    }
}
