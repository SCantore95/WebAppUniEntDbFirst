using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

using System.Linq;


namespace LibService
{
    public class DbManager
    {
        private readonly SqlConnection _connection = new();
        private SqlCommand _command = new();
        public readonly bool IsDbOnLine = false;
        
     

        public DbManager(string connectionString)
        {
            try
            {
                _connection.ConnectionString = connectionString;
                _connection.Open();
                IsDbOnLine = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }





        public List<Student> GetStudentsFromDatabase(String matricola)
        {
            List<Student> students = new List<Student>();

            Console.WriteLine("Inserisci la matricola dello studente da cercare: ");
            

            if (string.IsNullOrEmpty(matricola))
            {
                Console.WriteLine("Matricola non valida!");
                return students;
            }

            Console.WriteLine($"Ricerca studenti con matricola: {matricola}");

            // Definisci la query SQL per recuperare gli studenti
            string query = "SELECT Matricola, Department, AnnoDiIscrizione, name, Surename, Age, Gender FROM Students WHERE Matricola = @Matricola";

            // Usa ADO.NET per aprire la connessione e ottenere i dati
            using (SqlConnection conn = new SqlConnection(_connection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                // Aggiungi il parametro alla query
                cmd.Parameters.AddWithValue("@Matricola", matricola);

                try
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Studente non presente nel database.");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                Student student = new Student
                                {
                                    Matricola = reader["Matricola"].ToString(),
                                    Name = reader["name"]?.ToString(),
                                    Department = reader["Department"]?.ToString(),
                                    AnnoDiIscrizione = reader["AnnoDiIscrizione"] != DBNull.Value ? Convert.ToDateTime(reader["AnnoDiIscrizione"]) : null,
                                    SureName = reader["surename"]?.ToString(),
                                    Age = reader["Age"] != DBNull.Value ? Convert.ToInt32(reader["Age"]) : 0,
                                    Gender = reader["Gender"]?.ToString()
                                };

                                students.Add(student);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Errore nella connessione al database: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }

            return students;
        }
        public bool UpdateStudentInDatabase(string matricola, string newName, string newSurename, int newAge, string newGender, string newDepartment, DateTime? newAnnoDiIscrizione)
        {
            // Controllo se la matricola è valida
            if (string.IsNullOrEmpty(matricola))
            {
                Console.WriteLine("Matricola non valida!");
                return false;
            }

            // Definisci la query SQL per aggiornare lo studente
            string query = @"UPDATE Students 
                     SET Name = @NewName, 
                         Surename = @NewSurename, 
                         Age = @NewAge, 
                         Gender = @NewGender, 
                         Department = @NewDepartment, 
                         AnnoDiIscrizione = @NewAnnoDiIscrizione 
                     WHERE Matricola = @Matricola";

            // Usa ADO.NET per aprire la connessione e aggiornare i dati
            using (SqlConnection conn = new SqlConnection(_connection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                // Aggiungi i parametri alla query
                cmd.Parameters.AddWithValue("@Matricola", matricola);
                cmd.Parameters.AddWithValue("@NewName", newName);
                cmd.Parameters.AddWithValue("@NewSurename", newSurename);
                cmd.Parameters.AddWithValue("@NewAge", newAge);
                cmd.Parameters.AddWithValue("@NewGender", newGender);
                cmd.Parameters.AddWithValue("@NewDepartment", newDepartment);

                // Gestisci il valore null per AnnoDiIscrizione
                if (newAnnoDiIscrizione.HasValue)
                {
                    cmd.Parameters.AddWithValue("@NewAnnoDiIscrizione", newAnnoDiIscrizione.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@NewAnnoDiIscrizione", DBNull.Value);
                }

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery(); // Esegui l'aggiornamento

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Studente aggiornato correttamente.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Nessuno studente trovato con la matricola specificata.");
                        return false;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Errore durante l'aggiornamento dello studente: {ex.Message}");
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public bool AddStudent(Student newStudent)
        {
            // Definisci la query SQL per l'inserimento
            string query = "INSERT INTO Students (Matricola, Name, Surename, Age, Gender, Department, AnnoDiIscrizione) " +
                           "VALUES (@Matricola, @Name, @Surename, @Age, @Gender, @Department, @AnnoDiIscrizione)";

            // Usa ADO.NET per aprire la connessione e inserire i dati
            using (SqlConnection conn = new SqlConnection(_connection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                // Aggiungi i parametri alla query
                cmd.Parameters.AddWithValue("@Matricola", newStudent.Matricola);
                cmd.Parameters.AddWithValue("@Name", newStudent.Name);
                cmd.Parameters.AddWithValue("@Surename", newStudent.SureName);
                cmd.Parameters.AddWithValue("@Age", newStudent.Age);
                cmd.Parameters.AddWithValue("@Gender", newStudent.Gender);
                cmd.Parameters.AddWithValue("@Department", newStudent.Department);
                cmd.Parameters.AddWithValue("@AnnoDiIscrizione", newStudent.AnnoDiIscrizione);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery(); // Esegui l'inserimento

                    // Restituisci true se almeno una riga è stata influenzata
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Errore durante l'inserimento dello studente: {ex.Message}");
                    return false; // Restituisci false in caso di errore
                }
            }
        }

        public bool DeleteStudentFromDatabase(string matricola)
        {
            string query = "DELETE FROM Students WHERE Matricola = @Matricola";

            using (SqlConnection conn = new SqlConnection(_connection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Matricola", matricola);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Se viene eliminata almeno una riga, l'operazione ha avuto successo
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Errore durante l'eliminazione dello studente: {ex.Message}");
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void CheckOpenedDB()
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
                _connection.Open();
        }

        private void CheckClosedDB(SqlDataReader? dataReader)
        {
            dataReader?.Close();

            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }
}
