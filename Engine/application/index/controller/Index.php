<?php
namespace app\index\controller;
// import the Intervention Image Manager Class
use Intervention\Image\ImageManager;

class Index extends TflmsBase
{
    public function index()
    {
        // 渲染首页
        return $this->fetch();
    }

    // 上传文件 - 用户删除请求
    public function upcontrol_del()
    {
        if(request()->isAjax()){
            if(input('type') == "delUpFile"){
                $fid = input("fid");
                $delFile = db("files")->where("id", $fid)->find();
                $datas = db("files")->where("id", $fid)->where("cid", session("Uploader"))->where('uptime','between time',[time() - 60 * 60, time()])->order('id desc')->delete();
                // CID
                $companyName = db("invitation")->where("id", session("Uploader"))->find()['company'];
                if($datas){
                    $this->wLog("[用户行为]{$companyName}，删除了上传了文件{$delFile['fileUrl']}.{$delFile['fileType']}");
                    // 正文件
                    if(!unlink($this->upPath . DS . $delFile['fileUrl'] . '.' . $delFile['fileType'])){
                        $this->wLog("[系统行为]{$delFile['fileUrl']}.{$delFile['fileType']}数据记录被删除，但文件删除失败。");
                    }
                    // 缩略文件
                    if(!unlink($this->upPath . DS . $delFile['fileUrl'] . '_thumb.' . $delFile['fileType'])){
                        $this->wLog("[系统行为]{$delFile['fileUrl']}_thumb.{$delFile['fileType']}数据记录被删除，但文件删除失败。");
                    }
                    return $datas;
                } else {
                    return;
                }
            }
            return;
        } else {
           $this->error("发生了一个严重的错误", "/");
        }

        $this->error("发生了一个严重的错误", "/");
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
                $fileName = explode(".", $info->getSaveName());

                if($info){
                    $data = [
                        'cid'           =>  session("Uploader"),
                        'fileUrl'       =>  $fileName[0],
                        'fileType'      =>  $fileName[1],
                        'uptime'        =>  date("Y-m-d H:i:s", time()),
                        'carousel'      =>  0
                    ];
                    $res = db('files')->insert($data, true);
                    $companyName = db("invitation")->where("id", session("Uploader"))->find()['company'];

                    if($res){
                        $this->wLog("[用户行为]{$companyName}，上传了文件{$info->getSaveName()}，成功存储！");
                    } else {
                        $this->wLog("[用户行为]{$companyName}，上传了文件{$info->getSaveName()}，存储失败！");
                    }

                    if(in_array($fileName[1], ['gif', 'jpg', 'jpeg', 'bmp', 'png'])){
                        // $newPic = $this->image_resize(file_get_contents($this->upPath . DS . $info->getSaveName()), 384, 216);
                        // if(file_put_contents($this->upPath . DS . $fileName[0] . "_thumb." . $fileName[1], $newPic)){
                        $manager = new ImageManager(array('driver' => 'imagick'));

                        $image = $manager->make(file_get_contents($this->upPath . DS . $info->getSaveName()))->resize(384, 216);

                        if($image->save($this->upPath . DS . $fileName[0] . "_thumb." . $fileName[1])){
                            $this->wLog("[系统行为]{$info->getSaveName()}，成功生成了缩略图。");
                        } else {
                            $this->wLog("[系统行为]{$info->getSaveName()}，成功缩略图失败了。");
                        }
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
                $uploads = db('files')->where("cid", $cid)->where('uptime','between time',[time() - 60 * 60, time()])->order('id desc')->paginate(5);
                $this->assign("uploads", $uploads);
                $this->assign("filePath", $this->upPath);
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

