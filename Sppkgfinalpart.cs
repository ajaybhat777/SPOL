// Deploy the app package by adding it to the App Catalog's app catalog list
        var appCatalogList = appCatalog.RootFolder.Lists.GetByTitle("App Catalog");
        var listItemCreateInfo = new ListItemCreationInformation();
        var listItem = appCatalogList.AddItem(listItemCreateInfo);
        listItem["Title"] = fileInfo.Name;
        listItem["AppAuthor"] = "Author Name"; // Replace with the appropriate author name
        listItem["AppPackage"] = appPackage;
        listItem.Update();
        context.ExecuteQuery();
