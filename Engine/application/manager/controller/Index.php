<?php
namespace app\manager\controller;

class Index extends TflmsMBase
{
    public function index()
    {
    	// 渲染首页
    	return $this->fetch("index");
    }

}

