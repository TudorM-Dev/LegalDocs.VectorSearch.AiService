using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using LegalDocs.VectorSearch.Core;

namespace LegalDocs.VectorSearch.Desktop
{
    public partial class MainWindow : Window
    {
        private TaskCompletionSource<string> _waitForUserInput;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _ = RunConsoleLogicAsync();
        }

        private async Task RunConsoleLogicAsync()
        {
            try
            {
                Dispatcher.Invoke(() => {
                    SendButton.IsEnabled = false;
                    UserInput.IsEnabled = false;
                    ChatHistory.Text += "=== LexCopilot: Legal AI Analysis Service ===\n";
                });

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string docsFolder = Path.Combine(baseDir, "LegalDocuments");
                string caseFilePath = Path.Combine(baseDir, "CurrentCase.txt");

                if (!Directory.Exists(docsFolder))
                {
                    Directory.CreateDirectory(docsFolder);
                    UpdateUI($"Created missing directory at: {docsFolder}\n");
                }

                UpdateUI($"Scanning for legal documents in: {docsFolder}\n");
                var ingestor = new DocumentIngestor();
                string fullLegalText = ingestor.ExtractTextFromDirectory(docsFolder);
                UpdateUI($"Successfully extracted {fullLegalText.Length} characters of legal text.\n");

                UpdateUI("Initializing Custom Vector Database...\n");
                var vectorManager = new VectorMemoryManager();
                await vectorManager.IngestTextAsync(fullLegalText);

                UpdateUI("Initializing AI Engine (Ollama - Llama 3)...\n");

                var aiEngine = new LegalAiEngine();
                UpdateUI("Engine activated. Type your questions below.\n--------------------------------------------------\n");

                if (!File.Exists(caseFilePath))
                {
                    File.WriteAllText(caseFilePath, "The suspect was caught taking a laptop from the store.");
                    UpdateUI($"Warning: Case file not found. Created a sample case at: {caseFilePath}\n");
                }

                UpdateUI("\n[System: Automatically loading case details from CurrentCase.txt]\n");
                string caseDescription = File.ReadAllText(caseFilePath);

                UpdateUI("Searching for relevant laws in the Vector Database...\n");
                string relevantContext = await vectorManager.SearchRelevantContextAsync(caseDescription);

                UpdateUI("AI is analyzing the case... (please wait, processing locally)\n");
                string initialResponse = await aiEngine.AnalyzeTextAsync(caseDescription, relevantContext);

                UpdateUI($"\n=== AI INITIAL VERDICT ===\n{initialResponse}\n==========================\n\n");

                Dispatcher.Invoke(() => {
                    SendButton.IsEnabled = true;
                    UserInput.IsEnabled = true;
                });


                while (true)
                {
                    _waitForUserInput = new TaskCompletionSource<string>();

                    string userInput = await _waitForUserInput.Task;

                    if (userInput.ToLower() == "exit") break;
                    if (string.IsNullOrWhiteSpace(userInput)) continue;

                    UpdateUI($"\nYou: {userInput}\n\n");
                    UpdateUI("AI is thinking... (please wait, processing locally)\n");

                    Dispatcher.Invoke(() => SendButton.IsEnabled = false);

                    string response = await aiEngine.AnalyzeTextAsync(userInput, relevantContext);

                    UpdateUI($"\n=== AI VERDICT ===\n{response}\n==================\n");

                    Dispatcher.Invoke(() => {
                        SendButton.IsEnabled = true;
                        ScrollToBottom();
                    });
                }
            }
            catch (Exception ex)
            {
                UpdateUI($"\n[Critical Error]: {ex.Message}\n");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string query = UserInput.Text;
            if (string.IsNullOrWhiteSpace(query)) return;

            UserInput.Clear();

            _waitForUserInput?.TrySetResult(query);
        }

        private void UpdateUI(string text)
        {
            Dispatcher.Invoke(() => {
                ChatHistory.Text += text;
            });
        }

        private void ScrollToBottom()
        {
            var scrollViewer = GetDescendantByType(ChatHistory, typeof(System.Windows.Controls.ScrollViewer)) as System.Windows.Controls.ScrollViewer;
            scrollViewer?.ScrollToBottom();
        }

        private static System.Windows.DependencyObject GetDescendantByType(System.Windows.DependencyObject element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;
            System.Windows.DependencyObject foundElement = null;
            if (element is FrameworkElement frameworkElement) frameworkElement.ApplyTemplate();
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(element); i++)
            {
                System.Windows.DependencyObject visual = System.Windows.Media.VisualTreeHelper.GetChild(element, i);
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null) break;
            }
            return foundElement;
        }
    }
}