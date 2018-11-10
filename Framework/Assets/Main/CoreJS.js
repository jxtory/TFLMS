
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

	// Define IC Url
	var IcUrl = "/ht/ic";
	InvitationControl(IcUrl);

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

// Invitation Behavior
function InvitationControl(url)
{
	$("#cp_serch").bind("input propertychange", function(){
		// layer.msg($("#cp_serch").val());
		if($("#cp_serch").val() != ""){
		// if($("#cp_serch").val() != "" || $("#cp_serch").val() != null || $("#cp_serch").val() != "undefined"){
		    $("#curPage").hide();
		    $(".fixed_page").hide();
		} else {
		    $("#curPage").show();
		    $(".fixed_page").show();
		}

	    $.post(
	            "invitation.html",
	            {
	                type: "getInvitation",
	                content: function(){return $("#cp_serch").val();}
	            },
	            function(data, status){
	                // $("#invitatehd").html("");
	                if(status == "success" && data != ""){
	                    for(v in data){
	                        // var content = "<tr>";
	                        // content += "<td>" + data[v]['id'] + "</td>";
	                        // content += "<td>" + data[v]['an'] + "</td>";
	                        // content += "<td>" + data[v]['personnel'] + "</td>";
	                        // content += "<td>" + data[v]['grant_date'] + "</td>";
	                        // content += "<td>" + data[v]['components'] + "</td>";
	                        // content += '<td><a href="javascript: void(0);" class="btn btn-blue btn-xs" data-hid="' + data[v]['id'] + '">回收</a></td>';
	                        // content += "</tr>";
	                        // $("#getHoldersInfo tbody").html($("#getHoldersInfo tbody").html() + content);

	                    }
	                } else {

	                }
	            }
	        );

	});

	$(".invitatehd a").click(function(event) {
		if($(this).data('ev') == "GetInfo"){
			//JS里指定复制的内容
			var clipboard = new Clipboard('.GetInfo');
			layer.msg("已复制到剪切板");
			return;
		}

		$.post(url, {
			type: 'invitateControl',
			act: $(this).data('ev'),
			cid: $(this).data('cid')
		} ,function(data, status){
			if(status == "success"){
				layer.msg(data + "3秒后刷新！");
				setTimeout(function(){
					location.replace(location.href);
				},3000); 

			} else {
				layer.msg("有问题！");
			}
		});


	});
}


//判断客户端
function isMobile() {
    var userAgentInfo = navigator.userAgent;

    var mobileAgents = [ "Android", "iPhone", "SymbianOS", "Windows Phone", "iPad","iPod"];

    var mobile_flag = false;

    //根据userAgent判断是否是手机
    for (var v = 0; v < mobileAgents.length; v++) {
        if (userAgentInfo.indexOf(mobileAgents[v]) > 0) {
            mobile_flag = true;
            break;
        }
    }

     var screen_width = window.screen.width;
     var screen_height = window.screen.height;    

     //根据屏幕分辨率判断是否是手机
     if(screen_width < 500 && screen_height < 800){
         mobile_flag = true;
     }

     return mobile_flag;
}

//判断设备是否是手机还是电脑
function isMobileClient() {
    var userAgentInfo = navigator.userAgent;
    var Agents = new Array("Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod");
    var agentinfo = null;
    for(var i = 0; i < Agents.length; i++) {
        if(userAgentInfo.indexOf(Agents[i]) > 0) {
            agentinfo = userAgentInfo;
            break;
        }
    }
    if(agentinfo) {
        return true;
    } else {
        return false;
    }
}