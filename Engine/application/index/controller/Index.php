<?php
namespace app\index\controller;

class Index extends TflmsBase
{
    public function index()
    {
    	// 渲染首页
    	return $this->fetch("index");
    }

    // 规格
    public function format()
    {
    	// 
    	return $this->fetch("format");
    }

    // 文件类型
    public function filetype()
    {
        // 
        return $this->fetch("filetype");
    }

}

