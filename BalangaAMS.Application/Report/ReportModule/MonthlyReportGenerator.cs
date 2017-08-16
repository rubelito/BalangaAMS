using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.ApplicationLayer.Report.ReportModule
{
    public class MonthlyReportGenerator : IMonthlyReport
    {
        private readonly IBrethrenManager _brethrenManager;
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly IStatusIdentifier _statusIdentifier;
        private readonly ILateIdentifier _lateIdentifier;
        private Dictionary<string, object> _reportDictionary;
        private DataTable _reportTable;

        private long _brethrenId;
        private DateTime _lastUpdatedStatus;
        private MonthofYear _monthofYear;
        private int _year;
        private List<GatheringSession> _totalGatheringSessions;
        private List<GatheringSession> _attendedGatheringSession;

        public MonthlyReportGenerator(IBrethrenManager brethrenManager, IChurchGatheringRetriever sessionRetriever,
            IStatusIdentifier statusIdentifier, ILateIdentifier lateIdentifier){
            _brethrenManager = brethrenManager;
            _sessionRetriever = sessionRetriever;
            _statusIdentifier = statusIdentifier;
            _lateIdentifier = lateIdentifier;
        }

        public DataTable GenerateBrethrenReport(long brethrenId, MonthofYear monthofYear, int year){
            _brethrenId = brethrenId;
            _monthofYear = monthofYear;
            _year = year;
            _reportDictionary = new Dictionary<string, object>();
            _reportTable = new DataTable();
            var brethren = _brethrenManager.GetBrethrenbyId(brethrenId);
            _lastUpdatedStatus = brethren.LastStatusUpdate;
            AddAttendanceInfo(brethren);

            return _reportTable;
        }

        private void AddAttendanceInfo(BrethrenBasic brethren){
            AddBrehtrenInfo(brethren);
            GetAllGatheringSessionsInAMonth();
            ProcessPrayerMeeting();
            ProcessWorshipService();
            ProcessThanksGiving();
            SeperatorColumn();
            ProcessTotalAbsent();
            ProcessTotalPresent();
            ProcessTotalLate();
            ProcessStatus();
            MergeDictionaryAndTable();
        }

        private void AddBrehtrenInfo(BrethrenBasic brethren){
            _reportDictionary.Add("ChurchId", brethren.ChurchId);
            _reportDictionary.Add("Name", brethren.Name);
            _reportTable.Columns.Add("ChurchId", typeof (string));
            _reportTable.Columns.Add("Name", typeof (string));
        }

        private void GetAllGatheringSessionsInAMonth(){
            _totalGatheringSessions = _sessionRetriever.GetAllStartedRegularGatheringsForMonthOf(_monthofYear, _year);
            _attendedGatheringSession =
                _sessionRetriever.GetGatheringsThatBrethrenAttendedForTheMonthOf(_brethrenId, _monthofYear,
                    _year);
        }

        private void ProcessPrayerMeeting(){
            var prayerMeeting =
                _totalGatheringSessions.Where(g => g.Gatherings == Gatherings.Prayer_Meeting).ToList();

            int attendedTotal = 0;
            foreach (GatheringSession session in prayerMeeting){
                CreateSessionInfo(session, "dtcol_P_", ref attendedTotal);
            }
            _reportDictionary.Add("dtcol_P_Total", attendedTotal);
            _reportTable.Columns.Add("dtcol_P_Total", typeof (int));
        }

        private void ProcessWorshipService(){
            var worshipService =
                _totalGatheringSessions.Where(g => g.Gatherings == Gatherings.Worship_Service).ToList();

            int attendedTotal = 0;
            foreach (GatheringSession session in worshipService){
                CreateSessionInfo(session, "dtcol_W_", ref attendedTotal);
            }
            _reportDictionary.Add("dtcol_W_Total", attendedTotal);
            _reportTable.Columns.Add("dtcol_W_Total", typeof (int));
        }

        private void ProcessThanksGiving(){
            var thanksGiving = _totalGatheringSessions.Where(g => g.Gatherings == Gatherings.Thanks_Giving).ToList();

            int attendedTotal = 0;
            foreach (GatheringSession session in thanksGiving){
                CreateSessionInfo(session, "dtcol_T_", ref attendedTotal);
            }
            _reportDictionary.Add("dtcol_T_Total", attendedTotal);
            _reportTable.Columns.Add("dtcol_T_Total", typeof (int));
        }

        private void CreateSessionInfo(GatheringSession session, string columnName, ref int attendedTotal){
            if (session.IsStarted){
                var dayInfo = GetDayInfo(session);
                attendedTotal = dayInfo.IsAttended ? attendedTotal + 1 : attendedTotal;
                _reportDictionary.Add(columnName + session.Date.ToString("MMZddZyy_") + session.Id,
                    dayInfo.DayAttendanceStatus);
                _reportTable.Columns.Add(columnName + session.Date.ToString("MMZddZyy_") + session.Id, typeof (Enum));
            }
            else{
                _reportDictionary.Add(columnName + session.Date.ToString("MMZddZyy_") + session.Id,
                    DayAttendanceStatus.None);
                _reportTable.Columns.Add(columnName + session.Date.ToString("MMZddZyy_") + session.Id, typeof (Enum));
            }
        }

        private AttendanceDayInfo GetDayInfo(GatheringSession session){
            var dayInfo = new AttendanceDayInfo{IsAttended = _attendedGatheringSession.Any(b => b.Id == session.Id)};
            if (dayInfo.IsAttended){
                dayInfo.DayAttendanceStatus = DayAttendanceStatus.Present;
                if (_lateIdentifier.IsBrethrenIsLate(_brethrenId, session.Id))
                    dayInfo.DayAttendanceStatus = DayAttendanceStatus.Late;
            }
            else if (session.Date < _lastUpdatedStatus)
                dayInfo.DayAttendanceStatus = DayAttendanceStatus.NA;
            else{
                dayInfo.DayAttendanceStatus = DayAttendanceStatus.Absent;
            }
            return dayInfo;
        }

        private void SeperatorColumn(){
            _reportDictionary.Add("Separator", " ");
            _reportTable.Columns.Add("Separator", typeof (string));
        }

        private void ProcessTotalAbsent(){
            var totalAbsent = _sessionRetriever.CountBrethrenAbsentInAMonth(_brethrenId, _monthofYear, _year);
            _reportDictionary.Add("Absent", totalAbsent);
            _reportTable.Columns.Add("Absent", typeof (int));
        }

        private void ProcessTotalPresent(){
            var totalPresent = _attendedGatheringSession.Count;
            _reportDictionary.Add("Present", totalPresent);
            _reportTable.Columns.Add("Present", typeof (int));
        }

        private void ProcessTotalLate(){
            var totalLate = _lateIdentifier.CountTheLateOfBrethrenForMonthOf(_brethrenId, _monthofYear, _year);
            _reportDictionary.Add("Late", totalLate);
            _reportTable.Columns.Add("Late", typeof (int));
        }

        private void ProcessStatus(){
            var status = _statusIdentifier.GetStatusForMonthOf(_brethrenId, _monthofYear, _year);
            DayAttendanceStatus finalStatus = status == AttendanceStatus.Active
                ? DayAttendanceStatus.Active
                : DayAttendanceStatus.Inactive;
            _reportDictionary.Add("Status", finalStatus);
            _reportTable.Columns.Add("Status", typeof (Enum));
        }

        private void MergeDictionaryAndTable(){
            var row = _reportTable.NewRow();
            foreach (KeyValuePair<string, object> keyValuePair in _reportDictionary){
                string columnName = keyValuePair.Key;
                row[columnName] = Convert.ChangeType(keyValuePair.Value, _reportTable.Columns[columnName].DataType);
            }
            _reportTable.Rows.Add(row);
        }
    }
}