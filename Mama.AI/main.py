from fastapi import FastAPI

app = FastAPI(
    title="NeetMama AI Service",
    description="AI service for PDF processing, RAG, and NEET question generation",
    version="1.0.0"
)

@app.get("/")
def home():
    return {
        "message": "NeetMama AI Service is running"
    }

@app.get("/health")
def health():
    return {
        "status": "ok"
    }