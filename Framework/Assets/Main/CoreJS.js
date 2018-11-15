
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

	// Define Fc Url
	var FcUrl = "/ht/fc";
	FileControl(FcUrl);

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
	// $("#cp_serch").bind("input propertychange", function(){
	$("#cp_serch").bind("keydown", function(event){
		if(!(event && event.keyCode == "13")){
			return;
		}

		cp_serch();

	});

	$("#cp_serch_a").on('click', function(event) {
		cp_serch();

	});

	$(".invitatehd a").click(function(event) {
		if($(this).data('ev') == "GetInfo"){
			//JS里指定复制的内容
			var clipboard = new Clipboard('.GetInfo');
			layer.msg("已复制到剪切板");
			return;
		}

		$(this).parent().parent().hide();
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

$("#invitatehd").on('click', 'a',function(event) {
	if($(event.target).data('ev') == "GetInfo"){
		//JS里指定复制的内容
		var clipboard = new Clipboard('.GetInfo');
		layer.msg("已复制到剪切板");
		return;
	}

	$(event.target).parent().parent().hide();
	$.post("/ht/ic", {
		type: 'invitateControl',
		act: $(event.target).data('ev'),
		cid: $(event.target).data('cid')
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
	/* Act on the event */
});

// 公司搜索
function cp_serch()
{
	// layer.msg($("#cp_serch").val());
	if($("#cp_serch").val() != ""){
	// if($("#cp_serch").val() != "" || $("#cp_serch").val() != null || $("#cp_serch").val() != "undefined"){
	    $("#curPage").hide();
	    $(".fixed_page").hide();
	} else {
		$("#invitatehd").html("");
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
                $("#invitatehd").html("");
                if(status == "success" && data != ""){
                    for(v in data){
                        var content = '<div class="row">';
                        content += '<div class="col-xs-1">' + data[v]['id'] + '</div>';
                        content += '<div class="col-xs-4">' + data[v]['company'] + '</div>';
                        if(data[v]['invitecode'] != "" && data[v]['invitecode'] != null){
                        	content += '<div class="col-xs-1"><a href="javascript: void(0);" class="GetInfo" data-ev="GetInfo" style="color: #00c;" data-clipboard-text="' + data[v]['invitecode'] + '">' + data[v]['invitecode'] + '</a></div>';
                        } else {
							content += '<div class="col-xs-1"><span style="color: #c00;">未授权</span></div>';
                        }

                        if(data[v]['invitecodelifetime'] != ""  && data[v]['invitecodelifetime'] != null){
                        	var ctime = new Date(data[v]['invitecodelifetime']);
                        	var nowtime = new Date(new Date());
                        	if(ctime.getTime() > nowtime.getTime()){
								content += '<div class="col-xs-2"><span>' + data[v]['invitecodelifetime'] + '</span></div>';
                        	} else {
								content += '<div class="col-xs-2"><span style="color: #c00;">已过期</span></span></div>';
                        	}

                        } else {
							content += '<div class="col-xs-2"><span>无</span></div>';
                        }

                        content += '<div class="col-xs-4">';
                        content += '<p><a href="javascript: void(0);" data-ev="CreIc" data-cid="' + data[v]['id'] + '">生成邀请码</a></p>';
                        if(data[v]['invitecode'] != "" && data[v]['invitecode'] != null){
                        	content += '<p><a href="javascript: void(0);" class="GetInfo" data-ev="GetInfo" data-cid="' + data[v]['id'] + '" data-clipboard-text="您的邀请码是：' + data[v]['invitecode'] + ';请打开 led.tqcen.com 上传内容.">复制信息</a></p>';
                        	content += '<p><a href="javascript: void(0);" data-ev="Delay" data-cid="' + data[v]['id'] + '">延长一天</a></p>';
                        	content += '<p><a href="javascript: void(0);" data-ev="UnIc" data-cid="' + data[v]['id'] + '">取消邀请</a></p>';
                        }
                    	content += '<p><a href="javascript: void(0);" data-ev="PassID" data-cid="' + data[v]['id'] + '">删除公司</a></p>';
                        content += '</div>';
                        content += '</div>';
                        content += '<div class="row"><hr></div>';
                        $("#invitatehd").html($("#invitatehd").html() + content);

                    }
                } else {

                }
            }
        );

}

// File Behavior
function FileControl(url)
{
	$("#cpf_serch").bind("keydown", function(event){
		if(!(event && event.keyCode == "13")){
			return;
		}

		cpf_serch();

	});

	$("#cpf_serch_a").on('click', function(event) {
		cpf_serch();

	});

	$(".filehd a").click(function(event) {
		$(this).parent().parent().hide();
		$.post(url, {
			type: 'fileControl',
			act: $(this).data('ev'),
			cid: $(this).data('fid')
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

$("#filehd").on('click', 'a',function(event) {
	$(event.target).parent().parent().hide();
	$.post("/ht/fc", {
		type: 'fileControl',
		act: $(event.target).data('ev'),
		cid: $(event.target).data('fid')
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
	/* Act on the event */
});

// 公司搜索
function cpf_serch()
{
	// layer.msg($("#cp_serch").val());
	if($("#cpf_serch").val() != ""){
	// if($("#cp_serch").val() != "" || $("#cp_serch").val() != null || $("#cp_serch").val() != "undefined"){
	    $("#curPage_File").hide();
	    $(".fixed_page_File").hide();
	} else {
		$("#filehd").html("");
	    $("#curPage_File").show();
	    $(".fixed_page_File").show();
	}

    $.post(
            "file.html",
            {
                type: "getFile",
                content: function(){return $("#cpf_serch").val();}
            },
            function(data, status){
                $("#filehd").html("");
                if(status == "success" && data != ""){
                    for(v in data){
                        var content = '<div class="row">';
                        content += '<div class="col-xs-1">' + data[v]['id'] + '</div>';
                        content += '<div class="col-xs-3">' + data[v]['company'] + '</div>';
                        content += '<div class="col-xs-6">';
                        if(in_array(data[v]['fileType'], ['png', 'jpg', 'jpeg', 'bmp', 'gif'])){
                        	content += '<img class="img-thumbnail" src="/tqupload/' + data[v]['fileUrl'] + '_thumb.' + data[v]['fileType'] + '" alt="' + data[v]['fileUrl'] + '">';
                        } else if(in_array(data[v]['fileType'], ['mp4'])) {
							content += '<video id="my-video" style="margin:0 auto;" class="video-js vjs-big-play-centered" controls preload="none" poster="" data-setup playsinline width="384" height="216">';
							content += '<source src="/tqupload/' + data[v]['fileUrl'] + '.' + data[v]['fileType'] + '" type="video/' + data[v]['fileType'] + '"></video>';
                        }
                        content += '</div>';
                        content += '<div class="col-xs-2">';
                        content += '<p><a href="javascript: void(0);" data-ev="playIt" data-fid="' + data[v]['id'] + '">放映资源</a></p>';
                        content += '<p><a href="javascript: void(0);" data-ev="delIt" data-fid="' + data[v]['id'] + '">删除文件</a></p>';
                        if(!in_array(data[v]['fileType'], ['mp4'])){
                        	if(data[v]['carousel'] == "0"){
		                        content += '<p><a href="javascript: void(0);" data-ev="cygdbf" data-fid="' + data[v]['id'] + '">参与滚动播放</a></p>';
                        	} else {
		                        content += '<p><a href="javascript: void(0);" data-ev="qxgdbf" data-fid="' + data[v]['id'] + '">取消滚动播放</a></p>';
                        	}
                        }
                        content += '</div>';
                        content += '</div>';
                        content += '<div class="row"><hr></div>';
                        $("#filehd").html($("#filehd").html() + content);

                    }
                } else {

                }
            }
        );

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

// JS in_array
function in_array(search, array){
    for(var i in array){
        if(array[i] == search){
            return true;
        }
    }
    return false;
}