namespace Tracky.Domain.Entities.Music
{
    public class PlaylistTrack
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual int PlaylistId
        {
            get;
            set;
        }

        public virtual int PlaylistTrackNumber
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

        public virtual Playlist Playlist
        {
            get;
            set;
        }
    }
}
