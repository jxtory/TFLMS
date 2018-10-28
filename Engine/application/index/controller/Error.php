<?php
namespace app\index\controller;

use \think\Controller;

class Error
{
    public function index()
    {
    	// 404页面
    	abort(404, "Error!");
    	return;
    }

}
