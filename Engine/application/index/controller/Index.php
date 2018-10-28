<?php
namespace app\index\controller;

class Index extends TflmsBase
{
    public function index()
    {
    	// 渲染首页
    	return $this->fetch("index");
    }

}

