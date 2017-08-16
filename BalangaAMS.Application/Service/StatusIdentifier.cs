using System.Linq;
using BalangaAMS.Core.Domain.Enum;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class StatusIdentifier : IStatusIdentifier
    {
        private readonly IBrethrenManager _brethrenManager;
        private readonly IChurchGatheringRetriever _sessionRetriever;

        public StatusIdentifier(IBrethrenManager brethrenManager, IChurchGatheringRetriever sessionRetriever){
            _brethrenManager = brethrenManager;
            _sessionRetriever = sessionRetriever;
        }

        public AttendanceStatus GetStatusForMonthOf(long brethrenId, MonthofYear monthofYear, int year){
            AttendanceStatus attendanceStatus;
            var brethren = _brethrenManager.FindBrethren(b => b.Id == brethrenId).FirstOrDefault();

            if (IsBrethrenActive(brethren, monthofYear, year))
                attendanceStatus = AttendanceStatus.Active;
            else
                attendanceStatus = AttendanceStatus.Inactive;
            return attendanceStatus;
        }

            private bool IsBrethrenActive(BrethrenBasic brethren, MonthofYear monthofYear, int year){
                return IsAttendedIsGreaterThanAbsent(brethren, monthofYear, year) ||
                       IsBaptisedThisMonth(brethren, monthofYear, year) ||
                       IsAddedOnlyThisMonth(brethren, monthofYear, year);
            }

                private bool IsAttendedIsGreaterThanAbsent(BrethrenBasic brethren, MonthofYear monthofYear, int year){
                    var attendedSession = _sessionRetriever.GetGatheringsThatBrethrenAttendedForTheMonthOf(
                        brethren.Id, monthofYear, year);
                    return attendedSession.Count >= GetNumberOfPresentTobeActive(monthofYear, year);
                }

                private bool IsBaptisedThisMonth(BrethrenBasic brethren, MonthofYear monthofYear, int year){
                    bool isBaptisedThisMonth = brethren.BrethrenFull.DateofBaptism.HasValue;
                    if (isBaptisedThisMonth){
                        isBaptisedThisMonth = brethren.BrethrenFull.DateofBaptism.Value.Year == year &&
                                              brethren.BrethrenFull.DateofBaptism.Value.Month == (int) monthofYear;
                    }
                    return isBaptisedThisMonth;
                }

                private bool IsAddedOnlyThisMonth(BrethrenBasic brethren, MonthofYear monthofYear, int year){
                    return brethren.LastStatusUpdate.Year == year &&
                           brethren.LastStatusUpdate.Month == (int) monthofYear;
                }

        public AttendanceStatus GetStatusForLast12Session(long brethrenId){
            AttendanceStatus attendanceStatus;
            var missedGatheringSession = _sessionRetriever
                .GetGatheringsThatBrethrenDidntAttendForLast12Session(brethrenId);
            var brethren = _brethrenManager.FindBrethren(b => b.Id == brethrenId).FirstOrDefault();
            var totalMissedGatheringSession =
                missedGatheringSession.Where(m => m.Date >= brethren.LastStatusUpdate).ToList();

            if (totalMissedGatheringSession.Count() > 6)
                attendanceStatus = AttendanceStatus.Inactive;
            else
                attendanceStatus = AttendanceStatus.Active;
            return attendanceStatus;
        }

        public int GetNumberOfAbsentToBeInactive(MonthofYear monthofYear, int year){
            var gatherings =
                _sessionRetriever.GetAllStartedRegularGatheringsForMonthOf(monthofYear, year)
                    .Where(g => g.IsStarted)
                    .ToList();
            int absetToBeInactive = gatherings.Count/2;
            return absetToBeInactive;
        }

        public int GetNumberOfAbsentToBeInactive(BrethrenBasic brethren, MonthofYear monthofYear, int year){
            var gatherings =
                _sessionRetriever.GetAllStartedRegularGatheringsForMonthOf(monthofYear, year)
                    .Where(g => g.IsStarted && g.Date >= brethren.LastStatusUpdate).ToList();

            int absetToBeInactive = gatherings.Count/2;
            return absetToBeInactive;
        }

        public int GetNumberOfPresentTobeActive(MonthofYear monthofYear, int year){
            int presentToBeActive;
            var gatherings =
                _sessionRetriever.GetAllStartedRegularGatheringsForMonthOf(monthofYear, year)
                    .Where(g => g.IsStarted).ToList();

            if (IsGatheringCountIsOddNumber(gatherings.Count))
                presentToBeActive = AddOneToMakeEvenNumber(gatherings.Count);
            else
                presentToBeActive = gatherings.Count/2;

            return presentToBeActive;
        }

            private bool IsGatheringCountIsOddNumber(int gatheringsCount){
                return gatheringsCount%2 != 0;
            }

            private int AddOneToMakeEvenNumber(int gatheringsCount){
                return (gatheringsCount / 2) + 1;
            }
    }
}