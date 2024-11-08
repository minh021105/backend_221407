using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend_221407.Models
{
    [Table("Suppliers")]
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }
        public String? CompanyName { get; set; }
        public String? ContactTitle { get; set; }
        public String? Address { get; set; }
        public String? City { get; set; }
        public String? Region { get; set; }
        public String? PostalCode { get; set; }
        public String? Country { get; set; }
        public String? Phone { get; set; }
        public String? Fax { get; set; }
        public String? HomePage { get; set; }

    }
}
