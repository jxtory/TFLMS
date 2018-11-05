<?php
namespace app\manager\controller;

class Passport extends TflmsMBase
{
    public function _initialize()
    {
        $this->AllAssign();
        return;
    }

    // 后台登陆
    public function login()
    {
        $auth = include($this->managerKey);

        if(request()->isPost()){
            $data = input('post.');
            if(isset($auth[$data['username']]) && !empty($auth[$data['username']])){
                if($data['password'] == $auth[$data['username']]){
                    session("manager", "ok");
                    session("manager_who", $data['username']);
                    $this->wLog("[{$data['username']}]" . "登录了.", $data['username']);
                    $this->redirect("manager/index/index");
                } else {
                    $this->wLog("检测到密码登录失败，来自管理者{$data['username']}");
                    $this->redirect("passport/loginerror");
                }
            } else {
                $this->wLog("检测到失败的登录请求，用户拟名：{$data['username']}");
                $this->redirect("passport/loginerror");
            }
        }
        return $this->fetch('');
    }

    // 退出登录
    public function logout()
    {
        session(null);
        $this->redirect('/ht');
    }

    public function loginerror()
    {
        return $this->fetch('');
    }

}
