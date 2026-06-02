from fastapi import FastAPI
from pydantic import BaseModel

from services.pdf_service import extract_text_from_pdf


app = FastAPI(
    title="Mama.AI Service",
    description="AI service for PDF processing, RAG, and NEET question generation",
    version="1.0.0"
)


class PdfProcessRequest(BaseModel):
    file_path: str


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


@app.post("/extract-pdf-text")
def extract_pdf_text(request: PdfProcessRequest):
    result = extract_text_from_pdf(request.file_path)
    return result