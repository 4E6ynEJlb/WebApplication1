using System.ComponentModel.DataAnnotations.Schema;

namespace LogicsLib
{
    public class ResizePicArgs
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public string PicName { get; set; }
    }
}
