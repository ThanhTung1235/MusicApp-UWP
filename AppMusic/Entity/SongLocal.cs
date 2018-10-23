using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AppMusic.Entity
{
    class SongLocal
    {
        private string _name;
        private string _singer;
        private string _album;
        private string _author;
        private string _time;
        private BitmapImage _thumbnai;
        private IRandomAccessStream _stream;
        private string type;

        public string Name { get => _name; set => _name = value; }
        public string Singer { get => _singer; set => _singer = value; }
        public string Album { get => _album; set => _album = value; }
        public string Author { get => _author; set => _author = value; }
        public string Time { get => _time; set => _time = value; }
        public BitmapImage Thumbnai { get => _thumbnai; set => _thumbnai = value; }
        public IRandomAccessStream Stream { get => _stream; set => _stream = value; }
        public string Type { get => type; set => type = value; }
    }
}
