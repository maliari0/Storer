using System;
using System.ComponentModel.DataAnnotations;

namespace Storer.Models
{
    public class Hakkında
    {
        [Key]
        public int Id { get; set; }
     
        public string Title { get; set; }
       
    }
}
