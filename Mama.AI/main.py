from fastapi import FastAPI
from pydantic import BaseModel

from services.pdf_service import extract_text_from_pdf
from services.chunk_service import chunk_text
from services.embedding_service import create_embedding
from services.ollama_service import ask_qwen
from vectorstore.chroma_db import store_chunks, search_chunks
import json


app = FastAPI(
    title="Mama.AI Service",
    description="AI service for PDF processing, RAG, and NEET question generation",
    version="1.0.0"
)


class PdfProcessRequest(BaseModel):
    file_path: str


class StoreChunksRequest(BaseModel):
    document_id: int
    title: str
    subject: str
    chunks: list[dict]


class SearchRequest(BaseModel):
    query: str
    top_k: int = 5


class AskFromRagRequest(BaseModel):
    query: str
    top_k: int = 3


class GenerateQuestionsRequest(BaseModel):
    subject: str
    topic: str
    difficulty: str = "Medium"
    count: int = 5
    top_k: int = 3


@app.get("/")
def home():
    return {
        "message": "Mama.AI Service is running"
    }


@app.get("/health")
def health():
    return {
        "status": "ok"
    }


@app.get("/test-embedding")
def test_embedding():
    embedding = create_embedding("Photosynthesis occurs in chloroplasts.")

    return {
        "success": True,
        "dimension": len(embedding),
        "sample": embedding[:5]
    }


@app.post("/extract-pdf-text")
def extract_pdf_text(request: PdfProcessRequest):
    result = extract_text_from_pdf(request.file_path)

    if result.get("success"):
        chunks = chunk_text(result.get("full_text", ""))

        result["chunk_count"] = len(chunks)
        result["chunks"] = chunks

    return result


@app.post("/store-chunks")
def store_document_chunks(request: StoreChunksRequest):
    return store_chunks(
        document_id=request.document_id,
        title=request.title,
        subject=request.subject,
        chunks=request.chunks
    )


@app.post("/search-chunks")
def search_document_chunks(request: SearchRequest):
    return search_chunks(
        query=request.query,
        top_k=request.top_k
    )


@app.post("/ask-rag")
def ask_rag(request: AskFromRagRequest):
    search_result = search_chunks(
        query=request.query,
        top_k=request.top_k
    )

    documents = search_result["results"]["documents"][0]

    context = "\n\n".join(documents)

    prompt = f"""
You are an expert NEET Biology, Physics, and Chemistry tutor.

Use only the context below to answer the question.
Give a short, clear answer suitable for NEET students.
If the answer is not found in the context, say:
"The answer is not available in the provided context."

Context:
{context}

Question:
{request.query}

Answer:
"""

    answer = ask_qwen(prompt)

    return {
        "success": True,
        "query": request.query,
        "answer": answer.strip(),
        "context_used": documents
    }


@app.post("/generate-questions")
def generate_questions(request: GenerateQuestionsRequest):
    query = f"""
        Generate NEET MCQs about:
        {request.topic}

        Key concepts:
        fertilisation
        zona pellucida
        acrosome
        ampullary-isthmic junction
        """

    search_result = search_chunks(
        query=query,
        top_k=request.top_k
    )

    documents = search_result["results"]["documents"][0]
    metadatas = search_result["results"]["metadatas"][0]

    context = "\n\n".join(documents[:3])

    prompt = f"""
You are a NEET Biology, Physics, and Chemistry question paper setter.

IMPORTANT RULES:
1. Return ONLY valid JSON.
2. Do NOT add markdown.
3. Do NOT add explanations outside JSON.
4. Do NOT summarize the chapter.
5. Generate exactly {request.count} MCQs.
6. Generate questions only from the requested topic.
7. Do not generate questions from unrelated topics.
8. Use ONLY the provided NCERT context.
9. Do not return only one question unless count is 1.
10. Each MCQ must have:
   - question
   - optionA
   - optionB
   - optionC
   - optionD
   - correctAnswer
   - explanation

Requested Subject:
{request.subject}

Requested Topic:
{request.topic}

Difficulty:
{request.difficulty}

Output JSON format:
{{
  "questions": [
    {{
      "question": "Question text",
      "optionA": "Option A",
      "optionB": "Option B",
      "optionC": "Option C",
      "optionD": "Option D",
      "correctAnswer": "A",
      "explanation": "Short explanation"
    }}
  ]
}}

The "questions" array must contain exactly {request.count} objects.

NCERT Context:
{context}

Generate exactly {request.count} MCQs now.

IMPORTANT:

The retrieved context contains information from multiple parts of the chapter.

Generate questions ONLY about:
{request.topic}

Ignore any information unrelated to:
{request.topic}

If the context contains unrelated information, do not use it.
"""

    answer = ask_qwen(prompt)
    
    try:
        parsed_questions = json.loads(answer)
    except json.JSONDecodeError:
        return {
            "success": False,
            "error": "Qwen returned invalid JSON",
            "raw_response": answer.strip()
        }

    return {
        "success": True,
        "subject": request.subject,
        "questions": parsed_questions["questions"],
        "topic": request.topic,
        "difficulty": request.difficulty,
        "raw_response": answer.strip(),
        "retrieved_metadata": metadatas,
        "context_used": documents
    }