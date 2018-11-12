<?php
namespace app\index\controller;
use think\Controller;
use think\Db;
use think\Validate;
use think\Loader;

class Allbase extends Controller
{
    // *各种定义
    // >应用名称
    public $appName = "四分之三·区块链全球商业中心(大屏幕管理系统)";
    // >页面名称
    public $pageName = "";
    // Path
    protected $upPath = "tqupload";

    public function _initialize()
    {
        // 通用初始化

        // 渲染全局变量
        $this->AllAssign();
        $this->SetPageName("");
        
        // 创建上传中心目录
        mkdirs($this->upPath);

        // 创建配置目录
        // mkdirs("config");                       // 配置目录

    }

    // *公共或全局渲染
    public function AllAssign()
    {
    	// >应用名称
    	$this->assign("appName", $this->appName);

    }

    // *页面名称渲染
    public function SetPageName($pageName)
    {
        $this->pageName = $pageName;
        $this->assign("pageName", $pageName);
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

    public function image_resize($imagedata, $width, $height, $per = 0) {
        /**
         * @param $imagedata    图像数据
         * @param $width        缩放宽度
         * @param $height       缩放高度
         * @param int $per      缩放比例，为0不缩放，>0忽略参数2、3的宽高
         * @return bool|string
         */
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