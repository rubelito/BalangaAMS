using System.Collections.Generic;
using BalangaAMS.Core.HelperDomain;
using BalangaAMS.Core.Interfaces;

namespace BalangaAMS.ApplicationLayer.Interfaces.ImportExcelData
{
    public interface IImportbrethren
    {
        List<DatatoImport> Loadbretrhen();
    }
}
