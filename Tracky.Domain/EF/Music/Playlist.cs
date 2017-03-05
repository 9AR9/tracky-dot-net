using System.Collections.Generic;

namespace Tracky.Domain.EF.Music
{
    public class Playlist
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual ICollection<PlaylistTrack> PlaylistTracks
        {
            get;
            set;
        }
    }
}
