using System.Collections.Generic;

namespace WebApi.DAL.Entities
{
    public class Material
    {
        public int Id { get; set; }
        public string MaterialName { get; set; }
        public int Category { get; set; }
        public int ActualVersion { get; set; }
        public List<MaterialVersion> Versions { get; set; }
    }
}
