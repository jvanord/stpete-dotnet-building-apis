'use strict';

var ApiTest = (function (defaultOptions) {

	var options,
		$output,
		me = {};

	// Setup
	me.init = function (overrideOptions) {
		options = $.extend({}, defaultOptions, overrideOptions);
		$output = $(options.outputElement);
		me.log.info('Tests Initialized');
	};

	// Logging
	me.log = (function(){
		function sendOutput(message, cssClass, tag) {
			tag = tag || 'p';
			$output.append($('<' + tag + '/>').addClass(cssClass).html(message));
		}
		return {
			info: function (message) {
				console.info(message);
				sendOutput(message, 'info');
			},
			success: function (message) {
				console.log(message);
				sendOutput(message, 'success');
			},
			warning: function (message) {
				console.warn(message);
				sendOutput(message, 'warning');
			},
			error: function (message) {
				console.error(message);
				sendOutput(message, 'error');
			},
		};
	})();

	// Expose
	return me;

})({

	// Add Default Options Here
	outputElement: '#output'

});