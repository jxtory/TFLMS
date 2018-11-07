
$(function(){
	// ht login
	$(".managerhd span").click(function(event) {
		window.location.href = "/ht";
	});	

	// Define DC Url
	var DCUrl = "/ht/dc";
	DoorControl(DCUrl);
});

// 获取$pageName；激活Nav高亮；
function GetPageName(pagename){
	if(typeof pagename != "undefined" && pagename != null && pagename != ""){
		$("li>a:contains('" + pagename + "')").parent().addClass("active");
	}
}

// DoorControl Behavior
function DoorControl(url)
{
	$("a.doorhd").click(function(event) {
		layer.msg($("this").data('ev'));return;
		$.post(url, {
			type: 'doorControl',
			act: $("this").data('ev')
		} ,function(data, status){
			if(status == "ok"){
				layer.msg(data);
			}
		});

	});
}