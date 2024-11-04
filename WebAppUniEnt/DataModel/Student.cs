using System;
using System.Collections.Generic;

namespace WebAppUniEnt.DataModel;

public partial class Student
{
    public string Matricola { get; set; } = null!;

    public string Department { get; set; } = null!;

    public DateTime? AnnoDiIscrizione { get; set; }

    public string Name { get; set; } = null!;

    public string SureName { get; set; } = null!;

    public int Age { get; set; }

    public string Gender { get; set; } = null!;
}
