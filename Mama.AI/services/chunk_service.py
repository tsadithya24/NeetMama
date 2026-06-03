def chunk_text(
    text: str,
    chunk_size: int = 1200,
    overlap: int = 200
) -> list[dict]:

    if not text:
        return []

    # Safely split lines across different OS platforms and drop empty lines
    paragraphs = [p.strip() for p in text.splitlines() if p.strip()]

    chunks = []
    current_paragraphs = []
    current_length = 0
    chunk_number = 1

    for paragraph in paragraphs:
        # Safety check: If a single paragraph is larger than the maximum allowed chunk size
        if len(paragraph) > chunk_size:
            # Slices the mega-paragraph down into safe pieces
            sub_paragraphs = [paragraph[i:i+chunk_size] for i in range(0, len(paragraph), chunk_size)]
        else:
            sub_paragraphs = [paragraph]

        for sub_p in sub_paragraphs:
            # +1 accounts for the newline character used to join paragraphs later
            added_len = len(sub_p) + (1 if current_paragraphs else 0)

            if current_length + added_len <= chunk_size:
                current_paragraphs.append(sub_p)
                current_length += added_len
            else:
                # Commit the current chunk before handling overlap
                if current_paragraphs:
                    chunk_text = "\n".join(current_paragraphs)
                    chunks.append({
                        "chunk_number": chunk_number,
                        "text": chunk_text,
                        "length": len(chunk_text)
                    })
                    chunk_number += 1

                # Clean Overlap: Roll backward to extract only WHOLE paragraphs that fit the overlap budget
                overlap_paragraphs = []
                overlap_len = 0
                
                for p_prev in reversed(current_paragraphs):
                    p_len = len(p_prev) + (1 if overlap_paragraphs else 0)
                    if overlap_len + p_len <= overlap:
                        overlap_paragraphs.insert(0, p_prev)
                        overlap_len += p_len
                    else:
                        break  # Stop rolling back if the next whole paragraph exceeds the overlap limit
                
                # Form the foundation of the next chunk
                current_paragraphs = overlap_paragraphs + [sub_p]
                current_length = sum(len(p) for p in current_paragraphs) + (len(current_paragraphs) - 1)

    # Append any remaining text left in the buffer
    if current_paragraphs:
        chunk_text = "\n".join(current_paragraphs)
        chunks.append({
            "chunk_number": chunk_number,
            "text": chunk_text,
            "length": len(chunk_text)
        })

    return chunks
