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
                    $this->redirect("manager/index/index");
                } else {
                    $this->redirect("passport/loginerror");
                }
            } else {
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
