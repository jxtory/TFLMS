<?php
namespace app\index\controller;
use think\Controller;
use think\Db;
use think\Validate;
use think\Loader;

class Allbase extends Controller
{
    // 各种定义
    public function _initialize()
    {
        // 通用初始化
        
        // 创建上传中心目录
        // mkdirs("");

        // 创建配置目录
        // mkdirs("config");                       // 配置目录

    }

}