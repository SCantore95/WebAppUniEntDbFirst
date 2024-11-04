using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using LibService;
using MongoDB.Driver;
using Academy6MongoDb.Controllers;
using WebAppUniEnt.DataModel;
using MongoDB.Bson;
using static LibService.Student;

namespace WebAppUniEnt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MongoDBController : Controller
    {
        private IMongoCollection<LibService.Student> mongoCollection;

        public MongoDBController(IOptions<StudentDbCnfig> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            var mongoDB = mongoClient.GetDatabase(options.Value.DatabaseName);
            mongoCollection = mongoDB.GetCollection<LibService.Student>(options.Value.CollectionName);
        }
        [HttpGet]
        public async Task<List<LibService.Student>> GetStudent()
        {
            return await mongoCollection.Find(_ => true).ToListAsync();
        }
        [HttpGet("{matricola}")]
        public async Task<List<LibService.Student>> GetMatricola(string matricola)
        {
            return await mongoCollection.Find(emp => emp.Matricola.ToLower() == matricola.ToLower()).ToListAsync();
        }

        [HttpDelete("{matricola}")]
        public async Task<DeleteResult> DeleteEmployee(string matricola){
            return await mongoCollection.DeleteOneAsync(emp => emp.Matricola == matricola);
        }
        [HttpPost]
        public async Task<ActionResult> InsertStudent(LibService.Student student)
        {
            if (student == null)
            {
                return BadRequest("Student object cannot be null.");
            }

            // Validate required fields
            if (string.IsNullOrEmpty(student.Name) ||
                string.IsNullOrEmpty(student.SureName) ||
                string.IsNullOrEmpty(student.Matricola) ||
                string.IsNullOrEmpty(student.Department))
            {
                return BadRequest("Name, SureName, Matricola, and Department are required fields.");
            }

            // No need to set MongoId or call InitializeTimestamps here
            // The constructor will handle it automatically

            try
            {
                await mongoCollection.InsertOneAsync(student);
                return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student); // Assuming you have a method to retrieve the student
            }
            catch (MongoWriteException ex)
            {
                return Conflict($"A student with the same Matricola already exists: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudent(int id, LibService.Student student)
        {
            var filter = Builders<LibService.Student>.Filter.Eq(s => s.Id, id);

            var update = Builders<LibService.Student>.Update
                .Set(s => s.Matricola, student.Matricola)
                .Set(s => s.Department, student.Department);

            try
            {
                var result = await mongoCollection.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    return NotFound($"No student found with ID: {id}");
                }

                if (result.ModifiedCount == 0)
                {
                    return Conflict("Document found but no fields were modified. Possible identical data.");
                }

                return Ok("Student updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception here (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
