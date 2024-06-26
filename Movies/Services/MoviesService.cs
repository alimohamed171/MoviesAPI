
using Microsoft.EntityFrameworkCore;

namespace Movies.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly AppDbContext _context;

        public MoviesService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Movie> Add(Movie movie)
        {
            await _context.AddAsync(movie);
            _context.SaveChanges();

            return movie;

        }

        public Movie Delete(Movie movie)
        {
            _context.Remove(movie);
            _context.SaveChanges();

            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
           return await _context.Movies
                .Where(m=> m.GenreId == genreId || genreId==0)
                .OrderByDescending(x => x.Rate)
                .Include(m => m.Genre)
                //.Select(m => new MovieDetailsDto
                //{
                //    Id = m.Id,
                //    GenreId = m.GenreId,
                //    GenreName = m.Genre.Name,
                //    Poster = m.Poster,
                //    Rate = m.Rate,
                //    Storyline = m.Storyline,
                //    Title = m.Title,
                //    Year = m.Year
                //})
                .ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
           return await _context.Movies
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public Movie Update(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();

            return movie;
        }
    }
}
