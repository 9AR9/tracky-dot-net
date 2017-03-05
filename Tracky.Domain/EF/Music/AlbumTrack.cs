namespace Tracky.Domain.EF.Music
{
    public class AlbumTrack
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual int AlbumId
        {
            get;
            set;
        }

        public virtual int AlbumTrackNumber
        {
            get;
            set;
        }

        public virtual int SongId
        {
            get;
            set;
        }

        public virtual Song Song
        {
            get;
            set;
        }

        public virtual Album Album
        {
            get;
            set;
        }
    }
}
