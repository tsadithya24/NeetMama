import requests


OLLAMA_BASE_URL = "http://localhost:11434"
EMBEDDING_MODEL = "nomic-embed-text"


def create_embedding(text: str) -> list[float]:
    response = requests.post(
        f"{OLLAMA_BASE_URL}/api/embeddings",
        json={
            "model": EMBEDDING_MODEL,
            "prompt": text
        },
        timeout=120
    )

    response.raise_for_status()

    data = response.json()

    return data["embedding"]