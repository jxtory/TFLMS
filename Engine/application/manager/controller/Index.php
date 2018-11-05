<?php
namespace app\manager\controller;

class Index extends TflmsMBase
{
    public function index()
    {
    	// 渲染首页
    	$this->SetPageName("后台主页");
    	return $this->fetch("index");
    }

}

