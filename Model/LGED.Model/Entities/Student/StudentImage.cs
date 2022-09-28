using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LGED.Model.Entities.Student
{
    public class StudentImage : EntityBase
    {
        public Guid EmpId { get; set; }
        public string EmpName { get; set; }
        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string ? ImageSrc { get; set; }
    }
}