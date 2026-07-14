# LexCopilot: Legal Tech AI Assistant ⚖️

LexCopilot is a local AI-powered legal document analysis engine built in C# (.NET). It leverages a Local Large Language Model (Llama 3 via Ollama) and a Retrieval-Augmented Generation (RAG) architecture to provide factual, context-aware legal verdicts without external API dependencies or data privacy risks.

## Core Features
*   **100% Local Execution:** Processes sensitive legal documents entirely on-device, ensuring zero data leakage.
*   **RAG Architecture:** Extracts text from local PDF documents (`UglyToad.PdfPig`) and constrains the AI to generate answers strictly based on the provided legal context.
*   **Conversational Memory:** Retains conversation history using `SemanticKernel`, allowing for complex, multi-turn follow-up questions about the active case.
*   **Automated Ingestion:** Dynamically ingests laws and case descriptions from designated directories and text files.

## Tech Stack
*   **Language:** C# / .NET
*   **AI Orchestration:** Microsoft Semantic Kernel
*   **Local LLM Engine:** Ollama (Llama 3 8B)
*   **Document Processing:** UglyToad.PdfPig

## Architecture
The system prevents "AI hallucinations" by using strict System Prompts combined with local document injection. If a legal query falls outside the provided context, the model is hard-coded to reject the assumption rather than invent legal statutes.
