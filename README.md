# VideoForm 视频预览
## 本地视频文件
VideoForm.exe "file://c:/video.mp4"
## 海康视频流协议
VideoForm.exe "hikvision://admin:pwd@192.168.1.64:8000"
## 宇视视频流协议
VideoForm.exe "uniview://admin:pwd@192.168.1.64:80"
## 通用视频协议 (rtmp/rtsp/http)
VideoForm.exe "rtmp://192.168.1.64/live/stream"
VideoForm.exe "rtsp://192.168.1.64:554/live/stream"
VideoForm.exe "http://192.168.1.64:8000/stream.m3u8"
VideoForm.exe "http://192.168.1.64:8000/mjpg/video.mjpg"