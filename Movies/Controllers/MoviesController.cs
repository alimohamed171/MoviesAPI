using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Services;

namespace Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;


        public MoviesController(IMoviesService moviesService, IGenresService genresService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genresService = genresService;
            _mapper = mapper;
        }

        private new List<string> _allowedExtenstions = new List<string>
        {
            ".jpg",
            ".png"
        };
        private long _maxAllowedPostersSize = 1048576; // 1MB //1024*1024
        


        [HttpGet("GetAllMovies")]
        public async Task<IActionResult> GetAllAsync() {

            var movies = await _moviesService.GetAll();

            //MAP DTO
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);

            return Ok(movies);
        } 

        [HttpGet("GetMovieByID{Id}")]
        public async Task<IActionResult> GetByIdAsync(int Id) {

            var movie = await _moviesService.GetById(Id);

            if (movie == null) {
                return NotFound();
            }

            
            //var dto = new MovieDetailsDto
            //{
            //    Id = movie.Id,
            //    GenreId = movie.GenreId,
            //    GenreName = movie.Genre.Name,
            //    Poster = movie.Poster,
            //    Rate = movie.Rate,
            //    Storyline = movie.Storyline,
            //    Title = movie.Title,
            //    Year = movie.Year
            //};
            var dto = _mapper.Map<MovieDetailsDto>(movie);      
                
            return Ok(dto);
        }

        [HttpGet("GetMoviesByGenreID{Id}")]
        public async Task<IActionResult> GetByGenreIdAsync(byte Id)
        {

            var movies = await _moviesService.GetAll(Id);
            //MAP DTO
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);

            return Ok(movies);

            return Ok(movies);
        }



        [HttpPost("CreateMovie")]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null) return BadRequest("poster is requierd");

            if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg are allowed");

            if (dto.Poster.Length > _maxAllowedPostersSize)
                return BadRequest("Max allowed size is 1MB");

            var isValidGenre = await _genresService.IsvalidGenre(dto.GenreId);
           
            if(!isValidGenre)
                return BadRequest($"No Genre with this id: {dto.GenreId} ");

            using var dataStream = new MemoryStream();

            await dto.Poster.CopyToAsync(dataStream);

            //var movie = new Movie
            //{
            //    GenreId = dto.GenreId,
            //    Title = dto.Title,
            //    Poster = dataStream.ToArray(),
            //    Rate = dto.Rate,
            //    Storyline = dto.Storyline,
            //    Year = dto.Year,
            //};
            // MAP
            // MAP
            // MAP
            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();

            
            _moviesService.Add(movie);

            return Ok(movie);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovieAsync(int id ,[FromForm] MovieDto dto)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null) return NotFound("No movie");

            var isValidGenre = await _genresService.IsvalidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest($"No Genre with this id: {dto.GenreId} ");

            if (dto.Poster != null)
            {

                if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg are allowed");

                if (dto.Poster.Length > _maxAllowedPostersSize)
                    return BadRequest("Max allowed size is 1MB");

                using var dataStream = new MemoryStream();

                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();

            }

            movie.Title = dto.Title;    
            movie.Storyline = dto.Storyline;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.GenreId = dto.GenreId;


           _moviesService.Update(movie);

            return Ok(movie);


        }






        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null) return NotFound("No movie");

            _moviesService.Delete(movie);

            return Ok(movie);
        }








    }
}
