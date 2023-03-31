interface ILinkUrlDetail {
  title: string;
  url: string;
}

interface IAtoZLink {
  letter: string;
  links: ILinkUrlDetail[];
}

// Sample input data
const inputData = [
  { letter: 'A', url: 'http://example.com/a1', title: 'A1' },
  { letter: 'A', url: 'http://example.com/a2', title: 'A2' },
  { letter: 'B', url: 'http://example.com/b1', title: 'B1' },
  { letter: 'B', url: 'http://example.com/b2', title: 'B2' },
  { letter: 'B', url: 'http://example.com/b3', title: 'B3' },
  { letter: 'C', url: 'http://example.com/c1', title: 'C1' },
];

// Group the input data by letter and create the IAtoZLink output model
const outputData: IAtoZLink[] = inputData.reduce((acc, curr) => {
  const index = acc.findIndex((item) => item.letter === curr.letter);
  if (index === -1) {
    acc.push({ letter: curr.letter, links: [{ title: curr.title, url: curr.url }] });
  } else {
    acc[index].links.push({ title: curr.title, url: curr.url });
  }
  return acc;
}, []);

console.log(outputData);
