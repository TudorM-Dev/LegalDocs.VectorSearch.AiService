# LexCopilot: Legal Tech AI Assistant ⚖️

LexCopilot is a local AI-powered legal document analysis engine built in C# (.NET). It leverages a Local Large Language Model (Llama 3 via Ollama) and a Retrieval-Augmented Generation (RAG) architecture to provide factual, context-aware legal verdicts entirely on-device.

<img width="880" height="595" alt="Screenshot 2026-07-14 223614" src="https://github.com/user-attachments/assets/beb9a809-b9c4-4ee8-80cb-608b876e0ef4" />

## 🖥️ Desktop Interface
**LexCopilot Desktop** provides a fluid, modern interface for legal professionals to interact with the AI engine, manage cases, and review verdicts in real-time.

<img width="883" height="593" alt="Screenshot 2026-07-14 223819" src="https://github.com/user-attachments/assets/07700452-1768-40e6-8de2-ff895e6aa06b" />

## Core Features
*   **100% Local Execution:** Processes sensitive legal documents entirely on-device, ensuring zero data leakage and total privacy.
*   **RAG Architecture:** Constrains the AI to generate answers strictly based on your uploaded legal context, preventing hallucinations.
*   **Intuitive UI:** A clean, modern WPF interface designed for high-focus legal research.
*   **Conversational Memory:** Retains multi-turn conversation history for complex case investigations.
*   **Automated Ingestion:** Seamlessly processes PDF libraries and case descriptions.

<img width="1475" height="760" alt="Screenshot 2026-07-14 223948" src="https://github.com/user-attachments/assets/54efd213-b160-47ee-aadf-15d17b16de4b" />
<img width="1460" height="367" alt="Screenshot 2026-07-14 224229" src="https://github.com/user-attachments/assets/733a130c-758a-48ce-9912-dfb6ac12d6cc" />

## Tech Stack
*   **Language:** C# / .NET
*   **Desktop UI:** WPF (Native)
*   **AI Orchestration:** Microsoft Semantic Kernel
*   **Local LLM Engine:** Ollama (Llama 3 8B)
*   **Document Processing:** UglyToad.PdfPig

## Architecture
The system prevents "AI hallucinations" by using strict System Prompts combined with local document injection. If a legal query falls outside the provided context, the model is hard-coded to reject the assumption rather than invent legal statutes.
