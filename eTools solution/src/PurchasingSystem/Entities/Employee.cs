﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PurchasingSystem.Entities
{
    internal partial class Employee
    {
        public Employee()
        {
            PurchaseOrders = new HashSet<PurchaseOrder>();
        }

        [Key]
        public int EmployeeID { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(25, ErrorMessage = "First Name has a limit of 25 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(25, ErrorMessage = "Last Name has a limit of 25 characters.")]
        public string LastName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateHired { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateReleased { get; set; }
        public int PositionID { get; set; }
        [StringLength(30, ErrorMessage = "Login ID has a limit of 25 characters.")]
        public string LoginID { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(75, ErrorMessage = "Address has a limit of 25 characters.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "City is required.")]
        [StringLength(30, ErrorMessage = "City has a limit of 25 characters.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Contact Phone is required.")]
        [StringLength(12, ErrorMessage = "Contact Phone has a limit of 25 characters.")]
        public string ContactPhone { get; set; }
        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(6, ErrorMessage = "Postal Code has a limit of 25 characters.")]
        public string PostalCode { get; set; }

        [ForeignKey("PositionID")]
        [InverseProperty("Employees")]
        public virtual Position Position { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}