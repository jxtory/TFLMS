<?php
// +----------------------------------------------------------------------
// | ThinkPHP [ WE CAN DO IT JUST THINK ]
// +----------------------------------------------------------------------
// | Copyright (c) 2006~2016 http://thinkphp.cn All rights reserved.
// +----------------------------------------------------------------------
// | Licensed ( http://www.apache.org/licenses/LICENSE-2.0 )
// +----------------------------------------------------------------------
// | Author: liu21st <liu21st@gmail.com>
// +----------------------------------------------------------------------

return [
    '__pattern__' => [
        'name' => '\w+',
    ],
    // Start - 前端功能区
    'ht' =>	'manager/index/index',
    'upload'	=>	'index/index/upload',
    'instruction'	=>	'index/index/instruction',
    // End - 前端功能区
    // 管理-创建账号
    'ht/cac'	=>	'manager/passport/creaccount',
    // 管理-大门控制器
    'ht/dc'	=>	'manager/index/doorcontrol',
    // 管理-大屏幕控制器
    'ht/lc'	=>	'manager/index/ledcontrol',
    // 管理-停止系统工作
    'ht/clc' => 'manager/index/stopsystem',
    // 管理-清除日志信息
    'ht/clog' =>    'manager/index/clearlog',
    // 管理-邀请信息控制
    'ht/ic' =>  'manager/index/invitationcontrol',
    // 用户发起上传
    'up' => 'index/index/upcontrol',
    // 用户发起删除
    'upd' =>	'index/index/upcontrol_del'
];
