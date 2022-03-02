using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonLibraries.Tests
{
    [TestClass]
    public class MicrosoftStats
    {

        [TestMethod]
        public void SystemStats()
        {
            var stats = new CommonLibraries.Microsoft.SystemStatisticsMonitor().WindowsStatistics;
            foreach (var stat in stats.OrderBy(r=> r.StatisticName))
            {
                System.Diagnostics.Debug.WriteLine($"{stat.StatisticName} = {stat.StatisticValue}");
            }
            Assert.IsNotNull(stats);
        }
    }
}
