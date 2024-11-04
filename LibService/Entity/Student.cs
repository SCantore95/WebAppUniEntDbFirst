using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace LibService
{
    public class Student : Person
    {
        [BsonId]
        public ObjectId MongoId { get; set; } // Renamed to avoid conflict with the 'id' property
        // Change Id type back to int, map to the "id" field
        [BsonElement("id")]
        public int Id { get; set; } // Match the 'id' field from the MongoDB document

        public string Matricola { get; set; }
        public string Department { get; set; }

        [BsonElement("AnnoDiIscrizione")]
        public DateTime? AnnoDiIscrizione { get; set; } // Match field names for deserialization

        public List<Exam> Exams { get; set; } = new();

        public long Timestamp { get; set; }
        public DateTime CreationTime { get; set; }
        public Student() : base() { }


        public Student(string name, string sureName, int age, string gender, string matricola, string department, DateTime annoDiIscrizione)
            : base(name, sureName, age, gender)
        {
            Department = department;
            Matricola = matricola;
            AnnoDiIscrizione = annoDiIscrizione;
        }

        public override string ToString()
        {
            return $"{base.ToString()}|Matricola: {Matricola}| Corso: {Department}| Anno di iscrizione: {AnnoDiIscrizione}|";
        }

       
       
    }
}
