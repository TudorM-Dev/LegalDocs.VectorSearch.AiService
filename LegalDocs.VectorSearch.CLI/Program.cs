using LegalDocs.VectorSearch.Core;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== LexCopilot: Legal AI Analysis Service ===");

string baseDir = AppDomain.CurrentDomain.BaseDirectory;
string docsFolder = Path.Combine(baseDir, "LegalDocuments");
string caseFilePath = Path.Combine(baseDir, "CurrentCase.txt");

if (!Directory.Exists(docsFolder))
{
    Directory.CreateDirectory(docsFolder);
    Console.WriteLine($"Created missing directory at: {docsFolder}");
}

Console.WriteLine($"Scanning for legal documents in: {docsFolder}");
var ingestor = new DocumentIngestor();
string fullLegalText = ingestor.ExtractTextFromDirectory(docsFolder);
Console.WriteLine($"Successfully extracted {fullLegalText.Length} characters of legal text.");

Console.WriteLine("Initializing Custom Vector Database...");
var vectorManager = new VectorMemoryManager();
await vectorManager.IngestTextAsync(fullLegalText);

Console.WriteLine("Initializing AI Engine (Ollama - Llama 3)...");
var aiEngine = new LegalAiEngine();
Console.WriteLine("Engine activated. Type 'exit' at any prompt to close the application.");
Console.WriteLine("--------------------------------------------------");

if (!File.Exists(caseFilePath))
{
    File.WriteAllText(caseFilePath, "The suspect was caught taking a laptop from the store.");
    Console.WriteLine($"Warning: Case file not found. Created a sample case at: {caseFilePath}");
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n[System: Automatically loading case details from CurrentCase.txt]");
Console.ResetColor();

string caseDescription = File.ReadAllText(caseFilePath);

Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("Searching for relevant laws in the Vector Database...");
string relevantContext = await vectorManager.SearchRelevantContextAsync(caseDescription);
Console.ResetColor();

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("AI is analyzing the case... (please wait, processing locally)");
Console.ResetColor();

string initialResponse = await aiEngine.AnalyzeTextAsync(caseDescription, relevantContext);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n=== AI INITIAL VERDICT ===");
Console.WriteLine(initialResponse);
Console.WriteLine("==========================\n");
Console.ResetColor();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("Do you have any further questions about this case? (Type 'exit' to quit): ");
    Console.ResetColor();

    string userInput = Console.ReadLine() ?? string.Empty;

    if (userInput.ToLower() == "exit") break;
    if (string.IsNullOrWhiteSpace(userInput)) continue;

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("AI is thinking... (please wait, processing locally)");
    Console.ResetColor();

    string response = await aiEngine.AnalyzeTextAsync(userInput, relevantContext);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n=== AI VERDICT ===");
    Console.WriteLine(response);
    Console.WriteLine("==================\n");
    Console.ResetColor();
}