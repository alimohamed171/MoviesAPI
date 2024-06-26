﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Services;


namespace Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;

        public GenresController(IGenresService genresService)
        {
            _genresService = genresService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genresService.GetAll();   
            return Ok(genres);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(GenreDto dto)
        {
            Genre genre = new Genre
            {
                Name = dto.Name
            };
           await  _genresService.Add(genre);
            return Ok(genre);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(byte id, [FromBody] GenreDto dto)
        {
            var genre = await _genresService.GetById(id);    
            if (genre == null)
            {
                return NotFound($"No genre was found");
            }
            genre.Name = dto.Name;  
            
            _genresService.Update(genre);

            return Ok(genre);
        }

        [HttpDelete("{id}")]    
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genresService.GetById(id);
            if (genre == null)
            {
                return NotFound($"No genre was found");
            }
           _genresService.Delete(genre);

            return Ok(genre);
        }










    }









}
