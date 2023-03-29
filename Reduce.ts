interface ILinkUrlDetail {
  title: string;
  url: string;
}

interface IGroupedList {
  letter: string;
  links: ILinkUrlDetail[];
}

function groupList(list: ListItem[]): IGroupedList[] {
  const result = Object.keys(list.reduce((acc, item) => {
    if (!acc[item.letter]) {
      acc[item.letter] = {
        letter: item.letter,
        links: [],
      };
    }
    acc[item.letter].links.push({ title: item.title, url: item.url });
    return acc;
  }, {})).map(key => {
    return {
      letter: key,
      links: acc[key].links,
    };
  });

  return result;
}
