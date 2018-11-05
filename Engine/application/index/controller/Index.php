<?php
namespace app\index\controller;

class Index extends TflmsBase
{
    public function index()
    {
        // 渲染首页
        return $this->fetch();
    }
    
    // >内容上传
    public function upload()
    {
        $this->SetPageName("内容上传");
    	return $this->fetch("upload");
    }

    // >制作说明
    public function instruction($type = "Picture")
    {
        $this->SetPageName("制作说明");
        if($type == "video"){
            return $this->fetch("instruction_video");
        }
        return $this->fetch("instruction");
    }

    // 规格
    public function format()
    {
        $this->SetPageName("大屏规格");
    	return $this->fetch("format");
    }

    // 文件类型
    public function filetype()
    {
        $this->SetPageName("格式说明");
        return $this->fetch("filetype");
    }

}

