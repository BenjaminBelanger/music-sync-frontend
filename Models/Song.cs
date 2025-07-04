using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_sync_frontend.Models
{
    public class Song
    {
        private string albumId;
        private int position;
        private int time;
        private int length;

        public Song(string albumId, int position, int time)
        {
            this.albumId = albumId;
            this.position = position;
            this.time = time;
        }

        public Song(string albumId, int position, int time, int length)
        {
            this.albumId = albumId;
            this.position = position;
            this.time = time;
            this.length = length;
        }

        public string AlbumId
        {
            get { return albumId; }
            set { albumId = value; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public override string ToString()
        {
            return $"AlbumId={AlbumId}, Position={Position}, Time={Time}";
        }
    }
}
