using CSCore;
using CSCore.Codecs;
using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace CSharpUI2DImpl
{
    public class LoadingItemEventArgs : EventArgs
    {
        public int Percentage { set; get; }
        public string Name { set; get; }
        public LoadingItemEventArgs(int percentage,string name)
        {
            Percentage = percentage;
            Name = name;
        }
    }
    public delegate void LoadingItemHandler(object sender, LoadingItemEventArgs e);
    class CS2DResource
    {
        public event LoadingItemHandler LoadingItem;

        public string CurrentPath;
        public Image GroundGrass;
        public Image HeroA;

        public IWaveSource BackgroundMusic;

        public CS2DResource() { }

        public bool Load()
        {
            CurrentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                if (LoadingItem != null)
                    LoadingItem(this, new LoadingItemEventArgs(0, "Ground"));
                GroundGrass = Image.FromFile(Path.Combine(CurrentPath, @"CS2DResources\Map\ground.png"));
                Thread.Sleep(1500);
                if (LoadingItem != null)
                    LoadingItem(this, new LoadingItemEventArgs(50, "Music"));
                BackgroundMusic = CodecFactory.Instance.GetCodec(Path.Combine(CurrentPath, @"CS2DResources\Music\17 You Win (Original).mp3"));
                Thread.Sleep(1500);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
    }
}
