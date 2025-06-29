using Microsoft.AspNetCore.Mvc;
using LMS_ManagementAPI.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace LMS_ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private static readonly string FilePath = "courses.json";
        private static List<Course> _courses = LoadFromFile();
        private static int _nextId = _courses.Any() ? _courses.Max(c => c.Id) + 1 : 1;

        private static List<Course> LoadFromFile()
        {
            if (!System.IO.File.Exists(FilePath))
                return new List<Course>();

            var json = System.IO.File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Course>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Course>();
        }

        private static void SaveToFile() =>
            System.IO.File.WriteAllText(FilePath,
                JsonSerializer.Serialize(
                    _courses,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }));

        [HttpGet]
        public ActionResult<IEnumerable<Course>> GetCourses() => _courses;

        [HttpPost]
        public ActionResult<Course> PostCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            course.Id = _nextId++;
            _courses.Add(course);
            SaveToFile();
            return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        public IActionResult PutCourse(int id, [FromBody] Course updated)
        {
            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            course.CourseName = updated.CourseName;
            course.Instructor = updated.Instructor;
            course.DurationWeeks = updated.DurationWeeks;
            SaveToFile();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            _courses.Remove(course);
            SaveToFile();
            return NoContent();
        }
    }
}
