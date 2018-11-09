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
            'logtime'   =>  date("Y-m-d H:i:s", time()),
            'form'      =>  $manager
        ];
        $res = db("log")->insert($datas);        
    }

    // 创建控制钥匙
    public function CreControlKey($keyName = "", $desc = "")
    {
        if(!file_exists($keyName)){
            file_put_contents($keyName, $desc);
            return true;
        } else {
            return false;
        }
        return false;
    } 

    // 创建邀请码
    public function GetInviteCode($length = 8) 
    { 
        // 密码字符集，可任意添加你需要的字符 
        // $chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_ []{}<>~`+=,.;:/?|'; 
        $chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'; 
        $invitecode = ""; 
        for ($i = 0; $i < $length; $i++){ 
            // 这里提供两种字符获取方式 
            // 第一种是使用 substr 截取$chars中的任意一位字符； 
            // 第二种是取字符数组 $chars 的任意元素 
            // $password .= substr($chars, mt_rand(0, strlen($chars) – 1), 1); 
            $invitecode .= $chars[ mt_rand(0, strlen($chars) - 1) ]; 
        } 
        return $invitecode; 
    } 

}