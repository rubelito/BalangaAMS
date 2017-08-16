using BalangaAMS.Core.Domain;

namespace BalangaAMS.WPF.View.DTO
{
    public class GatheringsListViewCheckDTO
    {
        public GatheringSession GatheringSession { get; set; }
        public bool IsCheck { get; set; }

        public string Date{
            get{
                return GatheringSession.Date.ToString("MMM dd, yyyy");
            }
        }
    }
}
