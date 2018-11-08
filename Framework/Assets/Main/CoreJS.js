
$(function(){
	// ht login
	$(".managerhd span").click(function(event) {
		window.location.href = "/ht";
	});

	// Define DC Url
	var DCUrl = "/ht/dc";
	DoorControl(DCUrl);

	// Define Led Url
	var LedUrl = "/ht/lc";
	LedControl(LedUrl);
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
	$("#doorhd a").click(function(event) {
		$.post(url, {
			type: 'doorControl',
			act: $(this).data('ev')
		} ,function(data, status){
			if(status == "success"){
				layer.msg(data);
			}
		});

	});
}

// LedControl Behavior
function LedControl(url)
{
	$("#ledhd a").click(function(event) {
		$.post(url, {
			type: 'ledControl',
			act: $(this).data('ev')
		} ,function(data, status){
			if(status == "success"){
				layer.msg(data);
			} else {
				layer.msg("有问题！");
			}
		});

	});
}