using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.ApplicationLayer.Interfaces.ExportData
{
    public interface IExportBrethren
    {
        void ExportBrethren(List<BrethrenBasic> brethrenList, string destinationPath);
    }
}
