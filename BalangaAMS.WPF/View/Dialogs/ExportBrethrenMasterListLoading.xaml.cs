using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Domain;
using BalangaAMS.WPF.View.HelperClass;
using Microsoft.Practices.Unity;

namespace BalangaAMS.WPF.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportLoading.xaml
    /// </summary>
    public partial class ExportBrethrenMasterListLoading
    {
        private readonly List<BrethrenBasic> _brethrenList;
        private readonly string _fileName;
        private readonly BackgroundWorker _backgroundWorker;

        public ExportBrethrenMasterListLoading(List<BrethrenBasic> brethrenList, string fileName)
        {
            _brethrenList = brethrenList;
            _fileName = fileName;
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            InitializeBackGroundWorker();
        }

        private void InitializeBackGroundWorker()
        {
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            var threadObject = new ThreadObjectBrethrenMasterList(_brethrenList, _fileName);
            _backgroundWorker.RunWorkerAsync(threadObject);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var threadObject = (ThreadObjectBrethrenMasterList) e.Argument;
            var brethrenExporter = UnityBootstrapper.Container.Resolve<IExportBrethren>();
            brethrenExporter.ExportBrethren(threadObject.BrethrenList, threadObject.FileName);
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
            Close();
        }
    }
}
