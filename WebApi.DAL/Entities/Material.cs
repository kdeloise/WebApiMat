using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.DAL.Entities
{
    public enum MaterialCategories
    {
        Presentation = 1,
        Application = 2,
        Other = 3
    }

    public class Material
    {
        public int id { get; set; }
        public string materialName { get; set; }
        public string path { get; set; }
        public MaterialCategories category { get; set; }
        public int versionNumber { get; set; }
        public DateTime metaDateTime { get; set; }
        public double metaFileSize { get; set; }
    }
}
