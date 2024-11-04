using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibService;

namespace WebAppUniEnt.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentController2 : ControllerBase
    {
        private readonly DbManager accessDB;

        public StudentController2()
        {
            accessDB = new DbManager(StaticaDbConnection.ConnectionString);
        }

        // POST api/<StudentController> - Aggiungi un nuovo studente
        [HttpPost("AggiungiNuovoStudente")]
        public IActionResult Post([FromBody] Student newStudent)
        {
            if (newStudent == null)
            {
                return BadRequest("Dati dello studente non validi.");
            }

            // Aggiungi il nuovo studente al database
            bool success = accessDB.AddStudent(newStudent);
            if (success)
            {
                return CreatedAtAction(nameof(Get), new { matricola = newStudent.Matricola }, newStudent); // 201 Created
            }

            return BadRequest("Errore durante l'aggiunta dello studente.");
        }

        // GET: api/<StudentController> - Recupera uno o più studenti tramite la matricola
        [HttpGet("{Matricola}")]
        public IActionResult Get(string Matricola)
        {
            var students = accessDB.GetStudentsFromDatabase(Matricola);

            if (students == null || students.Count == 0)
            {
                return NotFound("Studente non trovato.");
            }

            return Ok(students);
        }

        // PUT api/<StudentController> - Aggiorna i dati dello studente
        [HttpPut("AggiornaStudente/{Matricola}")]
        public IActionResult Put(string Matricola, [FromBody] Student updatedStudent)
        {
            if (updatedStudent == null || Matricola != updatedStudent.Matricola)
            {
                return BadRequest("Dati non validi o matricola errata.");
            }

            // Chiama il metodo per aggiornare i dati dello studente
            bool success = accessDB.UpdateStudentInDatabase(
                Matricola,
                updatedStudent.Name,
                updatedStudent.SureName,
                updatedStudent.Age,
                updatedStudent.Gender,
                updatedStudent.Department,
                updatedStudent.AnnoDiIscrizione
            );

            if (success)
            {
                return NoContent(); // 204 No Content, aggiornamento riuscito
            }

            return NotFound("Studente non trovato.");
        }

        // DELETE api/<StudentController> - Elimina uno studente in base alla matricola
        [HttpDelete("EliminaStudente/{Matricola}")]
        public IActionResult Delete(string Matricola)
        {
            if (string.IsNullOrEmpty(Matricola))
            {
                return BadRequest("Matricola non valida.");
            }

            // Chiama il metodo per eliminare lo studente dal database
            bool success = accessDB.DeleteStudentFromDatabase(Matricola);
            if (success)
            {
                return NoContent(); // 204 No Content, eliminazione riuscita
            }

            return NotFound("Studente non trovato.");
        }
    }
}
