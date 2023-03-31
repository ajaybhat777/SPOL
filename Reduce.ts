// Sample input data
const objectsList = [
  { letter: 'A', url: 'https://example.com/a', title: 'Title A' },
  { letter: 'B', url: 'https://example.com/b', title: 'Title B' },
  { letter: 'A', url: 'https://example.com/a2', title: 'Title A2' },
  { letter: 'C', url: 'https://example.com/c', title: 'Title C' },
  { letter: 'B', url: 'https://example.com/b2', title: 'Title B2' },
];

// Group the objects by letter
const grouped = objectsList.reduce((acc, obj) => {
  const letter = obj.letter;
  if (!acc[letter]) {
    acc[letter] = [];
  }
  acc[letter].push({ title: obj.title, url: obj.url });
  return acc;
}, {});

// Create the output model
const IAtoZLink = Object.keys(grouped).map((letter) => ({
  letter: letter,
  Ilinkurldetail: grouped[letter],
}));
