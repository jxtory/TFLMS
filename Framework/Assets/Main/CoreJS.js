
$(function(){
	// ht login
	$(".managerhd span").click(function(event) {
		window.location.href = "/ht";
	});	
});

// 获取$pageName；激活Nav高亮；
function GetPageName(pagename){
	if(typeof pagename != "undefined" && pagename != null && pagename != ""){
		$("li>a:contains('" + pagename + "')").parent().addClass("active");
	}
}