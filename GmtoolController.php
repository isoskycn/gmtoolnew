<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;
use App\Http\Controllers\Encrypt;

class GmtoolController extends Controller
{
    public function index(Request $request)
	{
      return $this->errorPost();
	}
		
	//客户端启动时初始化
    public function init(Request $request)
    {
        if ($request->isMethod('post')) {
			$post = $this->checkData($request->input('data'));
			if (!$post) return $this->errorPost();
			$chk = json_decode($this->getCardInfo($post->code,$post->product_id));
            $code = 0;
            $msg = $chk->msg;
            $data = [];
            if ($chk->code == 204 && $post->machine_code == $chk->data->machine_code) {
                $code = 200;
                $msg = '授权成功!到期时间:' . date("Y-m-d H:i", $chk->data->expire_time);
                $data = [$chk->data];
                if($post->type !=1) //1为心跳检测,不记录此类日志
				{
				$map = [
					'product_id' => $post->product_id,
                    'code' => $post->code,
                    'machine_code' => $post->machine_code,
                    'ip' => $request->getClientIp(),
                    'login_type' => 1,
                    'login_time' => time()
                ];
                Db::table('codelogs')->insert($map);
				}
            }
            return $this->encryptData($code, $msg, $data);
        }
        return $this->errorPost();
    }
	
	//激活
	public function reg(Request $request)
    {
        if ($request->isMethod('post')) {
            $post = $this->checkData($request->input('data'));
            if (!$post) return $this->errorPost();
            $chk = json_decode($this->getCardInfo($post->code,$post->product_id));
            $code = 0;
            $msg = $chk->msg;
            $data = [];
            if ($chk->code == 204) {
                if ($post->machine_code == $chk->data->machine_code) {
                    $code = 200;
                    $msg = "授权成功!到期时间:" . date("Y-m-d H:i", $chk->data->expire_time);
                    $data = [$chk->data];
                    $map = [
						'product_id' => $post->product_id,
                        'code' => $post->code,
                        'machine_code' => $post->machine_code,
                        'ip' => $request->getClientIp(),
                        'login_type' => 1,
                        'login_time' => time()
                    ];
                    Db::table('codelogs')->insert($map);
                }
            }
            if ($chk->code == 200) {

                $sTime = time();
                $eTime = 0;
                switch ($chk->data->card_type) {
                    case 1:
                        $eTime = $sTime + (60 * 60 * 24);
                        break;
                    case 2:
                        $eTime = $sTime + (60 * 60 * 24 * 7);
                        break;
                    case 3:
                        $eTime = $sTime + (60 * 60 * 24 * 30);
                        break;
                    case 4:
                        $eTime = $sTime + (60 * 60 * 24 * 30 * 3);
                        break;
                    case 5:
                        $eTime = $sTime + (60 * 60 * 24 * 30 * 6);
                        break;
                    case 6:
                        $eTime = $sTime + (60 * 60 * 24 * 365);
                        break;
                }
                $map = [
                    "status" => '1',
                    "machine_code" => $post->machine_code,
                    "start_time" => $sTime,
                    "expire_time" => $eTime
                ];
                Db::table('codes')->where('id', $chk->data->id)->update($map);
				//更新售卡状态
				$status = [
					"card_status" => '2'
				];
				DB::table('cards')->where('card_info',$post->code)->update($status);
                $code = 200;
                $msg = "授权成功!到期时间:" . date("Y-m-d H:i", $eTime);
                $data = [$map];
                $map2 = [
					'product_id' => $post->product_id,
                    'code' => $post->code,
                    'machine_code' => $post->machine_code,
                    'ip' => $request->getClientIp(),
                    'login_type' => 2,
                    'login_time' => time()
                ];
                Db::table('codelogs')->insert($map2);
            }
            return $this->encryptData($code, $msg, $data);
        }
        return $this->errorPost();
    }
	
