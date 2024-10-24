using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace LibService
{
    public class StudentRepository
    {
        public List<Student> Students { get; set; } = new List<Student>();



        // Metodo per importare gli studenti dal file di configurazione
        //public void ImportStudents()
        //{
        //    string? url = ConfigurationManager.AppSettings["PathImportStudents"];
        //    try
        //    {
        //        string sStudent = File.ReadAllText(url);
        //        Students = JsonSerializer.Deserialize<List<Student>>(sStudent);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Errore durante l'importazione degli studenti: " + ex.Message);
        //    }
        //}

        // Metodo per stampare tutti gli studenti





    }
}
