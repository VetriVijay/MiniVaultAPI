# MiniVaultAPI

A lightweight **.NET 8 Minimal API** simulating **ModelVault’s core prompt-response feature** using a **local LLM (Ollama)** with:

**Streaming support**  
**Structured logging**

---

### **Key Features**

- Receives a text prompt and returns either:
  - **Full generated response** from local Ollama LLM
  - **Streaming token-by-token response** via `/generate/stream` endpoint
- Logs all input/output to a structured JSON log file: `logs/log.jsonl`

---

### ⚙️ **Prerequisites**

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Ollama installed and running locally](https://ollama.ai/) (**Installation Guide**)
- *(Optional)* [Postman](https://www.postman.com/downloads/) for API testing

------------------------------------------------

### **Project Setup**

1. **Clone the repository** (if applicable) or download the project files.

2. **Navigate** to the API project folder in VS Code terminal:

   ```bash
   cd MiniVaultApi


### **1. Restore Dependencies**

Execute the following command in the **VS Code terminal**:

```bash
dotnet restore

Install Ollama in our local machine

Configure LLM endpoint:
Update appsettings.json with your local Ollama URL if different:
"LlmSettings": {
  "BaseUrl": "http://localhost:11434/api"
}

------------------------------------------------

Running the API:
VS code terminal run this command dotnet run
After starting, visit:

http://localhost:5111/swagger

to view Swagger UI documentation.

Endpoint 1  - POST /generate
Returns the full response as JSON.

Request:
{ "prompt": "Tell me about Taj Mahal" }

Endpoint 1  - POST /generate/stream

Returns the response token-by-token as plain text stream.

------------------------------------------------

### ** Testing via CLI**

You can test this Minimal API using the provided **`TestingCLI.sh` file**.

**Steps to test via CLI:**

1. **Navigate** to the project folder containing `TestingCLI.sh` in your terminal.

2. **Make it executable:**
   chmod +x TestingCLI.sh

3. **Run the tests:
./TestingCLI.sh
------------------------------------------------

### ** Testing via Postman**
Import this “MiniVault_API_postman_collection.json” json file into Postman

POST http://localhost:5111/generate

Body: { "prompt": "Tell me about Taj Mahal" }

Content-Type: application/json

POST http://localhost:5111/generate/stream

Body: { "prompt": "Tell me about Mount Everest" }

Content-Type: application/json

Send requests and view responses in Postman.

Note: Streaming responses may appear as a single response block in Postman UI. Therefore, streaming logic is validated using CLI. You can also test streaming token by token directly by calling: http://localhost:5111/generate/stream