	//改绑
    public function change(Request $request)
    {
        if ($request->isMethod('post')) {
            $post = $this->checkData($request->input('data'));
            if (!$post) return $this->errorPost();
            $chk = json_decode($this->getCardInfo($post->code,$post->product_id));
            $code = 0;
            $msg = $chk->msg;
            $data = [];
            if ($chk->code == 204) {
                if($chk->data->change_times>=3)
                {
                    $msg = '换绑次数已达上限,无法换绑!';
                    return $this->encryptData($code, $msg, $data);
                }
                if($post->oldmachine_code == $chk->data->machine_code)
                {
                    Db::table('codes')->where('code',$post->code)->update(['machine_code'=>$post->machine_code]);
					Db::table('codes')->where('code',$post->code)->increment('change_times');
                    $code = 200;
                    $msg = '改绑成功!';
                    $data = $chk->data->expire_time;
                    $map = [
						'product_id' => $post->product_id,
                        'code' => $post->code,
                        'machine_code' => $post->machine_code,
                        'ip' => $request->getClientIp(),
                        'login_type' => 3,
                        'login_time' => time()
                    ];
                    Db::table('codelogs')->insert($map);
                }else{
                    $msg = '旧机器码错误,无法换绑!';
                }
            }
            return $this->encryptData($code, $msg, $data);
        }
        return $this->errorPost();

    }
	
	//激活码校验
    public function getCardInfo($code,$pid)
    {
		$res = Db::table("codes")->where(["code" =>$code,"product_id" =>$pid])->first();
        if (!$res) {
            $code = 201;
            $msg = "授权码不存在!";
            return json_encode(["code" => $code, "msg" => $msg]);
        }
        if ($res->status == 2 || ($res->expire_time != null && time() > $res->expire_time)) {
            Db::table("codes")->where('code', $res->code)->update(["status" => 2]);
            $code = 202;
            $msg = "授权码已过期!";
            return json_encode(["code" => $code, "msg" => $msg]);
        }
        if ($res->onlineswitch == 1) {
            $code = 203;
            $msg = "授权码已被封禁!";
            return json_encode(["code" => $code, "msg" => $msg]);
        }
        if ($res->status == 1) {
            $code = 204;
            $msg = "授权码已使用,可尝试换绑!";
            return json_encode(["code" => $code, "msg" => $msg, "data" => $res]);
        }
        if ($res->status == 0 && $res->onlineswitch == 0) {
            $code = 200;
            $msg = "授权码正常,可激活!";
            return json_encode(["code" => $code, "msg" => $msg, "data" => $res]);
        }
        return json_encode(["code" => 0, "msg" => "未知错误"]);
    }
	
	//更新程序版本
    public function update()
    {
        $res = new Encrypt();
		$map =[
			"code" =>1,
			"version" => "0.9.4",
			"download_url" => "https://shop.isoskycn.com/uploads/my/gmtoolnew/gmtoolNew.exe",
			"file_name" => "gmtoolNewV0.9.4.exe",
			"update_time" => "2021-03-15 22:45",
			"hash"=>$res->getiv(),
			"time"=>time()
		];
		return json_encode($map,JSON_UNESCAPED_SLASHES);
    }
    //错误处理
    public function errorPost()
    {
        $code = 0;
        $msg = '请求非法!';
        $data = [time()];
        return $this->encryptData($code, $msg, $data);
    }
    //数据检测
    public function checkData($data)
    {
        try {
            return $this->decryptData($data);
        } catch (Exception $ex) {
            return false;
        }
    }

    //解密数据
    public function decryptData($data)
    {
        $res = new Encrypt();
        return json_decode($res->decrypt($data));
    }

    //加密数据
    public function encryptData($code, $msg, $data = '')
    {
        $res = new Encrypt();
        $json = json_encode(["code" => $code, "msg" => $msg, "data" => $data]);
        return $res->encrypt($json);
    }
}