using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiDemo.Models;
using ApiDemo.Data.Exceptions;

namespace ApiDemo.Data
{
	public class ProductRepository
	{
		private static ProductRepository _singleton;
		private List<Product> _allProducts;

		private ProductRepository() => initializeRepository();

		///<summary>Returns a Singleton instance of the Product Repository</summary>
		public static ProductRepository Current
		{
			get
			{
				if (_singleton == null) _singleton = new ProductRepository();
				return _singleton;
			}
		}

		///<summary>Returns all products in the current repository.</summary>
		public async Task<List<Product>> All() => _allProducts;

		///<summary>Returns all products in the current repository that match a given expression.</summary>
		public async Task<List<Product>> Find(Func<Product, bool> predicate) => _allProducts?.Where(predicate).ToList();

		///<summary>Returns one product from the current repository that matches a specified ID, or null if not found.</summary>
		public async Task<Product> GetById(string id) => _allProducts?.SingleOrDefault(p => p.ID == id);

		public async Task Insert(Product product)
		{
			if (product == null)
				throw new ResourceException($"No product data to insert.");
			if (string.IsNullOrWhiteSpace(product.ID))
				throw new ResourceException("Product must have an ID.");
			if (string.IsNullOrWhiteSpace(product.Description))
				throw new ResourceException("Product must have a description.");
			if (product.Price < 0.01m)
				throw new ResourceException("Product must have a valid price.");
			var match = _allProducts?.FirstOrDefault(p => p.ID == product.ID);
			if (match != null)
				throw new ResourceException($"A product with ID {product.ID} already exists.");
			else
				_allProducts.Add(product);
		}

		public async Task Update(Product product)
		{
			if (product == null)
				throw new ResourceException($"No product data to update.");
			if (string.IsNullOrWhiteSpace(product.ID))
				throw new ResourceException("Product must have an ID.");
			if (string.IsNullOrWhiteSpace(product.Description))
				throw new ResourceException("Product must have a description.");
			if (product.Price < 0.01m)
				throw new ResourceException("Product must have a valid price.");
			var match = _allProducts?.FirstOrDefault(p => p.ID == product.ID);
			if (match != null)
				match = product;
			else
				throw new ResourceNotFoundException($"No product with ID {product.ID} exists.");
		}

		public async void RemoveById(string id) => _allProducts?.Remove(_allProducts?.FirstOrDefault(p => p.ID == id));

		private void initializeRepository()
		{
			_allProducts = new List<Product>
			{
				new Product{ ID = "N029", Description = "Product #N029", Price = 12.99m },
				new Product{ ID = "K865", Description = "Product #K865", Price = 14.99m },
				new Product{ ID = "E654", Description = "Product #E654", Price = 19.99m },
				new Product{ ID = "A982", Description = "Product #A982", Price = 11.99m },
			};
		}
	}
}