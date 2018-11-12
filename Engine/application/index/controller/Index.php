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
                if($info){
                    $data = [
                        'cid'           =>  session("Uploader"),
                        'fileUrl'       =>  $info->getSaveName(),
                        'fileType'      =>  explode(".", $info->getSaveName())[1],
                        'carousel'      =>  0
                    ];
                    $res = db('files')->insert($data, true);
                    $companyName = db("invitation")->where("id", session("Uploader"))->find()['company'];

                    if($res){
                        $this->wLog("[用户行为]{$companyName}，上传了文件{$info->getSaveName()}，成功存储！");
                    } else {
                        $this->wLog("[用户行为]{$companyName}，上传了文件{$info->getSaveName()}，存储失败！");
                    }

                    $newPic = $this->image_resize(file_get_contents($this->upPath . DS . $info->getSaveName()), 384, 216);
                    $fileName = explode(".", $info->getSaveName());

                    if(file_put_contents($this->upPath . DS . $fileName[0] . "_thumb." . $fileName[1], $newPic)){
                        $this->wLog("[系统行为]{$info->getSaveName()}，成功生成了缩略图。");
                    } else {
                        $this->wLog("[系统行为]{$info->getSaveName()}，成功缩略图失败了。");
                    }

                } else {
                    $this->wLog("[用户行为]{$companyName}，有文件上传失败");
                }
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

