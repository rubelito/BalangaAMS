using System.Collections.Generic;
using BalangaAMS.Core.Domain;

namespace BalangaAMS.WPF.View.HelperClass
{
    internal class ThreadObjectBrethrenMasterList
    {
        public ThreadObjectBrethrenMasterList(List<BrethrenBasic> brethrenList, string fileName)
        {
            BrethrenList = brethrenList;
            FileName = fileName;
        }

        public List<BrethrenBasic> BrethrenList { get; set; }
        public string FileName { get; set; }
    }
}