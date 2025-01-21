using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoForm.Handler
{
    internal class OCHandler : IDisposable
    {
        private CancellationTokenSource _cancelTokenSource;
        private readonly PictureBox _pictureBox;
        private VideoCapture _capture;
        private Bitmap _displayImage;
        private OpenCvSharp.Size _displaySize;
        private Mat _resizedMat;//用于缩放显示图片

        public Action<string> Msg;
        private Action<Bitmap> PreviewCallback { get; set; }
        public virtual void OnMsg(string msg)
        {
            Msg?.Invoke(msg);
        }

        public OCHandler(PictureBox pictureBox)
        {
            _pictureBox = pictureBox;
            _pictureBox.Resize += PictureBox_Resize;
            _displaySize = new OpenCvSharp.Size(pictureBox.Width, pictureBox.Height);
            PreviewCallback = _worker_ProgressChanged;
        }

        private void PictureBox_Resize(object sender, EventArgs e)
        {
            _displaySize = new OpenCvSharp.Size(_pictureBox.Width, _pictureBox.Height);
        }

        public void Preview(string videoPath)
        {
            if (_cancelTokenSource != null)
                _cancelTokenSource.Cancel();

            _cancelTokenSource = new CancellationTokenSource();
            _capture = new VideoCapture();           

            Task.Factory.StartNew(() =>
            {
                if (!_capture.Open(videoPath))
                {
                    OnMsg("打开视频失败！");
                    return;
                }
                int _sleepTime = (int)(1000 / _capture.Get(VideoCaptureProperties.Fps));
                _worker_DoWork(_sleepTime);
            }, _cancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ContinueWith(t =>
            {
                //OnMsg("视频播放已取消！");
                if (_displayImage != null)
                {
                    _displayImage.Dispose();
                    _displayImage = null;
                }
            });
        }

        private void _worker_DoWork(int sleepTime)
        {
            while (!_cancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (_capture == null || !_capture.IsOpened())
                        break;

                    using (var frameMat = _capture.RetrieveMat())
                    {
                        if (frameMat.Empty())
                        {
                            _cancelTokenSource.Cancel();
                            OnMsg("视频播放已结束！");
                            break;
                        }
                        if (_resizedMat == null || _resizedMat.Size() != _displaySize)
                        {
                            _resizedMat = new Mat();
                        }
                        Cv2.Resize(frameMat, _resizedMat, _displaySize);
                        var frameBitmap = BitmapConverter.ToBitmap(_resizedMat);
                        PreviewCallback?.Invoke(frameBitmap);
                    }
                    Thread.Sleep(sleepTime);
                }
                catch (Exception ex)
                {
                    OnMsg("视频播放异常：" + ex.Message);
                    _cancelTokenSource.Cancel();
                    break;
                }

            }
            if (_capture != null && _capture.IsOpened())
            {
                _capture.Release();
                _capture.Dispose();
                _capture = null;
            }
            if (_resizedMat != null)
            {
                _resizedMat.Dispose();
                _resizedMat = null;
            }
        }

        private void _worker_ProgressChanged(Bitmap bitmap)
        {
            if (_pictureBox.Image != null)
            {
                _pictureBox.Image.Dispose();
                _pictureBox.Image = null;
            }
            try
            {
                _displayImage = bitmap;
                _pictureBox.Image = _displayImage;
            }
            catch (Exception ex)
            {
                OnMsg("显示图片异常：" + ex.Message);
            }
        }

        public void StopPreview()
        {
            if (_cancelTokenSource != null)
                _cancelTokenSource.Cancel();
            if (_capture != null && _capture.IsOpened())
            {
                _capture.Release();
                _capture.Dispose();
                _capture = null;
            }
        }

        public void Dispose()
        {
            StopPreview();
        }
    }
}
