<?php
namespace app\index\controller;

class Index extends TflmsBase
{
    public function index()
    {
        // 渲染首页
        return $this->fetch();
    }

    // 上传控制器
    public function upcontrol()
    {
        if(request()->isPost()){
            // 文件上传处理
            $files = request()->file();

            foreach ($files as $file) {
                $mUR = 'myUploadRule';
                $info = $file->rule($mUR)->move($this->upPath);
            }

            if($info){
                return "上传成功!";
            }

        }

        // 验证页面
        if(request()->isGet() && session("UploadAuth") == "Allow"){
            $cid = input("cid");
            $datas = db('invitation')->where('id', $cid)->find();
            if(session("Uploader") == $cid && strtotime($datas['invitecodelifetime']) > time()){
                $this->assign("cinfo", $datas);
                return $this->fetch();
            } else {
                return $this->error("邀请码已过期", "/");
            }
        }
        $this->error("发生了一个严重的错误", "/");
    }
    
    // >内容上传
    public function upload()
    {
        // 验证归零
        session("UploadAuth", "Disallow");
        session("Uploader", "none");
        if(request()->isPost()){
            $upkey = input("post.upkey");
            $checkIt = db("invitation")->where("invitecode", $upkey)->find();
            if($checkIt){
                session("UploadAuth", "Allow");
                session("Uploader", $checkIt['id']);
                $this->success("验证成功", url('/up', ['cid' => $checkIt['id']]));
            } else {
                $this->error("验证失败");
            }
        }

        $this->SetPageName("内容上传");
    	return $this->fetch("upload");
    }

    // >制作说明
    public function instruction($type = "Picture")
    {
        $this->SetPageName("制作说明");
        if($type == "video"){
            return $this->fetch("instruction_video");
        }
        return $this->fetch("instruction");
    }

    // 规格
    public function format()
    {
        $this->SetPageName("大屏规格");
    	return $this->fetch("format");
    }

    // 文件类型
    public function filetype()
    {
        $this->SetPageName("格式说明");
        return $this->fetch("filetype");
    }

}

