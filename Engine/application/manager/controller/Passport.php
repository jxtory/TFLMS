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
        if(request()->isPost()){
            $datas = input('post.');

            $user = db('account')->where('account', $datas['username'])->find();
            if($user){
                if($user['password'] == md5($datas['password'])){
                    session("manager", "Manager_AuthOk");
                    session("manager_who", $datas['username']);
                    session("uid", $user['id']);
                    $this->wLog("[登录行为]检测到管理者[{$datas['username']}]" . "登录了后台系统", $datas['username']);
                    $this->redirect("manager/index/index");
                } else {
                    $this->wLog("[登录行为]检测到密码登录失败，来自管理者{$datas['username']}");
                    $this->redirect("passport/loginerror");
                }

            } else {
                $cname = $datas['username'] ?: '匿名';
                $this->wLog("[登录行为]检测到失败的登录请求，用户拟名：{$cname}");
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

    // 登录失败
    public function loginerror()
    {
        return $this->fetch('');
    }

    // 创建账号
    public function creaccount()
    {
        if(!session('manager') && session("manager_who") != "admin"){
            $this->wLog("[系统检测] 账户攻击检测告警！");
            return $this->redirect('/ht');
        }

        // if(session("manager_who"))
        if(request()->isPost()){
            // 载入账号
            $datas = input("post.");

            if(!strlen($datas['username']) > 0){
                $this->error("账号不能为空！");
            }

            if($datas['username'] == $datas['valusername'] && strlen($datas['username']) > 0){
                $checkIt = db("account")->where("account", $datas['username'])->find();
                if(!$checkIt){
                    $data = [
                        'account'       =>      $datas['username'],
                        'password'      =>      md5("lcd888")
                    ];
                    $creIt = db("account")->insert($data);
                    if($creIt == 1){
                        $this->wLog("[管理行为] 系统账号创建了管理者账号：{$datas['username']}！");
                        $this->success($datas['username'] . ", 创建成功！");
                    } else {
                        $this->error("账号创建失败！");
                    }

                } else {
                    $this->error("账号已存在！");
                }

            } else {
                $this->error("验证失败？请确保两次输入一致！");
            }

        }

        return $this->fetch('');
    }

    // 修改密码
    public function repw()
    {
        if(!session('manager')){
            return $this->redirect('passport/login');
        }

        if(request()->isPost()){
            // 载入密码
            $datas = input("post.");

            $nowUser = session("manager_who");
            $user = db('account')->where('account', $nowUser)->find();

            $data = [
                'password'      =>      md5($datas['newpassword'])
            ];

            if(md5($datas['oldpassword']) == $user['password']){
                if($datas['newpassword'] == ""){$this->error("密码设置不能为空!");}
                if($datas['newpassword'] == $datas['oldpassword']){$this->error("新密码不能和旧密码相同!");}
                if($datas['newpassword'] == $datas['valpassword']){
                    // 修改！
                    $user = db('account')->where('account', session('manager_who'))->update($data);

                    if($user){
                        $this->wLog("[管理行为][{$nowUser}]修改了自己的密码");
                        $this->success("密码修改成功", "/ht");
                    } else {
                        $this->error("密码修改失败");
                    }

                } else {
                    $this->error("验证失败？请确保两次输入一致！");
                }

            } else {
                $this->error("旧密码输入有误！");
            }

        }

        return $this->fetch('');
    }
}
