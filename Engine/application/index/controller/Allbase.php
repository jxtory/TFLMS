<?php
namespace app\index\controller;
use think\Controller;
use think\Db;
use think\Validate;
use think\Loader;

class Allbase extends Controller
{
    // *各种定义
    // >应用名称
    public $appName = "四分之三·区块链全球商业中心(大屏幕管理系统)";
    public function _initialize()
    {
        // 通用初始化

        // 渲染全局变量
        $this->AllAssign();
        
        // 创建上传中心目录
        // mkdirs("");

        // 创建配置目录
        // mkdirs("config");                       // 配置目录

    }

    // *公共或全局渲染
    public function AllAssign()
    {
    	// >应用名称
    	$this->assign("appName", $this->appName);

    }

}