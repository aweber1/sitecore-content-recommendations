jQuery(document).ready(function () {
	var contextItemId = jQuery('#pnlRecommendations').data("context-id");
	var token = jQuery("input[name='__RequestVerificationToken']").val();

	//var args = {
	//	"treedefinitionid": "{68E713D8-A382-4378-8FB0-9D7F7AD14B25}",
	//	"itemid": contextItemId,
	//	"start": "20151207T091203",
	//	"end": "20151210T091203",
	//	"group": "{00000000-0000-0000-0000-000000000000}",
	//	"pathfilter": "{85D23EC5-044C-4554-9B78-380E6628DF6A}"
	//}
	//jQuery.ajax({
	//		url: "/sitecore/api/PathAnalyzer/Paths",
	//		method: "GET",
	//		data: args,
	//		contentType: "application/json",
	//		dataType: "json",
	//		headers: {
	//			"X-RequestVerificationToken": token
	//		}
	//	})
	//	.done(function(data) {
	//		jQuery('#pnlRecommendations').append("<div>" + data + "</div>");
	//	})
	//	.fail(function(xhr, status, error) {
	//		jQuery('#pnlRecommendations').append("<div>" + error + "</div>");
	//	})
	//	.always(function(data, status) {
	//		console.log('request complete: ' + data);
	//	});
});