﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Data.SQL.Models
{
    [Table("QA_TestPoints")]
    internal class TestPointDao
    {
        public Guid Id { get; set; }
        public int Level { get; set; }

        [Required]
        public Guid TestRunId { get; set; }
        public virtual TestRunDao TestRun { get; set; }

        public Guid? PressureId { get; set; }
        public virtual PressureTestDao Pressure { get; set; }

        public Guid? TemperatureId { get; set; }
        public virtual TemperatureTestDao Temperature { get; set; }

        public Guid? VolumeId { get; protected set; }
        public virtual VolumeTestDao Volume { get; set; }
    }
}