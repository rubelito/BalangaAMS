using System;
using System.Collections.Generic;
using System.Linq;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.ApplicationLayer.Report.ReportModule
{
    public class MonthlyReportSummaryGetter : IMonthlyReportSummaryGetter
    {
        private readonly IBrethrenManager _brethrenManager;
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly IGroupManager _groupManager;
        private readonly IStatusIdentifier _statusIdentifier;
        private MonthofYear _monthofYear;
        private Group _group;
        private int _year;
        private int _activecount;
        private int _inactivecount;

        public MonthlyReportSummaryGetter(IBrethrenManager brethrenManager, IChurchGatheringRetriever sessionRetriever,
            IStatusIdentifier identifier,
            IGroupManager groupManager){
            _brethrenManager = brethrenManager;
            _sessionRetriever = sessionRetriever;
            _groupManager = groupManager;
            _statusIdentifier = identifier;
        }

        public int DaysToConsiderNewlyBaptised { get; set; }

        public MontlyReportSummary GetSummaryReport(Group group, MonthofYear monthofYear, int year){
            _group = group;
            _monthofYear = monthofYear;
            _year = year;
            CountActiveAndInactive();
            var reportSummary = new MontlyReportSummary();
            reportSummary.ActiveCount = _activecount;
            reportSummary.InactiveCount = _inactivecount;
            reportSummary.GatheringsTotal = GetTotalGatherings();
            reportSummary.GroupName = _group.GroupName;
            reportSummary.MonthofYear = GetMonthofYear();

            return reportSummary;
        }

        private void CountActiveAndInactive(){
            _activecount = 0;
            _inactivecount = 0;
            if (IsNewlyBaptisedGroup(_group))
                ProcessNewlyBaptisedGroup();
            else if (IsNoGroup(_group))
                ProcessNoGroup();
            else
                ProcessBrethrenWithinAGroup();
        }

        private bool IsNewlyBaptisedGroup(Group group){
            return group.GroupName == "Newly Baptised";
        }

        private bool IsNoGroup(Group group){
            return group.GroupName == "No Group";
        }

        private void ProcessNoGroup(){
            var brethrenWithNoGroup = _groupManager.GetBrethrenWithNoGroup();
            var brethrens = RemoveNewlyBaptised(brethrenWithNoGroup);
            foreach (var brethren in brethrens){
                var status = _statusIdentifier.GetStatusForMonthOf(brethren.Id, _monthofYear, _year);
                if (status == AttendanceStatus.Active)
                    _activecount++;
                else
                    _inactivecount++;
            }
        }

        private void ProcessNewlyBaptisedGroup(){
            var brethrenWithNoGroup = _groupManager.GetBrethrenWithNoGroup();
            var newlyBapstised = GetNewlyBapstisedInNoGroup(brethrenWithNoGroup);
            foreach (var brethren in newlyBapstised){
                var status = _statusIdentifier.GetStatusForMonthOf(brethren.Id, _monthofYear, _year);
                if (status == AttendanceStatus.Active)
                    _activecount++;
                else
                    _inactivecount++;
            }
        }

        private List<BrethrenBasic> RemoveNewlyBaptised(List<BrethrenBasic> brethrenList){
            return
                brethrenList.Where(b => !_brethrenManager.IsNewlyBaptised(b, DaysToConsiderNewlyBaptised, DateTime.Now))
                    .ToList();
        }

        private List<BrethrenBasic> GetNewlyBapstisedInNoGroup(List<BrethrenBasic> brethrenWithNoGroup){
            return brethrenWithNoGroup.Where(
                b =>
                    b.BrethrenFull.DateofBaptism.HasValue &&
                    _brethrenManager.IsNewlyBaptised(b, DaysToConsiderNewlyBaptised, DateTime.Now))
                .ToList();
        }

        private void ProcessBrethrenWithinAGroup(){
            var brethrenList = _groupManager.GetBrethrenWithInGroup(_group.Id);
            foreach (BrethrenBasic brethren in brethrenList){
                var status = _statusIdentifier.GetStatusForMonthOf(brethren.Id, _monthofYear, _year);
                if (status == AttendanceStatus.Active)
                    _activecount++;
                else
                    _inactivecount++;
            }
        }

        private int GetTotalGatherings(){
            return _sessionRetriever.CountAllStartedRegularGatheringsInAMonth(_monthofYear, _year);
        }

        private string GetMonthofYear(){
            return _monthofYear.ToString() + ", " + _year;
        }

        public IndividualReportSummary GetIndividualReportSummary(MonthofYear monthofYear, int year){
            _monthofYear = monthofYear;
            _year = year;
            var summaryReport = new IndividualReportSummary();
            summaryReport.MonthofYear = _monthofYear.ToString() + ", " + _year;
            summaryReport.GatheringsTotal = GetTotalGatherings();

            return summaryReport;
        }
    }
}