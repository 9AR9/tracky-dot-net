namespace Tracky.Domain.EF.Music
{
    public class Song
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

        public virtual Genre Genre
        {
            get;
            set;
        }

        public virtual Artist Artist
        {
            get;
            set;
        }

    }
}
