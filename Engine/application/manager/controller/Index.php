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
                        $this->CreControlKey("opendoor");
                        break;
                    case "cld":
                        $this->CreControlKey("closedoor");
                        break;
                    case "op15":
                        $this->CreControlKey("opendoor15");
                        break;
                    default:
                    }
                    return "操作成功！";
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

