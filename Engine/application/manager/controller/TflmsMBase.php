<?php
namespace app\manager\controller;
use app\index\controller\Allbase as Allbase;

class TflmsMBase extends Allbase
{
	// 跳转回首页的设置
    protected $rehome = "<script>window.location.replace('/');</script>";

    public function _initialize()
    {
        // 基础初始化
    	// 基础初始化的东西开始

        parent::_initialize();
        if(empty(db("account")->select())){
            db("account")->insert(['account' => 'admin', 'password' => md5('led888')], true);
        }
        
        //检测登陆状态
        if(!session('manager')){
            return $this->redirect('passport/login');
        } else {

        }

    }

	public function _empty()
	{
        // 404页面
		abort(404, "Error!");
        return;
	}

    // 写日志
    public function wLog($content, $manager = "system")
    {
        $datas = [
            'logtext'   =>  $content,
            'logtime'   =>  date("Y-m-d h:i:s", time()),
            'form'      =>  $manager
        ];
        $res = db("log")->insert($datas);        
    }
}