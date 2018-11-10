<?php
// +----------------------------------------------------------------------
// | ThinkPHP [ WE CAN DO IT JUST THINK ]
// +----------------------------------------------------------------------
// | Copyright (c) 2006-2016 http://thinkphp.cn All rights reserved.
// +----------------------------------------------------------------------
// | Licensed ( http://www.apache.org/licenses/LICENSE-2.0 )
// +----------------------------------------------------------------------
// | Author: 流年 <liu21st@gmail.com>
// +----------------------------------------------------------------------

// 应用公共文件


// 创建目录
function mkdirs($dir, $mode = 0777)
{
    if (is_dir($dir) || mkdir($dir, $mode)) return true;
    if (!mkdirs(dirname($dir), $mode)) return false;
 
    return @mkdir($dir, $mode);
}

// 上传文件命名规则
function myUploadRule()
{
	return date('YmdHis');
	// return date('Ymd') . md5(microtime(true));
}

// 查找Banner文件夹下所有文件
function scanBannerFile($path = 'uploadcenter/banner')
{
	$files = scandir($path);
	$getFiles = [];
	foreach ($files as $file) {
		if($file != '.' && $file != '..'){
			if(is_dir($path . '/' . $file)){
				scandir($path . '/' . $file);
			} else {
				$getFiles[] = basename($file);
			}

		}

	}
	return $getFiles;
}