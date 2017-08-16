using System;
using System.Collections.Generic;
using System.Linq;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.ApplicationLayer.HelperClass;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;
using System.Data;

namespace BalangaAMS.ApplicationLayer.Report.ReportModule
{
    public class WeeklyReportGenerator : IWeeklyReport
    {
        private readonly IBrethrenManager _brethrenManager;
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly ILateIdentifier _lateIdentifier;

        private long _brethrenId;
        private List<GatheringSession> _sessionList;
        private Dictionary<string, object> _reportDictionary;
        private DataTable _reportTable;
        private DateTime _lastUpdatedStatus;
        private int _remarksNumber;

        public WeeklyReportGenerator(IBrethrenManager brethrenManager, IChurchGatheringRetriever sessionRetriever,
            ILateIdentifier lateIdentifier)
        {
            _brethrenManager = brethrenManager;
            _sessionRetriever = sessionRetriever;
            _lateIdentifier = lateIdentifier;
        }

        public DataTable GetBrethrenReport(long brethrenId, List<GatheringSession> sessionList){
            _remarksNumber = 1;
            _brethrenId = brethrenId;
            _sessionList =  GatheringsDateArranger.ArrangeSessionsByDate(sessionList);
            _reportDictionary = new Dictionary<string, object>();
            _reportTable = new DataTable();
            var brethren = _brethrenManager.GetBrethrenbyId(brethrenId);
            _lastUpdatedStatus = brethren.LastStatusUpdate;
            AddAttendanceInfo(brethren);
            return _reportTable;
        }

        private void AddAttendanceInfo(BrethrenBasic brethren)
        {
            AddBrethrenInfo(brethren);
            ProcessWeekGathering();
            MergeDictionaryAndTable();
        }      

        private void AddBrethrenInfo(BrethrenBasic brethren)
        {
            _reportDictionary.Add("ChurchId", brethren.ChurchId);
            _reportDictionary.Add("Name", brethren.Name);
            _reportTable.Columns.Add("ChurchId", typeof(string));
            _reportTable.Columns.Add("Name", typeof(string));
        }

        private void ProcessWeekGathering(){
            foreach (var session in _sessionList){
                if (session.IsStarted){
                    CreateAttendanceInfoRow(session);
                }
                else{
                    CreateEmptyAttendanceInfoRow(session);
                }
                CreateRemarksFields();
            }
        }

        private void CreateAttendanceInfoRow(GatheringSession session){
            var dayInfo = GetDayInfo(session);
            _reportDictionary.Add(session.Gatherings + "Z" + session.Date.ToString("MMMZddZyyyy") + "_" + session.Id,
                dayInfo);
            _reportTable.Columns.Add(session.Gatherings + "Z" + session.Date.ToString("MMMZddZyyyy") + "_" + session.Id,
                typeof (AttendanceDayInfo));
        }

        private void CreateEmptyAttendanceInfoRow(GatheringSession session)
        {
            _reportDictionary.Add(session.Gatherings + "Z" + session.Date.ToString("MMMZddZyyyy") + session.Id,
                new AttendanceDayInfo { DayAttendanceStatus = DayAttendanceStatus.None });
            _reportTable.Columns.Add(
                session.Gatherings + "Z" + session.Date.ToString("MMMZddZyyyy") + session.Id,
                typeof(AttendanceDayInfo));
        }

        private void CreateRemarksFields(){
            _reportDictionary.Add("REMARKS_" + _remarksNumber, "      ");
            _reportTable.Columns.Add("REMARKS_" + _remarksNumber, typeof (string));
            _remarksNumber++;
        }

        private AttendanceDayInfo GetDayInfo(GatheringSession session)
        {
            var isAttended = _sessionRetriever.IsAttendedThisGathering(session.Id, _brethrenId);
            var dayInfo = new AttendanceDayInfo { IsAttended = isAttended };
            if (dayInfo.IsAttended){
                CreateAttendedDayInfo(session, dayInfo);
            }
            else if (IsBrethrenIsNewlyAdded(session))
                dayInfo.DayAttendanceStatus = DayAttendanceStatus.NA;
            else{
                dayInfo.DayAttendanceStatus = DayAttendanceStatus.Absent;
            }
            return dayInfo;
        }

        private void CreateAttendedDayInfo(GatheringSession session, AttendanceDayInfo dayInfo)
        {
            dayInfo.DayAttendanceStatus = DayAttendanceStatus.Present;
            if (_lateIdentifier.IsBrethrenIsLate(_brethrenId, session.Id))
                dayInfo.DayAttendanceStatus = DayAttendanceStatus.Late;
        }

        private bool IsBrethrenIsNewlyAdded(GatheringSession session)
        {
            return session.Date < _lastUpdatedStatus;
        }

        private void MergeDictionaryAndTable()
        {
            var row = _reportTable.NewRow();
            foreach (KeyValuePair<string, object> keyValuePair in _reportDictionary)
            {
                string columnName = keyValuePair.Key;
                row[columnName] = Convert.ChangeType(keyValuePair.Value, _reportTable.Columns[columnName].DataType);
            }
            _reportTable.Rows.Add(row);
        }
    }
}
