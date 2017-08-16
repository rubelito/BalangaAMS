using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.ApplicationLayer.Interfaces.ExportData;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Interfaces;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class test_BrethrenExcelExporter
    {
        [Test]
        public void test_should_throw_error_if_filepath_is_invalid()
        {
            UnityBootstrapper.Configure();
            var brethrenExporter = UnityBootstrapper.Container.Resolve<IExportBrethren>();
            var filePath = @"d:\mastelist\sgsf.xlsx";

            var brethrenList = new List<BrethrenBasic>()
            {
                new BrethrenBasic()
                {
                    ChurchId = "00610865",
                    Name = "Rubelito Isiderio",
                    BrethrenFull = new BrethrenFull() {DateofBaptism = DateTime.Now}
                },
                new BrethrenBasic()
                {
                    ChurchId = "0dfgdfgd",
                    Name = "Rufino Dizon Jr.",
                    BrethrenFull = new BrethrenFull() {DateofBaptism = DateTime.Now}
                }
            };

            brethrenExporter.ExportBrethren(brethrenList, filePath);
        }

    }
}
