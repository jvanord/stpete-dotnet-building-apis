'use strict';

// This is a simply app using jQuery and the Vanilla.js framework.

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
    getAll: function(callback) {
      $.get('/api/products').done(function(products) {
        if (typeof callback === 'function') callback.call(this, products);
      });
    }
  };
})();

// Orders Module
App.orders = (function() {
  return {
    start: function(callback) {
      var order = JSON.parse(localStorage.getItem('currentOrder'));
      if (!!order) {
        console.log('Order Found', order);
        if (typeof callback === 'function') callback.call(this, order);
      } else {
        $.post('/api/orders/', { id: null, items: [] }).done(function(
          newOrder
        ) {
          console.log('New Order Created', newOrder);
          if (typeof callback === 'function') callback.call(this, newOrder);
          localStorage.setItem('currentOrder', JSON.stringify(newOrder));
        });
      }
    },
    addItem: function(item, callback) {
      var order = JSON.parse(localStorage.getItem('currentOrder'));
      $.post('/api/orders/' + order.id + '/items', item).done(function(order) {
        console.log('Order Item Added', order);
        if (typeof callback === 'function') callback.call(this, order);
        localStorage.setItem('currentOrder', JSON.stringify(order));
      });
    },
    changeQuantity: function(id, qty, callback) {
      var order = JSON.parse(localStorage.getItem('currentOrder'));
      var patchData = [
        {
          op: 'replace',
          path: '/quantity',
          value: qty
        }
      ];
      $.ajax({
        method: 'patch',
        url: '/api/orders/' + order.id + '/items/' + id,
        data: JSON.stringify(patchData),
        contentType: 'application/json',
        processData: false,
        dataType: 'json'
      }).done(function(order) {
        console.log('Order Item Quantity Changed', order);
        if (typeof callback === 'function') callback.call(this, order);
        localStorage.setItem('currentOrder', JSON.stringify(order));
      });
    },
    removeItem: function(id, callback) {
      var order = JSON.parse(localStorage.getItem('currentOrder'));
      $.ajax({
        method: 'delete',
        url: '/api/orders/' + order.id + '/items/' + id
      }).done(function(order) {
        console.log('Order Item ' + id + ' Removed', order);
        if (typeof callback === 'function') callback.call(this, order);
        localStorage.setItem('currentOrder', JSON.stringify(order));
      });
    }
  };
})();
