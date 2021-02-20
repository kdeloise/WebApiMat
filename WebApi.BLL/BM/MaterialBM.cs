using System.Collections.Generic;

namespace WebApi.BLL.BM
{
    public class MaterialBM
    {
        public string MaterialName { get; set; }
        public int Category { get; set; }
        public int ActualVersion { get; set; }
        public List<MaterialVersionBM> Versions { get; set; }
    }
}
