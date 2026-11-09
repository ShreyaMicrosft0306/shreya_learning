from html.parser import HTMLParser
from urllib.request import Request, urlopen


class _TableParser(HTMLParser):
    """Collects text from every cell in every HTML table row."""

    def __init__(self):
        super().__init__(convert_charrefs=True)
        self.rows = []
        self._current_row = None
        self._current_cell = None

    def handle_starttag(self, tag, attrs):
        tag = tag.lower()

        if tag == "tr":
            self._current_row = []
        elif tag in ("td", "th") and self._current_row is not None:
            self._current_cell = []
        elif tag == "br" and self._current_cell is not None:
            self._current_cell.append("\n")

    def handle_data(self, data):
        if self._current_cell is not None:
            self._current_cell.append(data)

    def handle_endtag(self, tag):
        tag = tag.lower()

        if tag in ("td", "th") and self._current_cell is not None:
            self._current_row.append("".join(self._current_cell).strip())
            self._current_cell = None
        elif tag == "tr" and self._current_row is not None:
            if self._current_row:
                self.rows.append(self._current_row)
            self._current_row = None
            self._current_cell = None


def print_secret_message(document_url):
    """Downloads a published Google Doc and prints its character grid."""

    request = Request(
        document_url,
        headers={"User-Agent": "Mozilla/5.0"},
    )

    with urlopen(request, timeout=30) as response:
        html = response.read().decode("utf-8")

    parser = _TableParser()
    parser.feed(html)

    characters = {}
    max_x = -1
    max_y = -1

    for row in parser.rows:
        if len(row) < 3:
            continue

        try:
            x = int(row[0])
            y = int(row[2])
        except ValueError:
            # Ignore the table header and any unrelated table rows.
            continue

        character = row[1]
        if not character:
            continue

        characters[(x, y)] = character
        max_x = max(max_x, x)
        max_y = max(max_y, y)

    if not characters:
        raise ValueError(
            "No coordinate data was found. Make sure the Google Doc is "
            "published and that the URL ends with /pub."
        )

    # y = 0 is the bottom row in the document's coordinate system.
    for y in range(max_y, -1, -1):
        line = "".join(characters.get((x, y), " ") for x in range(max_x + 1))
        print(line.rstrip())


# Example:
# print_secret_message("https://docs.google.com/document/d/e/.../pub")
