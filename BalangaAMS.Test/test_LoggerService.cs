using BalangaAMS.ApplicationLayer.Service.LoggingAttendance;
using BalangaAMS.Core.Interfaces;
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
    class test_LoggerService
    {
        [Test]
        public void test_we_dont_know()
        {
            var authenticater = UnityBootstrapper.Container.Resolve<ILogAuthenticaterByChurchId>();
            var brethren = authenticater.Authenticate("00610865");
            
            if (brethren != null)
            {
                
            }


        }
    }

    public class LoggerService
    {
        private readonly ILogAuthenticaterByChurchId _logAuthenticater;
        private readonly IAttendanceLogger _iAttendanceLogger;

        public LoggerService(ILogAuthenticaterByChurchId logAuthenticater, IAttendanceLogger iAttendanceLogger)
        {
            _logAuthenticater = logAuthenticater;
            _iAttendanceLogger = iAttendanceLogger;
        }

        public void Logbrethren()
        {
            
        }
    }
}
