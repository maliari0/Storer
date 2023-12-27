using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storer.Models
{
    public class CartItem
    {
        [Key]
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total
        {
            get { return Price * Quantity; }
        }
        //[ForeignKey("MenuId")]
        //public Menu Menu { get; set; }
    }


}
