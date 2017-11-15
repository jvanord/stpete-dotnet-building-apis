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
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        // GET api/products[?{searchCriteria}]
        [HttpGet]
        public async Task<IActionResult> Get(ProductSearchCriteria searchCriteria = null)
        {
            if (searchCriteria == null || !searchCriteria.IsValid)
                return Ok(await ProductRepository.Current.All());
            else
                return Ok(await ProductRepository.Current.Find(searchCriteria.Match));
        }

        // GET api/products/{id}
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
        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            await ProductRepository.Current.Insert(product);
			return Created(Url.Action("Get", new { id = product.ID }), product);
        }

        // PUT api/products/{id}
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

    public class ProductSearchCriteria
    {
        public decimal? PriceGreaterThan { get; set; }
        public decimal? PriceLessThan { get; set; }

        public bool IsValid
        {
            get
            {
                return PriceGreaterThan.HasValue || PriceLessThan.HasValue;
            }
        }
        public bool Match(Product product)
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
