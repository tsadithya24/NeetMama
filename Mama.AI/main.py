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

class GenerateFlashCardsRequest(BaseModel):
    subject: str
    topic: str
    card_type: str = "Concept"
    count: int = 10
    top_k: int = 3

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
    question_type: str = "MCQ"
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
{request.subject}
{request.topic}
{request.question_type}
{request.difficulty}
"""

    search_result = search_chunks(
        query=query,
        top_k=request.top_k
    )

    documents = search_result["results"]["documents"][0]
    metadatas = search_result["results"]["metadatas"][0]

    context = "\n\n".join(documents[:3])

    if request.question_type == "MCQ":
        question_type_prompt = """
Generate normal NEET-style MCQs.
"""

    elif request.question_type == "FillBlank":
        question_type_prompt = """
Generate fill-in-the-blank questions, but still provide four MCQ options.

Example:
Question: The process of fusion of sperm and ovum is called ________.
OptionA: Fertilisation
OptionB: Ovulation
OptionC: Implantation
OptionD: Parturition
CorrectAnswer: A
"""

    elif request.question_type == "ScientistFact":
        question_type_prompt = """
Generate questions about scientists, discoveries, experiments, contributors, years, or important scientific facts.
Still provide four MCQ options.
"""

    elif request.question_type == "AssertionReason":
        question_type_prompt = """
Generate assertion-reason style questions.

Question format:
Assertion (A): ...
Reason (R): ...

Options:
A. Both A and R are true and R is the correct explanation of A.
B. Both A and R are true but R is not the correct explanation of A.
C. A is true but R is false.
D. A is false but R is true.
"""

    elif request.question_type == "ReactionBased":
        question_type_prompt = """
Generate chemistry reaction-based questions about reactants, products, catalysts, reagents, named reactions, or conditions.
Still provide four MCQ options.
"""

    else:
        question_type_prompt = """
Generate normal NEET-style MCQ-compatible questions.
"""

    prompt = f"""
You are a NEET Biology, Physics, and Chemistry question paper setter.

IMPORTANT RULES:
1. Return ONLY valid JSON.
2. Do NOT add markdown.
3. Do NOT add explanations outside JSON.
4. Do NOT summarize the chapter.
5. Generate exactly {request.count} questions.
6. Generate questions only from the requested topic.
7. Do not generate questions from unrelated topics.
8. Use ONLY the provided NCERT context.
9. Do not return only one question unless count is 1.
10. All generated questions must be compatible with this structure:
   - question
   - optionA
   - optionB
   - optionC
   - optionD
   - correctAnswer
   - explanation
11. correctAnswer must be only A, B, C, or D.
12. The selected question type is: {request.question_type}

Requested Subject:
{request.subject}

Requested Topic:
{request.topic}

Question Type:
{request.question_type}

Instructions:
{question_type_prompt}

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

Generate exactly {request.count} {request.question_type} questions now.

IMPORTANT:
The retrieved context may contain information from multiple parts of the chapter.

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
        "topic": request.topic,
        "difficulty": request.difficulty,
        "question_type": request.question_type,
        "questions": parsed_questions["questions"],
        "raw_response": answer.strip(),
        "retrieved_metadata": metadatas,
        "context_used": documents
    }


@app.post("/generate-flashcards")
def generate_flashcards(request: GenerateFlashCardsRequest):
    query = f"""
{request.subject}
{request.topic}
flash cards
quick revision
"""

    search_result = search_chunks(
        query=query,
        top_k=request.top_k
    )

    documents = search_result["results"]["documents"][0]
    metadatas = search_result["results"]["metadatas"][0]

    context = "\n\n".join(documents[:3])

    prompt = f"""
You are an expert NEET tutor.

Generate exactly {request.count} flash cards for quick revision.

Rules:
1. Return ONLY valid JSON.
2. Do NOT add markdown.
3. Use ONLY the provided NCERT context.
4. Each flash card must have:
   - frontText
   - backText
   - cardType
5. Keep frontText short and question-like.
6. Keep backText clear and exam-focused.

Subject:
{request.subject}

Topic:
{request.topic}

Card Type:
{request.card_type}

Output JSON format:
{{
  "flashcards": [
    {{
      "frontText": "Question or prompt",
      "backText": "Answer or explanation",
      "cardType": "Concept"
    }}
  ]
}}

NCERT Context:
{context}

Generate exactly {request.count} flash cards now.
"""

    answer = ask_qwen(prompt)

    try:
        parsed_flashcards = json.loads(answer)
    except json.JSONDecodeError:
        return {
            "success": False,
            "error": "Qwen returned invalid JSON",
            "raw_response": answer.strip()
        }

    return {
        "success": True,
        "subject": request.subject,
        "topic": request.topic,
        "flashcards": parsed_flashcards["flashcards"],
        "retrieved_metadata": metadatas
    }