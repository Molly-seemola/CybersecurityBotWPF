using System.Media;
using System.Threading.Tasks;

private void PlayVoiceGreeting()
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