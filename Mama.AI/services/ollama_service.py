import requests


OLLAMA_BASE_URL = "http://localhost:11434"
LLM_MODEL = "qwen2.5-coder:14b"


def ask_qwen(prompt: str) -> str:
    response = requests.post(
        f"{OLLAMA_BASE_URL}/api/generate",
        json={
            "model": LLM_MODEL,
            "prompt": prompt,
            "stream": False,
            "format": "json"
        },
        timeout=300
    )

    response.raise_for_status()

    data = response.json()

    return data.get("response", "")