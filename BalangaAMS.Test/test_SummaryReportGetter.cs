using BalangaAMS.ApplicationLayer.Report;
using BalangaAMS.ApplicationLayer.Report.ReportModule;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.HelperDomain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class test_ReportSummaryGetter
    {
        [Test]
        public void test_getSummaryReport()
        {
            UnityBootstrapper.Configure();
            var summaryGetter = UnityBootstrapper.Container.Resolve<MonthlyReportSummaryGetter>();
            var group = new Group() {GroupName = "Newly Baptised"};
            var reportSummary = summaryGetter.GetSummaryReport(group, MonthofYear.May, 2013);
        }
    }
}
