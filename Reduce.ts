type ListItem = {
  letter: string,
  url: string,
  title: string,
};

type GroupedList = {
  [letter: string]: {
    url: string[],
    title: string[],
  },
};

function groupList(list: ListItem[]): GroupedList {
  return list.reduce((result: GroupedList, item: ListItem) => {
    if (!result[item.letter]) {
      result[item.letter] = {
        url: [],
        title: [],
      };
    }
    result[item.letter].url.push(item.url);
    result[item.letter].title.push(item.title);
    return result;
  }, {});
}
