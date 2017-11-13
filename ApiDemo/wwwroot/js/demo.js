'use strict';

// This is a simple app using jQuery and the Vanilla.js framework.

// Main App Module
var App = (function(defaultOptions) {
	var options,
		me = {};

	// Global Error Handler
	function ajaxErrorHandler(e, jqxhr, settings) {
		console.error('API Error', {
			event: e,
			request: jqxhr,
			settings: settings
		});
		var $error = $('<div/>')
			.addClass('error')
			.text('Error Occured - See Log')
			.appendTo('body');
		setTimeout(function() {
			$error.fadeOut();
		}, 3500);
	}

	// Setup
	me.init = function(overrideOptions) {
		options = $.extend({}, defaultOptions, overrideOptions);
		$(document).ajaxError(ajaxErrorHandler);
		console.info('App Initialized', options);
	};

	// Expose
	return me;
})({
	// Add Default Options Here
});

// Products Module
App.products = (function() {
	return {

		// Get All Products
		getAll: function(callback) {
			// GET api/products - get all products
			$.get('/api/products')
				.done(function(products) {
					if (typeof callback === 'function') callback.call(this, products);
				});
		}

	};
})();

// Orders Module
App.orders = (function() {
	return {

		// Create a New Order - or Use One Already Started
		start: function(callback) {
			var order = JSON.parse(localStorage.getItem('currentOrder')); // check local storage
			if (!!order) {
				console.log('Order Found', order);
				if (typeof callback === 'function') callback.call(this, order);
			} else {
				// POST api/orders - create a new empty order
				$.post('/api/orders/') 
					.done(function(newOrder) {
						console.log('New Order Created', newOrder);
						if (typeof callback === 'function') callback.call(this, newOrder);
						localStorage.setItem('currentOrder', JSON.stringify(newOrder));
					});
			}
		},

		// Add an Item to the Current Order
		addItem: function(item, callback) {
			var order = JSON.parse(localStorage.getItem('currentOrder'));
			// POST api/orders{id}/items - add new item to specified order
			$.post('/api/orders/' + order.id + '/items', item) 
				.done(function(order) {
					console.log('Order Item Added', order);
					if (typeof callback === 'function') callback.call(this, order);
					localStorage.setItem('currentOrder', JSON.stringify(order));
				});
		},

		// Update the Quantity of an Existing Item on the Current Order - uses JsonPatch protocol
		changeQuantity: function(productId, qty, callback) {
			var order = JSON.parse(localStorage.getItem('currentOrder'));
			var patchData = [
				{
					op: 'replace',
					path: '/quantity',
					value: qty
				}
			];
			// PATCH api/orders/{id}/items/{productId} - send JsonPatch instructions for items on specified order
			$.ajax({
				method: 'patch',
				url: '/api/orders/' + order.id + '/items/' + productId,
				data: JSON.stringify(patchData),
				contentType: 'application/json',
				processData: false,
				dataType: 'json'
			}).done(function(order) {
				console.log('Order Item ' + productId + ' Quantity Changed to ' + qty, order);
				if (typeof callback === 'function') callback.call(this, order);
				localStorage.setItem('currentOrder', JSON.stringify(order));
			});
		},

		// Remove a Single Item from the Current Order
		removeItem: function(productId, callback) {
			var order = JSON.parse(localStorage.getItem('currentOrder'));
			// DELETE api/orders/{id}/items/{productId}
			$.ajax({
				method: 'delete',
				url: '/api/orders/' + order.id + '/items/' + productId
			}).done(function(order) {
				console.log('Order Item ' + productId + ' Removed', order);
				if (typeof callback === 'function') callback.call(this, order);
				localStorage.setItem('currentOrder', JSON.stringify(order));
			});
		}

	};
})();
