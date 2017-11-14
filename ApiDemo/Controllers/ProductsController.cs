using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiDemo.Data;
using ApiDemo.Data.Exceptions;
using ApiDemo.Models;

namespace ApiDemo.Controllers
{
	/// <summary>
	/// Manage Product Resources
	/// </summary>
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        // GET api/products[?{searchCriteria}]
		/// <summary>
		/// Get a list of Product representations.
		/// </summary>
		/// <param name="searchCriteria">Parameters that define search criteria. If null or empty, all products will be returned.</param>
		/// <returns>Array of Product representations.</returns>
        [HttpGet]
        public async Task<IActionResult> Get(ProductSearchCriteria searchCriteria = null)
        {
            if (searchCriteria == null || !searchCriteria.IsValid)
                return Ok(await ProductRepository.Current.All());
            else
                return Ok(await ProductRepository.Current.Find(searchCriteria.Match));
        }

        // GET api/products/{id}
		/// <summary>
		/// Get a single Product representaion by its ID.
		/// </summary>
		/// <param name="id">The string ID of the desired Product.</param>
		/// <returns>The matching Product representaion.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Product ID not specified.");
            var match = await ProductRepository.Current.GetById(id);
            if (match == null)
                return NotFound($"Product {id} Not Found");
            return Ok(match);
        }

        // POST api/products
		/// <summary>
		/// Create a new Product resource.
		/// </summary>
		/// <param name="product">The Product to create.</param>
		/// <returns>
		/// A successful response returns the Location header identifying the new resource.
		/// </returns>
        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            await ProductRepository.Current.Insert(product);
			return Created(Url.Action("Get", new {}), product);
        }

		// PUT api/products/{id}
		/// <summary>
		/// Update a Product resource.
		/// </summary>
		/// <param name="id">The string ID of the Product being updated.</param>
		/// <param name="product">All values for the Product to update.</param>
		/// <returns></returns>
		[HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, Product product)
        {
            try
            {
                await ProductRepository.Current.Update(product);
            }
            catch (ResourceNotFoundException ex)
            {
				return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(product);
        }

		// PUT api/products/{id}/description
		/// <summary>
		/// Change the description of a specified Product resource.
		/// </summary>
		/// <param name="id">The string ID of the Product being updated.</param>
		/// <param name="description">The new description.</param>
		/// <returns></returns>
		[HttpPut("{id}/description")]
        public async Task<IActionResult> Put(string id, [FromBody]string description)
        {
            var match = await ProductRepository.Current.GetById(id);
            if (match == null) return NotFound();
            match.Description = description;
            await ProductRepository.Current.Update(match);
            return Ok(match);
        }

		// PATCH api/products/{id}/price
		/// <summary>
		/// Change the price of a specified Product resource.
		/// </summary>
		/// <param name="id">The string ID of the Product being updated.</param>
		/// <param name="price">The new price.</param>
		/// <returns></returns>
		[HttpPatch("{id}/price")]
        public async Task<IActionResult> Patch(string id, [FromBody]decimal? price)
        {
            var match = await ProductRepository.Current.GetById(id);
            if (match == null) return NotFound();
            if (!price.HasValue) return BadRequest("Price must be specified.");
            match.Price = price.Value;
            await ProductRepository.Current.Update(match);
            return Ok(match);
        }

		// DELETE api/products/{id}
		/// <summary>
		/// Delete a specified Product resource.
		/// </summary>
		/// <param name="id">The string ID of the Product being deleted.</param>
		/// <returns></returns>
		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await ProductRepository.Current.RemoveById(id);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (ResourceException ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
    }

	/// <summary>
	/// Defines parameters for Product searches.
	/// </summary>
    public class ProductSearchCriteria
    {
		/// <summary>If set, matching Products must have a Price greater than this value.</summary>
		public decimal? PriceGreaterThan { get; set; }

		/// <summary>If set, matching Products must have a Price less than this value.</summary>
		public decimal? PriceLessThan { get; set; }

        internal bool IsValid
        {
            get
            {
                return PriceGreaterThan.HasValue || PriceLessThan.HasValue;
            }
        }
        internal bool Match(Product product)
        {
            if (!IsValid) return false;
            var isMatch = true;
            if (PriceGreaterThan.HasValue)
                isMatch = isMatch && product.Price > PriceGreaterThan.Value;
            if (PriceLessThan.HasValue)
                isMatch = isMatch && product.Price < PriceLessThan.Value;
            return isMatch;
        }
    }
}
