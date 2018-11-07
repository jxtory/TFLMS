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

    // 日志管理
    public function log()
    {
    	// 系统日志
        $logs = db("log")->order("id desc")->paginate(15);
        $this->assign("logs", $logs);
    	$this->SetPageName("系统日志");
    	return $this->fetch();
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
                    unset($datas['type']);
                    switch ($datas['act'])
                    {
                    case "opd":
                        if($this->CreControlKey("opendoor")){return "操作成功！";}
                        break;
                    case "cld":
                        if($this->CreControlKey("closedoor")){return "操作成功！";}
                        break;
                    case "op15":
                        if($this->CreControlKey("opendoor15")){return "操作成功！";}
                        break;
                    default:
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

