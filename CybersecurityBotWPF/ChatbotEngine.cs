using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CybersecurityBotWPF
{
    public class ChatbotEngine
    {
        private MainWindow ui; // reference to update chat display
        private string userName = "";
        private string lastTopic = "";      // for follow-up questions
        private Dictionary<string, List<string>> responses; // multiple responses per keyword
        private List<string> conversationHistory;

        // Simple sentiment detection keywords
        private string[] sadWords = { "sad", "worried", "scared", "anxious", "upset" };
        private string[] happyWords = { "happy", "great", "good", "excited" };
        private string[] curiousWords = { "tell me more", "explain", "how", "why", "what" };

        public ChatbotEngine(MainWindow window)
        {
            ui = window;
            conversationHistory = new List<string>();
            InitializeResponses();
        }

        // Store multiple responses per topic (random selection)
        private void InitializeResponses()
        {
            responses = new Dictionary<string, List<string>>();

            responses["password"] = new List<string>
            {
                "Use a mix of uppercase, lowercase, numbers, and symbols. Aim for 12+ characters.",
                "Never reuse passwords across sites. Try a password manager!",
                "A strong password is like a toothbrush: change it regularly and don't share it."
            };

            responses["phishing"] = new List<string>
            {
                "Phishing emails often have urgent requests or bad grammar. Always check the sender.",
                "Don't click suspicious links. Hover to see the real URL first.",
                "If an offer seems too good to be true, it's probably a phishing scam."
            };

            responses["privacy"] = new List<string>
            {
                "Limit what you share on social media. Cybercriminals use that info.",
                "Use two-factor authentication wherever possible.",
                "Regularly review app permissions on your phone."
            };

            responses["scam"] = responses["phishing"]; // reuse

            responses["greeting"] = new List<string>
            {
                "Hello! How can I help you stay safe online today?",
                "Hi there! Ready to learn about cybersecurity?",
                "Greetings! Ask me about passwords, phishing, or privacy."
            };

            responses["default"] = new List<string>
            {
                "Interesting. Could you tell me more?",
                "I'm not sure about that. Ask me about passwords, phishing, or privacy.",
                "Let's focus on cybersecurity. What would you like to know?"
            };
        }

        // Add user message to UI
        public void AddUserMessage(string msg)
        {
            conversationHistory.Add($"User: {msg}");
            ui.ChatDisplay.Items.Add($"You: {msg}");
        }

        // Add bot message to UI
        public void AddBotMessage(string msg)
        {
            conversationHistory.Add($"Bot: {msg}");
            ui.ChatDisplay.Items.Add($"Bot: {msg}");
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            ui.ChatDisplay.ScrollIntoView(ui.ChatDisplay.Items[ui.ChatDisplay.Items.Count - 1]);
        }

        // Play voice greeting asynchronously
        public async Task PlayGreetingAsync()
        {
            await AudioService.PlayGreetingAsync();
        }

        // Main response logic: keyword recognition, memory, sentiment, flow
        public string GetResponse(string input)
        {
            string lowerInput = input.ToLower();

            // 1. Sentiment detection – adjust tone
            string sentiment = DetectSentiment(lowerInput);
            string empathyPrefix = "";
            if (sentiment == "sad")
                empathyPrefix = "I'm sorry you're feeling that way. ";
            else if (sentiment == "happy")
                empathyPrefix = "That's great to hear! ";

            // 2. Memory: if user gives name (first time)
            if (string.IsNullOrEmpty(userName) && lowerInput.Contains("my name is"))
            {
                userName = ExtractName(input);
                return $"Nice to meet you, {userName}! I'll remember that. Now, ask me about cybersecurity.";
            }
            if (!string.IsNullOrEmpty(userName) && lowerInput.Contains("my name"))
            {
                return $"I already know you as {userName}. Want to change it? Just say 'my name is new name'.";
            }

            // 3. Follow-up questions (conversation flow)
            if (IsFollowUp(lowerInput) && !string.IsNullOrEmpty(lastTopic))
            {
                return empathyPrefix + GetRandomResponse(lastTopic);
            }

            // 4. Keyword recognition (cybersecurity topics)
            string topic = IdentifyTopic(lowerInput);
            if (!string.IsNullOrEmpty(topic))
            {
                lastTopic = topic; // remember for follow-up
                return empathyPrefix + GetRandomResponse(topic);
            }

            // 5. Default fallback
            return empathyPrefix + GetRandomResponse("default");
        }

        private string IdentifyTopic(string input)
        {
            if (input.Contains("password") || input.Contains("passphrase")) return "password";
            if (input.Contains("phish") || input.Contains("scam")) return "phishing";
            if (input.Contains("privacy") || input.Contains("data") || input.Contains("personal")) return "privacy";
            if (input.Contains("hello") || input.Contains("hi") || input.Contains("hey")) return "greeting";
            return null;
        }

        private string GetRandomResponse(string topic)
        {
            if (responses.ContainsKey(topic))
            {
                var list = responses[topic];
                Random rand = new Random();
                return list[rand.Next(list.Count)];
            }
            return responses["default"][new Random().Next(responses["default"].Count)];
        }

        private bool IsFollowUp(string input)
        {
            string[] followPhrases = { "tell me more", "explain more", "another tip", "continue", "more" };
            return followPhrases.Any(phrase => input.Contains(phrase));
        }

        private string DetectSentiment(string input)
        {
            if (sadWords.Any(w => input.Contains(w))) return "sad";
            if (happyWords.Any(w => input.Contains(w))) return "happy";
            if (curiousWords.Any(w => input.Contains(w))) return "curious";
            return "neutral";
        }

        private string ExtractName(string input)
        {
            // simple: assume after "my name is"
            int index = input.ToLower().IndexOf("my name is");
            if (index >= 0)
            {
                string namePart = input.Substring(index + 10).Trim();
                return namePart.Split(' ')[0]; // first word
            }
            return "friend";
        }
    }
}