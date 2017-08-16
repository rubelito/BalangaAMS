using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.ApplicationLayer.ExportData;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class test_closedXML
    {
        [Test]
        public void test_writing_something_on_Excel()
        {
            UnityBootstrapper.Configure();
            var brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            var brethrenList = brethrenManager.GetAllBrethren();
            var filePath = @"C:\Program Files\Common Files\BrethrenMasterList.xlsx";

            var brethrenExporter = new BrethrenExcelExporter();
            brethrenExporter.ExportBrethren(brethrenList, filePath);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void test_should_fail_if_brethrenList_is_null_or_zero_list()
        {
            var brethrenList = new List<BrethrenBasic>();
            var invalidPath = @"D:\brethrenMasterList.xlsx";

            var brethrenExporter = new BrethrenExcelExporter();
            brethrenExporter.ExportBrethren(brethrenList, invalidPath);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void test_should_fail_if_didnt_provide_valid_filepath()
        {
            UnityBootstrapper.Configure();
            var brethrenManager = UnityBootstrapper.Container.Resolve<IBrethrenManager>();
            var brethrenList = brethrenManager.GetAllBrethren();
            var invalidPath = @":\brethrenMasterList.xlsx";

            var brethrenExporter = new BrethrenExcelExporter();
            brethrenExporter.ExportBrethren(brethrenList, invalidPath);
        }
    }
}
