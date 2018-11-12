<?php
namespace app\manager\controller;
use app\index\controller\Allbase as Allbase;

class TflmsMBase extends Allbase
{
	// 跳转回首页的设置
    protected $rehome = "<script>window.location.replace('/');</script>";

    public function _initialize()
    {
        // 基础初始化
    	// 基础初始化的东西开始

        parent::_initialize();
        if(empty(db("account")->select())){
            db("account")->insert(['account' => 'admin', 'password' => md5('led888')], true);
        }
        
        //检测登陆状态
        if(!session('manager')){
            return $this->redirect('passport/login');
        } else {

        }

    }

	public function _empty()
	{
        // 404页面
		abort(404, "Error!");
        return;
	}

    // 写日志
    public function wLog($content, $manager = "system")
    {
        $datas = [
            'logtext'   =>  $content,
            'logtime'   =>  date("Y-m-d H:i:s", time()),
            'form'      =>  $manager
        ];
        $res = db("log")->insert($datas);        
    }

    // 创建控制钥匙
    public function CreControlKey($keyName = "", $desc = "")
    {
        if(!file_exists($keyName)){
            file_put_contents($keyName, $desc);
            return true;
        } else {
            return false;
        }
        return false;
    } 

    // 创建邀请码
    public function GetInviteCode($length = 8) 
    { 
        // 密码字符集，可任意添加你需要的字符 
        // $chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_ []{}<>~`+=,.;:/?|'; 
        $chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'; 
        $invitecode = ""; 
        for ($i = 0; $i < $length; $i++){ 
            // 这里提供两种字符获取方式 
            // 第一种是使用 substr 截取$chars中的任意一位字符； 
            // 第二种是取字符数组 $chars 的任意元素 
            // $password .= substr($chars, mt_rand(0, strlen($chars) – 1), 1); 
            $invitecode .= $chars[ mt_rand(0, strlen($chars) - 1) ]; 
        } 
        return $invitecode; 
    } 

    /**
     * @param $imagedata    图像数据
     * @param $width        缩放宽度
     * @param $height       缩放高度
     * @param int $per      缩放比例，为0不缩放，>0忽略参数2、3的宽高
     * @return bool|string
     */
    public function image_resize($imagedata, $width, $height, $per = 0) {
        // 1 = GIF，2 = JPG，3 = PNG，4 = SWF，5 = PSD，6 = BMP，7 = TIFF(intel byte order)，8 = TIFF(motorola byte order)，9 = JPC，10 = JP2，11 = JPX，12 = JB2，13 = SWC，14 = IFF，15 = WBMP，16 = XBM
     
        // 获取图像信息
        list($bigWidth, $bigHight, $bigType) = getimagesizefromstring($imagedata);
     
        // 缩放比例
        if ($per > 0) {
            $width  = $bigWidth * $per;
            $height = $bigHight * $per;
        }
     
        // 创建缩略图画板
        $block = imagecreatetruecolor($width, $height);
     
        // 启用混色模式
        imagealphablending($block, false);
     
        // 保存PNG alpha通道信息
        imagesavealpha($block, true);
     
        // 创建原图画板
        $bigImg = imagecreatefromstring($imagedata);
     
        // 缩放
        imagecopyresampled($block, $bigImg, 0, 0, 0, 0, $width, $height, $bigWidth, $bigHight);
     
        // 生成临时文件名
        $tmpFilename = tempnam(sys_get_temp_dir(), 'image_');
     
        // 保存
        switch ($bigType) {
            case 1: imagegif($block, $tmpFilename);
                break;
     
            case 2: imagejpeg($block, $tmpFilename);
                break;
     
            case 3: imagepng($block, $tmpFilename);
                break;
        }
     
        // 销毁
        imagedestroy($block);
     
        $image = file_get_contents($tmpFilename);
     
        unlink($tmpFilename);
     
        return $image;
    }

}