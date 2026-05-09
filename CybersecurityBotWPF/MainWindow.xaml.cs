using System;
using System.Windows;
using System.Windows.Input;

namespace CybersecurityBotWPF
{
    public partial class MainWindow : Window
    {
        private ChatbotEngine engine; // Core logic

        public MainWindow()
        {
            InitializeComponent();
            engine = new ChatbotEngine(this);
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Show ASCII art
            AsciiArtBox.Text = AsciiArtHelper.GetLogo();
            // Play voice greeting on startup (optional)
            await engine.PlayGreetingAsync();
            // Initial bot message
            engine.AddBotMessage("Hello! I'm your Cybersecurity Assistant. What's your name?");
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ProcessUserInput();
        }

        private void ProcessUserInput()
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            engine.AddUserMessage(input);
            string reply = engine.GetResponse(input);
            engine.AddBotMessage(reply);
            UserInput.Clear();
        }

        private async void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            await engine.PlayGreetingAsync();
        }
    }
}