using System.Collections.Generic;

namespace Tracky.Domain.EF.Music
{
    public class Album
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual string Title
        {
            get;
            set;
        }

        public virtual int ArtistId
        {
            get;
            set;
        }

        public virtual int GenreId
        {
            get;
            set;
        }
        public virtual int Year
        {
            get;
            set;
        }

        public virtual string Label
        {
            get;
            set;
        }

        public virtual ICollection<AlbumTrack> AlbumTracks
        {
            get;
            set;
        }

        public virtual Artist Artist
        {
            get;
            set;
        }

        public virtual Genre Genre
        {
            get;
            set;
        }
    }
}
