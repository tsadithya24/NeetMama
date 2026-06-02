import fitz  # PyMuPDF


def extract_text_from_pdf(file_path: str) -> dict:
    extracted_pages = []
    full_text = ""

    try:
        pdf_document = fitz.open(file_path)

        for page_index in range(len(pdf_document)):
            page = pdf_document[page_index]
            page_text = page.get_text()

            extracted_pages.append({
                "page_number": page_index + 1,
                "text": page_text
            })

            full_text += f"\n\n--- Page {page_index + 1} ---\n\n"
            full_text += page_text

        pdf_document.close()

        return {
            "success": True,
            "page_count": len(extracted_pages),
            "text_length": len(full_text),
            "full_text": full_text,
            "pages": extracted_pages
        }

    except Exception as e:
        return {
            "success": False,
            "error": str(e)
        }