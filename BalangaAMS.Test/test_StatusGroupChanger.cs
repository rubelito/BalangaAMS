using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.HelperDomain;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.Test
{
    [TestFixture]
    public class test_StatusGroupChanger
    {
        [Test]
        public void idontknow()
        {
            UnityBootstrapper.Configure();
            var identifier = UnityBootstrapper.Container.Resolve<IStatusIdentifier>();
            var status = identifier.GetStatusForMonthOf(49, MonthofYear.December, 2012);
        }

    }
}
