using BalangaAMS.ApplicationLayer.ImportExcelData;
using BalangaAMS.DataLayer.EntityFramework;
using BalangaAMS.DataLayer.Repository;
using DocumentFormat.OpenXml.Drawing.Charts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalangaAMS.Test
{
    [TestFixture]
    class test_ImportBrethrenExcel
    {
        [Test]
        public void test_import()
        {
            var excel = new BrethrenImporter(@"d:\members.xlsx");
            var brethrens = excel.Loadbretrhen();

            var connection = new EfSQLite("SQLiteDb"); ;
            using (var repo = new AMSUnitofWork(connection))
            {
                var brethrentodb = new ImportbrethrentoDb(repo, new DateTime(2013, 9,1));
                brethrentodb.Import(brethrens);
                Console.WriteLine(brethrentodb.Statusmessage());
            }
        }
    }
}
