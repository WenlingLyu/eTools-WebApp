﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesReturnsSystem.Entities
{
    internal partial class Category
    {
        public Category()
        {
            StockItems = new HashSet<StockItem>();
        }

        [Key]
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(50, ErrorMessage = "Description has a limit of 50 characters.")]
        [Unicode(false)]
        public string Description { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<StockItem> StockItems { get; set; }
    }
}