interface InputObject {
  PrimaryName: string;
  Letter: string;
  alternateLetter1: string;
  alternateName1: string;
  alternateLetter2: string;
  alternateName2: string;
  description: string;
}

interface OutputObject {
  Title: string;
  AlsoKnownAs: string;
  Description: string;
}

function transformList(inputList: InputObject[], selectedLetter: string): OutputObject[] {
  const outputList: OutputObject[] = inputList.map((obj) => {
    if (obj.Letter === selectedLetter) {
      return {
        Title: obj.PrimaryName,
        AlsoKnownAs: '',
        Description: obj.description,
      };
    }

    if (obj.alternateLetter1 === selectedLetter) {
      return {
        Title: obj.alternateName1,
        AlsoKnownAs: obj.PrimaryName,
        Description: obj.description,
      };
    }

    if (obj.alternateLetter2 === selectedLetter) {
      return {
        Title: obj.alternateName2,
        AlsoKnownAs: obj.PrimaryName,
        Description: obj.description,
      };
    }

    return null;
  }).filter(Boolean);

  return outputList;
}
