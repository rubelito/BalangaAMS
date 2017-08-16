using System;
using System.Collections.Generic;
using System.Linq;
using BalangaAMS.ApplicationLayer.DTO;
using BalangaAMS.ApplicationLayer.Interfaces;
using BalangaAMS.Core.Domain;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class AttendanceRetriever : IAttendanceRetriever
    {
        private readonly IChurchGatheringRetriever _sessionRetriever;
        private readonly IBrethrenManager _brethrenManager;
        private readonly IAttendanceLogRetriever _logRetriever;

        public int DaysToConsiderNewlyBaptised { get; set; }

        public AttendanceRetriever(IChurchGatheringRetriever sessionRetriever,
            IBrethrenManager brethrenManager, IAttendanceLogRetriever logRetriever){
            _sessionRetriever = sessionRetriever;
            _brethrenManager = brethrenManager;
            _logRetriever = logRetriever;
        }

        public List<BrethrenBasic> GetBrethrenWhoAttendedThisGathering(GatheringSession session){
            var attendancelogs = _logRetriever.GetAttendanceLogInSession(session);
            return GetBrethrenInAttendanceLogs(attendancelogs);
        }

            private List<BrethrenBasic> GetBrethrenInAttendanceLogs(List<AttendanceLog> attendanceLogs){
                var brethrenList = new List<BrethrenBasic>();
                foreach (var log in attendanceLogs){
                    var brethren = _brethrenManager.GetBrethrenbyId(log.BrethrenId);
                    if (brethren != null && brethren.LocalStatus == LocalStatus.Present_Here)
                        brethrenList.Add(brethren);
                }
                return brethrenList.Distinct().ToList();
            }

        public List<BrethrenBasic> GetBrethrenWhoIsAbsentToThisGathering(GatheringSession session){
            var attendedBrethren = GetBrethrenWhoAttendedThisGathering(session);
            var brethrenList = _brethrenManager.FindBrethren(b => b.LocalStatus == LocalStatus.Present_Here);
            return brethrenList.Except(attendedBrethren).ToList();
        } 

        public List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGathering(GatheringSession session){
            var attendanceInfoList = new List<AttendanceInfoDTO>();
            var brethrenList = GetBrethrenWhoAttendedThisGathering(session);
            foreach (var brethren in brethrenList){
                var attendanceLogs = _logRetriever.GetBrethrenAttendanceLogInSession(brethren.Id, session);
                var groupName = GetBrethrenGroupName(brethren);
                var attendanceDTO = CreateAttendanceInfoDTO(brethren, groupName, attendanceLogs);
                attendanceInfoList.Add(attendanceDTO);
            }
            return attendanceInfoList;
        }

            private AttendanceInfoDTO CreateAttendanceInfoDTO(BrethrenBasic brethren, string groupName,
                AttendanceLog attendanceLog){
                var dayInfo = new AttendanceDayInfo{
                    DateAndTime = attendanceLog.DateTime,
                    IsAttended = true,
                    DayAttendanceStatus = attendanceLog.IsLate ? DayAttendanceStatus.Late : DayAttendanceStatus.Present
                };
                var attendanceDTO = new AttendanceInfoDTO{
                    AttendaceDayInfo = dayInfo,
                    Brethren = brethren,
                    GroupName = groupName
                };
                return attendanceDTO;
            }

        private string GetBrethrenGroupName(BrethrenBasic brethren){
            string groupName;
            if (brethren.Group == null)
                groupName = "No Group";
            else if (_brethrenManager.IsNewlyBaptised(brethren, DaysToConsiderNewlyBaptised, DateTime.Now)){
                groupName = "Newly Baptized";
            }
            else{
                groupName = brethren.Group.GroupName;
            }
            return groupName;
        }

        public List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringLive(GatheringSession session){
            var attendanceInfoList = GetAttendanceInfoOfBrethrenWhoAttendedThisGathering(session);
            var attendedLive = new List<AttendanceInfoDTO>();
            foreach (var infoDTO in attendanceInfoList){
                if (IsAttendedThatDay(infoDTO, session))
                    attendedLive.Add(infoDTO);
            }
            return attendedLive;
        }

        public List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringNotLive(GatheringSession session){
            var attendanceInfoList = GetAttendanceInfoOfBrethrenWhoAttendedThisGathering(session);
            var attendedNotLive = new List<AttendanceInfoDTO>();
            foreach (var infoDTO in attendanceInfoList)
            {
                if (!IsAttendedThatDay(infoDTO, session))
                    attendedNotLive.Add(infoDTO);
            }
            return attendedNotLive;
        }

        private bool IsAttendedThatDay(AttendanceInfoDTO attendanceDayInfo, GatheringSession session)
        {
            var date = attendanceDayInfo.AttendaceDayInfo.DateAndTime;
            return session.Date.Year == date.Year &&
                   session.Date.Month == date.Month &&
                   session.Date.Day == date.Day;
        }

        public List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoAttendedThisGatheringLate(GatheringSession session)
        {
            var attendanceInfoList = GetAttendanceInfoOfBrethrenWhoAttendedThisGathering(session);
            return
                attendanceInfoList.Where(a => a.AttendaceDayInfo.DayAttendanceStatus == DayAttendanceStatus.Late)
                    .ToList();
        }

        public List<AttendanceInfoDTO> GetAttendanceInfoOfBrethrenWhoIsAbsentInThisGathering(GatheringSession session){
            var brethrenList = _brethrenManager.GetAllBrethren()
                 .Where(b => b.LocalStatus == LocalStatus.Present_Here).ToList();
            var absentBrethrenList = GetAbsentBrethren(brethrenList, session);
            var absentAttendanceInfo = CreateAbsentAttendanceInfoDTOList(absentBrethrenList);
            return absentAttendanceInfo;
        }

            private List<BrethrenBasic> GetAbsentBrethren(List<BrethrenBasic> brethrenList, GatheringSession session){
                var absentBrethrenList = new List<BrethrenBasic>();
                foreach (var brethren in brethrenList){
                    if (!_sessionRetriever.IsAttendedThisGathering(session.Id, brethren.Id))
                        absentBrethrenList.Add(brethren);
                }
                return absentBrethrenList;
            }

            private List<AttendanceInfoDTO> CreateAbsentAttendanceInfoDTOList(List<BrethrenBasic> absentBrethrenList){
                var absentAttendanceInfo = new List<AttendanceInfoDTO>();
                foreach (var brethren in absentBrethrenList){
                    var groupName = GetBrethrenGroupName(brethren);
                    var attendanceInfo = CreateAbsentAttendanceInfoDTO(brethren, groupName);
                    absentAttendanceInfo.Add(attendanceInfo);
                }
                return absentAttendanceInfo;
            }

                private AttendanceInfoDTO CreateAbsentAttendanceInfoDTO(BrethrenBasic brethren, string groupName){
                    var dayInfo = new AttendanceDayInfo{DayAttendanceStatus = DayAttendanceStatus.Absent};
                    var attendanceDTO = new AttendanceInfoDTO{
                        AttendaceDayInfo = dayInfo,
                        Brethren = brethren,
                        GroupName = groupName
                    };
                    return attendanceDTO;
                }

        public bool IsAlreadyLogin(long brethrenId, GatheringSession session){
            var log = _logRetriever.GetBrethrenAttendanceLogInSession(brethrenId, session);
            return log != null;
        }

    }
}
