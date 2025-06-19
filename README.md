# VideoForm 视频预览
## 本地视频文件
- VideoForm.exe "file://c:/video.mp4"
## 海康视频流协议
- VideoForm.exe "hikvision://username:password@192.168.1.64:8000"
## 宇视视频流协议
- VideoForm.exe "uniview://username:password@192.168.1.64:80"
## 通用视频协议 (rtmp/rtsp/http)
- VideoForm.exe "rtmp://192.168.1.64/live/stream"
- VideoForm.exe "rtsp://192.168.1.64:554/live/stream"
- VideoForm.exe "http://192.168.1.64:8000/stream.m3u8"
- VideoForm.exe "http://192.168.1.64:8000/mjpg/video.mjpg"
  
## 摄像头RTSP地址参考格式
- 海康摄像头：
  - 通道1主码流：rtsp://username:password@192.168.1.64:554/Streaming/Channels/101
  - 通道1子码流：rtsp://username:password@192.168.1.64:554/Streaming/Channels/102

- 海康摄像头 (2012年之前)
  - 通道1主码流：rtsp://username:password@192.168.1.64:554/h264/ch1/main/av_stream
  - 通道1子码流：rtsp://username:password@192.168.1.64:554/h264/ch1/sub/av_stream

- 海康录像机：
  - 通道1主码流: rtsp://username:password@192.168.1.64:554/Streaming/tracks/101?starttime=20250414t144720z&endtime=20250414t151415z

- 大华摄像头：
  - 通道1主码流：rtsp:///username:password@192.168.1.64/cam/realmonitor?channel=1&subtype=1
  - 通道1子码流：rtsp:///username:password@192.168.1.64/cam/realmonitor?channel=1&subtype=2

- 宇视摄像头：
  - 主码流：rtsp://username:password@192.168.1.64:554/media/video1
  - 子码流：rtsp://username:password@192.168.1.64:554/media/video2

- 华为摄像头
  - 主码流：rtsp://username:password@192.168.1.64:554/LiveMedia/ch1/Media1
  - 主码流：rtsp://username:password@192.168.1.64:554/LiveMedia/ch1/Media2

- TP-LINK摄像头
  - 主码流：rtsp://username:password@192.168.1.64:554/stream1
  - 子码流：rtsp://username:password@192.168.1.64:554/stream2

- 天地伟业摄像头
  - 主码流：rtsp://username:password@192.168.1.64:554/1/1
  - 子码流：rtsp://username:password@192.168.1.64:554/1/2

- 天视通摄像头
  - 主码流：rtsp://username:password@192.168.1.64:554/mpeg4
  - 子码流：rtsp://username:password@192.168.1.64:554/mpeg4cif
