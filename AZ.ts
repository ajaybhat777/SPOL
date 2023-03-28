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
  const outputList: OutputObject[] = [];

  for (const obj of inputList) {
    if (obj.Letter === selectedLetter) {
      const outputObj: OutputObject = {
        Title: obj.PrimaryName,
        AlsoKnownAs: '',
        Description: obj.description,
      };
      outputList.push(outputObj);
    }

    if (obj.alternateLetter1 === selectedLetter) {
      const outputObj: OutputObject = {
        Title: obj.alternateName1,
        AlsoKnownAs: obj.PrimaryName,
        Description: obj.description,
      };
      outputList.push(outputObj);
    }

    if (obj.alternateLetter2 === selectedLetter) {
      const outputObj: OutputObject = {
        Title: obj.alternateName2,
        AlsoKnownAs: obj.PrimaryName,
        Description: obj.description,
      };
      outputList.push(outputObj);
    }
  }

  return outputList;
}
