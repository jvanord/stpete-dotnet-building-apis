using System;
using System.ComponentModel.DataAnnotations;

namespace ApiDemo.Models
{
	/// <summary>A Product Resource</summary>
	public class Product
	{
		/// <summary>The unique string identifier of this resource.</summary>
		[Required, MaxLength(32)]
		public string ID { get; set; }

		/// <summary>A human-readable description of the resource.</summary>
		[Required, MaxLength(512)]
		public string Description { get; set; }

		/// <summary>The list price of this product.</summary>
		[Range(0.01, double.MaxValue)]
		public decimal Price { get; set; }
	}
}