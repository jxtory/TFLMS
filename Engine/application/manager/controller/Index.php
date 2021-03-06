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

    public function file()
    {
        if(request()->isAjax()){
            if(input('type') == "getFile"){
                $search = input("content");
                if($search){
                    // $datas = db("invitation")
                    //     ->where("company","like", "%" . $search ."%")
                    //     ->select();
                    // // 
                    $datas = db('files a')
                        ->field("a.*, b.company")
                        ->join("invitation b", "b.id = a.cid")
                        ->where('b.company',"like", "%" . $search ."%")
                        ->order("id desc")
                        ->select();

                    if($datas){
                        return $datas;
                    } else {
                        return;
                    }
                }
            }

            if(input('type') == 'getFile_Car'){
                $datas = db('files a')
                    ->field("a.*, b.company")
                    ->join("invitation b", "b.id = a.cid")
                    ->where('carousel', '1')
                    ->select();

                if($datas){
                    return $datas;
                } else {
                    return;
                }

            }

            return;
        } else {
            // 公司信息
            $files = db('files a')
                ->field("a.*, b.company")
                ->join("invitation b", "b.id = a.cid")
                ->order("id desc")
                ->paginate(15);

            $this->assign("files", $files);
            $this->assign("filePath", $this->upPath);
            $this->assign("carCount", db("files")->where("carousel", "1")->count());

            $this->SetPageName("数据审核");
            return $this->fetch();
        }

    }

    // 文件管理控制器
    public function filecontrol()
    {
        if((session('manager') == "Manager_AuthOk") && (strlen(session("manager_who")) > 0)){
            if(request()->isPost()){
                if(input("post.type") == "fileControl"){
                    $datas = input("post.");
                    $who = session("manager_who");
                    $filecur = db('files')->where("id", $datas['fid'])->find();
                    $companyName = db("invitation")->where('id', $filecur['cid'])->find()['company'];
                    unset($datas['type']);
                    switch ($datas['act'])
                    {
                    case "playIt":
                        if(file_exists("playfile")){unlink("playfile");}
                        if($this->CreControlKey("playfile", $filecur['fileUrl'] . "." . $filecur['fileType'])){
                            $this->wLog("[管理行为]{$companyName}-的文件被提出播放", $who);
                            return "操作成功！";
                        }

                        break;
                    case "delIt":
                        $delFile = db("files")->where("id", $filecur['id'])->delete();
                        if($delFile){
                            $this->wLog("[管理行为]{$companyName}的{$filecur['fileUrl']}.{$filecur['fileType']}文件已删除!", $who);
                            // 正文件
                            if(!unlink($this->upPath . DS . $filecur['fileUrl'] . '.' . $filecur['fileType'])){
                                $this->wLog("[系统行为]{$filecur['fileUrl']}.{$filecur['fileType']}数据记录被删除，但文件删除失败。");
                            }
                            // 缩略文件
                            if($filecur['fileType'] != "mp4"){
                                if(!unlink($this->upPath . DS . $filecur['fileUrl'] . '_thumb.' . $filecur['fileType'])){
                                    $this->wLog("[系统行为]{$filecur['fileUrl']}_thumb.{$filecur['fileType']}数据记录被删除，但文件删除失败。");
                                }
                            }
                            return "操作成功！";
                        }
                        break;
                    case "cygdbf":
                    case "qxgdbf":
                        $cars = db("files")->where("id", $filecur['id'])->find()['carousel'];

                        if($cars == "0"){
                            $carcount = db("files")->where("carousel", "1")->count();
                            if($carcount < 10){
                                $res = db("files")->where("id", $filecur['id'])->update(['carousel' => "1"]);
                            } else {
                                return "滚动图像不能超过10张！_";
                            }
                        } else {
                            $res = db("files")->where("id", $filecur['id'])->update(['carousel' => "0"]);
                        }

                        if($res){
                            $this->wLog("[管理行为]{$companyName}的{$filecur['fileUrl']}.{$filecur['fileType']}文件,加入滚动播放！", $who);
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

    // 邀请管理
    public function invitation()
    {
        if(request()->isAjax()){
            if(input('type') == "getInvitation"){
                $search = input("content");
                if($search){
                    $datas = db("invitation")
                        ->where("company","like", "%" . $search ."%")
                        ->select();
                    // 
                    if($datas){
                        return $datas;
                    } else {
                        return;
                    }
                }
            }
            return;
        } else {
            // 公司信息
            $companys = db("invitation")->order("id desc")->paginate(15);
            $this->assign("companys", $companys);

            $this->SetPageName("邀请信息");
            return $this->fetch();
        }
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
                            $files = db("files")->where("cid", $datas['cid'])->select();
                            $delFiles = db("files")->where("cid", $datas['cid'])->delete();
                            foreach ($files as $key => $value) {
                                # code...
                                if($value['fileType'] != "mp4"){
                                    unlink($this->upPath . DS . $value['fileUrl'] . '.' . $value['fileType']);
                                    unlink($this->upPath . DS . $value['fileUrl'] . '_thumb.' . $value['fileType']);
                                } else {
                                    unlink($this->upPath . DS . $value['fileUrl'] . '.' . $value['fileType']);
}
                            }
                            $this->wLog("[管理行为]删除公司-{$companyName}, 及所有相关文件", $who);
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
        // 放映信息
        if(file_exists("playfile")){
            $file = file_get_contents("playfile");
            $playcontent['type'] = explode(".", $file)[1];
            $playcontent['content'] = explode(".", $file)[0];
            $this->assign("playcontent", $playcontent);

        } else {
            $this->assign("playcontent", "暂无放映信息");
        }

        $this->assign("filePath", $this->upPath);
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
                    case "fstop":
                        $this->wLog("[管理行为]取消了放映任务", $who);
                        if($this->CreControlKey("led_stop")){return "操作成功！";}
                        break;
                    case "fplay":
                        if(!file_exists("playfile")){return "放映材料异常！";}
                        $this->wLog("[管理行为]执行了一次放映任务", $who);
                        if($this->CreControlKey("led_play")){return "操作成功！";}
                        break;
                    case "fplay_car":
                        if(file_exists("playfiles")){unlink("playfiles");}
                        $carFile = db("files")->where("carousel", "1")->field("fileUrl,fileType")->select();
                        $pfc = [];
                        foreach ($carFile as $key => $value) {
                            # code...
                            $pfc[] = $value['fileUrl'] . "." . $value['fileType'];
                        }

                        if(count($pfc) < 2){return "至少要2张滚动图!";}

                        $pfc = implode(",", $pfc);
                        file_put_contents("playfiles", $pfc);

                        if(!file_exists("playfiles")){return "放映材料异常！";}
                        $this->wLog("[管理行为]执行了一次滚动放映任务", $who);
                        if($this->CreControlKey("led_carousel")){return "操作成功！";}
                        break;
                    case "fdel":
                        if(file_exists("playfile") && unlink("playfile")){
                            return "DEL操作成功";
                        } else {
                            return "当前或无设置";
                        }
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

