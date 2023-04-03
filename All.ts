interface ILinkUrlDetail {
  title: string;
  url: string;
}

interface ISharePointLinkItem {
  Title: string;
  Letter: string;
  Description: string;
  AlternateName1: string;
  Alternateletter1: string;
  Alternatename2: string;
  alternateLetter2: string;
  url: string;
}

interface ILinkGroup {
  alphabet: string;
  links: ILinkUrlDetail[];
}

function groupLinksByLetters(linkItems: ISharePointLinkItem[]): ILinkGroup[] {
  const linkGroups: ILinkGroup[] = [];

  for (const item of linkItems) {
    const { Title, Letter, Description, AlternateName1, Alternateletter1, Alternatename2, alternateLetter2, url } = item;

    // Check if a group with the Letter exists in the output array
    let group = linkGroups.find((g) => g.alphabet === Letter);

    // If not, add a new group to the output array
    if (!group) {
      group = { alphabet: Letter, links: [] };
      linkGroups.push(group);
    }

    // Add the current link to the group's links array
    group.links.push({ title: Title, url: url });

    // Check if a group with the Alternateletter1 exists in the output array
    if (Alternateletter1) {
      let altGroup = linkGroups.find((g) => g.alphabet === Alternateletter1);

      // If not, add a new group to the output array
      if (!altGroup) {
        altGroup = { alphabet: Alternateletter1, links: [] };
        linkGroups.push(altGroup);
      }

      // Add the current link to the alternate group's links array
      altGroup.links.push({ title: AlternateName1, url: url });
    }

    // Check if a group with the alternateLetter2 exists in the output array
    if (alternateLetter2) {
      let altGroup = linkGroups.find((g) => g.alphabet === alternateLetter2);

      // If not, add a new group to the output array
      if (!altGroup) {
        altGroup = { alphabet: alternateLetter2, links: [] };
        linkGroups.push(altGroup);
      }

      // Add the current link to the alternate group's links array
      altGroup.links.push({ title: Alternatename2, url: url });
    }
  }

  // Sort link groups by alphabet
  linkGroups.sort((a, b) => a.alphabet.localeCompare(b.alphabet));

  return linkGroups;
}
