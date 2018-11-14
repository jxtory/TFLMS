	$(function(){
		$("#uploaderhd").on('dblclick', 'a', function(event) {
			layer.msg("真的要删除这个内容吗？此操作不可逆！", {
				time: 0,
				btn: ['删除', '放弃'],
				btn1:function(){
						$(event.target).hide();

						$.post(
							"/upd",
							{
								types: "delUpFile",
								fid: $(event.target).data('fid')
							},

							function(data){
								if(data == "1"){
									layer.msg("删除成功");
								    setTimeout(function(){
								        window.location.reload();
								    },1000); 
								} else if (data == "2"){
									layer.msg("未知的错误");
								} else {
									layer.msg("删除失败");
									setTimeout(function(){
									    window.location.reload();
									},1000); 

								}

						});
				}
			});
		});		
	});