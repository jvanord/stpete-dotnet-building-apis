'use strict';

var ApiTest = (function (defaultOptions) {

	var options,
		$output,
		me = {};

	// Setup
	me.init = function (overrideOptions) {
		options = $.extend({}, defaultOptions, overrideOptions);
		$output = $(options.outputElement);
		me.log.info('Tests Initialized', options);
	};

	// Logging
	me.log = (function(){
		function sendOutput(message, cssClass, tag) {
			if (!message) return;
			tag = tag || 'p';
			$output.append($('<' + tag + '/>').addClass(cssClass).html(message));
		}
		function outputObject(obj) {
			$output.append($('<div/>').addClass('code').text(JSON.stringify(obj, null, '  ')));
		}
		return {
			info: function (message, obj) {
				console.info(message, obj);
				sendOutput(message, 'info');
				if (!!obj) outputObject(obj);
			},
			success: function (message, obj) {
				console.log(message, obj);
				sendOutput(message, 'success');
				if (!!obj) outputObject(obj);
			},
			warning: function (message, obj) {
				console.warn(message, obj);
				sendOutput(message, 'warning');
				if (!!obj) outputObject(obj);
			},
			error: function (message, obj) {
				console.error(message, obj);
				sendOutput(message, 'error');
				if (!!obj) outputObject(obj);
			},
			clear: function () { $output.html(''); }
		};
	})();


	// Expose
	return me;

})({
	// Add Default Options Here
	outputElement: '#output'
});

ApiTest.values = (function (options) {
	var uri = options.apiUrlBase + '/values';
	return {
		get: function (id) {
			$.get(uri, { id }).done(function (result) { ApiTest.log.success('API Success', result); }).fail(function (e) { ApiTest.log.error('API Failure', e); })
		}
	};
})({
	apiUrlBase: 'http://localhost:51014/api'
});
