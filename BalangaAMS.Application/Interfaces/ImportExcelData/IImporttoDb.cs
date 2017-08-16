using System.Collections.Generic;
using BalangaAMS.Core.Interfaces;
using BalangaAMS.Core.HelperDomain;
namespace BalangaAMS.ApplicationLayer.Interfaces.ImportExcelData
{
    public interface IImporttoDb: IReturnStatus
    {
        bool Import(List<DatatoImport> members);
    }
}