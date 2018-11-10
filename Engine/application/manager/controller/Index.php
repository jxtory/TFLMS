<?php
namespace app\manager\controller;

class Index extends TflmsMBase
{
    public function index()
    {
    	// 渲染首页
    	$this->SetPageName("后台主页");
    	return $this->fetch();
    }

    // 邀请管理
    public function invitation()
    {
        // 公司信息
        $companys = db("invitation")->order("id desc")->paginate(15);
        $this->assign("companys", $companys);

        $this->SetPageName("邀请信息");
        return $this->fetch();
    }

    // 邀请管理控制器
    public function invitationcontrol()
    {
        if((session('manager') == "Manager_AuthOk") && (strlen(session("manager_who")) > 0)){
            if(request()->isPost()){
                if(input("post.type") == "invitateControl"){
                    $datas = input("post.");
                    $who = session("manager_who");
                    $companyName = db("invitation")->where('id', $datas['cid'])->find()['company'];
                    unset($datas['type']);
                    switch ($datas['act'])
                    {
                    case "CreIc":
                        do {
                            $invitecode = $this->GetInviteCode();
                        } while (db('invitation')->where("invitecode", $invitecode)->find() != null);

                        $data = [
                            'invitecode'            =>  $invitecode,
                            'invitecodelifetime'    =>  date("Y-m-d H:i:s",time() + 60 * 60 * 24 * 5)
                        ];

                        $res = db("invitation")->where('id', $datas['cid'])->update($data);
                        if($res){
                            $this->wLog("[管理行为]为公司-{$companyName}-生成了邀请码", $who);
                            return "操作成功！";
                        }
                        break;
                    case "Delay":
                        $data = [
                            'invitecodelifetime'    =>  date("Y-m-d H:i:s", (strtotime(db("invitation")->where('id', $datas['cid'])->find()['invitecodelifetime']) + 60 * 60 * 24))
                        ];

                        $res = db("invitation")->where('id', $datas['cid'])->update($data);
                        if($res){
                            $this->wLog("[管理行为]为公司-{$companyName}-延长时间1天", $who);
                            return "操作成功！";
                        }
                        break;
                    case "UnIc":
                        $data = [
                            'invitecode'            =>  "",
                            'invitecodelifetime'    =>  null
                        ];

                        $res = db("invitation")->where('id', $datas['cid'])->update($data);
                        if($res){
                            $this->wLog("[管理行为]为公司-{$companyName}-取消了邀请授权", $who);
                            return "操作成功！";
                        }
                        break;
                    case "PassID":
                        $res = db("invitation")->where('id', $datas['cid'])->delete();
                        if($res){
                            $this->wLog("[管理行为]删除公司-{$companyName}", $who);
                            return "操作成功！";
                        }
                        break;
                    default:
                        return "操作失败！行为异常！0";
                    }
                    return "操作失败！";
                } else {
                    return "操作失败！";
                }
            }
        } else {
            return "Error!";
        }

        return "Error!";
    }

    // 添加公司
    public function addCompany()
    {
        $nowUser = session("manager_who");

        if(request()->isPost()){
            $datas = input();
            if(!strlen($datas['company']) > 0){
                $this->error("公司名称不能为空！");
            }
            $checkIt = db("invitation")->where("company", $datas['company'])->find();
            if(!$checkIt){
                $data = [
                    'company'       =>      $datas['company'],
                ];
                $creIt = db("invitation")->insert($data);
                if($creIt == 1){
                    $this->wLog("[管理行为] 添加了公司名称{$datas['company']}！", $nowUser);
                    $this->success($datas['company'] . ", 添加成功！");
                } else {
                    $this->error("添加失败！");
                }

            } else {
                $this->error("该公司已存在！");
            }

        }
        return $this->error("无法添加");
    }

    // 日志管理
    public function log()
    {
    	// 系统日志
        $logs = db("log")->order("id desc")->paginate(15);
        $this->assign("logs", $logs);
    	$this->SetPageName("系统日志");
    	return $this->fetch();
    }

    // 清理日志
    public function clearlog()
    {
        if((session('manager') == "Manager_AuthOk") && (strlen(session("manager_who")) > 0)){
            $who = session("manager_who");
            $sql = "truncate table tflms_log";
            db("log")->query($sql);
            $this->success("处理成功", "/ht");
        } else {
            $this->error("错误！");
        }

        $this->error("错误！");
    }

    // LED控制
    public function led()
    {
        $this->SetPageName("屏幕控制");
        return $this->fetch();
    }

    // 停止程序
    public function stopsystem()
    {
        if((session('manager') == "Manager_AuthOk") && (strlen(session("manager_who")) > 0)){
            $who = session("manager_who");
            $this->wLog("[管理行为]停止系统！", $who);
            if($this->CreControlKey("app_quit")){$this->success("操作成功", "/ht");}
        } else {
            $this->error("错误！");
        }

        $this->error("错误！");
    }

    // Led控制管理器
    public function ledControl()
    {
        if((session('manager') == "Manager_AuthOk") && (strlen(session("manager_who")) > 0)){
            if(request()->isPost()){
                if(input("post.type") == "ledControl"){
                    $datas = input("post.");
                    $who = session("manager_who");
                    unset($datas['type']);
                    switch ($datas['act'])
                    {
                    case "max":
                        $this->wLog("[管理行为]开启LED窗口复位", $who);
                        if($this->CreControlKey("app_max")){return "操作成功！";}
                        break;
                    case "oled":
                        $this->wLog("[管理行为]控制大屏幕开机", $who);
                        if($this->CreControlKey("led_open")){return "操作成功！";}
                        break;
                    case "cled":
                        $this->wLog("[管理行为]控制大屏幕关机", $who);
                        if($this->CreControlKey("led_close")){return "操作成功！";}
                        break;
                    default:
                        return "操作失败！行为异常！0";
                    }
                    return "操作失败！行为异常！1";
                } else {
                    return "操作失败！行为异常！2";
                }
            }
        } else {
            return "Error!";
        }

        return "Error!";
    }

    // 大门控制
    public function door()
    {
        // 大门控制
        $this->SetPageName("大门控制");
        return $this->fetch();
    }

    // 大门控制管理器
    public function doorcontrol()
    {
        if((session('manager') == "Manager_AuthOk") && (strlen(session("manager_who")) > 0)){
            if(request()->isPost()){
                if(input("post.type") == "doorControl"){
                    $datas = input("post.");
                    $who = session("manager_who");
                    unset($datas['type']);
                    switch ($datas['act'])
                    {
                    case "opd":
                        $this->wLog("[管理行为]控制大门常开", $who);
                        if($this->CreControlKey("opendoor")){return "操作成功！";}
                        break;
                    case "cld":
                        $this->wLog("[管理行为]控制大门自动", $who);
                        if($this->CreControlKey("closedoor")){return "操作成功！";}
                        break;
                    case "op15":
                        $this->wLog("[管理行为]临时开门15秒", $who);
                        if($this->CreControlKey("opendoor15")){return "操作成功！";}
                        break;
                    default:
                        return "操作失败！行为异常！0";

                    }
                    return "操作失败！";
                } else {
                    return "操作失败！";
                }
            }
        } else {
            return "Error!";
        }

        return "Error!";
    }

}

