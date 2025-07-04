using System;
using System.ComponentModel.DataAnnotations; 

namespace WebUI.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Order Date is required.")]
        [DataType(DataType.DateTime)] 
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Total Amount is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Total Amount must be greater than zero.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Shipping Address is required.")]
        [StringLength(500, ErrorMessage = "Shipping Address cannot exceed 500 characters.")]
        public string ShippingAddress { get; set; }
    }
}