using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicsLib
{
    public class PicProps
    {
        public PicProps(string path, string extension, string name)
        {
            Path = path;
            Extension = extension;
            Name = name;
        }
        public string Path;
        public string Extension;
        public string Name;
    }
}
