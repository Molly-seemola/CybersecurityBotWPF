using System.Media;
using System.Threading.Tasks;

namespace CybersecurityBotWPF
{
    public static class AudioService
    {
        public static async Task PlayGreetingAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (SoundPlayer player = new SoundPlayer("greeting.wav"))
                    {
                        player.PlaySync();
                    }
                }
                catch
                {
                    // Silent fail – GUI still works
                }
            });
        }
    }
}