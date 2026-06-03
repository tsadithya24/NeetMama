import chromadb
from services.embedding_service import create_embedding


client = chromadb.PersistentClient(path="./chroma_db")

collection = client.get_or_create_collection(
    name="neet_documents"
)


def store_chunks(
    document_id: int,
    title: str,
    subject: str,
    chunks: list[dict]
) -> dict:
    ids = []
    documents = []
    embeddings = []
    metadatas = []

    for chunk in chunks:
        chunk_id = f"doc_{document_id}_chunk_{chunk['chunk_number']}"

        ids.append(chunk_id)
        documents.append(chunk["text"])
        embeddings.append(create_embedding(chunk["text"]))

        metadatas.append({
            "document_id": document_id,
            "title": title,
            "subject": subject,
            "chunk_number": chunk["chunk_number"],
            "length": chunk["length"]
        })

    collection.upsert(
        ids=ids,
        documents=documents,
        embeddings=embeddings,
        metadatas=metadatas
    )

    return {
        "success": True,
        "stored_chunks": len(chunks)
    }


def search_chunks(
    query: str,
    top_k: int = 5
) -> dict:
    query_embedding = create_embedding(query)

    results = collection.query(
        query_embeddings=[query_embedding],
        n_results=top_k
    )

    return {
        "success": True,
        "results": results
    }